//
// SuspensionManager.cpp
// SuspensionManager 类的实现
//

#include "pch.h"
#include "SuspensionManager.h"

#include <collection.h>
#include <algorithm>

using namespace NETGEAR_Genie_Win8::Common;

using namespace Concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::Storage::FileProperties;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;

namespace
{
	Map<String^, Object^>^ _sessionState = ref new Map<String^, Object^>();
	String^ sessionStateFilename = "_sessionState.dat";

	// 对象的前向声明读取/写入支持
	void WriteObject(Windows::Storage::Streams::DataWriter^ writer, Platform::Object^ object);
	Platform::Object^ ReadObject(Windows::Storage::Streams::DataReader^ reader);
}

/// <summary>
/// 提供对当前会话的全局会话状态的访问。此状态由
/// <see cref="SaveAsync"/> 序列化并由 <see cref="RestoreAsync"/> 还原，这需要值为
/// 以下项之一: 包含整数的装箱值、浮点单精度值和双精度值、
/// 宽字符、布尔值、字符串和 GUID，或者映射值受相同约束制约的 Map<String^, Object^>。
/// 会话状态应尽可能紧凑。
/// </summary>
IMap<String^, Object^>^ SuspensionManager::SessionState::get(void)
{
	return _sessionState;
}

/// <summary>
/// 将 WeakReference 包装为引用对象以便在集合中使用。
/// </summary>
private ref class WeakFrame sealed
{
private:
	WeakReference _frameReference;

internal:
	WeakFrame(Frame^ frame) { _frameReference = frame; }
	property Frame^ ResolvedFrame
	{
		Frame^ get(void) { return _frameReference.Resolve<Frame>(); }
	};
};

namespace
{
	std::vector<WeakFrame^> _registeredFrames;
	DependencyProperty^ FrameSessionStateKeyProperty =
		DependencyProperty::RegisterAttached("_FrameSessionStateKeyProperty",
		TypeName(String::typeid), TypeName(SuspensionManager::typeid), nullptr);
	DependencyProperty^ FrameSessionStateProperty =
		DependencyProperty::RegisterAttached("_FrameSessionStateProperty",
		TypeName(IMap<String^, Object^>::typeid), TypeName(SuspensionManager::typeid), nullptr);
}

/// <summary>
/// 注册 <see cref="Frame"/> 实例以允许将其导航历史记录保存到
/// <see cref="SessionState"/> 并从中还原。如果框架将参与会话状态管理，
/// 则应在创建框架后立即注册。在
/// 注册时，如果已还原指定键的状态，
/// 则将立即还原导航历史记录。
/// <see cref="RestoreAsync(String)"/> 的后续调用也将还原导航历史记录。
/// </summary>
/// <param name="frame">其导航历史记录应由
/// <see cref="SuspensionManager"/> 托管的实例</param>
/// <param name="sessionStateKey"><see cref="SessionState"/> 的唯一键，用于
/// 存储与导航相关的信息。</param>
void SuspensionManager::RegisterFrame(Frame^ frame, String^ sessionStateKey)
{
	if (frame->GetValue(FrameSessionStateKeyProperty) != nullptr)
	{
		throw ref new FailureException("Frames can only be registered to one session state key");
	}

	if (frame->GetValue(FrameSessionStateProperty) != nullptr)
	{
		throw ref new FailureException("Frames must be either be registered before accessing frame session state, or not registered at all");
	}

	// 使用依赖项属性可会话键与框架相关联，并记录其
	// 导航状态应托管的框架
	frame->SetValue(FrameSessionStateKeyProperty, sessionStateKey);
	_registeredFrames.insert(_registeredFrames.begin(), ref new WeakFrame(frame));

	// 查看导航状态是否可还原
	RestoreFrameNavigationState(frame);
}

/// <summary>
/// 解除之前由 <see cref="RegisterFrame"/> 注册的 <see cref="Frame"/>
/// 与 <see cref="SessionState"/> 的关联。之前捕获的任何导航状态都将
/// 移除。
/// </summary>
/// <param name="frame">其导航历史记录不应再
/// 托管的实例。</param>
void SuspensionManager::UnregisterFrame(Frame^ frame)
{
	// 移除会话状态并移除框架列表中其导航
	// 状态将被保存的框架(以及无法再访问的任何弱引用)
	auto key = safe_cast<String^>(frame->GetValue(FrameSessionStateKeyProperty));
	if (SessionState->HasKey(key)) SessionState->Remove(key);
	_registeredFrames.erase(
		std::remove_if(_registeredFrames.begin(), _registeredFrames.end(), [=](WeakFrame^& e)
		{
			auto testFrame = e->ResolvedFrame;
			return testFrame == nullptr || testFrame == frame;
		}),
		_registeredFrames.end()
	);
}

/// <summary>
/// 为与指定的 <see cref="Frame"/> 相关联的会话状态提供存储。
/// 之前已向 <see cref="RegisterFrame"/> 注册的框架已自动
/// 保存其会话状态且还原为全局
/// <see cref="SessionState"/> 的一部分。未注册的框架具有
/// 在还原已从导航缓存中丢弃的页面时仍然有用的
/// 瞬时状态。
/// </summary>
/// <remarks>应用程序可能会选择依赖 <see cref="LayoutAwarePage"/> 管理
/// 特定于页面的状态，而非直接使用框架会话状态。</remarks>
/// <param name="frame">需要会话状态的实例。</param>
/// <returns>状态集合受限于与
/// <see cref="SessionState"/> 相同的序列化机制。</returns>
IMap<String^, Object^>^ SuspensionManager::SessionStateForFrame(Frame^ frame)
{
	auto frameState = safe_cast<IMap<String^, Object^>^>(frame->GetValue(FrameSessionStateProperty));

	if (frameState == nullptr)
	{
		auto frameSessionKey = safe_cast<String^>(frame->GetValue(FrameSessionStateKeyProperty));
		if (frameSessionKey != nullptr)
		{
			// 已注册框架反映相应的会话状态
			if (!_sessionState->HasKey(frameSessionKey))
			{
				_sessionState->Insert(frameSessionKey, ref new Map<String^, Object^>());
			}
			frameState = safe_cast<IMap<String^, Object^>^>(_sessionState->Lookup(frameSessionKey));
		}
		else
		{
			// 未注册框架具有瞬时状态
			frameState = ref new Map<String^, Object^>();
		}
		frame->SetValue(FrameSessionStateProperty, frameState);
	}
	return frameState;
}

void SuspensionManager::RestoreFrameNavigationState(Frame^ frame)
{
	auto frameState = SessionStateForFrame(frame);
	if (frameState->HasKey("Navigation"))
	{
		frame->SetNavigationState(safe_cast<String^>(frameState->Lookup("Navigation")));
	}
}

void SuspensionManager::SaveFrameNavigationState(Frame^ frame)
{
	auto frameState = SessionStateForFrame(frame);
	frameState->Insert("Navigation", frame->GetNavigationState());
}

/// <summary>
/// 保存当前 <see cref="SessionState"/>。任何 <see cref="Frame"/> 实例
/// (已向 <see cref="RegisterFrame"/> 注册)都还将保留其当前的
/// 导航堆栈，从而使其活动 <see cref="Page"/> 可以
/// 保存其状态。
/// </summary>
/// <returns>反映会话状态保存时间的异步任务。</returns>
task<void> SuspensionManager::SaveAsync(void)
{
	// 保存所有已注册框架的导航状态
	for (auto&& weakFrame : _registeredFrames)
	{
		auto frame = weakFrame->ResolvedFrame;
		if (frame != nullptr) SaveFrameNavigationState(frame);
	}

	// 以同步方式序列化会话状态以避免对共享
	// 状态的异步访问
	auto sessionData = ref new InMemoryRandomAccessStream();
	auto sessionDataWriter = ref new DataWriter(sessionData->GetOutputStreamAt(0));
	WriteObject(sessionDataWriter, _sessionState);

	// 以同步方式捕获会话状态之后，开始
	// 将结果写入磁盘的异步处理
	return task<unsigned int>(sessionDataWriter->StoreAsync()).then([=](unsigned int)
	{
		return sessionDataWriter->FlushAsync();
	}).then([=](bool flushSucceeded)
	{
		(void)flushSucceeded; // 未使用的参数
		return ApplicationData::Current->LocalFolder->CreateFileAsync(sessionStateFilename,
			CreationCollisionOption::ReplaceExisting);
	}).then([=](StorageFile^ createdFile)
	{
		return createdFile->OpenAsync(FileAccessMode::ReadWrite);
	}).then([=](IRandomAccessStream^ newStream)
	{
		return RandomAccessStream::CopyAndCloseAsync(
			sessionData->GetInputStreamAt(0), newStream->GetOutputStreamAt(0));
	}).then([=](UINT64 copiedBytes)
	{
		(void)copiedBytes; // 未使用的参数
		return;
	});
}

/// <summary>
/// 还原之前保存的 <see cref="SessionState"/>。任何 <see cref="Frame"/> 实例
/// (已向 <see cref="RegisterFrame"/> 注册)都还将还原其先前的导航
/// 状态，从而使其活动 <see cref="Page"/> 可以还原其
/// 状态。
/// </summary>
/// <param name="version">将版本标识符与会话状态相比较以防止
/// 会话状态的不兼容版本访问应用程序代码。不同版本的
/// 已保存状态将被忽略，从而导致 <see cref="SessionState"/>
/// 字典为空。</param>
/// <returns>反映何时读取会话状态的异步任务。
/// 在此任务完成之前，不应依赖 <see cref="SessionState"/>
/// 的内容。</returns>
task<void> SuspensionManager::RestoreAsync(void)
{
	_sessionState->Clear();

	task<StorageFile^> getFileTask(ApplicationData::Current->LocalFolder->GetFileAsync(sessionStateFilename));
	return getFileTask.then([=](StorageFile^ stateFile)
	{
		task<BasicProperties^> getBasicPropertiesTask(stateFile->GetBasicPropertiesAsync());
		return getBasicPropertiesTask.then([=](BasicProperties^ stateFileProperties)
		{
			auto size = unsigned int(stateFileProperties->Size);
			if (size != stateFileProperties->Size) throw ref new FailureException("Session state larger than 4GB");
			task<IRandomAccessStreamWithContentType^> openReadTask(stateFile->OpenReadAsync());
			return openReadTask.then([=](IRandomAccessStreamWithContentType^ stateFileStream)
			{
				auto stateReader = ref new DataReader(stateFileStream);
				return task<unsigned int>(stateReader->LoadAsync(size)).then([=](unsigned int bytesRead)
				{
					(void)bytesRead; // 未使用的参数
					// 反序列化会话状态
					Object^ content = ReadObject(stateReader);
					_sessionState = (Map<String^, Object^>^)content;

					// 将任何已注册框架还原为其已保存状态
					for (auto&& weakFrame : _registeredFrames)
					{
						auto frame = weakFrame->ResolvedFrame;
						if (frame != nullptr)
						{
							frame->ClearValue(FrameSessionStateProperty);
							RestoreFrameNavigationState(frame);
						}
					}
				}, task_continuation_context::use_current());
			});
		});
	});
}

#pragma region Object serialization for a known set of types

namespace
{
	// 用于识别序列化类型的代码
	enum StreamTypes {
		NullPtrType = 0,

		// 支持的 IPropertyValue 类型
		UInt8Type, UInt16Type, UInt32Type, UInt64Type, Int16Type, Int32Type, Int64Type,
		SingleType, DoubleType, BooleanType, Char16Type, GuidType, StringType,

		// 其他受支持类型
		StringToObjectMapType,

		// 用于确保数据流完整性的标记值
		MapEndMarker
	};

	void WriteString(DataWriter^ writer, String^ string)
	{
		writer->WriteByte(StringType);
		writer->WriteUInt32(writer->MeasureString(string));
		writer->WriteString(string);
	}

	void WriteProperty(DataWriter^ writer, IPropertyValue^ propertyValue)
	{
		switch (propertyValue->Type)
		{
		case PropertyType::UInt8:
			writer->WriteByte(UInt8Type);
			writer->WriteByte(propertyValue->GetUInt8());
			return;
		case PropertyType::UInt16:
			writer->WriteByte(UInt16Type);
			writer->WriteUInt16(propertyValue->GetUInt16());
			return;
		case PropertyType::UInt32:
			writer->WriteByte(UInt32Type);
			writer->WriteUInt32(propertyValue->GetUInt32());
			return;
		case PropertyType::UInt64:
			writer->WriteByte(UInt64Type);
			writer->WriteUInt64(propertyValue->GetUInt64());
			return;
		case PropertyType::Int16:
			writer->WriteByte(Int16Type);
			writer->WriteUInt16(propertyValue->GetInt16());
			return;
		case PropertyType::Int32:
			writer->WriteByte(Int32Type);
			writer->WriteUInt32(propertyValue->GetInt32());
			return;
		case PropertyType::Int64:
			writer->WriteByte(Int64Type);
			writer->WriteUInt64(propertyValue->GetInt64());
			return;
		case PropertyType::Single:
			writer->WriteByte(SingleType);
			writer->WriteSingle(propertyValue->GetSingle());
			return;
		case PropertyType::Double:
			writer->WriteByte(DoubleType);
			writer->WriteDouble(propertyValue->GetDouble());
			return;
		case PropertyType::Boolean:
			writer->WriteByte(BooleanType);
			writer->WriteBoolean(propertyValue->GetBoolean());
			return;
		case PropertyType::Char16:
			writer->WriteByte(Char16Type);
			writer->WriteUInt16(propertyValue->GetChar16());
			return;
		case PropertyType::Guid:
			writer->WriteByte(GuidType);
			writer->WriteGuid(propertyValue->GetGuid());
			return;
		case PropertyType::String:
			WriteString(writer, propertyValue->GetString());
			return;
		default:
			throw ref new InvalidArgumentException("Unsupported property type");
		}
	}

	void WriteStringToObjectMap(DataWriter^ writer, IMap<String^, Object^>^ map)
	{
		writer->WriteByte(StringToObjectMapType);
		writer->WriteUInt32(map->Size);
		for (auto&& pair : map)
		{
			WriteObject(writer, pair->Key);
			WriteObject(writer, pair->Value);
		}
		writer->WriteByte(MapEndMarker);
	}

	void WriteObject(DataWriter^ writer, Object^ object)
	{
		if (object == nullptr)
		{
			writer->WriteByte(NullPtrType);
			return;
		}

		auto propertyObject = dynamic_cast<IPropertyValue^>(object);
		if (propertyObject != nullptr)
		{
			WriteProperty(writer, propertyObject);
			return;
		}

		auto mapObject = dynamic_cast<IMap<String^, Object^>^>(object);
		if (mapObject != nullptr)
		{
			WriteStringToObjectMap(writer, mapObject);
			return;
		}

		throw ref new InvalidArgumentException("Unsupported data type");
	}

	String^ ReadString(DataReader^ reader)
	{
		int length = reader->ReadUInt32();
		String^ string = reader->ReadString(length);
		return string;
	}

	IMap<String^, Object^>^ ReadStringToObjectMap(DataReader^ reader)
	{
		auto map = ref new Map<String^, Object^>();
		auto size = reader->ReadUInt32();
		for (unsigned int index = 0; index < size; index++)
		{
			auto key = safe_cast<String^>(ReadObject(reader));
			auto value = ReadObject(reader);
			map->Insert(key, value);
		}
		if (reader->ReadByte() != MapEndMarker)
		{
			throw ref new InvalidArgumentException("Invalid stream");
		}
		return map;
	}

	Object^ ReadObject(DataReader^ reader)
	{
		auto type = reader->ReadByte();
		switch (type)
		{
		case NullPtrType:
			return nullptr;
		case UInt8Type:
			return reader->ReadByte();
		case UInt16Type:
			return reader->ReadUInt16();
		case UInt32Type:
			return reader->ReadUInt32();
		case UInt64Type:
			return reader->ReadUInt64();
		case Int16Type:
			return reader->ReadInt16();
		case Int32Type:
			return reader->ReadInt32();
		case Int64Type:
			return reader->ReadInt64();
		case SingleType:
			return reader->ReadSingle();
		case DoubleType:
			return reader->ReadDouble();
		case BooleanType:
			return reader->ReadBoolean();
		case Char16Type:
			return (char16_t)reader->ReadUInt16();
		case GuidType:
			return reader->ReadGuid();
		case StringType:
			return ReadString(reader);
		case StringToObjectMapType:
			return ReadStringToObjectMap(reader);
		default:
			throw ref new InvalidArgumentException("Unsupported property type");
		}
	}
}

#pragma endregion

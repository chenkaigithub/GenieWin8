#pragma once

using namespace Windows::Foundation;

namespace WinRT_DLNA
{
	public ref class mydlna sealed
	{
	public:
		int Add(int x, int y);
		void Startupnp();
		void Stopupnp();
		void Refreshdevices();
		void Getlist();
		void Importfile();
	};
}
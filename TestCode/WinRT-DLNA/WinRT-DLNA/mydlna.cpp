// mydlna.cpp : 定义控制台应用程序的入口点。
//

#include "pch.h"
#include "DLNADelegation.h"
#include "mydlna.h"

//using namespace SiteView;

//int _tmain(int argc, char* argv[])
//{
//	DLNADelegation * pDlna = DLNADelegation::GetInstance();
//
//	char buf[256];
//	while (gets_s(buf))
//	{
//		if (strcmp("a", buf) == 0)
//		{
//			pDlna->startUPNPServer();
//			pDlna->startUPNPRender();
//			pDlna->startUPNPControlPoint();
//			printf("add.....\n");
//		}
//		else if (strcmp("e", buf) == 0)
//		{
//			pDlna->stopUPNPServer();
//			pDlna->stopUPNPRender();
//			pDlna->stopUPNPControlpoint();
//			printf("exit.....\n");
//		}
//		else if (strcmp("r", buf) == 0)
//		{
//			pDlna->refreshDevices(DLNACore::FlushMode_All);
//		}
//		else if (strcmp("i", buf) == 0)
//		{
//			NPT_List<NPT_String> dirs;
//			dirs.Add("C:\\MyMedia\\video");
//
//			NPT_List<NPT_String> names;
//			names.Add("video");
//			pDlna->importFileSystemToMediaServer(dirs, names, ShareType_VAI);
//		}
//		else if (strcmp("ii", buf) == 0)
//		{
//			NPT_List<NPT_String> dirs;
//			dirs.Add("C:\\MyMedia\\video");
//			dirs.Add("C:\\MyMedia\\music");
//
//			NPT_List<NPT_String> names;
//			names.Add("video");
//			names.Add("music");
//			pDlna->importFileSystemToMediaServer(dirs, names, ShareType_VAI);
//		}
//		else if (strcmp("iii", buf) == 0)
//		{
//			NPT_List<NPT_String> dirs;
//			dirs.Add("C:\\MyMedia\\video");
//			dirs.Add("C:\\MyMedia\\music");
//			dirs.Add("C:\\MyMedia\\image");
//
//			NPT_List<NPT_String> names;
//			names.Add("video");
//			names.Add("music");
//			names.Add("image");
//			pDlna->importFileSystemToMediaServer(dirs, names, ShareType_Image);
//		}
//		else if (strcmp("p", buf) == 0)
//		{
//			pDlna->openMediaObj(NULL);
//		}
//		else if (strcmp("g", buf) == 0)
//		{
//			DeviceDescList dmsls = pDlna->getMediaServerList();
//			DeviceDescList dmrls = pDlna->getMediaRenderList();
//			for (NPT_Ordinal i = 0; i < dmsls.count(); i++)
//			{
//				DeviceDesc * sdesc = dmsls.itemAt(i);
//				NPT_String url = sdesc->rootDescURL();	// http://172.16.0.12:5414/devices/
//				url = url.SubString(7);
//				int nPos = url.Find(':');
//				url = url.Left(nPos);
//				printf("Server:%s, xmpp:%s\n", sdesc->friendlyName(), sdesc->xmpp());
//			}
//			for (NPT_Ordinal j = 0; j < dmrls.count(); j++)
//			{
//				DeviceDesc * rdesc = dmrls.itemAt(j);
//				printf("Render:%s\n", rdesc->friendlyName());
//			}
//		}
//		else if (strcmp("b", buf) == 0)
//		{
//			int nNum = 0;
//			scanf_s("%d", &nNum);
//			pDlna->browse(nNum, "0", "0");
//		}
//		else if (strcmp("q", buf) == 0)
//		{
//			break;
//		}	
//	}
//
//	return 0;
//}


using namespace WinRT_DLNA;
using namespace Platform;

//DLNADelegation * pDlna = DLNADelegation::GetInstance();

int mydlna::Add(int x, int y)
{
	return x+y;
}

void mydlna::Startupnp()
{
	DLNADelegation * pDlna = DLNADelegation::GetInstance();
	pDlna->startUPNPServer();
	pDlna->startUPNPRender();
	pDlna->startUPNPControlPoint();
}

void mydlna::Stopupnp()
{
	DLNADelegation * pDlna = DLNADelegation::GetInstance();
	pDlna->stopUPNPServer();
	pDlna->stopUPNPRender();
	pDlna->stopUPNPControlpoint();
}

void mydlna::Refreshdevices()
{
	DLNADelegation * pDlna = DLNADelegation::GetInstance();
	pDlna->refreshDevices(DLNACore::FlushMode_All);
}

void mydlna::Getlist()
{
	DLNADelegation * pDlna = DLNADelegation::GetInstance();
	DeviceDescList dmsls = pDlna->getMediaServerList();
	DeviceDescList dmrls = pDlna->getMediaRenderList();
	for (NPT_Ordinal i = 0; i < dmsls.count(); i++)
	{
		DeviceDesc * sdesc = dmsls.itemAt(i);
		NPT_String url = sdesc->rootDescURL();	// http://172.16.0.12:5414/devices/
		url = url.SubString(7);
		int nPos = url.Find(':');
		url = url.Left(nPos);
		printf("Server:%s, xmpp:%s\n", sdesc->friendlyName(), sdesc->xmpp());
	}
	for (NPT_Ordinal j = 0; j < dmrls.count(); j++)
	{
		DeviceDesc * rdesc = dmrls.itemAt(j);
		printf("Render:%s\n", rdesc->friendlyName());
	}
}

void mydlna::Importfile()
{
	DLNADelegation * pDlna = DLNADelegation::GetInstance();
	NPT_List<NPT_String> dirs;
	dirs.Add("C:\\MyMedia\\video");

	NPT_List<NPT_String> names;
	names.Add("video");
	pDlna->importFileSystemToMediaServer(dirs, names, ShareType_VAI);
}
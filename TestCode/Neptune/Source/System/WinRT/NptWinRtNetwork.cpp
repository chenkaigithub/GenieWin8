/*****************************************************************
|
|      Neptune - Network :: WinRT Implementation
|
|      (c) 2011-2012 Gilles Boccon-Gibod
|      Author: Gilles Boccon-Gibod (bok@bok.net)
|
 ****************************************************************/

/*----------------------------------------------------------------------
|       includes
+---------------------------------------------------------------------*/
#include "NptConfig.h"
#include "NptTypes.h"
#include "NptStreams.h"
#include "NptThreads.h"
#include "NptNetwork.h"
#include "NptUtils.h"
#include "NptConstants.h"
#include "NptSockets.h"
#include <string>

using namespace std;
using namespace Platform;
using namespace Windows::Networking;
using namespace Windows::Networking::Connectivity;

/*----------------------------------------------------------------------
|   NPT_NetworkInterface::GetNetworkInterfaces
+---------------------------------------------------------------------*/
NPT_Result
NPT_NetworkInterface::GetNetworkInterfaces(NPT_List<NPT_NetworkInterface*>& interfaces)
{
	auto hostnames = NetworkInformation::GetHostNames();
	unsigned int iface_index = 0;
	for (int i = 0; i < hostnames->Size; i++)
	{
		if (hostnames->GetAt(i)->IPInformation != nullptr && hostnames->GetAt(i)->IPInformation->NetworkAdapter != nullptr)
		{
			NPT_Flags    flags = 0;

			auto iAddress = hostnames->GetAt(i)->DisplayName;
			std::wstring ws1(iAddress->Data());
			std::string str(ws1.length(), ' ');
			std::copy(ws1.begin(), ws1.end(), str.begin());

			unsigned long ip[4] = {0}; 
			int position1 = str.find(".");
			int position2 = str.find(".", position1 + 1);
			int position3 = str.find(".", position2 + 1);

			ip[0] = atol(str.substr(0, position1).c_str());
			ip[1] = atol(str.substr(position1 + 1, position2).c_str());
			ip[2] = atol(str.substr(position2 + 1, position3).c_str());
			ip[3] = atol(str.substr(position3 + 1).c_str());
			unsigned long ipAddress = (ip[0] << 24) + (ip[1] << 16) + (ip[2] << 8) + ip[3];
			NPT_IpAddress primary_address(ipAddress);

			auto iFlag = hostnames->GetAt(i)->IPInformation->NetworkAdapter->IanaInterfaceType;
			if (iFlag == 23)
			{
				flags |= NPT_NETWORK_INTERFACE_FLAG_POINT_TO_POINT; 
			}
			if (iFlag == 24)
			{
				flags |= NPT_NETWORK_INTERFACE_FLAG_LOOPBACK;
			}
			
			NPT_IpAddress broadcast_address; // not supported yet
			NPT_IpAddress netmask;           // not supported yet
			NPT_MacAddress mac;				 // not supported yet
			
			char iface_name[5];
			iface_name[0] = 'i';
			iface_name[1] = 'f';
			iface_name[2] = '0'+(iface_index/10);
			iface_name[3] = '0'+(iface_index%10);
			iface_name[4] = '\0';
			NPT_NetworkInterface* iface = new NPT_NetworkInterface(iface_name, mac, flags);

			// set the interface address
			NPT_NetworkInterfaceAddress iface_address(
				primary_address,
				broadcast_address,
				NPT_IpAddress::Any,
				netmask);
			iface->AddAddress(iface_address);  

			// add the interface to the list
			interfaces.Add(iface);   

			// increment the index (used for generating the name
			iface_index++;
		}
	}
    return NPT_SUCCESS;
}

/*----------------------------------------------------------------------
|   NPT_IpAddress::ResolveName
+---------------------------------------------------------------------*/
NPT_Result
NPT_IpAddress::ResolveName(const char* name, NPT_Timeout timeout)
{
	m_HostName = name;
	return NPT_SUCCESS;
}
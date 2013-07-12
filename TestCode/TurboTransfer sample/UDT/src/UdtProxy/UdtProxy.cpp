#include "UdtProxy.h"
#include "TurboTransfer.h"

CUdtProxy * CUdtProxy::m_pUdtProxy = nullptr;

CUdtProxy::CUdtProxy()
	: m_pUdt(new CUdtCore(this))
{

}


CUdtProxy::~CUdtProxy()
{
}

CUdtProxy * CUdtProxy::GetInStance()
{
	if (m_pUdtProxy == nullptr)
	{
		m_pUdtProxy = new CUdtProxy;
	}

	return m_pUdtProxy;
}


int CUdtProxy::listenFileSend(const int nPort)
{
	m_pUdt->listenFileSend(nPort);
	return 0;
}

void CUdtProxy::sendMessage(const char* pstrAddr, const int nPort, const char* pstrMessage, const char* pstrHostname, const char* pstrSendtype)
{
	m_pUdt->sendMessage(pstrAddr, nPort, pstrMessage, pstrHostname, pstrSendtype);
}

void CUdtProxy::sendfile(const char* pstrAddr, const int nPort, const char* pstrFileName, const char* pstrHostname, const char *pstrSendtype)
{
	m_pUdt->sendfile(pstrAddr, nPort, pstrFileName, pstrHostname, pstrSendtype);
}

void CUdtProxy::sendMultiFiles(const char* pstrAddr, const int nPort, const std::vector<std::string> strArray, const char* pstrHostName, const char* pstrSendtype)
{
	m_pUdt->sendMultiFiles(pstrAddr, nPort, strArray, pstrHostName, pstrSendtype);
}

void CUdtProxy::sendFolderFiles(const char* pstrAddr, const int nPort, const char* pstrFolderName, const char* pstrHostName, const char* pstrSendtype)
{
	m_pUdt->sendFolderFiles(pstrAddr, nPort, pstrFolderName, pstrHostName, pstrSendtype);
}

void CUdtProxy::replyAccept(const char* pstrReply)
{
	m_pUdt->replyAccept(pstrReply);
}

void CUdtProxy::stopTransfer(const int nType)
{
	m_pUdt->stopTransfer(nType);
}

void CUdtProxy::stopListen()
{
	m_pUdt->stopListen();
}

//////////////////////////////////////////////////////////////////////////
// Call Back

void CUdtProxy::onAccept(const char* pstrIpAddr, const char* pstrHostName, const char* pstrSendType, const char* pstrFileName, int nFileCount)
{
	extern PFOnAccept gOnAccept;
	gOnAccept(pstrIpAddr, pstrHostName, pstrSendType, pstrFileName, nFileCount);
}

void CUdtProxy::onSendFinished(const char * pstrMsg)
{
	extern PFOnFinished gOnFinished;
	gOnFinished(1, pstrMsg);
}

void CUdtProxy::onRecvFinished(const char * pstrMsg)
{
	extern PFOnFinished gOnFinished;
	gOnFinished(2, pstrMsg);
}

void CUdtProxy::onSendTransfer(const int64_t nFileTotalSize, const int64_t nCurrent, const char* pstrFileName)
{
	extern PFOnTransfer gOnTransfer;
	gOnTransfer(1, nFileTotalSize, nCurrent, pstrFileName);
}

void CUdtProxy::onRecvTransfer(const int64_t nFileTotalSize, const int64_t nCurrent, const char* pstrFileName)
{
	extern PFOnTransfer gOnTransfer;
	gOnTransfer(2, nFileTotalSize, nCurrent, pstrFileName);
}

void CUdtProxy::onRecvMessage(const char* pstrIpAddr, const char* pstrHostName, const char* pstrMsg)
{
	char * temp = "onRecvMessage";
	//extern Dispatch pDispatch;
	//(*pDispatch)(temp);
}
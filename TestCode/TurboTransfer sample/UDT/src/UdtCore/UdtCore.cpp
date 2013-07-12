#include "UdtCore.h"


using namespace std;

CUdtCore::CUdtCore(CUDTCallBack * pCallback)
	: m_pCallBack(pCallback)
	, m_sockListen(-1)
	, m_bSend(false)
	, m_bRecv(false)
{
}


CUdtCore::~CUdtCore()
{
}

int CUdtCore::listenFileSend(const int nPort)
{
	CUDT::startup();

	m_sockListen = CUDT::socket(AF_INET, SOCK_STREAM, 0);

	struct sockaddr_in srv_addr;
	int addr_len = sizeof(srv_addr);

	memset(&srv_addr, 0, addr_len);
	srv_addr.sin_family = AF_INET;
	srv_addr.sin_port = htons(nPort);
	srv_addr.sin_addr.s_addr = htons(INADDR_ANY);

	// Windows UDP issue
	// For better performance, modify HKLM\System\CurrentControlSet\Services\Afd\Parameters\FastSendDatagramThreshold
#ifdef WIN32
	int mss = 1052;
	CUDT::setsockopt(m_sockListen, 0, UDT_MSS, &mss, sizeof(int));
#else
	int snd_buf = 64000;
	int rcv_buf = 64000;
	CUDT::setsockopt(m_sockListen, 0, UDT_SNDBUF, &snd_buf, sizeof(snd_buf));
	CUDT::setsockopt(m_sockListen, 0, UDT_RCVBUF, &rcv_buf, sizeof(rcv_buf));

	CUDT::setsockopt(m_sockListen, 0, UDT_SNDBUF, &snd_buf, sizeof(snd_buf));
	CUDT::setsockopt(m_sockListen, 0, UDT_RCVBUF, &rcv_buf, sizeof(rcv_buf));
#endif

	// bind socket
	if (CUDT::ERROR_SOCK == CUDT::bind(m_sockListen, (sockaddr*)&srv_addr, addr_len))
	{
		cout << "bind fail: " << CUDT::getlasterror().getErrorMessage() << endl;
		return -1;
	}

	// listen socket
	if (CUDT::ERROR_SOCK == CUDT::listen(m_sockListen, 10))
	{
		cout << "listen fail: " << CUDT::getlasterror().getErrorMessage() << endl;
		return -1;
	}

	// begin the thread, wait client connect
#ifndef WIN32
	pthread_mutex_init(&m_AcceptLock, NULL);
	pthread_cond_init(&m_AcceptCond, NULL);
	pthread_create(&m_hListenThread, NULL, _ListenThreadProc, this);
#else
	DWORD dwThreadID;
	m_AcceptLock = CreateMutex(NULL, false, NULL);
	m_AcceptCond = CreateEvent(NULL, false, false, NULL);
	m_hListenThread = CreateThread(NULL, 0, _ListenThreadProc, this, NULL, &dwThreadID);
#endif

	return 0;
}

void CUdtCore::SendEx(const char* pstrAddr, const int nPort, const char* pstrHostName, const char* pstrSendtype, const std::vector<std::string> vecArray, int nType)
{
	m_pSrvCxt = new _stServerContext();

	memset(m_pSrvCxt, 0, sizeof(_stServerContext));
	memcpy(m_pSrvCxt->strAddr, pstrAddr, 32);
	memcpy(m_pSrvCxt->strHostName, pstrHostName, 128);
	memcpy(m_pSrvCxt->strSendtype, pstrSendtype, 128);

	m_pSrvCxt->vecArray = vecArray;
	m_pSrvCxt->nPort = nPort;
	if (nType == 1)
	{
		memcpy(m_pSrvCxt->strXSR, "TSR", 3);
		memcpy(m_pSrvCxt->strXSP, "TSP1", 4);
		memcpy(m_pSrvCxt->strXCS, "TCS", 3);
		memcpy(m_pSrvCxt->strXSF, "TSF", 3);
	}
	else if (nType == 2)
	{
		memcpy(m_pSrvCxt->strXSR, "FSR", 3);
		memcpy(m_pSrvCxt->strXSP, "FSP1", 4);
		memcpy(m_pSrvCxt->strXCS, "FCS", 3);
		memcpy(m_pSrvCxt->strXSF, "FSF", 3);
	}
	else if (nType == 3)
	{
		memcpy(m_pSrvCxt->strXSR, "MSR", 3);
		memcpy(m_pSrvCxt->strXSP, "MSP1", 4);
		memcpy(m_pSrvCxt->strXCS, "MCS", 3);
		memcpy(m_pSrvCxt->strXSF, "MSF", 3);
	}
	else if (nType == 4)
	{
		memcpy(m_pSrvCxt->strXSR, "DSR", 3);
		memcpy(m_pSrvCxt->strXSP, "DSP1", 4);
		memcpy(m_pSrvCxt->strXCS, "DFS", 3);
		memcpy(m_pSrvCxt->strXSF, "DSF", 3);
	}

#ifndef WIN32
	pthread_mutex_init(&m_SendLock, NULL);
	pthread_cond_init(&m_SendCond, NULL);
	pthread_create(&m_hSendThread, NULL, _SendThreadProc, this);
	pthread_detach(m_hSendThread);
#else
	DWORD ThreadID;
	m_SendLock = CreateMutex(NULL, false, NULL);
	m_SendCond = CreateEvent(NULL, false, false, NULL);
	m_hSendThread = CreateThread(NULL, 0, _SendThreadProc, this, NULL, &ThreadID);
#endif
}

void CUdtCore::replyAccept(const char* pstrReply)
{
	m_szReplyfilepath = pstrReply;
}

void CUdtCore::stopTransfer(const int nType)
{
	// 1:stop recv; 2:stop send
	if (nType == 1)
	{
		m_bSend = false;
		#ifndef WIN32
			pthread_cond_signal(&m_SendCond);
			pthread_join(m_hSendThread, NULL);
			pthread_mutex_destroy(&m_SendLock);
			pthread_cond_destroy(&m_SendCond);
		#else
			SetEvent(m_SendCond);
			WaitForSingleObject(m_hSendThread, INFINITE);
			CloseHandle(m_hSendThread);
			CloseHandle(m_SendLock);
			CloseHandle(m_SendCond);
		#endif
	}
	else if (nType == 2)
	{
		m_bRecv = false;
		#ifndef WIN32
			pthread_cond_signal(&m_RecvCond);
			pthread_join(m_hRecvThread, NULL);
			pthread_mutex_destroy(&m_RecvLock);
			pthread_cond_destroy(&m_RecvCond);
		#else
			SetEvent(m_RecvCond);
			WaitForSingleObject(m_hRecvThread, INFINITE);
			CloseHandle(m_hRecvThread);
			CloseHandle(m_RecvLock);
			CloseHandle(m_RecvCond);
		#endif
	}
}

void CUdtCore::stopListen()
{
#ifndef WIN32
	pthread_mutex_lock(&m_AcceptLock);
	pthread_cond_signal(&m_AcceptCond);
	pthread_mutex_unlock(&m_AcceptLock);
#else
	SetEvent(m_AcceptCond);
#endif
}

#ifndef WIN32
void * CUdtCore::_ListenThreadProc(void * pParam)
#else
DWORD WINAPI CUdtCore::_ListenThreadProc(LPVOID pParam)
#endif
{
	CUdtCore * pThis = (CUdtCore *)pParam;

	struct sockaddr_in cli_add;
	int addrLen = sizeof(cli_add);
	memset(&cli_add, 0, addrLen);

	while (true)
	{
#ifndef WIN32
		pthread_mutex_lock(&pThis->m_AcceptLock);
		timeval now;
		timespec timeout;
		gettimeofday(&now, 0);
		timeout.tv_sec = now.tv_sec + 1;
		timeout.tv_nsec = now.tv_usec * 1000;

		int rc = pthread_cond_timedwait(&pThis->m_AcceptCond, &pThis->m_AcceptLock, &timeout);
		pthread_mutex_unlock(&pThis->m_AcceptLock);
		if (rc != ETIMEDOUT)
		{
			cout << "_ListenThreadProc timeout" << endl;
			break;
		}

#else
		if (WAIT_TIMEOUT != WaitForSingleObject(pThis->m_AcceptCond, 1000))
		{
			cout << "_ListenThreadProc timeout" << endl;
			break;
		}
#endif

		UDTSOCKET sockAccept = -1;
		// accept client connect
		if (CUDT::INVALID_SOCK == (sockAccept = CUDT::accept(pThis->m_sockListen, (sockaddr*)&cli_add, &addrLen)))
		{
			cout << "accept: " << CUDT::getlasterror().getErrorMessage() << endl;
			return 0;
		}

		// char clienthost[1025];		// NI_MAXHOST = 1025
		// char clientservice[32];		// NI_MAXSERV = 32
		//	sprintf(clienthost, "%s", inet_ntoa(cli_add.sin_addr));
		//	sprintf(clientservice, "%s", ntohs(cli_add.sin_port));

		_stClientSocket * pCXT = new _stClientSocket;
		memset(pCXT, 0, sizeof(_stClientSocket));
		pCXT->sock = sockAccept;
		pCXT->nPort = ntohs(cli_add.sin_port);

#ifdef WIN32
		sprintf_s(pCXT->strAddr, "%s", inet_ntoa(cli_add.sin_addr));
#else
		sprintf(pCXT->strAddr, "%s", inet_ntoa(cli_add.sin_addr));
#endif

		// save connext clientSocket
		pThis->m_pClientSocket = pCXT;

#ifndef WIN32
		pthread_mutex_init(&pThis->m_RecvLock, NULL);
		pthread_cond_init(&pThis->m_RecvCond, NULL);
		pthread_create(&pThis->m_hRecvThread, NULL, _RecvThreadProc, pThis);
		pthread_detach(pThis->m_hRecvThread);
#else
		DWORD ThreadID;
		pThis->m_RecvLock = CreateMutex(NULL, false, NULL);
		pThis->m_RecvCond = CreateEvent(NULL, false, false, NULL);
		pThis->m_hRecvThread = CreateThread(NULL, 0, _RecvThreadProc, pThis, 0, &ThreadID);
#endif
	}

	#ifndef WIN32
		return NULL;
	#else
		return 0;
	#endif
}

#ifndef WIN32
void * CUdtCore::_SendThreadProc(void * pParam)
#else
DWORD WINAPI CUdtCore::_SendThreadProc(LPVOID pParam)
#endif
{
	CUdtCore * pThis = (CUdtCore *)pParam;

	_stServerContext * cxt = pThis->m_pSrvCxt;
	int nLen = 0, nPos = 0, nFileCount = 0;
	int64_t nFileTotalSize = 0, nSendSize = 0, iLastPercent = 0;
	string szTmp, szFileName, szFilePath, szFinish = "FAIL", szError = "";
	vector<string> vecFileName;
	vector<string> vecDir;

	CUDT::startup();
	struct sockaddr_in srv_addr;
	int addr_len = sizeof(srv_addr);

	memset(&srv_addr, 0, addr_len);
	srv_addr.sin_family = AF_INET;
	srv_addr.sin_port = htons(cxt->nPort);
	srv_addr.sin_addr.s_addr = inet_addr(cxt->strAddr);

	// create socket
	UDTSOCKET fhandle = CUDT::socket(AF_INET, SOCK_STREAM, 0);
	pThis->m_pSrvCxt->sock = fhandle;

	// connect to server
	if (CUDT::ERROR_SOCK == CUDT::connect(fhandle, (struct sockaddr *)&srv_addr, addr_len))
		goto Loop;

	char Head[8];
	memset(Head, 0, 8);
	memcpy(Head, cxt->strXSR, 3);
	if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)Head, 3, 0))
		goto Loop;

	if (memcmp(Head, "TSR", 3) == 0)
	{
		// send text message
		szFileName = cxt->strHostName;
		nLen = szFileName.size();
		if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)&nLen, sizeof(int), 0))
			goto Loop;
		if (CUDT::ERROR_SOCK == CUDT::send(fhandle, szFileName.c_str(), nLen, 0))
			goto Loop;
		// send message size and information
		int nLen = cxt->vecArray[0].size();
		if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)&nLen, sizeof(int), 0))
			goto Loop;
		if (CUDT::ERROR_SOCK == CUDT::send(fhandle, cxt->vecArray[0].c_str(), nLen, 0))
			goto Loop;

		szFinish = "";
		goto Loop;
	}
	else if (memcmp(Head, "MSR", 3) == 0 || memcmp(Head, "FSR", 3) == 0)
	{
		vecFileName = cxt->vecArray;
		for (int i = 0; i < vecFileName.size(); i++)
		{
			fstream ifs(cxt->vecArray[i].c_str(), ios::in | ios::binary);
			ifs.seekg(0, ios::end);
			int64_t size = ifs.tellg();
			nFileTotalSize += size;
			nFileCount++;
			ifs.close();
		}
		if (memcmp(Head, "MSR", 3) == 0)
		{
			// send file total size
			if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)&nFileTotalSize, sizeof(nFileTotalSize), 0))
				goto Loop;
			// send file count
			if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)&nFileCount, sizeof(nFileCount), 0))
				goto Loop;
		}
	}
	else if (memcmp(Head, "DSR", 3) == 0)
	{
		pThis->SearchFileInDirectroy(cxt->vecArray[0], nFileTotalSize, vecFileName);
		// send file total size
		if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)&nFileTotalSize, sizeof(nFileTotalSize), 0))
			goto Loop;
	}

	// send file name,(filename\hostname\sendtype)
	szFilePath = cxt->vecArray[0];
	nPos = szFilePath.find_last_of('/');
	if (nPos < 0)
	{
		nPos = szFilePath.find_last_of("\\");
	}
	szFileName = szFilePath.substr(nPos+1);
	// add hostname and sendtype
	szTmp = szFileName + "\\" + cxt->strHostName;
	if (0 == strcmp("GENIETURBO", cxt->strSendtype))
	{
		szTmp += "\\";
		szTmp += cxt->strSendtype;
	}
	// send name information of the requested file
	nLen = szTmp.size();
	if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)&nLen, sizeof(int), 0))
		goto Loop;
	if (CUDT::ERROR_SOCK == CUDT::send(fhandle, szTmp.c_str(), nLen, 0))
		goto Loop;

	// recv accept response
	memset(Head, 0, 8);
	if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)Head, 4, 0))
		goto Loop;
	if (memcmp(Head, cxt->strXSP, 4) != 0)
	{
		szFinish = "REJECT";
		goto Loop;
	}

	// send folder name
	if (memcmp(cxt->strXSR, "DSR", 3) == 0)
	{
		for (int i = 0; i < vecFileName.size(); i++)
		{
			// D:\\Genie
			// D:\\Genie\\123.txt
			// D:\\Genie\\test\\123.txt
			nPos = szFileName.find_last_of('/');
			if (nPos < 0)
			{
				nPos = szFileName.find_last_of("\\");
			}
			string szDir = szFileName.substr(0, nPos);

			string szTmpForlder = vecFileName[i];
			nPos = szTmpForlder.find_last_of('/');
			if (nPos < 0)
			{
				nPos = szTmpForlder.find_last_of("\\");
			}
			szTmp = szTmpForlder.substr(0, nPos);
			nLen = szTmpForlder.size() - szDir.size();
			szTmp = szTmp.substr(nLen);
			for (int j = 0; j < vecDir.size(); j++)
			{
				if (vecDir[j].compare(szTmp) == 0)
				{
					break;
				}
			}
			vecDir.push_back(szTmp);
			// send file tage (DCR)
			memset(Head, 0, 8);
			memcpy(Head, "DCR", 3);
			if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)Head, 3, 0))
				goto Loop;

			// send name information of the requested file
			nLen = szTmp.size();
			if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)&nLen, sizeof(int), 0))
				goto Loop;
			if (CUDT::ERROR_SOCK == CUDT::send(fhandle, szTmp.c_str(), nLen, 0))
				goto Loop;
		}
	}

	pThis->m_bSend = true;
	for (int i = 0; i < vecFileName.size(); i++)
	{
		// send file tage
		memset(Head, 0, 8);
		memcpy(Head, cxt->strXCS, 3);
		if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)Head, 3, 0))
			goto Loop;

		szFilePath = cxt->vecArray[i];
		nPos = szFilePath.find_last_of('/');
		if (nPos < 0)
		{
			nPos = szFilePath.find_last_of("\\");
		}
		szFileName = szFilePath.substr(nPos+1);

		// send filename size and filename
		if (memcmp(cxt->strXSR, "FSR", 3) != 0)
		{
			int nLen = szFileName.size();
			if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)&nLen, sizeof(int), 0))
				goto Loop;
			if (CUDT::ERROR_SOCK == CUDT::send(fhandle, szFileName.c_str(), nLen, 0))
				goto Loop;
		}

		// open the file
		fstream ifs(szFilePath.c_str(), ios::in | ios::binary);
		ifs.seekg(0, ios::end);
		int64_t size = ifs.tellg();
		ifs.seekg(0, ios::beg);
		// send file size information
		if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)&size, sizeof(int64_t), 0))
			goto Loop;

		// send the file
		int64_t nOffset = 0;
		int64_t left = size;
		while (true)
		{
			CGuard::enterCS(pThis->m_SendLock);
			if (!pThis->m_bSend)
				goto Loop;
			CGuard::leaveCS(pThis->m_SendLock);

			// check network states
			//	UDTSTATUS states = CUDT::getStatus(fhandle);
			//	if (states != UDTSTATUS::CONNECTED)
			//		goto Loop;

			int64_t send = 0;
			if (left > 51200)
				send = CUDT::sendfile(fhandle, ifs, nOffset, 51200);
			else
				send = CUDT::sendfile(fhandle, ifs, nOffset, left);

			if (CUDT::ERROR_SOCK == send)
				goto Loop;

			left -= send;
			nOffset += send;
			nSendSize += send;
			int64_t iPercent = (nSendSize*100)/nFileTotalSize;
			if (iPercent == 1)
			{
				iPercent = 1;
			}
			if (iPercent > 100)
			{
				iPercent = iPercent;
			}
			if (iPercent != iLastPercent)
			{
				// recv response（FCS）
				iLastPercent = iPercent;
				memset(Head, 0, 8);
				if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)Head, 4, 0))
					goto Loop;
			}

			pThis->m_pCallBack->onSendTransfer(nFileTotalSize, nSendSize, szFileName.c_str());
			if (left <= 0)
				break;
		}
		ifs.close();
	}

	if (memcmp(cxt->strXSR, "MSR", 3) == 0 || memcmp(cxt->strXSR, "DSR", 3) == 0)
	{
		memset(Head, 0, 8);
		memcpy(Head, cxt->strXSF, 3);
		if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)Head, 3, 0))
			goto Loop;
	}

	szFinish = "SUCCESS";
	goto Loop;

	// goto loop for end
Loop:
	if (!szFinish.empty())
	{
		pThis->m_pCallBack->onSendFinished(szFinish.c_str());
	}

	CUDT::close(fhandle);
	delete cxt;

	#ifndef WIN32
		return NULL;
	#else
		return 0;
	#endif
}

#ifndef WIN32
void * CUdtCore::_RecvThreadProc(void * pParam)
#else
DWORD WINAPI CUdtCore::_RecvThreadProc(LPVOID pParam)
#endif
{
	CUdtCore * pThis = (CUdtCore *)pParam;

	UDTSOCKET fhandle = pThis->m_pClientSocket->sock;

	char Head[8];
	int nLen = 0;
	int nCount = 0;
	int64_t recvSize = 0;
	int64_t iLastPercent = 0;
	int64_t nFileTotalSize = 0, nRecvSize = 0;
	vector<string> vecFileName;
	string strReplyPath = "", szFinish = "FAIL", szError = "", szFilePath = "/mnt/sdcard/";

	while (true)
	{
		memset(Head, 0, 8);
		if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char *)Head, 3, 0))
			goto Loop;

		if (memcmp(Head,"TSR",3) == 0)
		{
			//////////////////////////////////////////////////////////////////////////
			// 1.	recv message response（TSR）

			// recv hostname
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&nLen, sizeof(nLen), 0))
				goto Loop;
			char * pstrHostName = new char[nLen+2];
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, pstrHostName, nLen, 0))
				goto Loop;
			pstrHostName[nLen] = '\0';

			// recv message
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&nLen, sizeof(nLen), 0))
				goto Loop;
			char * pstrMsg = new char[nLen+2];
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, pstrMsg, nLen, 0))
				goto Loop;
			pstrMsg[nLen] = '\0';

			// notify to up
			pThis->m_pCallBack->onRecvMessage((char*)pThis->m_pClientSocket->strAddr, pstrHostName, pstrMsg);

			szFinish = "";
			goto Loop;
		}
		else if (memcmp(Head,"FSR",3) == 0)
		{
			//////////////////////////////////////////////////////////////////////////
			// 2.	recv unifile response（FSR）

			// recv filename hostname sendtype
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&nLen, sizeof(nLen), 0))
				goto Loop;
			char * pstrFileName = new char[nLen+2];
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, pstrFileName, nLen, 0))
				goto Loop;
			pstrFileName[nLen] = '\0';

			string szTemp = pstrFileName;
			string szCloud, szDevName, szFileName, szFilePath;
			int nPos = szTemp.find_last_of('\\');
			if (nPos > 0)
			{
				int iFirstPos = szTemp.find_first_of('\\');
				if (nPos == iFirstPos)
				{
					szDevName = szTemp.substr(nPos+1);
					szFileName = szTemp.substr(0,nPos);
					szTemp = szTemp.substr(0,nPos);
				}
				else
				{
					szDevName = szTemp.substr(iFirstPos+1,nPos-iFirstPos-1);
					szFileName = szTemp.substr(0,iFirstPos);
					szCloud = szTemp.substr(nPos+1);
					szTemp = szTemp.substr(0,iFirstPos);
				}
			}
			else
			{
				goto Loop;
			}

			string szSendType = "UNKNOWN";
			if (!szCloud.empty())
			{
				szSendType = szCloud;
			}

			pThis->m_szReplyfilepath = "";
			pThis->m_pCallBack->onAccept((char*)pThis->m_pClientSocket->strAddr, szDevName.c_str(), szSendType.c_str(), szFileName.c_str(), 1);

			int nWaitTime = 0;
			while(nWaitTime < 31)
			{
				CGuard::enterCS(pThis->m_RecvLock);
				strReplyPath = pThis->m_szReplyfilepath;
				CGuard::leaveCS(pThis->m_RecvLock);
				if (!strReplyPath.empty())
				{
					cout << "getReply:" << strReplyPath << endl;
					break;
				}

				nWaitTime += 1;
				#ifndef WIN32
					sleep(1);
				#else
					Sleep(1000);
				#endif
			}

			// file transfer response（FSP）
			memset(Head, 0, 8);
			if (strReplyPath.compare("REJECT")==0)
				memcpy(Head, "FSP0", 3);
			else if (strReplyPath.compare("REJECTBUSY") == 0 || strReplyPath.empty())
				memcpy(Head, "FSP2", 3);
			else
				memcpy(Head, "FSP1", 3);

			if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)Head, 4, 0))
				goto Loop;
			if (strcmp(Head, "FSP1") != 0)
			{
				szFinish = "";
				goto Loop;
			}

			// file transfer（FCS）
			memset(Head, 0, 8);
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)Head, 3, 0))
				goto Loop;

			szFilePath = strReplyPath + szFileName;

			int64_t size;
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&size, sizeof(int64_t), 0))
				goto Loop;

			nFileTotalSize = size;
			//cout << "receive fileName and size:" << szFilePath << "-" << size << endl;

			pThis->m_bRecv = true;
			// receive the file
			fstream ofs(szFilePath.c_str(), ios::out | ios::binary | ios::trunc);
			int64_t offset = 0;
			int64_t left = size;
			while(true)
			{
				CGuard::enterCS(pThis->m_SendLock);
				if (!pThis->m_bRecv)
					break;
				CGuard::leaveCS(pThis->m_SendLock);

				int64_t recv = 0;
				if (left > 51200)
					recv = CUDT::recvfile(fhandle, ofs, offset, 51200);
				else
					recv = CUDT::recvfile(fhandle, ofs, offset, left);

				if (CUDT::ERROR_SOCK == recv)
					goto Loop;

				left -= recv;
				offset +=recv;

				// 发送文件接收进度
				int64_t iPercent = (offset*100)/size;
				if (iPercent == 1)
				{
					iPercent = 1;
				}
				if (iPercent > 100)
				{
					iPercent = iPercent;
				}
				if (iPercent != iLastPercent)
				{
					iLastPercent = iPercent;
					memset(Head, 0, 8);
					memcpy(Head, "FSC1", 4);
					if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)Head, 4, 0))
						goto Loop;
				}

				if (size-left > 0)
				{
					pThis->m_pCallBack->onRecvTransfer((long)size, size-left, szFileName.c_str());
				}

				if (nLen <= 0)
					break;
			}

			szFinish = "SUCCESS";
			goto Loop;
		}
		else if (memcmp(Head,"MSR",3) == 0)
		{
			//////////////////////////////////////////////////////////////////////////
			// 2.	recv multifile response（MSR）

			// recv filename hostname sendtype
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&nFileTotalSize, sizeof(nFileTotalSize), 0))
				goto Loop;
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&nCount, sizeof(nCount), 0))
				goto Loop;

			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&nLen, sizeof(nLen), 0))
				goto Loop;
			char * pstrFileName = new char[nLen+2];
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, pstrFileName, nLen, 0))
				goto Loop;
			pstrFileName[nLen] = '\0';

			string szTemp = pstrFileName;
			string szCloud, szDevName, szFileName, szFilePath;
			int nPos = szTemp.find_last_of('\\');
			if (nPos > 0)
			{
				int iFirstPos = szTemp.find_first_of('\\');
				if (nPos == iFirstPos)
				{
					szDevName = szTemp.substr(nPos+1);
					szFileName = szTemp.substr(0,nPos);
					szTemp = szTemp.substr(0,nPos);
				}
				else
				{
					szDevName = szTemp.substr(iFirstPos+1,nPos-iFirstPos-1);
					szFileName = szTemp.substr(0,iFirstPos);
					szCloud = szTemp.substr(nPos+1);
					szTemp = szTemp.substr(0,iFirstPos);
				}
			}
			else
			{
				goto Loop;
			}

			string szSendType = "UNKNOWN";
			if (!szCloud.empty())
			{
				szSendType = szCloud;
			}

			pThis->m_szReplyfilepath = "";
			pThis->m_pCallBack->onAccept((char*)pThis->m_pClientSocket->strAddr, szDevName.c_str(), szSendType.c_str(), szFileName.c_str(), 1);

			int nWaitTime = 0;
			while(nWaitTime < 31)
			{
				CGuard::enterCS(pThis->m_RecvLock);
				strReplyPath = pThis->m_szReplyfilepath;
				CGuard::leaveCS(pThis->m_RecvLock);
				if (!strReplyPath.empty())
				{
					cout << "getReply:" << strReplyPath << endl;
					break;
				}

				nWaitTime += 1;
#ifndef WIN32
				sleep(1);
#else
				Sleep(1000);
#endif
			}

			// file transfer response（MSP）
			memset(Head, 0, 8);
			if (strReplyPath.compare("REJECT")==0)
				memcpy(Head, "MSP0", 4);
			else if (strReplyPath.compare("REJECTBUSY") == 0 || strReplyPath.empty())
				memcpy(Head, "MSP2", 4);
			else
				memcpy(Head, "MSP1", 4);

			if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)Head, 4, 0))
				goto Loop;
			if (strcmp(Head, "MSP1") != 0)
			{
				szFinish = "";
				goto Loop;
			}
			pThis->m_bRecv = true;
		}
		else if (memcmp(Head,"MCS",3) == 0)
		{
			// file transfer（MCS）
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&nLen, sizeof(nLen), 0))
				goto Loop;
			char * pstrFileName = new char[nLen+2];
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, pstrFileName, nLen, 0))
				goto Loop;
			pstrFileName[nLen] = '\0';
			vecFileName.push_back(pstrFileName);

			int64_t size;
			// recv file size
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&size, sizeof(int64_t), 0))
				goto Loop;

			string szFileName = pstrFileName;
			string szFilePath = strReplyPath + szFileName;
			// receive the file
			fstream ofs(szFilePath.c_str(), ios::out | ios::binary | ios::trunc);
			int64_t offset = 0;
			int64_t left = size;
			while(true)
			{
				CGuard::enterCS(pThis->m_SendLock);
				if (!pThis->m_bRecv)
					goto Loop;
				CGuard::leaveCS(pThis->m_SendLock);

				int64_t recv = 0;
				if (left > 51200)
					recv = CUDT::recvfile(fhandle, ofs, offset, 51200);
				else
					recv = CUDT::recvfile(fhandle, ofs, offset, left);

				if (CUDT::ERROR_SOCK == recv)
					goto Loop;

				left -= recv;
				offset +=recv;
				nRecvSize += recv;
				// 发送文件接收进度
				int64_t iPercent = (nRecvSize*100)/nFileTotalSize;
				if (iPercent == 1)
				{
					iPercent = 1;
				}
				if (iPercent > 100)
				{
					iPercent = iPercent;
				}
				if (iPercent != iLastPercent)
				{
					iLastPercent = iPercent;
					memset(Head, 0, 8);
					memcpy(Head, "MSC1", 4);
					if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)Head, 4, 0))
						goto Loop;
				}

				if (size-left > 0)
				{
					pThis->m_pCallBack->onRecvTransfer((long)nFileTotalSize, nRecvSize, szFileName.c_str());
				}

				if (left <= 0)
					break;
			}
			ofs.close();
		}
		else if (memcmp(Head,"MSF",3) == 0)
		{
			//pThis->m_pCallBack->onRecvTransfer((long)nFileTotalSize, nRecvSize, szFileName);
			szFinish = "SUCCESS";
			goto Loop;
		}
		else if (memcmp(Head,"DSR",3) == 0)
		{
			//////////////////////////////////////////////////////////////////////////
			// 2.	recv multifile response（DSR）

			// recv filename hostname sendtype
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&nFileTotalSize, sizeof(nFileTotalSize), 0))
				goto Loop;
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&nLen, sizeof(nLen), 0))
				goto Loop;
			char * pstrFileName = new char[nLen+2];
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, pstrFileName, nLen, 0))
				goto Loop;
			pstrFileName[nLen] = '\0';

			string szTemp = pstrFileName;
			string szCloud, szDevName, szFileName, szFilePath;
			int nPos = szTemp.find_last_of('\\');
			if (nPos > 0)
			{
				int iFirstPos = szTemp.find_first_of('\\');
				if (nPos == iFirstPos)
				{
					szDevName = szTemp.substr(nPos+1);
					szFileName = szTemp.substr(0,nPos);
					szTemp = szTemp.substr(0,nPos);
				}
				else
				{
					szDevName = szTemp.substr(iFirstPos+1,nPos-iFirstPos-1);
					szFileName = szTemp.substr(0,iFirstPos);
					szCloud = szTemp.substr(nPos+1);
					szTemp = szTemp.substr(0,iFirstPos);
				}
			}
			else
			{
				goto Loop;
			}

			string szSendType = "UNKNOWN";
			if (!szCloud.empty())
			{
				szSendType = szCloud;
			}

			pThis->m_szReplyfilepath = "";
			pThis->m_pCallBack->onAccept((char*)pThis->m_pClientSocket->strAddr, szDevName.c_str(), szSendType.c_str(), szFileName.c_str(), 1);

			int nWaitTime = 0;
			while(nWaitTime < 31)
			{
				CGuard::enterCS(pThis->m_RecvLock);
				strReplyPath = pThis->m_szReplyfilepath;
				CGuard::leaveCS(pThis->m_RecvLock);
				if (!strReplyPath.empty())
				{
					cout << "getReply:" << strReplyPath << endl;
					break;
				}

				nWaitTime += 1;
#ifndef WIN32
				sleep(1);
#else
				Sleep(1000);
#endif
			}

			// file transfer response（MSP）
			memset(Head, 0, 8);
			if (strReplyPath.compare("REJECT")==0)
				memcpy(Head, "DSP0", 4);
			else if (strReplyPath.compare("REJECTBUSY") == 0 || strReplyPath.empty())
				memcpy(Head, "DSP2", 4);
			else
				memcpy(Head, "DSP1", 4);

			if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)Head, 4, 0))
				goto Loop;
			if (strcmp(Head, "DSP1") != 0)
			{
				szFinish = "";
				goto Loop;
			}
			pThis->m_bRecv = true;
		}
		else if (memcmp(Head,"DCR",3) == 0)
		{
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&nLen, sizeof(nLen), 0))
				goto Loop;
			char * pstrFileName = new char[nLen+2];
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, pstrFileName, nLen, 0))
				goto Loop;
			pstrFileName[nLen] = '\0';
			
			string szFolder = strReplyPath + pstrFileName;
			pThis->CreateDirectroy(szFolder);
		}
		else if (memcmp(Head,"DFS",3) == 0)
		{
			// file transfer（DFS）
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&nLen, sizeof(nLen), 0))
				goto Loop;
			char * pstrFileName = new char[nLen+2];
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, pstrFileName, nLen, 0))
				goto Loop;
			pstrFileName[nLen] = '\0';
			vecFileName.push_back(pstrFileName);

			int64_t size;
			// recv file size
			if (CUDT::ERROR_SOCK == CUDT::recv(fhandle, (char*)&size, sizeof(int64_t), 0))
				goto Loop;

			string szFileName = pstrFileName;
			string szFilePath = strReplyPath + szFileName;
			// receive the file
			fstream ofs(szFilePath.c_str(), ios::out | ios::binary | ios::trunc);
			int64_t offset = 0;
			int64_t left = size;
			while(true)
			{
				CGuard::enterCS(pThis->m_SendLock);
				if (!pThis->m_bRecv)
					goto Loop;
				CGuard::leaveCS(pThis->m_SendLock);

				int64_t recv = 0;
				if (left > 51200)
					recv = CUDT::recvfile(fhandle, ofs, offset, 51200);
				else
					recv = CUDT::recvfile(fhandle, ofs, offset, left);

				if (CUDT::ERROR_SOCK == recv)
					goto Loop;

				left -= recv;
				offset +=recv;
				nRecvSize += recv;
				// 发送文件接收进度
				int64_t iPercent = (nRecvSize*100)/nFileTotalSize;
				if (iPercent == 1)
				{
					iPercent = 1;
				}
				if (iPercent > 100)
				{
					iPercent = iPercent;
				}
				if (iPercent != iLastPercent)
				{
					iLastPercent = iPercent;
					memset(Head, 0, 8);
					memcpy(Head, "DSC1", 4);
					if (CUDT::ERROR_SOCK == CUDT::send(fhandle, (char*)Head, 4, 0))
						goto Loop;
				}

				if (size-left > 0)
				{
					pThis->m_pCallBack->onRecvTransfer((long)nFileTotalSize, nRecvSize, szFileName.c_str());
				}

				if (left <= 0)
					break;
			}
			ofs.close();
		}
		else if (memcmp(Head,"DSF",3) == 0)
		{
			//pThis->m_pCallBack->onRecvTransfer((long)nFileTotalSize, nRecvSize, szFileName);
			szFinish = "SUCCESS";
			goto Loop;
		}
		else
			goto Loop;
	}

	// goto loop for end
Loop:
	// SUCCESS, FAIL
	if (!szFinish.empty())
	{
		pThis->m_pCallBack->onRecvFinished(szFinish.c_str());
	}

	CUDT::close(fhandle);

	#ifndef WIN32
		return NULL;
	#else
		return 0;
	#endif
}


//////////////////////////////////////////////////////////////////////////
// private method
void CUdtCore::SearchFileInDirectroy(const std::string & szPath, int64_t & nTotalSize, std::vector<std::string> & outList)
{
	char strPath[256] = {0};
	memcpy(strPath, szPath.c_str(), 256);

#ifdef WIN32
	WIN32_FIND_DATA sFindFileData = {0};
	HANDLE hFind = INVALID_HANDLE_VALUE;
	hFind = FindFirstFile(strPath, &sFindFileData);
	if (sFindFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
	{
		if ('\\' == strPath[strlen(strPath)-1])
		{
			strcat_s(strPath, ("\\*.*"));
		}
		else
			strcat_s(strPath, ("\\*.*"));

		hFind = INVALID_HANDLE_VALUE;
		if (INVALID_HANDLE_VALUE != (hFind = FindFirstFile(strPath, &sFindFileData)))
		{
			do 
			{
				if (sFindFileData.cFileName[0] == '.')
					continue;
				sprintf_s(strPath, "%s\\%s", szPath.c_str(), sFindFileData.cFileName);
				if (sFindFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
				{
					SearchFileInDirectroy(strPath, nTotalSize, outList);
				}
				else
				{
					fstream ifs(strPath, ios::in | ios::binary);
					ifs.seekg(0, ios::end);
					int64_t size = ifs.tellg();
					nTotalSize += size;
					outList.push_back(strPath);
				}
			} while (FindNextFile(hFind, &sFindFileData));

			FindClose(hFind);
			hFind = INVALID_HANDLE_VALUE;
		}
	}
	else
	{
		fstream ifs(strPath, ios::in | ios::binary);
		ifs.seekg(0, ios::end);
		int64_t size = ifs.tellg();
		nTotalSize += size;
		outList.push_back(strPath);

		FindClose(hFind);
		hFind = INVALID_HANDLE_VALUE;
	}
#else
	DIR * pDir;
	if ((pDir = opendir(strPath)) == NULL)
	{
		fstream ifs(szPath, ios::in | ios::binary);
		ifs.seekg(0, ios::end);
		int64_t size = ifs.tellg();
		nTotalSize += size;
		outList.push_back(strFileName);
		return ;
	}

	struct stat dirInfo;
	struct dirent * pDT;
	char strFileName[256];
	memset(strFileName, 0, sizeof(strFileName));

	while ((pDT = readdir(pDir)))
	{
		if (strcmp(pDT->d_name, ".") == 0 || strcmp(pDT->d_name, "..") == 0)
			continue;
		else
		{
			sprintf(strFileName, "%s/%s", strPath, pDT->d_name);
			struct stat buf;
			if (lstat(strFileName, &buf) >= 0 && S_ISDIR(buf.st_mode))
			{
				searchFileInDirectroy(strFileName, nTotalSize, outList);
			}
			else
			{
				fstream ifs(szPath, ios::in | ios::binary);
				ifs.seekg(0, ios::end);
				int64_t size = ifs.tellg();
				nTotalSize += size;
				outList.push_back(strFileName);
			}
		}
		memset(strFileName, 0, 256);
	}
	closedir(pDir);
#endif
}

void CUdtCore::CreateDirectroy(const std::string & szPath)
{
	int nLen = 0;
	string szFolder = "";
	string szTmp = szPath;
	while (true)
	{
		int nPos = szTmp.find_first_of('\\');
		if (nPos >= 0)
		{
			nLen += nPos;
			szTmp = szTmp.substr(nPos+1);
			szFolder = szPath.substr(0, nLen);
			nPos = szTmp.find_first_of('\\');
			if (nPos >= 0)
			{
				nLen = nLen + 1 + nPos;
				szTmp = szTmp.substr(nPos);
				szFolder = szPath.substr(0, nLen);
#ifdef WIN32
				if (_access(szFolder.c_str(), 0))
					_mkdir(szFolder.c_str());
#else
				if (access(szFolder.c_str(), F_OK) != 0)
					mkdir(szFolder.c_str(),770);
#endif
			}
			else
				break;
		}
		else
			break;
	}
}
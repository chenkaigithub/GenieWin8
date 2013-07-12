#ifndef __TURBOTRANSFER_H__
#define __TURBOTRANSFER_H__

#include <iostream>
#include <string>

//typedef void (*Dispatch)(long * pAddBuf, int nAddNum, long * pDelBuf, int nDelNum);

typedef void (WINAPI *PFOnAccept)(const char* pstrIpAddr, const char* pstrHostName, const char* pstrSendType, const char* pstrFileName, int nFileCount);
typedef void (WINAPI *PFOnTransfer)(const int nType, const long nFileTotalSize, const long nCurrent, const char* pstrFileName);
typedef void (WINAPI *PFOnFinished)(const int nType, const char* pstrText);


extern "C" __declspec(dllexport) long Init(PFOnAccept pFun1, PFOnTransfer pFun2, PFOnFinished pFun3);
extern "C" __declspec(dllexport) void ListenSendFile(long ptr, int nPort);
extern "C" __declspec(dllexport) void SendText(long ptr, int nPort, char * strAddr, char * strHostname, char * strSendtype, char * strMessage);
extern "C" __declspec(dllexport) void SendFile(long ptr, int nPort, char * strAddr, char * strHostName, char * strSendtype, char * strFileName);
extern "C" __declspec(dllexport) void SendMultiFile(long ptr, int nPort, char * strAddr, char * strHostName, char * strSendtype, char * strFolderName[]);
extern "C" __declspec(dllexport) void SendFolder(long ptr, int nPort, char * strAddr, char * strHostName, char * strSendtype, char * strFolderName);
extern "C" __declspec(dllexport) void StopTransfer(long ptr, int nType);
extern "C" __declspec(dllexport) void StopListen(long ptr);

#endif	// __TURBOTRANSFER_H__
/*****************************************************************************
Copyright (c) 2001 - 2009, The Board of Trustees of the University of Illinois.
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

* Redistributions of source code must retain the above
  copyright notice, this list of conditions and the
  following disclaimer.

* Redistributions in binary form must reproduce the
  above copyright notice, this list of conditions
  and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the University of Illinois
  nor the names of its contributors may be used to
  endorse or promote products derived from this
  software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*****************************************************************************/

/*****************************************************************************
written by
   Yunhong Gu, last updated 08/01/2009
*****************************************************************************/

#ifndef __UDT_COMMON_H__
#define __UDT_COMMON_H__


#ifndef WIN32
   #include <sys/time.h>
   #include <sys/types.h>
   #include <sys/socket.h>
   #include <sys/uio.h>
   #include <pthread.h>
	#include <netinet/in.h>
	#include <cstring>
	#include <cerrno>
#else
	#include <winsock2.h>
	#include <ws2tcpip.h>
	#include <windows.h>
	#ifdef __MINGW__
      #include <ws2tcpip.h>
   #endif
	#ifdef LEGACY_WIN32
	#include <wspiapi.h>
	#endif
#endif

#include <sys/stat.h>
#include <cstdlib>
#include <fstream>
#include <string>
#include <vector>
#include <set>


////////////////////////////////////////////////////////////////////////////////

//if compiling on VC6.0 or pre-WindowsXP systems
//use -DLEGACY_WIN32

//if compiling with MinGW, it only works on XP or above
//use -D_WIN32_WINNT=0x0501


#ifdef WIN32
   #ifndef __MINGW__
      // Explicitly define 32-bit and 64-bit numbers
      typedef __int32 int32_t;
      typedef __int64 int64_t;
      typedef unsigned __int32 uint32_t;
      #ifndef LEGACY_WIN32
         typedef unsigned __int64 uint64_t;
      #else
         // VC 6.0 does not support unsigned __int64: may cause potential problems.
         typedef __int64 uint64_t;
      #endif
   #endif
#endif

#define NO_BUSY_WAITING

#ifdef WIN32
   #ifndef __MINGW__
      typedef SOCKET UDPSOCKET;
   #else
      typedef int UDPSOCKET;
   #endif
#else
   typedef int UDPSOCKET;
#endif


typedef int UDTSOCKET;

typedef std::set<UDTSOCKET> ud_set;
#define UD_CLR(u, uset) ((uset)->erase(u))
#define UD_ISSET(u, uset) ((uset)->find(u) != (uset)->end())
#define UD_SET(u, uset) ((uset)->insert(u))
#define UD_ZERO(uset) ((uset)->clear())


#ifdef WIN32
   // Windows compability
   typedef HANDLE pthread_t;
   typedef HANDLE pthread_mutex_t;
   typedef HANDLE pthread_cond_t;
   typedef DWORD pthread_key_t;
#endif


////////////////////////////////////////////////////////////////////////////////

enum UDTOpt
{
   UDT_MSS,             // the Maximum Transfer Unit
   UDT_SNDSYN,          // if sending is blocking
   UDT_RCVSYN,          // if receiving is blocking
   UDT_CC,              // custom congestion control algorithm
   UDT_FC,		// Flight flag size (window size)
   UDT_SNDBUF,          // maximum buffer in sending queue
   UDT_RCVBUF,          // UDT receiving buffer size
   UDT_LINGER,          // waiting for unsent data when closing
   UDP_SNDBUF,          // UDP sending buffer size
   UDP_RCVBUF,          // UDP receiving buffer size
   UDT_MAXMSG,          // maximum datagram message size
   UDT_MSGTTL,          // time-to-live of a datagram message
   UDT_RENDEZVOUS,      // rendezvous connection mode
   UDT_SNDTIMEO,        // send() timeout
   UDT_RCVTIMEO,        // recv() timeout
   UDT_REUSEADDR,	// reuse an existing port or create a new one
   UDT_MAXBW		// maximum bandwidth (bytes per second) that the connection can use
};

////////////////////////////////////////////////////////////////////////////////

struct CPerfMon
{
   // global measurements
   int64_t msTimeStamp;                 // time since the UDT entity is started, in milliseconds
   int64_t pktSentTotal;                // total number of sent data packets, including retransmissions
   int64_t pktRecvTotal;                // total number of received packets
   int pktSndLossTotal;                 // total number of lost packets (sender side)
   int pktRcvLossTotal;                 // total number of lost packets (receiver side)
   int pktRetransTotal;                 // total number of retransmitted packets
   int pktSentACKTotal;                 // total number of sent ACK packets
   int pktRecvACKTotal;                 // total number of received ACK packets
   int pktSentNAKTotal;                 // total number of sent NAK packets
   int pktRecvNAKTotal;                 // total number of received NAK packets
   int64_t usSndDurationTotal;		// total time duration when UDT is sending data (idle time exclusive)

   // local measurements
   int64_t pktSent;                     // number of sent data packets, including retransmissions
   int64_t pktRecv;                     // number of received packets
   int pktSndLoss;                      // number of lost packets (sender side)
   int pktRcvLoss;                      // number of lost packets (receiver side)
   int pktRetrans;                      // number of retransmitted packets
   int pktSentACK;                      // number of sent ACK packets
   int pktRecvACK;                      // number of received ACK packets
   int pktSentNAK;                      // number of sent NAK packets
   int pktRecvNAK;                      // number of received NAK packets
   double mbpsSendRate;                 // sending rate in Mb/s
   double mbpsRecvRate;                 // receiving rate in Mb/s
   int64_t usSndDuration;		// busy sending time (i.e., idle time exclusive)

   // instant measurements
   double usPktSndPeriod;               // packet sending period, in microseconds
   int pktFlowWindow;                   // flow window size, in number of packets
   int pktCongestionWindow;             // congestion window size, in number of packets
   int pktFlightSize;                   // number of packets on flight
   double msRTT;                        // RTT, in milliseconds
   double mbpsBandwidth;                // estimated bandwidth, in Mb/s
   int byteAvailSndBuf;                 // available UDT sender buffer size
   int byteAvailRcvBuf;                 // available UDT receiver buffer size
};


////////////////////////////////////////////////////////////////////////////////

class CTimer
{
public:
   CTimer();
   ~CTimer();

public:

      // Functionality:
      //    Sleep for "interval" CCs.
      // Parameters:
      //    0) [in] interval: CCs to sleep.
      // Returned value:
      //    None.

   void sleep(const uint64_t& interval);

      // Functionality:
      //    Seelp until CC "nexttime".
      // Parameters:
      //    0) [in] nexttime: next time the caller is waken up.
      // Returned value:
      //    None.

   void sleepto(const uint64_t& nexttime);

      // Functionality:
      //    Stop the sleep() or sleepto() methods.
      // Parameters:
      //    None.
      // Returned value:
      //    None.

   void interrupt();

      // Functionality:
      //    trigger the clock for a tick, for better granuality in no_busy_waiting timer.
      // Parameters:
      //    None.
      // Returned value:
      //    None.

   void tick();

public:

      // Functionality:
      //    Read the CPU clock cycle into x.
      // Parameters:
      //    0) [out] x: to record cpu clock cycles.
      // Returned value:
      //    None.

   static void rdtsc(uint64_t &x);

      // Functionality:
      //    return the CPU frequency.
      // Parameters:
      //    None.
      // Returned value:
      //    CPU frequency.

   static uint64_t getCPUFrequency();

      // Functionality:
      //    check the current time, 64bit, in microseconds.
      // Parameters:
      //    None.
      // Returned value:
      //    current time in microseconds.

   static uint64_t getTime();

      // Functionality:
      //    trigger an event such as new connection, close, new data, etc. for "select" call.
      // Parameters:
      //    None.
      // Returned value:
      //    None.

   static void triggerEvent();

      // Functionality:
      //    wait for an event to br triggered by "triggerEvent".
      // Parameters:
      //    None.
      // Returned value:
      //    None.

   static void waitForEvent();

private:
   uint64_t m_ullSchedTime;             // next schedulled time

   pthread_cond_t m_TickCond;
   pthread_mutex_t m_TickLock;

   static pthread_cond_t m_EventCond;
   static pthread_mutex_t m_EventLock;

private:
   static uint64_t s_ullCPUFrequency;	// CPU frequency : clock cycles per microsecond
   static uint64_t readCPUFrequency();
};

////////////////////////////////////////////////////////////////////////////////

class CGuard
{
public:
   CGuard(pthread_mutex_t& lock);
   ~CGuard();

   static void enterCS(pthread_mutex_t& lock);
   static void leaveCS(pthread_mutex_t& lock);

private:
   pthread_mutex_t& m_Mutex;            // Alias name of the mutex to be protected
   int m_iLocked;                       // Locking status

   CGuard& operator=(const CGuard&);
};


////////////////////////////////////////////////////////////////////////////////

class CUDTException
{
public:
	//CUDTException(int major = 0, int minor = 0, int err = -1);
	CUDTException(int major, int minor, int err);
	CUDTException(const CUDTException& e);
	virtual ~CUDTException();

	// Functionality:
	//    Get the description of the exception.
	// Parameters:
	//    None.
	// Returned value:
	//    Text message for the exception description.

	virtual const char* getErrorMessage();

	// Functionality:
	//    Get the system errno for the exception.
	// Parameters:
	//    None.
	// Returned value:
	//    errno.

	virtual const int getErrorCode() const;

	// Functionality:
	//    Clear the error code.
	// Parameters:
	//    None.
	// Returned value:
	//    None.

	virtual void clear();

private:
	int m_iMajor;        // major exception categories

	// 0: correct condition
	// 1: network setup exception
	// 2: network connection broken
	// 3: memory exception
	// 4: file exception
	// 5: method not supported
	// 6+: undefined error

	int m_iMinor;		// for specific error reasons
	int m_iErrno;		// errno returned by the system if there is any
	//char * m_strMsg;
	std::string m_strMsg;	// text error message

	//std::string m_strAPI;	// the name of UDT function that returns the error
	//std::string m_strDebug;	// debug information, set to the original place that causes the error

public: // Error Code
	static const int SUCCESS;
	static const int ECONNSETUP;
	static const int ENOSERVER;
	static const int ECONNREJ;
	static const int ESOCKFAIL;
	static const int ESECFAIL;
	static const int ECONNFAIL;
	static const int ECONNLOST;
	static const int ENOCONN;
	static const int ERESOURCE;
	static const int ETHREAD;
	static const int ENOBUF;
	static const int EFILE;
	static const int EINVRDOFF;
	static const int ERDPERM;
	static const int EINVWROFF;
	static const int EWRPERM;
	static const int EINVOP;
	static const int EBOUNDSOCK;
	static const int ECONNSOCK;
	static const int EINVPARAM;
	static const int EINVSOCK;
	static const int EUNBOUNDSOCK;
	static const int ENOLISTEN;
	static const int ERDVNOSERV;
	static const int ERDVUNBOUND;
	static const int ESTREAMILL;
	static const int EDGRAMILL;
	static const int EDUPLISTEN;
	static const int ELARGEMSG;
	static const int EASYNCFAIL;
	static const int EASYNCSND;
	static const int EASYNCRCV;
	static const int EUNKNOWN;
};


////////////////////////////////////////////////////////////////////////////////

// UDT Sequence Number 0 - (2^31 - 1)

// seqcmp: compare two seq#, considering the wraping
// seqlen: length from the 1st to the 2nd seq#, including both
// seqoff: offset from the 2nd to the 1st seq#
// incseq: increase the seq# by 1
// decseq: decrease the seq# by 1
// incseq: increase the seq# by a given offset

class CSeqNo
{
public:
   inline static const int seqcmp(const int32_t& seq1, const int32_t& seq2)
   {return (abs(seq1 - seq2) < m_iSeqNoTH) ? (seq1 - seq2) : (seq2 - seq1);}

   inline static const int seqlen(const int32_t& seq1, const int32_t& seq2)
   {return (seq1 <= seq2) ? (seq2 - seq1 + 1) : (seq2 - seq1 + m_iMaxSeqNo + 2);}

   inline static const int seqoff(const int32_t& seq1, const int32_t& seq2)
   {
      if (abs(seq1 - seq2) < m_iSeqNoTH)
         return seq2 - seq1;

      if (seq1 < seq2)
         return seq2 - seq1 - m_iMaxSeqNo - 1;

      return seq2 - seq1 + m_iMaxSeqNo + 1;
   }

   inline static const int32_t incseq(const int32_t seq)
   {return (seq == m_iMaxSeqNo) ? 0 : seq + 1;}

   inline static const int32_t decseq(const int32_t& seq)
   {return (seq == 0) ? m_iMaxSeqNo : seq - 1;}

   inline static const int32_t incseq(const int32_t& seq, const int32_t& inc)
   {return (m_iMaxSeqNo - seq >= inc) ? seq + inc : seq - m_iMaxSeqNo + inc - 1;}

public:
   static const int32_t m_iSeqNoTH;             // threshold for comparing seq. no.
   static const int32_t m_iMaxSeqNo;            // maximum sequence number used in UDT
};

////////////////////////////////////////////////////////////////////////////////

// UDT ACK Sub-sequence Number: 0 - (2^31 - 1)

class CAckNo
{
public:
   inline static const int32_t incack(const int32_t& ackno)
   {return (ackno == m_iMaxAckSeqNo) ? 0 : ackno + 1;}

public:
   static const int32_t m_iMaxAckSeqNo;         // maximum ACK sub-sequence number used in UDT
};

////////////////////////////////////////////////////////////////////////////////

// UDT Message Number: 0 - (2^29 - 1)

class CMsgNo
{
public:
   inline static const int msgcmp(const int32_t& msgno1, const int32_t& msgno2)
   {return (abs(msgno1 - msgno2) < m_iMsgNoTH) ? (msgno1 - msgno2) : (msgno2 - msgno1);}

   inline static const int msglen(const int32_t& msgno1, const int32_t& msgno2)
   {return (msgno1 <= msgno2) ? (msgno2 - msgno1 + 1) : (msgno2 - msgno1 + m_iMaxMsgNo + 2);}

   inline static const int msgoff(const int32_t& msgno1, const int32_t& msgno2)
   {
      if (abs(msgno1 - msgno2) < m_iMsgNoTH)
         return msgno2 - msgno1;

      if (msgno1 < msgno2)
         return msgno2 - msgno1 - m_iMaxMsgNo - 1;

      return msgno2 - msgno1 + m_iMaxMsgNo + 1;
   }

   inline static const int32_t incmsg(const int32_t& msgno)
   {return (msgno == m_iMaxMsgNo) ? 0 : msgno + 1;}

public:
   static const int32_t m_iMsgNoTH;             // threshold for comparing msg. no.
   static const int32_t m_iMaxMsgNo;            // maximum message number used in UDT
};

////////////////////////////////////////////////////////////////////////////////

struct CIPAddress
{
   static bool ipcmp(const sockaddr* addr1, const sockaddr* addr2, const int& ver = AF_INET);
   static void ntop(const sockaddr* addr, uint32_t ip[4], const int& ver = AF_INET);
   static void pton(sockaddr* addr, const uint32_t ip[4], const int& ver = AF_INET);
};

////////////////////////////////////////////////////////////////////////////////

struct CMD5
{
   static void compute(const char* input, unsigned char result[16]);
};


#endif

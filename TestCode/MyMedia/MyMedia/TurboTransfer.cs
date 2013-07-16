using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MyMedia
{
    public delegate void PFOnRecvMsg(StringBuilder strIpAddr, StringBuilder strHostName, StringBuilder strMsg);
    public delegate void PFOnAccept(StringBuilder strIpAddr, StringBuilder strHostName, StringBuilder strSendType, StringBuilder strFileName, int nFileCount);
    public delegate void PFOnTransfer(int nType, Int32 nFileTotalSize, Int32 nCurrent, StringBuilder strFileName);
    public delegate void PFOnFineshed(int nType, StringBuilder strText);

    public class RefComm
    {
        // 初始化，注册回调函数、用于接收DLL的回调方法，
        // Init()函数的返回值保存好、给下面几个方法第一个参数IntPtr ptr使用
        [DllImport("F:\\GithubCode\\GenieWin8\\TestCode\\MyMedia\\MyMedia\\bin\\Debug\\TurboTransfer.dll", EntryPoint = "Init", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Init(PFOnAccept Fun1, PFOnTransfer Fun2, PFOnFineshed Fun3, PFOnRecvMsg Fun4);

        // 监听端口、接收客服端发送文件请求
        [DllImport("TurboTransfer.dll", EntryPoint = "ListenSendFile", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ListenSendFile(IntPtr ptr, int nPort);

        // 发送文本消息，ptr：Init函数的返回值， nPort：目标设备接收文件的监听端口，默认是7777， strAddr：目标设备的网络IP地址，
        // strHostName：本机的主机名称，strSendType：网络映射模块发送文件"GENIEMAP" 文件传输模块"GENIETURBO", strMsg：发送的消息内容
        [DllImport("TurboTransfer.dll", EntryPoint = "SendText", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendText(IntPtr ptr, int nPort, string strAddr, string strHostName, string strSendType, string strMsg);

        // 发送文本消息，ptr：Init函数的返回值， nPort：目标设备接收文件的监听端口，默认是7777， strAddr：目标设备的网络IP地址，
        // strHostName：本机的主机名称，strSendType：网络映射模块发送文件"GENIEMAP" 文件传输模块"GENIETURBO", strFileNameArr：文件名称字符数组， nArrayCount：数组个数
        [DllImport("TurboTransfer.dll", EntryPoint = "SendFiles", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendFiles(IntPtr ptr, int nPort, string strAddr, string strHostName, string strSendType, [MarshalAs(UnmanagedType.LPArray)] string[] strFileNameArr, int nArrayCount);

        // 发送文本消息，ptr：Init函数的返回值， nPort：目标设备接收文件的监听端口，默认是7777， strAddr：目标设备的网络IP地址，
        // strHostName：本机的主机名称，strSendType：网络映射模块发送文件"GENIEMAP" 文件传输模块"GENIETURBO", strFolderName：文件夹名称， nArrayCount：数组个数
        [DllImport("TurboTransfer.dll", EntryPoint = "SendFolder", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendFolder(IntPtr ptr, int nPort, string strAddr, string strHostName, string strSendType, string strFolderName);

        // nType, 1：停止发送文件， 2：停止接收文件
        [DllImport("TurboTransfer.dll", EntryPoint = "StopTransfer", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopTransfer(IntPtr ptr, int nType);

        // 停止监听本地端口
        [DllImport("TurboTransfer.dll", EntryPoint = "StopListen", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopListen(IntPtr ptr);

        // 收到OnAccept回调函数的通知后、30秒内调用此方法，接收文件给一个存储路径strReply = "D:\\Genie", 拒绝接收给strReply = "REJECT", 繁忙strReply = "REJECTBUSY"
        [DllImport("TurboTransfer.dll", EntryPoint = "ReplyAccept", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReplyAccept(IntPtr ptr, string strReply);

        public static void OnRecvMsg(StringBuilder strIpAddr, StringBuilder strHostName, StringBuilder strMsg)
        {
            System.Diagnostics.Debug.WriteLine("Recv msg:" + strMsg);
        }

        // 有文件发送到达、DLL回调此方法。 strIpAddr：对方IP，strHostName：对方主机名称，strSendType：对方发送类型，strFileName：文件名称或者文件夹名称，nFileCount：文件个数
        public static void OnAccept(StringBuilder strIpAddr, StringBuilder strHostName, StringBuilder strSendType, StringBuilder strFileName, int nFileCount)
        {
            System.Diagnostics.Debug.WriteLine("Recv file name:" + strFileName);
        }

        // 进行传输时，DLL回调此方法。nType， 1：正在发送 2：正在接收；nFileTotalSize：文件总字节数； nCurrent：当前接收或者发送字节数；strFileName：当前接收或者发送文件名称
        public static void OnTransfer(int nType, Int32 nFileTotalSize, Int32 nCurrent, StringBuilder strFileName)
        {
            System.Diagnostics.Debug.WriteLine("FileTotalSize:" + nFileTotalSize + "nCurrent:" + nCurrent + strFileName);
        }

        // nType   1：发送结束消息，2：接收结束消息
        // strText  发送成功："SUCCESS", 发送失败："FAIL", 拒绝接收："REJECT", 繁忙："REJECTBUSY" 
        public static void OnFinished(int nType, StringBuilder strText)
        {
            System.Diagnostics.Debug.WriteLine(strText);
        }
    }
}
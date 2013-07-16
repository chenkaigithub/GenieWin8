using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace udtTest
{
    public delegate void PFOnAccept(StringBuilder strIpAddr, StringBuilder strHostName, StringBuilder strSendType, StringBuilder strFileName, int nFileCount);
    public delegate void PFOnTransfer(int nType, Int32 nFileTotalSize, Int32 nCurrent, StringBuilder strFileName);
    public delegate void PFOnFineshed(int nType, StringBuilder strText);

    public class RefComm
    {
        [DllImport("TurboTransfer.dll", EntryPoint = "Init", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Init(PFOnAccept Fun1, PFOnTransfer Fun2, PFOnFineshed Fun3);

        [DllImport("TurboTransfer.dll", EntryPoint = "ListenSendFile", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ListenSendFile(IntPtr ptr, int nPort);

        [DllImport("TurboTransfer.dll", EntryPoint = "SendText", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendText(IntPtr ptr, int nPort, StringBuilder strAddr, StringBuilder strHostName, StringBuilder strSendType, StringBuilder strMsg);

        [DllImport("TurboTransfer.dll", EntryPoint = "SendFile", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendFile(IntPtr ptr, int nPort, string strAddr, string strHostName, string strSendType, string strFileName);

        [DllImport("TurboTransfer.dll", EntryPoint = "SendFolder", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendFolder(IntPtr ptr, int nPort, string strAddr, string strHostName, string strSendType, string strFolderName);

        [DllImport("TurboTransfer.dll", EntryPoint = "StopTransfer", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopTransfer(IntPtr ptr, int nType);

        [DllImport("TurboTransfer.dll", EntryPoint = "StopListen", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void StopListen(IntPtr ptr);


        public static void OnAccept(StringBuilder strIpAddr, StringBuilder strHostName, StringBuilder strSendType, StringBuilder strFileName, int nFileCount)
        {
            Console.WriteLine("Recv file name:" + strFileName);
        }
        public static void OnTransfer(int nType, Int32 nFileTotalSize, Int32 nCurrent, StringBuilder strFileName)
        {
            Console.WriteLine("FileTotalSize:" + nFileTotalSize + "nCurrent:" + nCurrent + strFileName);
        }
        public static void OnFinished(int nType, StringBuilder strText)
        {
            Console.WriteLine(strText);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PFOnAccept Fun1 = new PFOnAccept(RefComm.OnAccept);
            PFOnTransfer Fun2 = new PFOnTransfer(RefComm.OnTransfer);
            PFOnFineshed Fun3 = new PFOnFineshed(RefComm.OnFinished);

            IntPtr ptr = RefComm.Init(Fun1, Fun2, Fun3);

            RefComm.ListenSendFile(ptr, 7777);

            StringBuilder ip = new StringBuilder("172.16.0.47");
           // StringBuilder ip = new StringBuilder("192.168.0.34");
            StringBuilder host = new StringBuilder("WIN8");
            StringBuilder text = new StringBuilder("message");
            StringBuilder type = new StringBuilder("GENIEMAP");

            RefComm.SendText(ptr, 7777, ip, host, type, text);

            while (true)
            {
                Console.WriteLine("wait");
            }
        }
    }
}

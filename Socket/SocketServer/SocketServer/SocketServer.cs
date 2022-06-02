using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    internal class SocketServer
    {
        public static Socket StockReplyListener = null;
        public static bool StockReplyListenFlag = false;
        #region static 啟動接收監聽服務 (port:8885) StartListening()
        public static void StartListening()
        {
            IPHostEntry IPLocal = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = GetLocalIPAddress();
            int listenPort = 8885; //Listen Port

           
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, listenPort);

            try
            {
                if (StockReplyListener == null) //若已啟動，不可重新啟動
                {
                    //建立Listener
                    StockReplyListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    StockReplyListener.Bind(localEndPoint);
                    StockReplyListener.Listen(1000);


                    string message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                    message = message + "啟動監聽服務 (port:" + localEndPoint.ToString() + ") StartListening()" + "\r\n";
                    FrmMain.DGVQueue.Enqueue(message);
                    if (FrmMain.DGVQueueEventFlag)
                    {
                        FrmMain.DGVQueueEventFlag = false;
                        FrmMain.DGVQueueEvent.Set();
                    }
                    StockReplyListenFlag= true;  //服務是否已啟動

                    //開始接受連線
                    while (StockReplyListenFlag)
                    {
                        Socket mySocket = StockReplyListener.Accept();
                        SocketClient client = new SocketClient(mySocket);
                    }
                }
            }
            catch (Exception exc)
            {

                string message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                message = message + "啟動監聽服務 (port:" + listenPort.ToString() + ") StartErrorListening()：" + exc.Message + "\r\n";
                message = message + exc.StackTrace;
                FrmMain.DGVQueue.Enqueue(message);
                if (FrmMain.DGVQueueEventFlag)
                {
                    FrmMain.DGVQueueEventFlag = false;
                    FrmMain.DGVQueueEvent.Set();
                }
            }
            finally
            {
                if (StockReplyListener != null)
                {
                    StockReplyListener.Close();
                    StockReplyListener = null;
                    StockReplyListenFlag = false;  
                }
            }
        }
        #endregion

       


        public static IPAddress GetLocalIPAddress()
        {
            string text = "";
            IPAddress result = null;
            for (int i = 0; i < Dns.GetHostEntry(Dns.GetHostName()).AddressList.Length; i++)
            {
                text = Dns.GetHostEntry(Dns.GetHostName()).AddressList[i].ToString();
                int num = 0;
                int num2 = 0;
                while (num >= 0)
                {
                    num = text.IndexOf(".", num);
                    if (num >= 0)
                    {
                        num2++;
                        num++;
                    }
                }

                if (num2 == 3 && text != "0.0.0.0")
                {
                    result = Dns.GetHostEntry(Dns.GetHostName()).AddressList[i];
                }
            }

            return result;
        }

    }
   
}

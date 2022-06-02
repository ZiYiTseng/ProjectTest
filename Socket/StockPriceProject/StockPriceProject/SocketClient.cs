using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockPriceProject
{
    internal class SocketClient
    {
        public string serverIP = "";  //Server IP
        public string serverPort = "8885";  //Server Port 
        public static bool connectServerFlag = true;
        public Socket socket;

        public void connectServer()
        {
           
            int ServerPort = Convert.ToInt32(serverPort);
        
            try
            {
                IPHostEntry IPLocal = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = GetLocalIPAddress();

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipAddress, ServerPort);   
                if(connectServerFlag)
                {
                    connectServerFlag = false;

                    Thread threadc = new Thread(new ThreadStart(receive));
                    threadc.IsBackground = true;
                    threadc.Start();

                    Thread threadA = new Thread(new ThreadStart(StockPriceParse.Parse));
                    threadA.IsBackground = true;
                    threadA.Start();

                }
            }
            catch (Exception)
            {

                    
            }
            

        }
        public void receive()
        {
            byte[] bytes;
            while (true)
            {
                try
                {
                    bytes = ReceiveBytes(socket, 3); //先取前3碼
                    if (bytes[0] == 0x27)
                    {
                        string test = bytes[2].ToString("X2");
                        string test1 = bytes[1].ToString("X2");
                        int length = Convert.ToInt32(bytes[2].ToString("X2") + bytes[1].ToString("X2"), 16);
                        bytes = ReceiveBytes(socket, length); //再取內容

                        string msg =  Encoding.Default.GetString(bytes);
                        StockPriceParse.PriceParseQueue.Enqueue(msg);
                        if(StockPriceParse.PriceParseQueueEventFlag)
                        {
                            StockPriceParse.PriceParseQueueEventFlag = false;
                            StockPriceParse.PriceParseQueueEvent.Set();
                        }
                        bytes = ReceiveBytes(socket, 1); //再取內容
                    }
                }
                catch (Exception exc)
                {

                    
                }
            }
            
            
        }
        
        public static byte[] ReceiveBytes(Socket socket, int dataLength)
        {
            //建立封包物件
            byte[] bytesBody = new byte[dataLength];
            int receiveLength = 0;
            int tmpLen = dataLength;       // tmpLen 用來判斷還剩多少資料要收進來!!
            int StartIndex = 0;             // StartIndex 是用來記錄收進來的資料要從第幾個位元開始合併,

            while (receiveLength != tmpLen)   // 如果接收的資料長度不等於指定接收的 byte 長度, 表示資料尚不完整要繼續與之後的資料合併
            {
                StartIndex = StartIndex + receiveLength;
                tmpLen = tmpLen - receiveLength;

                byte[] tmpAcceptData = new byte[tmpLen];
                receiveLength = socket.Receive(tmpAcceptData);
                if (receiveLength == 0)
                {
                    Thread.Sleep(10); //
                }
                else
                {
                    Array.ConstrainedCopy(tmpAcceptData, 0, bytesBody, StartIndex, receiveLength);
                }
            }
            //傳回接收byte
            return bytesBody;
        }
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

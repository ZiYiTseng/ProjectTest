using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    class SocketClient
    {
        public Socket socket;
   
        public static ConcurrentQueue<string> sendQueue = new ConcurrentQueue<string>();
        public static AutoResetEvent sendQueueEvent = new AutoResetEvent(false);
        public static bool sendQueueEventFlag = false;
        public static bool ContinueFlag = true;
        public static int SendCnt=0;
        #region 建構式 SocketClient(Socket vSocket)
        public SocketClient(Socket vSocket)
        {
            socket = vSocket;
            socket.NoDelay = true;
            StockPriceSimulation SPS=new StockPriceSimulation();

            Thread threadA = new Thread(new ThreadStart(SPS.CreateStockPrice));
            threadA.IsBackground = true;
            threadA.Start();

            Thread threadB = new Thread(new ThreadStart(sendData));
            threadB.IsBackground = true;
            threadB.Start();

        }
        #endregion

        private void sendData()
        {
            
            byte[] byteMsg;
            try
            {
                while(ContinueFlag)
                {
                    while(sendQueue.Count>0)
                    {
                        sendQueue.TryDequeue(out string tempData);

                        byteMsg = Encoding.UTF8.GetBytes(tempData);

                        byte[] bytes = new byte[byteMsg.Length + 4];
                        bytes[0] = 0x27;
                        string DataLength = Convert.ToString(byteMsg.Length , 16).PadLeft(4, '0');//訊息長度(byteMsg長度+1)
                        bytes[1] = byte.Parse(DataLength.Substring(2, 2), NumberStyles.HexNumber);
                        bytes[2] = byte.Parse(DataLength.Substring(0, 2), NumberStyles.HexNumber);

                        Array.Copy(byteMsg, 0, bytes, 3, byteMsg.Length );
                        bytes[bytes.Length - 1] = 0x0D;

                        sendBytes(bytes);
                        Thread.Sleep(1);
                    }
                    
                }
                sendQueueEventFlag = true;
                sendQueueEvent.WaitOne();

            }
            catch (Exception e)
            {
                ContinueFlag = false;

            }
        }
       

        private void sendBytes(byte[] bytes)
        {
            try
            {
                int SendBytes = 0; //送出Bytes數
                while(SendBytes < bytes.Length && socket!=null && socket.Connected)
                {
                    SendBytes += socket.Send(bytes, SendBytes, bytes.Length - SendBytes, SocketFlags.None);
                    SendCnt++;
                }
            }
            catch (Exception)
            {


            }
        }
        

    }
   
}

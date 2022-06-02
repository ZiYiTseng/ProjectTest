using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    
    class StockPriceSimulation
    {
        public static double[,] Qutos = new double[200,4];//儲存股價
        double[] tempQutos = new double[4];//欲傳送股價
        
        public static int CreatCnt=0;
        public StockPriceSimulation()
        {
            //建立200檔股票虛擬資料
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            
            for (int i = 0; i < 200; i++)
            {
                StringBuilder sb = new StringBuilder();
                int vclosePrice = rnd.Next(1, 200);
                tempQutos [0]= Qutos[i,0] = i; //股票代號
                tempQutos[1] = Qutos[i,1] = Convert.ToDouble(vclosePrice);//股票開盤價
                tempQutos[2] = Qutos[i,2] = Convert.ToDouble(vclosePrice);//股票收盤價

                sb.Append("Symbol=" + tempQutos[0].ToString() + "#");
                sb.Append("OpenPrice=" + tempQutos[1].ToString() + "#");
                sb.Append("ClosePrice=" + tempQutos[2].ToString() + "#");
                sb.Append("Ratio=" + tempQutos[3].ToString() );
                SocketClient.sendQueue.Enqueue(sb.ToString());
                CreatCnt++;
            }
            
        }
        public void CreateStockPrice()
        {
        
            while (SocketClient.ContinueFlag)
            {
                try
                {
                    Random rnd = new Random(Guid.NewGuid().GetHashCode());
                    


                    int Symbol = rnd.Next(0, 200);
                    double vClosePrice = rnd.NextDouble();
                    if(vClosePrice<0.5)
                    {
                        vClosePrice = -vClosePrice;
                    }
                    tempQutos[0] = Symbol;
                    tempQutos[1] = Qutos[Symbol, 1];
                    tempQutos[2]=Qutos[Symbol, 2] =Math.Round( Qutos[Symbol, 2]+ vClosePrice,2, MidpointRounding.AwayFromZero);//股票成交價亂數
                    tempQutos[3] = Math.Round((( Qutos[Symbol, 2] - Qutos[Symbol, 1])/ Qutos[Symbol, 1])*100,2, MidpointRounding.AwayFromZero);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Symbol="+ tempQutos[0].ToString()+"#");
                    sb.Append("OpenPrice=" + tempQutos[1].ToString() + "#");
                    sb.Append("ClosePrice=" + tempQutos[2].ToString() + "#");
                    sb.Append("Ratio=" + tempQutos[3].ToString() );
                  
                    SocketClient.sendQueue.Enqueue(sb.ToString());
                    CreatCnt++;
                    if (SocketClient.sendQueueEventFlag)
                    {
                        SocketClient.sendQueueEventFlag = false;
                        SocketClient.sendQueueEvent.Set();
                    }
                }
                catch (Exception exc )
                {
                    string message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                    message = message + "CreateStockPrice()：" + exc.Message + "\r\n";
                    message = message + exc.StackTrace;
                    FrmMain.DGVQueue.Enqueue(message);
                    if (FrmMain.DGVQueueEventFlag)
                    {
                        FrmMain.DGVQueueEventFlag = false;
                        FrmMain.DGVQueueEvent.Set();
                    }
                }
                Thread.Sleep(1); //暫停1ms
            }
        }
  

    }
}

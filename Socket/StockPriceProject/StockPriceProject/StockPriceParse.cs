using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockPriceProject
{
    
    internal class StockPriceParse
    {
        public static double[,] Qutos = new double[200, 4];//儲存股價
        public static ConcurrentQueue<string> PriceParseQueue=new ConcurrentQueue<string>();
        public static AutoResetEvent PriceParseQueueEvent = new AutoResetEvent(false);
        public static bool PriceParseQueueEventFlag = false;
        
        public static void Parse()
        {
           while(true)
            {
                try
                {
                    while(PriceParseQueue.Count>0)
                    {
                        PriceParseQueue.TryDequeue(out string strPrice);
                        string[] strArray = strPrice.Split('#');
                        double[] tmpArray=new double[4];
                        for (int i = 0; i < strArray.Length ; i++) 
                        {
                            string[] keyValue = strArray[i].Split('='); //用=將tag跟value拆開
                            
                            if (keyValue.Length == 2)
                            {
                                tmpArray[i] = Convert.ToDouble(keyValue[1]);
                            }
                        }
                        Qutos[(int)tmpArray[0], 0] = tmpArray[0];
                        Qutos[(int)tmpArray[0], 1] = tmpArray[1];
                        Qutos[(int)tmpArray[0], 2] = tmpArray[2];
                        Qutos[(int)tmpArray[0], 3] = tmpArray[3];
                    }
                    PriceParseQueueEventFlag = true;
                    PriceParseQueueEvent.WaitOne(); 
               
                  
                }
                catch (Exception)
                {

                
                }
            }
        }
    }
}

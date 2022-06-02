using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockPriceProject
{
    
    public class StockPriceSimulation
    {
        public static ConcurrentDictionary<string, decimal[]> DicStockPrice = new ConcurrentDictionary<string, decimal[]>();
        public static double[,] Qutos = new double[200,4];
        public  StockPriceSimulation()
        {
            //建立200檔股票虛擬資料
            Random rnd = new Random(Guid.NewGuid().GetHashCode());

            for(int i=0;i<200;i++)
            {
                int vclosePrice = rnd.Next(1, 200);
                Qutos[i,0]=i; //股票代號
                Qutos[i,1] = Convert.ToDouble(vclosePrice);//股票開盤價
                Qutos[i,2] = Convert.ToDouble(vclosePrice);//股票收盤價
                
            }
        
        }

        #region 隨機生成股價
        public void CreateStockPrice()
        {
        
            while (FrmMain.flag)
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
                    Qutos[Symbol, 2] =Math.Round( Qutos[Symbol, 2]+ vClosePrice,2, MidpointRounding.AwayFromZero);//股票成交價亂數
                    Qutos[Symbol, 3] = Math.Round((( Qutos[Symbol, 2] - Qutos[Symbol, 1])/ Qutos[Symbol, 1])*100,2, MidpointRounding.AwayFromZero);
                    
                }
                catch (Exception exc )
                {
                   
                }
                Thread.Sleep(1); //暫停1ms
            }
        }
        #endregion
    }

   
}

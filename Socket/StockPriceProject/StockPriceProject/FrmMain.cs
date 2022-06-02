using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockPriceProject
{
    public partial class FrmMain : Form
    {

        public static int vSymbol;
        private static int ScrollIndexRow;
        public static bool flag=true;
        private static int UIResetCount=1000;
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            // StockPriceSimulation SPS=new StockPriceSimulation();

            // Thread threadA=new Thread(new ThreadStart(SPS.CreateStockPrice));   //啟動股價執行緒
            //threadA.IsBackground = true; 
            // threadA.Start();
            SocketClient SC = new SocketClient();
            Thread threadA = new Thread(new ThreadStart(SC.connectServer));   //啟動股價執行緒
            threadA.IsBackground = true;
            threadA.Start();

            Thread threadB = new Thread(new ThreadStart(DataGridViewStart)); //啟動DataGridViewControl
            threadB.IsBackground = true;
            threadB.Start();

            

            label4.Text= UIResetCount.ToString()+"ms";   


        }



        public void DataGridViewStart()
        {
            while (flag)
            {
                
                try
                {
                    SetControlDataGridViewMain(StockPriceParse.Qutos);
                    Thread.Sleep(UIResetCount);
                }
                catch (Exception esc)
                {
                }
                
            }

        }
        delegate void SetCallBackDataGridView(double[,] vItem);
        public void SetControlDataGridViewMain(double[,] vItem)
        {
            
            if (DataGridView.InvokeRequired)
            {
                SetCallBackDataGridView A = new SetCallBackDataGridView(SetControlDataGridViewMain);
                this.Invoke(A, new object[] { vItem });
            }
            else
            {
                ScrollIndexRow = DataGridView.FirstDisplayedScrollingRowIndex;
                
                if(DataGridView.Rows.Count<200)
                {
                    for (int i = 0; i < vItem.GetLength(0); i++)
                    {
                       
                        DataGridView.Rows.Add(new object[] { vItem[i,0], vItem[i,1], vItem[i,2], vItem[i,3] });
                    }
                }
                else
                {
                    for (int i = 0; i < vItem.GetLength(0); i++)
                    {
                        DataGridView.Rows[i].Cells[0].Value = vItem[i, 0];
                        DataGridView.Rows[i].Cells[1].Value = vItem[i, 1];
                        DataGridView.Rows[i].Cells[2].Value = vItem[i,2];
                        DataGridView.Rows[i].Cells[3].Value = vItem[i,3].ToString()+"%";
                    }
                }
                DataGridView.FirstDisplayedScrollingRowIndex = ScrollIndexRow;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(textBox1.Text) > 3000)
                {
                    MessageBox.Show("不可大於3000");
                }
                else if (Convert.ToInt32(textBox1.Text) < 0)
                {
                    MessageBox.Show("不可小於0");
                }
                else
                {
                    UIResetCount = Convert.ToInt32(textBox1.Text);
                    label4.Text = textBox1.Text + "ms";
                    textBox1.Text = null;
                }
            }
            catch (Exception)
            {

              
            }
          
            
        }
    }
}

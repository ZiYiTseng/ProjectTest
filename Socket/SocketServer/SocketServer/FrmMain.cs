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

namespace SocketServer
{
    public partial class FrmMain : Form
    {
        public static Queue<string> DGVQueue = new Queue<string>();
        public static AutoResetEvent DGVQueueEvent = new AutoResetEvent(false);
        public static bool DGVQueueEventFlag = false;
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Thread threadA = new Thread(new ThreadStart(SocketServer.StartListening));
            threadA.IsBackground= true; 
            threadA.Start();

            Thread threadB = new Thread(new ThreadStart(GridViewMainControl));
            threadB.IsBackground = true;
            threadB.Start();

        }

        public void GridViewMainControl()
        {
            int MsgCount = 0; //目前訊息筆數
            while (true)
            {
                try
                {
                    while (DGVQueue.Count > 0)
                    {
                        string strMessage = (string)FrmMain.DGVQueue.Dequeue();
                        MsgCount++;

                        if (DGVQueue.Count > 500)
                        {
                            DGVQueue.Clear();
                        }
                        object[] aryMessage = new object[] { MsgCount, DGV_MSG.Rows.Count, strMessage };
                        SetControlDataGridViewMain(aryMessage);
                    }

                }
                catch (Exception exc)
                {
                   
                }
                FrmMain.DGVQueueEventFlag = true;
                FrmMain.DGVQueueEvent.WaitOne();
            }
        }
        delegate void SetDataGridViewMainCallback(object[] myMessage);
        public void SetControlDataGridViewMain(object[] myMessage)
        {
            try
            {
                //進行資料委派
                if (DGV_MSG.InvokeRequired)
                {
                    SetDataGridViewMainCallback d = new SetDataGridViewMainCallback(SetControlDataGridViewMain);
                    this.Invoke(d, new object[] { myMessage });
                }
                else
                {
                    DGV_MSG.Rows.Add(new object[] { myMessage[0], myMessage[1], myMessage[2] });
                    DGV_MSG.ClearSelection();
                    DGV_MSG.Sort(DGV_MSG.Columns[0], System.ComponentModel.ListSortDirection.Descending); //大到小排序
                }
            }
            catch (Exception exc)
            {
             
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = StockPriceSimulation.CreatCnt.ToString();
            label3.Text = SocketClient.SendCnt.ToString();
        }
    }
}

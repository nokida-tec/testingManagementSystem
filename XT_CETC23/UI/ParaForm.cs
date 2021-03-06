﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using XT_CETC23.INTransfer;
using XT_CETC23.DataCom;
using XT_CETC23.DataManager;
using XT_CETC23.Common;
using XT_CETC23.Model;
using System.Threading;
using XT_CETC23;


namespace XT_CETC23.SonForm
{
    public partial class ParaForm : Form,IParaForm
    {
        CameraForm cf = new CameraForm();
        Thread th1;
        DataBase db;
        ComboBox[] cb;
        CheckBox[] chb;
        Plc plc;
        UserForms.SortAdd sortAdd1;
        string[] str=new string[6];
        bool[] bl=new bool[6];
        TextBox[] textBoxCmd;
        TextBox[] textBoxData;
        Button[] btnCmd;
        Button[] btnData;
         
        public class ProductTypeItem
        {

        }

        public ParaForm()
        {
            InitializeComponent();
            db = DataBase.GetInstanse();
            cb = new ComboBox[] { para_cbCabinet1, para_cbCabinet2, para_cbCabinet3, para_cbCabinet4, para_cbCabinet5, para_cbCabinet6 };
            chb = new CheckBox[] { para_chbService1, para_chbService2, para_chbService3, para_chbService4, para_chbService5, para_chbService6 };

            textBoxCmd = new TextBox[] { txtCmd1, txtCmd2, txtCmd3, txtCmd4, txtCmd5, txtCmd6 };
            textBoxData = new TextBox[] { txtSource1, txtSource2, txtSource3, txtSource4, txtSource5, txtSource6 };
            btnCmd = new Button[] { btnCmd1, btnCmd2, btnCmd3, btnCmd4, btnCmd5, btnCmd6 };
            btnData = new Button[] { btnData1, btnData2, btnData3, btnData4, btnData5, btnData6 };

            InitData();
            sortAdd1 = new UserForms.SortAdd(this);
            plc = Plc.GetInstanse();

            db = DataBase.GetInstanse();
            DataTable dt = db.DBQuery("select * from dbo.Path");
            for (int i = 0; i < 4; i++)
            { // Todo
                if (dt.Rows.Count == 0 || dt.Rows[0]["CmdPathName"].ToString().Trim() == null || dt.Rows[0]["DataPathName"].ToString().Trim() == null)
                {
                    MessageBox.Show("系统所需文件目录配置信息不完整，请通过参数配置页面配置完整！");
                    return;
                }
            }
            Config.Config.getInstance().logPath = dt.Rows[6]["CmdPathName"].ToString().Trim();
            txtLog.Text = Config.Config.getInstance().logPath;
            if (!Directory.Exists(Config.Config.getInstance().logPath))
            {
                Directory.CreateDirectory(Config.Config.getInstance().logPath);
                string filePath = Config.Config.getInstance().logPath + @"\log.txt";
                File.Create(filePath);
            }
            else
            {
                string filePath = Config.Config.getInstance().logPath + @"\log.txt";
                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }
            }
            Config.Config.getInstance().targetPath = dt.Rows[7]["CmdPathName"].ToString().Trim();
            txtTarget.Text = Config.Config.getInstance().targetPath;
            if (!Directory.Exists(Config.Config.getInstance().targetPath))
            {
                Directory.CreateDirectory(Config.Config.getInstance().targetPath);
            }

            CabinetData.pathCabinetStatus = new string[6];
            CabinetData.pathCabinetOrder = new string[6];
            CabinetData.sourcePath = new string[6];
            for (int i = 0; i < 6;i++ )
            {
                CabinetData.pathCabinetStatus[i] = @dt.Rows[i]["CmdPathName"].ToString().Trim() + @"\发送指令.txt";
                CabinetData.pathCabinetOrder[i] =  @dt.Rows[i]["CmdPathName"].ToString().Trim() + @"\接收指令.txt";
                textBoxCmd[i].Text = dt.Rows[i]["CmdPathName"].ToString().Trim();
                CabinetData.sourcePath[i] = @dt.Rows[i]["DataPathName"].ToString().Trim();
                textBoxData[i].Text = @dt.Rows[i]["DataPathName"].ToString().Trim();

                try
                {
                    //string path = Path.GetDirectoryName(CabinetData.pathCabinetStatus[i]);
                    //if (path != null && !Directory.Exists(path))
                    //{
                    //    Directory.CreateDirectory(path);
                    //}
                    //path = Path.GetDirectoryName(CabinetData.pathCabinetOrder[i]);
                    //if (path != null && !Directory.Exists(path))
                    //{
                    //    Directory.CreateDirectory(path);
                    //}
                    //path = CabinetData.sourcePath[i];
                    //if (path != null && !Directory.Exists(path))
                    //{
                    //    Directory.CreateDirectory(path);
                    //}
                }
                catch(Exception e)
                {
                    Logger.WriteLine(e);
                }
            }         
        }
        private void ParaForm_Load(object sender, EventArgs e)
        {
            chkBoxU8.Checked = Config.Config.getInstance().enableU8;
        }
        void InitData()
        {
            for (int i = 0; i < Math.Min(cb.Length, DeviceCount.TestingBedCount); ++i)
            {
                int capOfProduct = TestingCabinets.getInstance(i).Type;
                cb[i].Items.Clear();
                cb[i].Items.Add("未定义");
                cb[i].Items.Add("A组件");
                cb[i].Items.Add("B组件");
                cb[i].Items.Add("2类组件");
                cb[i].Items.Add("AB组件");
                cb[i].Items.Add("C组件");
                cb[i].Items.Add("D组件");
                cb[i].Text = TestingBedCapOfProduct.sTestingBedCapOfProduct[capOfProduct].ShowName;
                str[i] = TestingBedCapOfProduct.sTestingBedCapOfProduct[capOfProduct].ProductType; ;
                chb[i].Checked = TestingCabinets.getInstance(i).Enable == TestingCabinet.ENABLE.Enable;
                bl[i] = chb[i].Checked;
            }
        }

        private void para_btnWrite_Click(object sender, EventArgs e)
        {
            byte[] prodType= new byte[1];
            int cabinetStatus=0;
            if (db.DBConnect())
            {
                for (int i = 0; i < cb.Length; ++i)
                {
                    try
                    {
                        int sel = cb[i].SelectedIndex;
                        if (sel == -1)
                        {
                            sel = 0;
                        }

                        prodType[0] = TestingBedCapOfProduct.sTestingBedCapOfProduct[sel].PlcMode;
                        TestingCabinets.getInstance(i).Type = sel;
                        TestingCabinets.getInstance(i).ProductType = TestingBedCapOfProduct.sTestingBedCapOfProduct[sel].ProductType;
                        TestingCabinets.getInstance(i).Enable = chb[i].Checked ? TestingCabinet.ENABLE.Enable : TestingCabinet.ENABLE.Disable;
                        TestingCabinets.getInstance(i).doConfigChanged();
                        {
                            str[i] = cb[i].SelectedItem.ToString();
                            bl[i] = (bool)chb[i].Checked;
                            plc.DBWrite(PlcData.PlcWriteAddress, 21 + i, 1, prodType);
                            if (chb[i].Checked)
                            {
                                switch (i)
                                {
                                    case 0:
                                        cabinetStatus = cabinetStatus + 1;
                                        break;
                                    case 1:
                                        cabinetStatus = cabinetStatus + 2;
                                        break;
                                    case 2:
                                        cabinetStatus = cabinetStatus + 4;
                                        break;
                                    case 3:
                                        cabinetStatus = cabinetStatus + 8;
                                        break;
                                    case 4:
                                        cabinetStatus = cabinetStatus + 16;
                                        break;
                                    case 5:
                                        cabinetStatus = cabinetStatus + 32;
                                        break;
                                }
                            }
                        }
                    }
                    catch(Exception e1)
                    {
                        Logger.WriteLine(e1);
                    }
                }
                prodType[0] = Convert.ToByte(cabinetStatus);
                plc.DBWrite(PlcData.PlcWriteAddress, 20, 1, prodType);
            }
            
        }

        private void para_btnAdd_Click(object sender, EventArgs e)
        {
            if (Common.Account.user == "admin")
            {
                if (!sortAdd1.IsDisposed) { sortAdd1.Show(); }
                else { UserForms.SortAdd sortAdd2 = new UserForms.SortAdd(this);sortAdd2.Show(); }
            }
            else
            {
                MessageBox.Show("当前用户无此权限");
            }
        }

        public void getSort(string str)
        {
            for (int i = 0; i < cb.Length; ++i)
            {
                //db.DBUpdata("insert into CabinetData(number,sort,status) values('"+i+"','" + cb[i].SelectedItem.ToString() + "','" + chb[i].Checked + "')");
                cb[i].Items.Add(str);
            }
        }
        //机器人上电
        private void para_btnRobotPowerOn_Click(object sender, EventArgs e)
        {
            plc.DBWrite(PlcData.PlcWriteAddress, PlcData._writeRobot,PlcData._writeLength1, new byte[] { 11 });
        }
        //回主程序
        private void para_btnRobotRun_Click(object sender, EventArgs e)
        {
            if(PlcData._robotStatus==12)
            plc.DBWrite(PlcData.PlcWriteAddress, PlcData._writeRobot, PlcData._writeLength1, new byte[] { 12 });
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbLog = new FolderBrowserDialog();
            if (fbLog.ShowDialog() ==DialogResult.OK)
            {
                txtLog.Text = fbLog.SelectedPath;               
            }
        }

        private void btnCmd_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbLog = new FolderBrowserDialog();
            if (fbLog.ShowDialog() == DialogResult.OK)
            {
                txtCmd1.Text = fbLog.SelectedPath;
            }
        }

        private void btnSorce_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbLog = new FolderBrowserDialog();
            if (fbLog.ShowDialog() == DialogResult.OK)
            {
                txtSource1.Text = fbLog.SelectedPath;
            }
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbLog = new FolderBrowserDialog();
            if (fbLog.ShowDialog() == DialogResult.OK)
            {
                txtTarget.Text = fbLog.SelectedPath;
            }
        }

        private void btnLogSave_Click(object sender, EventArgs e)
        {
            
        }

        private void btnCmdSave_Click(object sender, EventArgs e)
        {
            
        }

        private void btnSourceSave_Click(object sender, EventArgs e)
        {
    
        }

        private void btnTargetSave_Click(object sender, EventArgs e)
        {
            db.DBUpdate("update dbo.Path set CmdPathName='" + txtCmd1.Text.Trim() + "'where PathID=" + 1);
            db.DBUpdate("update dbo.Path set DataPathName='" + txtSource1.Text.Trim() + "'where PathID=" + 1);
            db.DBUpdate("update dbo.Path set CmdPathName='" + txtCmd2.Text.Trim() + "'where PathID=" + 2);
            db.DBUpdate("update dbo.Path set DataPathName='" + txtSource2.Text.Trim() + "'where PathID=" + 2);
            db.DBUpdate("update dbo.Path set CmdPathName='" + txtCmd3.Text.Trim() + "'where PathID=" + 3);
            db.DBUpdate("update dbo.Path set DataPathName='" + txtSource3.Text.Trim() + "'where PathID=" + 3);
            db.DBUpdate("update dbo.Path set CmdPathName='" + txtCmd4.Text.Trim() + "'where PathID=" + 4);
            db.DBUpdate("update dbo.Path set DataPathName='" + txtSource4.Text.Trim() + "'where PathID=" + 4);
            db.DBUpdate("update dbo.Path set CmdPathName='" + txtCmd5.Text.Trim() + "'where PathID=" + 5);
            db.DBUpdate("update dbo.Path set DataPathName='" + txtSource5.Text.Trim() + "'where PathID=" + 5);
            db.DBUpdate("update dbo.Path set CmdPathName='" + txtCmd6.Text.Trim() + "'where PathID=" + 6);
            db.DBUpdate("update dbo.Path set DataPathName='" + txtSource6.Text.Trim() + "'where PathID=" + 6);

            db.DBUpdate("update dbo.Path set CmdPathName='" + txtLog.Text.Trim() + "'where PathID=" + 7);
            db.DBUpdate("update dbo.Path set DataPathName='" + txtLog.Text.Trim() + "'where PathID=" + 7);
            Config.Config.getInstance().logPath = txtLog.Text.Trim();
            if (!Directory.Exists(Config.Config.getInstance().logPath))
            {
                Directory.CreateDirectory(Config.Config.getInstance().logPath);
            }

            db.DBUpdate("update dbo.Path set CmdPathName='" + txtTarget.Text.Trim() + "'where PathID=" + 8);
            db.DBUpdate("update dbo.Path set DataPathName='" + txtTarget.Text.Trim() + "'where PathID=" + 8);
            Config.Config.getInstance().targetPath = txtTarget.Text.Trim();
            if (!Directory.Exists(Config.Config.getInstance().targetPath))
            {
                Directory.CreateDirectory(Config.Config.getInstance().targetPath);
            }

            DataTable dt = db.DBQuery("select * from dbo.Path");
            for (int i = 0; i < 6; i++)
            {
                CabinetData.pathCabinetStatus[i] = @dt.Rows[i]["CmdPathName"].ToString().Trim() + @"\发送指令.txt";
                CabinetData.pathCabinetOrder[i] = @dt.Rows[i]["CmdPathName"].ToString().Trim() + @"\接收指令.txt";
                textBoxCmd[i].Text = dt.Rows[i]["CmdPathName"].ToString().Trim();
                CabinetData.sourcePath[i] = @dt.Rows[i]["DataPathName"].ToString().Trim();
                textBoxData[i].Text = @dt.Rows[i]["DataPathName"].ToString().Trim();
            }         
        }

        private void selectPath (int index, bool isCmd)
        {
            FolderBrowserDialog fbLog = new FolderBrowserDialog();
            if (fbLog.ShowDialog() == DialogResult.OK)
            {
                if (isCmd)
                {
                    textBoxCmd[index].Text = fbLog.SelectedPath;
                }
                else
                {
                    textBoxData[index].Text = fbLog.SelectedPath;
                }
            }
        }

        private void btnCmd1_Click(object sender, EventArgs e)
        {
            selectPath(0, true);
        }

        private void btnData1_Click(object sender, EventArgs e)
        {
            selectPath(0, false);
        }

        private void btnCmd2_Click(object sender, EventArgs e)
        {
            selectPath(1, true);
        }

        private void btnData2_Click(object sender, EventArgs e)
        {
            selectPath(1, false);
        }

        private void btnCmd3_Click(object sender, EventArgs e)
        {
            selectPath(2, true);
        }

        private void btnData3_Click(object sender, EventArgs e)
        {
            selectPath(2, false);
        }

        private void btnCmd4_Click(object sender, EventArgs e)
        {
            selectPath(3, true);
        }

        private void btnData4_Click(object sender, EventArgs e)
        {
            selectPath(3, false);
        }

        private void btnCmd5_Click(object sender, EventArgs e)
        {
            selectPath(4, true);
        }

        private void btnData5_Click(object sender, EventArgs e)
        {
            selectPath(4, false);
        }

        private void btnCmd6_Click(object sender, EventArgs e)
        {
            selectPath(5, true);
        }

        private void btnData6_Click(object sender, EventArgs e)
        {
            selectPath(5, false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("清除任务前请确保测试柜中的产品已取出，料架的托盘已放回料架，机器人并无抓取任何组件！！！",
                 "是否要清除系统任务？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                TestingSystem.GetInstance().Clear();
            }
        }

        private void chkBoxU8_CheckedChanged(object sender, EventArgs e)
        {
            Config.Config.getInstance().enableU8 = chkBoxU8.Checked;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XT_CETC23.INTransfer;
using XT_CETC23.DataCom;
using XT_CETC23.DataManager;
using XT_CETC23.Common;
using XT_CETC23;
using System.Threading;

namespace XT_CETC23.SonForm
{
    public partial class RunForm : Form
    {
        private Label[] grab,mode;

        public RunForm()
        {
            InitializeComponent();

            mode = new Label[] { lb_Cabinet1_env, lb_Cabinet2_env, lb_Cabinet3_env, lb_Cabinet4_env, lb_Cabinet5_env, lb_Cabinet6_env };
            grab = new Label[] { lb_Cabinet1_gv, lb_Cabinet2_gv, lb_Cabinet3_gv, lb_Cabinet4_gv, lb_Cabinet5_gv, lb_Cabinet6_gv };
        }

        private void Form_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            // 注册显示函数
            TestingSystem.GetInstance().RegistryDelegate(onModeChanged);
            TestingSystem.GetInstance().RegistryDelegate(onInitializeChanged);
            TestingSystem.GetInstance().RegistryDelegate(onStatusChanged);
            Plc.GetInstanse().RegistryDelegate(onPlcModeChanged);
            Robot.GetInstanse().RegistryDelegate(onRobotStatusChanged);
            Frame.getInstance().RegistryDelegate(onFrameStatusChanged);
            onFrameStatusChanged();
            for (int i = 0; i < TestingCabinets.getCount(); i++)
            {
                TestingCabinets.getInstance(i).RegistryDelegate(onCabinetStatusChanged);
                TestingCabinets.getInstance(i).RegistryDelegate(onCabinetResultChanged);
                TestingCabinets.getInstance(i).RegistryDelegate(onCabinetConfigChanged);
                onCabinetConfigChanged(i);
            }
        }

        private void onClick_FrameUpdate(object sender, EventArgs e)
        {
            if (Frame.getInstance().frameUpdate == Frame.FrameUpdateStatus.Updating)             
            {
                if (MessageBox.Show("请确认料架更换已经完成", "确认消息", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    Frame.getInstance().excuteCommand(Frame.Lock.Command.Close);
                    Frame.getInstance().frameUpdate = Frame.FrameUpdateStatus.Updated;
                    Logger.WriteLine("确认料架更换已经完成");
                }
            }
        }

        private void onRobotStatusChanged(Robot.Status status)
        {
            lb_Robot_sv.Text = EnumHelper.GetDescription(status);
            switch (status)
            {
                case Robot.Status.Unkown:
                case Robot.Status.Alarming:
                    lb_Robot_sv.BackColor = Color.Red;
                    break;
                case Robot.Status.Closed:
                case Robot.Status.Busy:
                case Robot.Status.Initialized:
                case Robot.Status.Initializing:
                default:
                    lb_Robot_sv.BackColor = Color.Green;
                    break;
            }
        }     
        
        private void onFrameStatusChanged()
        {
            run_lbGramStatusv.Text = EnumHelper.GetDescription(Frame.getInstance().frameUpdate);
            if (Frame.getInstance().frameUpdate == Frame.FrameUpdateStatus.Updating)
            {
                btnFrameUpdate.BackColor = Color.Yellow;
                btnFrameUpdate.Enabled = true;
            }
            else
            {
                btnFrameUpdate.BackColor = Color.PowderBlue;
                btnFrameUpdate.Enabled = false;
            }
        }
        private bool onCabinetStatusChanged(int cabinetID, Cabinet.BedStatus status)
        {
            if (this.IsHandleCreated)
            {
                String message = EnumHelper.GetDescription(status);
                if (cabinetID == 0)
                    lb_Cabinet11_sv.Invoke(new Action<string>((s) => { lb_Cabinet11_sv.Text = message; }), message);
                if (cabinetID == 1)
                    lb_Cabinet21_sv.Invoke(new Action<string>((s) => { lb_Cabinet21_sv.Text = message; }), message);
                if (cabinetID == 2)
                    lb_Cabinet31_sv.Invoke(new Action<string>((s) => { lb_Cabinet31_sv.Text = message; }), message);
                if (cabinetID == 3)
                    lb_Cabinet41_sv.Invoke(new Action<string>((s) => { lb_Cabinet41_sv.Text = message; }), message);
                if (cabinetID == 4)
                    lb_Cabinet51_sv.Invoke(new Action<string>((s) => { lb_Cabinet51_sv.Text = message; }), message);
                if (cabinetID == 5)
                    lb_Cabinet61_sv.Invoke(new Action<string>((s) => { lb_Cabinet61_sv.Text = message; }), message);
            } 
            return true;
        }

        private bool onCabinetResultChanged(int cabinetID, TestingCabinet.STATUS status)
        {
            if (this.IsHandleCreated)
            {
                String message = EnumHelper.GetDescription(status);
                if (cabinetID == 0)
                    lb_Cabinet11_rv.Invoke(new Action<string>((s) => { lb_Cabinet11_rv.Text = message; }), message);
                if (cabinetID == 1)
                    lb_Cabinet21_rv.Invoke(new Action<string>((s) => { lb_Cabinet21_rv.Text = message; }), message);
                if (cabinetID == 2)
                    lb_Cabinet31_rv.Invoke(new Action<string>((s) => { lb_Cabinet31_rv.Text = message; }), message);
                if (cabinetID == 3)
                    lb_Cabinet41_rv.Invoke(new Action<string>((s) => { lb_Cabinet41_rv.Text = message; }), message);
                if (cabinetID == 4)
                    lb_Cabinet51_rv.Invoke(new Action<string>((s) => { lb_Cabinet51_rv.Text = message; }), message);
                if (cabinetID == 5)
                    lb_Cabinet61_rv.Invoke(new Action<string>((s) => { lb_Cabinet61_rv.Text = message; }), message);
            }
            return true;
        }

        private bool onCabinetConfigChanged(int cabinetID)
        {
            if (this.IsHandleCreated)
            {
                mode[cabinetID].Text = EnumHelper.GetDescription(TestingCabinets.getInstance(cabinetID).Enable);
                grab[cabinetID].Text = TestingCabinets.getInstance(cabinetID).getCap()[0];
            }
            return true;
        }

        private void onModeChanged(TestingSystem.Mode mode)
        {
            switch (mode)
            {
                case TestingSystem.Mode.Auto:
                    run_btnAuto.BackColor = Color.Green;
                    run_btnManul.BackColor = Color.PowderBlue;
                    break;
                case TestingSystem.Mode.Manual:
                    run_btnAuto.BackColor = Color.PowderBlue;
                    run_btnManul.BackColor = Color.Green;
                    break;
                default:
                    run_btnAuto.BackColor = Color.Green;
                    run_btnManul.BackColor = Color.Green;
                    break;
            }
        }

        private void onPlcModeChanged(bool status)
        {  // 显示PLC状态
            switch (status)
            {
                case true:
                    run_lbPlcStatusv.Text = "运行中";
                    run_lbPlcStatusv.BackColor = Color.Green;
                    break;
                default:
                    run_lbPlcStatusv.Text = "故障";
                    run_lbPlcStatusv.BackColor = Color.Red;
                    break;
            }
        }

        private void onInitializeChanged(TestingSystem.Initialize initialize)
        {
            switch (initialize)
            {
                case TestingSystem.Initialize.Initialize:
                    break;
                case TestingSystem.Initialize.Initialized:
                    break;
                default:
                    break;
            }
        }

        private void onStatusChanged(TestingSystem.Mode mode, TestingSystem.Status status)
        {
            if (mode == TestingSystem.Mode.Auto && status == TestingSystem.Status.Running)
            {
                run_btnInit.BackColor = Color.Green;
                run_btnOff.BackColor = Color.PowderBlue;
            }
            else
            {
                run_btnInit.BackColor = Color.PowderBlue;
                run_btnOff.BackColor = Color.Green;
            }
        }
        
    }
}

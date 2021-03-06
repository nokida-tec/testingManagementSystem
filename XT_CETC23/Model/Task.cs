﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using XT_CETC23.DataCom;

namespace XT_CETC23
{
    class Task 
    {
        protected int mID;        // 任务号
        protected int mCabinetID; // 分配的测试柜
        protected String mProductType; // 测试产品类型

        protected Object lockObject = new Object();     

        public int CreateTask(string ProductType, String BatchID)
        {
            lock (lockObject)
            {
                DataTable dt = DataBase.GetInstanse().DBQuery("select * from dbo.MTR where CabinetID = " + mID);
                if (dt!= null && dt.Rows.Count > 0)
                {
                    Logger.WriteLineWithStack("测试柜" + mID + "任务已经存在");
                    return -1;
                }
                mID = GetID.getID();
                mProductType = ProductType;

                string sql = 
                    "insert into dbo.MTR("
                    + "BasicID,"
                    + "CabinetID,"
                    + "ProductType,"
                    + "BatchID,"
                    + "BeginTime,"
                    + ")values("
                    + " '" + mID + "'"
                    + ",'" + mCabinetID + "'"
                    + ",'" + mProductType + "'"
                    + ",'" + BatchID + "'"
                    + ",'" + DateTime.Now + "'"
                    + ")";
                DataBase.GetInstanse().DBInsert(sql);
                return mID;
            }
        }

        public int FinishTask()
        {
            DataTable dt = DataBase.GetInstanse().DBQuery("select * from dbo.MTR where BasicID = " + mID);
            if (dt == null || dt.Rows.Count == 0)
            {
                return -1;
            }

            try
            {
                String prodCode = dt.Rows[0]["ProductID"].ToString();
                String prodType = dt.Rows[0]["ProductType"].ToString();
                String cabinetName = dt.Rows[0]["CabinetID"].ToString();
                String trayNo = dt.Rows[0]["FrameLocation"].ToString();
                String pieceNo = dt.Rows[0]["SalverLocation"].ToString();
                String BeginTime = dt.Rows[0]["BeginTime"].ToString();
                String EndTime = dt.Rows[0]["EndTime"].ToString();
                String BatchID = dt.Rows[0]["BatchID"].ToString();
                String CheckResult = dt.Rows[0]["CheckResult"].ToString();
                trayNo = Frame.getInstance().convertFrameLocationToA1(trayNo);
                try
                {
                    cabinetName = (Convert.ToInt32(cabinetName) + 1) + "#测试柜";
                }
                catch (Exception e)
                {
                    Logger.WriteLine(e);
                }

                DataBase.GetInstanse().DBInsert("insert into dbo.ActualData("
                    + " ProductID,ProductType,FrameLocation,SalverLocation,CheckCabinetA,CheckCabinetB,CheckDate,CheckTime,CheckBatch,BeginTime,EndTime,BatchID,CheckResult"
                    + " )values( '"
                    + prodCode + "','"
                    + prodType + "','"
                    + trayNo + "','"
                    + pieceNo + "','"
                    + cabinetName + "','"
                    + "0" + "','"
                    + "0" + "','"
                    + "0" + "','"
                    + "0" + "','"
                    + BeginTime + "','"
                    + EndTime + "','"
                    + BatchID + "','"
                    + CheckResult + "')");
                DataBase.GetInstanse().DBInsert("insert into dbo.FrameData("
                    + "BasicID,ProductID,ProductType,FrameLocation,SalverLocation,CheckCabinet,BeginTime,EndTime,BatchID,CheckResult"
                    + " )values("
                    + mID + ",'"
                    + prodCode + "','"
                    + prodType + "','"
                    + trayNo + "','"
                    + pieceNo + "','"
                    + cabinetName + "','"
                    + BeginTime + "','"
                    + EndTime + "','"
                    + BatchID + "','"
                    + CheckResult + "')");
                DataBase.GetInstanse().DBDelete("delete from dbo.MTR where BasicID = " + mID);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e);
            }
            return mID;
        }

        public bool FinishTesting(String checkResult, String failedReason = null)
        {
            DataBase.GetInstanse().DBUpdate("update dbo.MTR set "
                + " CheckResult = '" + checkResult + "'"
                + " ,FailedReason = '" + failedReason + "'"
                + " ,EndTime = '" + DateTime.Now + "'"
                + " where BasicID= " + this.mID);
            return true;
        }

        public bool UpdateStep(String step, String substep)
        {
            DataBase.GetInstanse().DBUpdate("update dbo.MTR set StationSign= '" + true + "' where BasicID=" + mID);
            return true;
        }

        private Frame.Location mLocation = new Frame.Location();
        public Frame.Location location
        {
            get { return mLocation; }
            set
            {
                if (mLocation != value)
                {
                    mLocation.Copy(value);
                    DataBase.GetInstanse().DBUpdate("update dbo.MTR set FrameLocation = " + mLocation.tray + "," + "SalverLocation=" + mLocation.slot + " where BasicID=" + mID);
                }
            }
        }

        private String mProductID;
        public String productID
        {
            get { return mProductID; }
            set
            {
                if (mProductID != value)
                {
                    mProductID = value;
                    DataBase.GetInstanse().DBUpdate("update dbo.MTR set ProductID = " + mProductID);
                }
            }
        }
    }
}

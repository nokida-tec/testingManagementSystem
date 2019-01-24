﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XT_CETC23.Common;

namespace XT_CETC23.EnumC
{
    enum Robot
    {
        [EnumDescription("机器人故障")]
        Fault,
        [EnumDescription("机器人上电完成")]
        PowerOnOver,
        [EnumDescription("机器人空闲中")]
        Freeing,
        [EnumDescription("机器人暂停中")]
        Pauseing,
        [EnumDescription("机器人运行中")]
        Running,
    }
    enum Frame
    {
        [EnumDescription("扫描完成")]
        ScanSort=31,
        [EnumDescription("取料完成")]
        GetPiece,
        [EnumDescription("产品扫码完成")]
        ScanPiece,
        [EnumDescription("放料完成")]
        PutPiece,
        [EnumDescription("扫码中")]
        Scaning,
        [EnumDescription("取料中")]
        Geting,
        [EnumDescription("放料中")]
        Puting,
        [EnumDescription("在原点")]
        Home,
    }
    enum Cabinet
    {
        [EnumDescription("Ready")]
        Ready = 30,
        [EnumDescription("Testing")]
        Testing = 31,
        [EnumDescription("Fault_Config")]
        Fault_Config = 32,
        [EnumDescription("Fault_Control")]
        Fault_Control = 33,
        [EnumDescription("Fault_Report")]
        Fault_Report = 34,
        [EnumDescription("Finished")]
        Finished = 40,
        [EnumDescription("OK")]
        OK=100,
        [EnumDescription("NG")]
        NG=101,
    }
    enum Plc
    {
        [EnumDescription("OFF模式")]
        OffMode = 21,
        [EnumDescription("手动，未准备好自动")]
        ManulNoReady = 22,
        [EnumDescription("手动准备好自动")]
        ManulReady = 23,
        [EnumDescription("自动模式")]
        Auto = 24,
        [EnumDescription("自动模式中")]
        AutoRuning = 25,
    }
    enum Grab
    {
        [EnumDescription("A组件")]
        SortA,
        [EnumDescription("B组件")]
        SortB,
        [EnumDescription("2类组件")]
        Sort2,
        [EnumDescription("AB组件")]
        SortAB,
        [EnumDescription("C组件")]
        SortC,
        [EnumDescription("D组件")]
        SortD,
    }
    enum RobotS
    {
        [EnumDescription("机器人在取料位")]
        GetPiece=100,
        [EnumDescription("机器人在1#机台放料位")]
        PutPiece_Cabinet1=101,
        [EnumDescription("机器人在2#机台放料位")]
        PutPiece_Cabinet2=102,
        [EnumDescription("机器人在3#机台放料位")]
        PutPiece_Cabinet3=103,
        [EnumDescription("机器人在4#机台放料位")]
        PutPiece_Cabinet4=104,
        [EnumDescription("机器人在5#机台放料位")]
        PutPiece_Cabinet5=105,
        [EnumDescription("机器人在6#机台放料位")]
        PutPiece_Cabinet6=106,
    }
}

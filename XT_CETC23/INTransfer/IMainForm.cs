﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XT_CETC23.INTransfer
{
    public interface IMainForm
    {
        string getMessageToMainForm();
        void manulEnable(string mode,string status);
    }
}

#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using FTOptix.Report;
using FTOptix.DataLogger;
using FTOptix.Store;
using FTOptix.SQLiteStore;
using System.Collections.Generic;
using FTOptix.WebUI;
#endregion

public class SimValue_RuntimeNetLogic : BaseNetLogic
{
    Random rnd = new Random(DateTime.Now.Second);
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started

        var vs = new List<IUAVariable>();
        for (int i = 1; i < 9; i++) {
            var v = Owner.GetVariable($"V{i}");
            vs.Add( v );
        }


        var task = new PeriodicTask(() => { 
            
            foreach(var v in vs)
            {
                v.Value = rnd.Next(100);
            }

        
        }, 1000, LogicObject);

        task.Start();
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
}

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
using System.IO;
using FTOptix.WebUI;
#endregion

public class Report_BatchRange_RuntimeNetLogic : BaseNetLogic
{

    Store dbBase;
    IUAVariable vBatchNo;
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        dbBase = LogicObject.GetAlias("DataStore") as Store;

        vBatchNo = LogicObject.GetVariable("BatchNo");


    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void Query(string outputPdfFolder, string batchNo, NodeId nResultId)
    {
        var report = Owner as Report;
        var pdfRootName = Session.BrowseName.Replace('-', '_') + DateTime.Now.ToString("yyyyMMddHHmmss.ffffff");

        var _rootName = Path.Combine(outputPdfFolder, pdfRootName);

      

        //var strStart = dtStart.ToString("yyyy-MM-dd HH:mm:ss");
        //var strEnd = dtEnd.ToString("yyyy-MM-dd HH:mm:ss");

        generateGraph(_rootName, 1, buildGraphTrendOption(batchNo), 1000, 1000);





        var pdfFile = _rootName + ".pdf";

        var pdfUri = ResourceUri.FromProjectRelativePath(pdfFile);

        vBatchNo.Value = batchNo;

        report.GeneratePdf(pdfUri, string.Empty, out var operationId);

        var ddd = new DelayedTask(() => {
            var n = InformationModel.Get(nResultId) as IUAVariable;
            if (n != null)
            {
                n.Value = pdfUri;
            }

        }, 1000, LogicObject);
        ddd.Start();
    }


    private void generateGraph(string root, int index, object option, int width, int height)
    {
        var svgfile = root + $"_{index}.svg";
        var uriSVG = ResourceUri.FromProjectRelativePath(svgfile);

        var result = EChartSSR.Instance.SaveAsSVGFile(width, height, option, uriSVG.Uri);
        if (result)
        {
            var v = LogicObject.GetVariable($"graph1_path");
            if (v != null)
            {
                v.Value = uriSVG;
            }
        }
    }

    private object buildGraphTrendOption(string batchNo)
    {

        var sql = string.Empty;
        sql = $"SELECT LocalTimestamp,V1,V2 FROM tbDatalog WHERE BatchNo = '{batchNo}' ORDER BY LocalTimestamp";
        dbBase.Query(sql, out var headers, out var resultSet);

        
        List<object[]> datas1 = new List<object[]>();
        List<object[]> datas2 = new List<object[]>();

        var rowCount = resultSet.GetLength(0);

        for (int i = 0; i < rowCount; i++)
        {
            var t = (DateTime)resultSet[i, 0];
            var ts = t.ToString("yyyy-MM-dd HH:mm:ss");
            var v1 = Convert.ToSingle(resultSet[i, 1]);
            var v2 = Convert.ToSingle(resultSet[i, 2]);
            datas1.Add(new object[] { ts, v1 });
            datas2.Add(new object[] { ts, v2 });



        }






        var option = new
        {
            legend= new {
            },
            xAxis = new
            {
                type = "time",
                axisLabel=new
                {
                    show = true,
                    formatter = new {
                        minute = "{HH}:{mm}",
                        second = "{HH}:{mm}:{ss}",
                    }
                }
            },
            yAxis = new
            {
                type = "value",
                min=0.0f,
                max=100.0f
            },
            series = new object[] {
                new {
                    name="V1",
                                data= datas1.ToArray(),
                  type = "line"
                },
                 new {
                     name="V2",
                                data= datas2.ToArray(),
                  type = "line"
                }
            },
            animation = false
        };



        return option;
    }


}

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
using FTOptix.Recipe;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using FTOptix.WebUI;
#endregion

public class Report_TimeRange_RuntimeNetLogic : BaseNetLogic
{

    Store dbBase;
    IUAVariable vStart,vEnd;
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        dbBase = LogicObject.GetAlias("DataStore") as Store;

        vStart = LogicObject.GetVariable("dtStart");
        vEnd = LogicObject.GetVariable("dtEnd");

    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void Query(string outputPdfFolder,DateTime dtStart,DateTime dtEnd,NodeId nResultId)
    {
        var report = Owner as Report;
        var pdfRootName = Session.BrowseName.Replace('-', '_') + DateTime.Now.ToString("yyyyMMddHHmmss.ffffff");

        var _rootName = Path.Combine(outputPdfFolder, pdfRootName );

        var strStart = DateTime.Now.ToString("yyyy-MM-dd") + "T" + dtStart.ToString("HH:mm:ss");

        var strEnd = DateTime.Now.ToString("yyyy-MM-dd") + "T" + dtEnd.ToString("HH:mm:ss");

        //var strStart = dtStart.ToString("yyyy-MM-dd HH:mm:ss");
        //var strEnd = dtEnd.ToString("yyyy-MM-dd HH:mm:ss");

        generateGraph(_rootName,1,buildGraphTrendOption(strStart,strEnd),1000,1000);





        var pdfFile = _rootName + ".pdf";

        var pdfUri = ResourceUri.FromProjectRelativePath(pdfFile);


        vStart.Value = strStart;
        vEnd.Value = strEnd;

        report.GeneratePdf(pdfUri,string.Empty,out var operationId);

        var ddd = new DelayedTask(() => {
            var n = InformationModel.Get(nResultId) as IUAVariable;
            if (n != null)
            {
                n.Value = pdfUri;
            }

        }, 1000, LogicObject);
        ddd.Start();
    }


    private void generateGraph(string root,int index,object option,int width,int height)
    {
        var svgfile = root + $"_{index}.svg";
        var uriSVG = ResourceUri.FromProjectRelativePath(svgfile);

        var result = EChartSSR.Render.Instance.RenderObjToSvgFile(width, height, option, uriSVG.Uri);
        if (result)
        {
            var v = LogicObject.GetVariable($"graph1_path");
            if (v != null)
            {
                v.Value = uriSVG;
            }
        }
    }

    private object buildGraphTrendOption(string dtStart,string dtEnd)
    {

        var sql = string.Empty;
        sql = $"SELECT LocalTimestamp,V1,V2,V3,V4 FROM tbDatalog WHERE LocalTimestamp >= '{dtStart}' and LocalTimestamp <= '{dtEnd}' ORDER BY LocalTimestamp";
        dbBase.Query(sql, out var headers, out var resultSet);

        List<object[]> datas1 = new List<object[]>();
        List<object[]> datas2 = new List<object[]>();
        List<object[]> datas3 = new List<object[]>();
        List<object[]> datas4 = new List<object[]>();

        var rowCount = resultSet.GetLength(0);

        for (int i = 0; i < rowCount; i++)
        {
            var t = (DateTime)resultSet[i, 0];
            var ts = t.ToString("yyyy-MM-dd HH:mm:ss");
            var v1 = Convert.ToSingle(resultSet[i, 1]);
            var v2 = Convert.ToSingle(resultSet[i, 2]);
            var v3 = Convert.ToSingle(resultSet[i, 3]);
            var v4 = Convert.ToSingle(resultSet[i, 4]);

            datas1.Add(new object[] { ts, v1 });
            datas2.Add(new object[] { ts, v2 });
            datas3.Add(new object[] { ts, v3 });
            datas4.Add(new object[] { ts, v4 });



        }










        var option = new
        {
            legend = new
            {
            },
            xAxis = new
            {
                type = "time",
                axisLabel = new
                {
                    show = true,
                    formatter = new
                    {
                        minute = "{HH}:{mm}",
                        second = "{HH}:{mm}:{ss}",
                    }
                }
            },
            yAxis = new
            {
                type = "value",
                min = 0.0f,
                max = 100.0f
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
                },
                new {
                    name="V3",
                                data= datas3.ToArray(),
                  type = "line"
                }
                ,new {
                    name="V4",
                                data= datas4.ToArray(),
                  type = "line"
                }
            },
            animation = false
        };





        return option;
    }


}

#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.DataLogger;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Report;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
using System.IO;
using EChartDotNet;
using EChartDotNet.Entities.data;
using EChartDotNet.Entities.series;
using EChartDotNet.Entities;
using System.Collections.Generic;
using EChartDotNet.Entities.axis;
#endregion

public class RuntimeNetLogic1 : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started

       
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        if(_task != null)
        {
            CloseDynamic();
        }
    }


    [ExportMethod]
    public void Change()
    {
        var svg = Owner.Get<AdvancedSVGImage>("AdvancedSVGImage1");
        var content = File.ReadAllText("C:\\Users\\YRen6\\OneDrive - Rockwell Automation, Inc\\Desktop\\AAA.svg");
    
        svg.SetImageContent(content);

    }


    private void changeSvg()
    {
        
   
            var option = JsonTools.ObjectToJson2(getGraph1Option());
            var result = EChartSSR.Render.Instance.RenderObjString(1000, 1000, option, out var content);
            var svg = Owner.Get("AdvancedSVGImage1") as AdvancedSVGImage;
      
            svg.SetImageContent(content);






            //Log.Info("11");


            var option2 = JsonTools.ObjectToJson2(getGraph2Option());
            var result2 = EChartSSR.Render.Instance.RenderObjString(1000, 1000, option2, out var content1);
            var svg2 = Owner.Get("AdvancedSVGImage2") as AdvancedSVGImage;

            svg2.SetImageContent(content1);


        

       


    }







    private ChartOption getGraph1Option()
    {
        var option = new ChartOption();
        option.title = new Title[]
        {
            new Title()
            {
                show = true,
                text = "this is the chart option generate"
            }
        };


        option.legend = new Legend() { show = true, top = "5%", left = "center" };
        option.animation = false;
        option.series = new List<object>()
            {
                new Pie()
                {
                    radius = new object[]{"40%","70%"},
                    label = new EChartDotNet.Entities.style.StyleLabel(){show= true,position =  StyleLabelTyle.center},
                    data = new List<Data>()
                    {
                        new Data(){value = rnd.Next(100),name="a"},
                        new Data(){value = rnd.Next(100),name="b"},
                        new Data(){value = rnd.Next(100),name="c"},
                        new Data(){value = rnd.Next(100),name="d"},
                        new Data(){value = rnd.Next(100),name="e"},

                    }

                }
            };
        return option;
    }


    private ChartOption getGraph2Option() {
        List<object[]> data1 = new List<object[]>() {
               new object[]{"2024-12-06 10:00:01", rnd.Next(50) },
               new object[]{"2024-12-06 11:00:01", rnd.Next(50) },
               new object[]{"2024-12-06 12:00:01", rnd.Next(50) },
               new object[]{"2024-12-06 13:00:01", rnd.Next(50) },


            };
        List<object[]> data2 = new List<object[]>() {
               new object[]{"2024-12-06 10:00:01", rnd.Next(100) },
               new object[]{"2024-12-06 11:00:01", rnd.Next(100) },
               new object[]{"2024-12-06 12:00:01", rnd.Next(100) },
               new object[]{"2024-12-06 13:00:01", rnd.Next(100) },


            };



        ChartOption option = new ChartOption();
        option.title = new Title[] {
                new Title()
                {
                    show = true,
                    text = "aa"
                }
            };





        option.legend = new Legend() { show = true };


        option.xAxis = new List<EChartDotNet.Entities.axis.Axis>()
            {

                new TimeAxis()
                {
                    type = AxisType.time,
                    axisLabel = new AxisLabel()
                    {
                        show = true,
						//formatter = new JRaw(" minute:'{HH}:{mm}',      second:'{HH}:{mm}:{ss}'").ToString(),
						formatter = new
                        {
                            minute = "{HH}:{mm}",
                            second = "{HH}:{mm}:{ss}"
                        }
                    }
                }

            };
        option.color = new string[] { "red", "#ff00ff" };
        option.yAxis = new List<EChartDotNet.Entities.axis.Axis>() {
                new EChartDotNet.Entities.axis.ValueAxis()
                {
                    type = AxisType.value,
                    min = 0,
                    max = 100
                }

            };

        option.series = new List<object>()
            {
                new Line()
                {
                    name = "a1",
                    data = data1 ,

                },
                    new Line()
                {
                    name = "a2",
                    data = data2
                }
            };
        option.animation = false;
        return option;
    }

    Random rnd = new Random(DateTime.Now.Second);
    PeriodicTask _task;
    [ExportMethod]
    public void OpenDynamic()
    {
        _task = new PeriodicTask(() => {
            changeSvg();
        
        
        }, 1000, LogicObject);
        _task.Start();
    }
    [ExportMethod]
    public void CloseDynamic() { 
        _task.Cancel();
    
        _task = null;
    }






}

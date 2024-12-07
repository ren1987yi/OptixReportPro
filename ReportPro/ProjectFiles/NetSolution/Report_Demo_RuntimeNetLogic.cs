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
using EChartDotNet.Entities;
using EChartDotNet.Entities.series;
using EChartDotNet.Entities.data;
using EChartDotNet;
#endregion

public class Report_Demo_RuntimeNetLogic : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }


    [ExportMethod]
    public void QueryAndRender(string outputPdfFolder,  NodeId nResultId)
    {
        var report = Owner as Report;
        var pdfRootName = Session.BrowseName.Replace('-', '_') + DateTime.Now.ToString("yyyyMMddHHmmss.ffffff");


        //graph1
        {
            var svgfile = pdfRootName + "_1.svg";
            var path = Path.Combine(outputPdfFolder, svgfile);
            var uriSVG = ResourceUri.FromProjectRelativePath(path);



            var option = getGraph1Option();
            var result = EChartSSR.Render.Instance.RenderObjToSvgFile(1000, 1000, option, uriSVG.Uri);
            if (result)
            {
                var v = LogicObject.GetVariable("graph1_Path");
                if (v != null)
                {
                    v.Value = uriSVG;
                }
            }

        }


        //graph2
        {
            var svgfile = pdfRootName + "_2.svg";
            var path = Path.Combine(outputPdfFolder, svgfile);
            var uriSVG = ResourceUri.FromProjectRelativePath(path);



            var option = getGraph2Option();
            var result = EChartSSR.Render.Instance.RenderOptionStringToSvgFile(1000, 1000, option, uriSVG.Uri);
            if (result)
            {
                var v = LogicObject.GetVariable("graph2_Path");
                if (v != null)
                {
                    v.Value = uriSVG;
                }
            }

        }


        //graph3
        {
            var svgfile = pdfRootName + "_3.svg";
            var path = Path.Combine(outputPdfFolder, svgfile);
            var uriSVG = ResourceUri.FromProjectRelativePath(path);



            var option = getGraph3Option_V2();
            var option2 = JsonTools.ObjectToJson2(option);
            var result = EChartSSR.Render.Instance.RenderObjStringToSvgFile(1000, 1000, option2, uriSVG.Uri);
            if (result)
            {
                var v = LogicObject.GetVariable("graph3_Path");
                if (v != null)
                {
                    v.Value = uriSVG;
                }
            }

        }

        //graph4
        {
            var svgfile = pdfRootName + "_4.svg";
            var path = Path.Combine(outputPdfFolder, svgfile);
            var uriSVG = ResourceUri.FromProjectRelativePath(path);



            var option = getGraph4Option();
            var result = EChartSSR.Render.Instance.RenderOptionStringToSvgFile(1000, 1000, option, uriSVG.Uri);
            if (result)
            {
                var v = LogicObject.GetVariable("graph4_Path");
                if (v != null)
                {
                    v.Value = uriSVG;
                }
            }

        }

        //graph5
        {
            var svgfile = pdfRootName + "_5.svg";
            var path = Path.Combine(outputPdfFolder, svgfile);
            var uriSVG = ResourceUri.FromProjectRelativePath(path);



            var option = getGraph5Option();
            var result = EChartSSR.Render.Instance.RenderOptionStringToSvgFile(1000, 1000, option, uriSVG.Uri);
            if (result)
            {
                var v = LogicObject.GetVariable("graph5_Path");
                if (v != null)
                {
                    v.Value = uriSVG;
                }
            }

        }




        var uid = Session.BrowseName.Replace('-', '_') + DateTime.Now.ToString("yyyyMMddHHmmss.ffffff") + ".pdf";
        var filepath = Path.Combine(outputPdfFolder, uid);
        var uri = ResourceUri.FromProjectRelativePath(filepath);

        Log.Info("Report", $"pdf file path : {uri.Uri}");
        report.GeneratePdf(uri, string.Empty, out var opId);
        Log.Info("Report", $"operationId:{opId}");


        var ddd = new DelayedTask(() => {
            var n = InformationModel.Get(nResultId) as IUAVariable;
            if (n != null)
            {
                n.Value = uri;
            }

        }, 1000, LogicObject);
        ddd.Start();
    }
    Random rnd = new Random(DateTime.Now.Second);









    private object getGraph1Option()
    {

        List<string> xdatas = new List<string>();
        List<float> ydatas = new List<float>();

        for (int i = 0; i < 10; i++)
        {
            xdatas.Add(i.ToString());
        }


        for (int i = 0; i < 10; i++)
        {
            ydatas.Add(rnd.Next(100));
        }


        var option = new
        {
            xAxis = new
            {
                type = "category",
                data = xdatas.ToArray()
            },
            yAxis = new
            {
                type = "value"
            },
            series = new object[] {
    new {
                    data= ydatas.ToArray(),
      type = "line"
    }
        },
            animation = false
        };



        return option;
    }


    private string getGraph2Option()
    {

        var option = @"{
  ""xAxis"": {
    ""type"": ""category"",
    ""data"": [
      ""Mon"",
      ""Tue"",
      ""Wed"",
      ""Thu"",
      ""Fri"",
      ""Sat"",
      ""Sun""
    ]
  },
  ""yAxis"": {
    ""type"": ""value""
  },
  ""series"": [
    {
      ""data"": [

        $KK$

      ],
      ""type"": ""bar""
    }
  ]
}";

        List<string> datas = new List<string>();

        for (int i = 0; i < 7; i++)
        {
            datas.Add(rnd.Next(100).ToString("f2"));
        }

        option = option.Replace("$KK$", string.Join(",", datas));


        return option;
    }


    private string getGraph3Option()
    {

        var option = @"{
  ""tooltip"": {
    ""trigger"": ""item""
  },
  ""legend"": {
    ""top"": ""5%"",
    ""left"": ""center""
  },
  ""series"": [
    {
      ""name"": ""Access From"",
      ""type"": ""pie"",
      ""radius"": [
        ""40%"",
        ""70%""
      ],
      ""avoidLabelOverlap"": false,
      ""itemStyle"": {
        ""borderRadius"": 10,
        ""borderColor"": ""#fff"",
        ""borderWidth"": 2
      },
      ""label"": {
        ""show"": false,
        ""position"": ""center""
      },
      ""emphasis"": {
        ""label"": {
          ""show"": true,
          ""fontSize"": 40,
          ""fontWeight"": ""bold""
        }
      },
      ""labelLine"": {
        ""show"": false
      },
      ""data"": [
        {
          ""value"": 1048,
          ""name"": ""Search Engine""
        },
        {
          ""value"": 735,
          ""name"": ""Direct""
        },
        {
          ""value"": 580,
          ""name"": ""Email""
        },
        {
          ""value"": 484,
          ""name"": ""Union Ads""
        },
        {
          ""value"": 300,
          ""name"": ""Video Ads""
        }
      ]
    }
  ]
}";




        var a = Newtonsoft.Json.JsonConvert.SerializeObject(option);

        return option;
    }


    private ChartOption getGraph3Option_V2()
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


    private string getGraph4Option()
    {

        var option = @"{
  ""xAxis"": {},
  ""yAxis"": {},
  ""series"": [
    {
      ""symbolSize"": 20,
      ""data"": [
        [
          10.0,
          8.04
        ],
        [
          8.07,
          6.95
        ],
        [
          13.0,
          7.58
        ],
        [
          9.05,
          8.81
        ],
        [
          11.0,
          8.33
        ],
        [
          14.0,
          7.66
        ],
        [
          13.4,
          6.81
        ],
        [
          10.0,
          6.33
        ],
        [
          14.0,
          8.96
        ],
        [
          12.5,
          6.82
        ],
        [
          9.15,
          7.2
        ],
        [
          11.5,
          7.2
        ],
        [
          3.03,
          4.23
        ],
        [
          12.2,
          7.83
        ],
        [
          2.02,
          4.47
        ],
        [
          1.05,
          3.33
        ],
        [
          4.05,
          4.96
        ],
        [
          6.03,
          7.24
        ],
        [
          12.0,
          6.26
        ],
        [
          12.0,
          8.84
        ],
        [
          7.08,
          5.82
        ],
        [
          5.02,
          5.68
        ]
      ],
      ""type"": ""scatter""
    }
  ]
}";


        return option;
    }



    private string getGraph5Option()
    {

        var option = @"{
  ""xAxis"": {
    ""data"": [
      ""2017-10-24"",
      ""2017-10-25"",
      ""2017-10-26"",
      ""2017-10-27""
    ]
  },
  ""yAxis"": {},
  ""series"": [
    {
      ""type"": ""candlestick"",
      ""data"": [
        [
          20,
          34,
          10,
          38
        ],
        [
          40,
          35,
          30,
          50
        ],
        [
          31,
          38,
          33,
          44
        ],
        [
          38,
          15,
          5,
          42
        ]
      ]
    }
  ]
}";


        return option;
    }



}

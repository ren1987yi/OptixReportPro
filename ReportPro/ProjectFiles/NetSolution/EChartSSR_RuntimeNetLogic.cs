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
using System.Threading;
#endregion
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using Microsoft.ClearScript;
using System.IO;
using FTOptix.WebUI;
public class EChartSSR_RuntimeNetLogic : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        var task = new LongRunningTask(() => {



            if (!EChartSSR.Instance.Inited)
            {
                var uri = ResourceUri.FromProjectRelativePath("jsLibrary\\");
                var folder = uri.Uri;
                EChartSSR.Instance.Init(folder);

                Log.Info("EChartSSR", "init ok");

            }

        }, LogicObject);

        task.Start();
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
}



public sealed class EChartSSR
{
    //static readonly string jsFolder = @"D:\code\csharp\Test\DotNetEchartSSR\bin\Debug\net6.0\echarts\";
    static readonly EChartSSR instance = new EChartSSR();
    static readonly V8ScriptEngine engine = new V8ScriptEngine();

    bool _inited;
    public bool Inited
    {
        get => _inited;

    }

    private EChartSSR()
    {

    }

    public static EChartSSR Instance
    {
        get
        {
            return instance;
        }
    }

    private void init()
    {
        engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableAllLoading;

        engine.AddHostType(typeof(Console));

        Action<ScriptObject, int> setTimeout = (func, delay) =>
        {
            var timer = new Timer(_ => func.Invoke(false));
            timer.Change(delay, Timeout.Infinite);
        };

        engine.Script._setTimeout = setTimeout;
        engine.Execute(@"
var echarts;

    function setTimeout(func, delay) {
        let args = Array.prototype.slice.call(arguments, 2);
        _setTimeout(func.bind(undefined, ...args), delay || 0);
    }

");

        engine.Execute(new DocumentInfo() { Category = ModuleCategory.CommonJS }, @"

                 echarts = require('echarts.min.js');
            ");

        engine.Execute(@"
    function renderChart(width,height,optionString){
    let chart = echarts.init(null, null, {
                  renderer: 'svg',
                  ssr: true,
                  width: width,
                  height: height,
                });
var option =JSON.parse(optionString);
chart.setOption(option);

let svgStr = chart.renderToSVGString();
return svgStr
}

");
    }

    public bool RenderSVG(int width, int height, object option, out string svgString)
    {
        try
        {
            var ss = Newtonsoft.Json.JsonConvert.SerializeObject(option);

            svgString = engine.Script.renderChart(width, height, ss);
            return true;
        }
        catch (Exception ex)
        {
            svgString = null;
            return false;
        }
    }

    public void Init(string jsFolder)
    {
        if (!_inited)
        {

            engine.DocumentSettings.SearchPath = jsFolder;
            init();
            _inited = true;
        }

    }


    public bool RenderSVG(int width, int height, string optionString, out string svgString)
    {
        try
        {
            svgString = engine.Script.renderChart(width, height, optionString);
            return true;
        }
        catch (Exception ex)
        {
            svgString = null;
            return false;
        }
    }


    public bool SaveAsSVGFile(int width, int height, string optionString, string filepath)
    {
        if (!Inited)
        {
            return false;
        }

        var result = RenderSVG(width, height, optionString, out var svgContent);
        if (result)
        {
            File.WriteAllText(filepath, svgContent);
        }
        return result;
    }


    public bool SaveAsSVGFile(int width, int height, object option, string filepath)
    {
        if (!Inited)
        {
            return false;
        }

        var result = RenderSVG(width, height, option, out var svgContent);
        if (result)
        {
            File.WriteAllText(filepath, svgContent);
        }
        return result;
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using System.Linq;
using System.Threading.Tasks;
using Console = Colorful.Console;//这个是重点

namespace Clipper2Lib
{
  public static class LogHelper
  {
    public static void Debug(object res)
    {
      Console.WriteLine(res);
    }

    public static void Info(object res)
    {
      Console.WriteLine(res, Color.LightGreen);
    }

    public static void Error(object res)
    {
      Console.WriteLine(res, Color.Red);
    }

    public static void Warning(object res)
    {
      Console.WriteLine(res, Color.Yellow);
    }

    public static void Blue(object res)
    {
      Console.WriteLine(res, Color.Blue);
    }
    public static void Red(object res)
    {
      Console.WriteLine(res, Color.Red);
    }
    public static void Orange(object res)
    {
      Console.WriteLine(res, Color.Orange);
    }
    public static void Cyan(object res)
    {
      Console.WriteLine(res, Color.Cyan);
    }
    public static void Gray(object res)
    {
      Console.WriteLine(res, Color.Gray);
    }
    public static void Tan(object res)
    {
      Console.WriteLine(res, Color.Tan);
    }
    public static void Snow(object res)
    {
      Console.WriteLine(res, Color.Snow);
    }
    public static void Honeydew(object res)
    {
      Console.WriteLine(res, Color.Honeydew);
    }
    public static void HotPink(object res)
    {
      Console.WriteLine(res, Color.HotPink);
    }
    public static void Brown(object res)
    {
      Console.WriteLine(res, Color.Brown);
    }
  }
}


//namespace WeatherApi.Utils
//{
//  public static class LogHelper
//  {
//    public static void Debug(object res)
//    {
//      Console.WriteLine(res);
//    }

//    public static void Info(object res)
//    {
//      Console.WriteLine(res, Color.LightGreen);
//    }

//    public static void Error(object res)
//    {
//      Console.WriteLine(res, Color.Red);
//    }

//    public static void Warning(object res)
//    {
//      Console.WriteLine(res, Color.Yellow);
//    }

//  }
//}



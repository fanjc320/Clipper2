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



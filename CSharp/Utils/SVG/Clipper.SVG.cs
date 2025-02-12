/*******************************************************************************
* Author    :  Angus Johnson                                                   *
* Date      :  24 March 2024                                                   *
* Website   :  https://www.angusj.com                                          *
* Copyright :  Angus Johnson 2010-2024                                         *
* License   :  https://www.boost.org/LICENSE_1_0.txt                           *
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Clipper2Lib
{
  public class SvgWriter
  {
    public const uint black = 0xFF000000;
    public const uint white = 0xFFFFFFFF;
    public const uint maroon = 0xFF800000;

    public const uint navy = 0xFF000080;
    public const uint blue = 0xFF0000FF;
    public const uint red = 0xFFFF0000;
    public const uint green = 0xFF008000;
    public const uint yellow = 0xFFFFFF00;
    public const uint lime = 0xFF00FF00;
    public const uint fuscia = 0xFFFF00FF;
    public const uint aqua = 0xFF00FFFF;

    private static RectD rectMax = Clipper.InvalidRectD;
    public static RectD RectMax => rectMax;

    private static RectD rectEmpty = new RectD(true);
    public static RectD RectEmpty => rectEmpty;
    internal static bool IsValidRect(RectD rec)
    {
      return rec.right >= rec.left && rec.bottom >= rec.top;
    }

    private readonly struct CoordStyle
    {
      public readonly string FontName;
      public readonly int FontSize;
      public readonly uint FontColor;
      public CoordStyle(string fontname, int fontsize, uint fontcolor)
      {
        FontName = fontname;
        FontSize = fontsize;
        FontColor = fontcolor;
      }
    }

    private readonly struct TextInfo
    {
      public readonly string text;
      public readonly int fontSize;
      public readonly uint fontColor;
      public readonly double posX;
      public readonly double posY;
      public TextInfo(string text, double x, double y,
        int fontsize = 12, uint fontcolor = black)
      {
        this.text = text;
        posX = x;
        posY = y;
        fontSize = fontsize;
        fontColor = fontcolor;
      }
    }

    private readonly struct PolyInfo
    {
      public readonly PathsD paths;
      public readonly uint BrushClr;
      public readonly uint PenClr;
      public readonly double PenWidth;
      public readonly bool ShowCoords;
      public readonly bool IsOpen;
      public PolyInfo(PathsD paths, uint brushcolor, uint pencolor,
        double penwidth, bool showcoords = false, bool isopen = false)
      {
        this.paths = new PathsD(paths);
        BrushClr = brushcolor;
        PenClr = pencolor;
        PenWidth = penwidth;
        ShowCoords = showcoords;
        IsOpen = isopen;
      }
    }

    public FillRule FillRule { get; set; }
    private readonly List<PolyInfo> PolyInfoList = new List<PolyInfo>();
    private readonly List<TextInfo> textInfos = new List<TextInfo>();
    private readonly CoordStyle coordStyle;

    private const string svg_header = "<?xml version=\"1.0\" standalone=\"no\"?>\n" +
      "<svg width=\"{0}px\" height=\"{1}px\" viewBox=\"0 0 {0} {1}\"" +
      " version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">\n\n";
    private const string svg_path_format = "\"\n style=\"fill:{0};" +
        " fill-opacity:{1:f2}; fill-rule:{2}; stroke:{3};" +
        " stroke-opacity:{4:f2}; stroke-width:{5:f2};\"/>\n\n";
    private const string svg_path_format2 = "\"\n style=\"fill:none; stroke:{0};" +
        "stroke-opacity:{1:f2}; stroke-width:{2:f2};\"/>\n\n";

    public SvgWriter(FillRule fillrule = FillRule.EvenOdd,
      string coordFontName = "Verdana", int coordFontsize = 9, uint coordFontColor = black)
    {
      coordStyle = new CoordStyle(coordFontName, coordFontsize, coordFontColor);
      FillRule = fillrule;
    }

    public void ClearPaths()
    {
      PolyInfoList.Clear();
    }

    public void ClearText()
    {
      textInfos.Clear();
    }

    public void ClearAll()
    {
      PolyInfoList.Clear();
      textInfos.Clear();
    }
    public void AddClosedPath(Path64 path, uint brushColor,
      uint penColor, double penWidth, bool showCoords = false)
    {
      Paths64 tmp = new Paths64();
      tmp.Add(path);
      AddClosedPaths(tmp, brushColor, penColor, penWidth, showCoords);
    }

    public void AddClosedPath(PathD path, uint brushColor,
      uint penColor, double penWidth, bool showCoords = false)
    {
      PathsD tmp = new PathsD();
      tmp.Add(path);
      AddClosedPaths(tmp, brushColor, penColor, penWidth, showCoords);
    }

    public void AddClosedPaths(Paths64 paths, uint brushColor,
      uint penColor, double penWidth, bool showCoords = false)
    {
      if (paths.Count == 0) return;
      PolyInfoList.Add(new PolyInfo(Clipper.PathsD(paths),
        brushColor, penColor, penWidth, showCoords, false));
    }

    public void AddClosedPaths(PathsD paths, uint brushColor,
      uint penColor, double penWidth, bool showCoords = false)
    {
      if (paths.Count == 0) return;
      PolyInfoList.Add(new PolyInfo(paths,
        brushColor, penColor, penWidth, showCoords, false));
    }

    public void AddOpenPath(Path64 path,  uint penColor, 
      double penWidth, bool showCoords = false)
    {
      Paths64 tmp = new Paths64();
      tmp.Add(path);
      AddOpenPaths(tmp, penColor, penWidth, showCoords);
    }

    public void AddOpenPath(PathD path, uint penColor, 
      double penWidth, bool showCoords = false)
    {
      PathsD tmp = new PathsD();
      tmp.Add(path);
      AddOpenPaths(tmp, penColor, penWidth, showCoords);
    }

    public void AddOpenPaths(Paths64 paths,
      uint penColor, double penWidth, bool showCoords = false)
    {
      if (paths.Count == 0) return;
      PolyInfoList.Add(new PolyInfo(Clipper.PathsD(paths),
        0x0, penColor, penWidth, showCoords, true));
    }

    public void AddOpenPaths(PathsD paths, uint penColor, 
      double penWidth, bool showCoords = false)
    {
      if (paths.Count == 0) return;
      PolyInfoList.Add(new PolyInfo(paths,
        0x0, penColor, penWidth, showCoords, true));
    }


    public void AddText(string cap, double posX, double posY, int fontSize, uint fontClr = black)
    {
      textInfos.Add(new TextInfo(cap, posX, posY, fontSize, fontClr));
    }

    private RectD GetBounds()
    {
      RectD bounds = new RectD(RectMax);
      foreach (PolyInfo pi in PolyInfoList)
        foreach (PathD path in pi.paths)
          foreach (PointD pt in path)
          {
            if (pt.x < bounds.left) bounds.left = Math.Min(pt.x, 0.0);
            if (pt.x > bounds.right) bounds.right = Math.Max(pt.x, 400.0);
            if (pt.y < bounds.top) bounds.top = Math.Min(pt.y, 0.0);
            if (pt.y > bounds.bottom) bounds.bottom = Math.Max(pt.y, 400.0);
          }
      return !IsValidRect(bounds) ? RectEmpty : bounds;
    }

    private static string ColorToHtml(uint clr)
    {
      return '#' + (clr & 0xFFFFFF).ToString("X6");
    }

    private static float GetAlpha(uint clr)
    {
      return ((float) (clr >> 24) / 255);
    }

    public bool SaveToFile(string filename, int maxWidth = 0, int maxHeight = 0, int margin = -1)
    {
      if (margin < 0) margin = 20;
      RectD bounds = GetBounds();
      if (bounds.IsEmpty()) 
        return false;

      double scale = 1.0;
      if (maxWidth > 0 && maxHeight > 0)
        scale = Math.Min(
           (maxWidth - margin * 2) / bounds.Width,
            (maxHeight - margin * 2) / bounds.Height);

      long offsetX = margin - (long) (bounds.left * scale);
      long offsetY = margin - (long) (bounds.top * scale);

      StreamWriter writer;
      try
      {
        writer = new StreamWriter(filename);
      }
      catch
      {
        return false;
      }

      if (maxWidth <= 0 || maxHeight <= 0)
        writer.Write(svg_header, (bounds.right - bounds.left) + margin * 2,
          (bounds.bottom - bounds.top) + margin * 2);
      else
        writer.Write(svg_header, maxWidth, maxHeight);

      if (true)
      {
        string show_coordinate = "<!-- Background -->\r\n  <rect width=\"100%\" height=\"100%\" fill=\"#f0f0f0\" />\r\n\r\n  <!-- Grid Lines (every 50 units) -->\r\n  <!-- Vertical Grid Lines -->\r\n  <path d=\"M50,0 V400 M100,0 V400 M150,0 V400 M200,0 V400 M250,0 V400 M300,0 V400 M350,0 V400\" \r\n        stroke=\"#ccc\" stroke-width=\"1\" fill=\"none\" />\r\n  <!-- Horizontal Grid Lines -->\r\n  <path d=\"M0,50 H400 M0,100 H400 M0,150 H400 M0,200 H400 M0,250 H400 M0,300 H400 M0,350 H400\" \r\n        stroke=\"#ccc\" stroke-width=\"1\" fill=\"none\" />\r\n\r\n  <!-- Axes -->\r\n  <line x1=\"0\" y1=\"0\" x2=\"400\" y2=\"0\" stroke=\"black\" stroke-width=\"2\" /> <!-- X-axis -->\r\n  <line x1=\"0\" y1=\"0\" x2=\"0\" y2=\"400\" stroke=\"black\" stroke-width=\"2\" /> <!-- Y-axis -->\r\n\r\n  <!-- X-axis Tick Marks and Labels (every 50 units) -->\r\n  <g id=\"x-axis-ticks\">\r\n    <line x1=\"50\" y1=\"0\" x2=\"50\" y2=\"10\" stroke=\"black\" /> <!-- Tick at x=50 -->\r\n    <text x=\"50\" y=\"25\" font-size=\"12\" text-anchor=\"middle\">50</text>\r\n    \r\n    <line x1=\"100\" y1=\"0\" x2=\"100\" y2=\"10\" stroke=\"black\" /> <!-- Tick at x=100 -->\r\n    <text x=\"100\" y=\"25\" font-size=\"12\" text-anchor=\"middle\">100</text>\r\n    \r\n    <line x1=\"150\" y1=\"0\" x2=\"150\" y2=\"10\" stroke=\"black\" /> <!-- Tick at x=150 -->\r\n    <text x=\"150\" y=\"25\" font-size=\"12\" text-anchor=\"middle\">150</text>\r\n    \r\n    <!-- Repeat for remaining ticks... -->\r\n    <line x1=\"200\" y1=\"0\" x2=\"200\" y2=\"10\" stroke=\"black\" />\r\n    <text x=\"200\" y=\"25\" font-size=\"12\" text-anchor=\"middle\">200</text>\r\n    \r\n    <line x1=\"250\" y1=\"0\" x2=\"250\" y2=\"10\" stroke=\"black\" />\r\n    <text x=\"250\" y=\"25\" font-size=\"12\" text-anchor=\"middle\">250</text>\r\n    \r\n    <line x1=\"300\" y1=\"0\" x2=\"300\" y2=\"10\" stroke=\"black\" />\r\n    <text x=\"300\" y=\"25\" font-size=\"12\" text-anchor=\"middle\">300</text>\r\n    \r\n    <line x1=\"350\" y1=\"0\" x2=\"350\" y2=\"10\" stroke=\"black\" />\r\n    <text x=\"350\" y=\"25\" font-size=\"12\" text-anchor=\"middle\">350</text>\r\n  </g>\r\n\r\n  <!-- Y-axis Tick Marks and Labels (every 50 units) -->\r\n  <g id=\"y-axis-ticks\">\r\n    <line x1=\"0\" y1=\"50\" x2=\"10\" y2=\"50\" stroke=\"black\" /> <!-- Tick at y=50 -->\r\n    <text x=\"25\" y=\"55\" font-size=\"12\" text-anchor=\"start\">50</text>\r\n    \r\n    <line x1=\"0\" y1=\"100\" x2=\"10\" y2=\"100\" stroke=\"black\" /> <!-- Tick at y=100 -->\r\n    <text x=\"25\" y=\"105\" font-size=\"12\" text-anchor=\"start\">100</text>\r\n    \r\n    <line x1=\"0\" y1=\"150\" x2=\"10\" y2=\"150\" stroke=\"black\" /> <!-- Tick at y=150 -->\r\n    <text x=\"25\" y=\"155\" font-size=\"12\" text-anchor=\"start\">150</text>\r\n    \r\n    <!-- Repeat for remaining ticks... -->\r\n    <line x1=\"0\" y1=\"200\" x2=\"10\" y2=\"200\" stroke=\"black\" />\r\n    <text x=\"25\" y=\"205\" font-size=\"12\" text-anchor=\"start\">200</text>\r\n    \r\n    <line x1=\"0\" y1=\"250\" x2=\"10\" y2=\"250\" stroke=\"black\" />\r\n    <text x=\"25\" y=\"255\" font-size=\"12\" text-anchor=\"start\">250</text>\r\n    \r\n    <line x1=\"0\" y1=\"300\" x2=\"10\" y2=\"300\" stroke=\"black\" />\r\n    <text x=\"25\" y=\"305\" font-size=\"12\" text-anchor=\"start\">300</text>\r\n    \r\n    <line x1=\"0\" y1=\"350\" x2=\"10\" y2=\"350\" stroke=\"black\" />\r\n    <text x=\"25\" y=\"355\" font-size=\"12\" text-anchor=\"start\">350</text>\r\n  </g>\r\n\r\n  <!-- Axis Labels -->\r\n  <text x=\"380\" y=\"20\" font-size=\"14\" fill=\"black\">X</text> <!-- X-axis label -->\r\n  <text x=\"20\" y=\"380\" font-size=\"14\" fill=\"black\">Y</text> <!-- Y-axis label -->\r\n\r\n  <!-- Example Point at (100, 100) -->\r\n  <circle cx=\"100\" cy=\"100\" r=\"3\" fill=\"red\" />\r\n  <text x=\"105\" y=\"105\" font-size=\"10\" fill=\"red\">(100, 100)</text>";
        writer.Write(string.Format(NumberFormatInfo.InvariantInfo, show_coordinate));
      }

      foreach (PolyInfo pi in PolyInfoList)
      {
        writer.Write(" <path id = \"test\" d=\"");
        foreach (PathD path in pi.paths)
        {
          if (path.Count < 2) continue;
          if (!pi.IsOpen && path.Count < 3) continue;
          writer.Write(string.Format(NumberFormatInfo.InvariantInfo, " M {0:f2} {1:f2}",
              (path[0].x * scale + offsetX),
              (path[0].y * scale + offsetY)));
          for (int j = 1; j < path.Count; j++)
          {
            writer.Write(string.Format(NumberFormatInfo.InvariantInfo, " L {0:f2} {1:f2}",
            (path[j].x * scale + offsetX),
            (path[j].y * scale + offsetY)));
          }
          if (!pi.IsOpen) writer.Write(" z");
        }

        foreach (PathD path in pi.paths)
        {
          Console.WriteLine("----------- new path -----------------");
          for (int j = 0; j < path.Count; j++)
          {
            Console.WriteLine("svg path point:" + path[j].ToString());
          }
        }

        if (!pi.IsOpen)
          writer.Write(string.Format(NumberFormatInfo.InvariantInfo, svg_path_format,
              ColorToHtml(pi.BrushClr), GetAlpha(pi.BrushClr),
              (FillRule == FillRule.EvenOdd ? "evenodd" : "nonzero"),
              ColorToHtml(pi.PenClr), GetAlpha(pi.PenClr), pi.PenWidth));
        else
          writer.Write(string.Format(NumberFormatInfo.InvariantInfo, svg_path_format2,
              ColorToHtml(pi.PenClr), GetAlpha(pi.PenClr), pi.PenWidth));

        if (true)
        {
          string show_path_format = "\"\n <circle r=\"10\" fill=\"skyblue\">" +
            "<animateMotion dur=\"{0}s\" repeatCount=\"indefinite\">" +
      "<mpath href=\"#test\"></mpath>" +
    "</animateMotion>" +
  "</circle> \"  \n\n ";
    writer.Write(string.Format(NumberFormatInfo.InvariantInfo, show_path_format,  10));
        }

        if (!pi.ShowCoords) continue;
        {
          writer.Write("<g font-family=\"{0}\" font-size=\"{1}\" fill=\"{2}\">\n", 
            coordStyle.FontName, coordStyle.FontSize, ColorToHtml(coordStyle.FontColor));
          foreach (PathD path in pi.paths)
          {
            foreach (PointD pt in path)
            {
#if USINGZ
              writer.Write("<text x=\"{0:f2}\" y=\"{1:f2}\">{2:f2},{3:f2},{4}</text>\n", 
                (pt.x * scale + offsetX), (pt.y * scale + offsetY), pt.x, pt.y, pt.z);
#else
              writer.Write("<text x=\"{0:f2}\" y=\"{1:f2}\">{2:f2},{3:f2}</text>\n", 
                (pt.x * scale + offsetX), (pt.y * scale + offsetY), pt.x, pt.y);
#endif
            }
          }
          writer.Write("</g>\n\n");
        }
      }

      foreach (TextInfo captionInfo in textInfos)
      {
        writer.Write("<g font-family=\"Verdana\" font-style=\"normal\" " +
                     "font-weight=\"normal\" font-size=\"{0}\" fill=\"{1}\">\n", 
                     captionInfo.fontSize, ColorToHtml(captionInfo.fontColor));
        writer.Write("<text x=\"{0:f2}\" y=\"{1:f2}\">{2}</text>\n</g>\n", 
          captionInfo.posX * scale + offsetX, captionInfo.posY * scale + offsetY, captionInfo.text);
      }

      writer.Write("</svg>\n");
      writer.Close();
      return true;
    }
  }

}

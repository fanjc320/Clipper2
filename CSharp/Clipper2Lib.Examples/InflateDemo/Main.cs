/*******************************************************************************
* Author    :  Angus Johnson                                                   *
* Date      :  24 September 2023                                               *
* Website   :  https://www.angusj.com                                          *
* Copyright :  Angus Johnson 2010-2023                                         *
* License   :  https://www.boost.org/LICENSE_1_0.txt                           *
*******************************************************************************/

using System;
using System.IO;
using System.Reflection;
using Clipper2Lib;

namespace ClipperDemo1
{

  public static class Application
  {

    public static void Main()
    {
      //DoSimpleShapes();
      //DoSimpleInflatePaths();
      //DoSimpleInflatePaths1();
      //DoSimpleInflatePaths2();
      //DoSimpleClipperOffset();
      //DoSimpleClipperOffset1();
      //DoSimpleClipperOffset2();
      //DoSimpleClipperOffset3();
      DoSimpleClipperOffset4();
      //DoRabbit();
      //DoVariableOffset();
    }

    public static void DoSimpleShapes()
    {
      SvgWriter svg = new();

      PathsD pp = new() { Clipper.MakePath(new double[] { 30,150, 60,350, 0,350 }) };
      PathsD solution = new();
      for (int i = 0; i < 5; ++i)
      {
        //nb: the last parameter here (10) greatly increases miter limit
        pp = Clipper.InflatePaths(pp, 5, JoinType.Miter, EndType.Polygon, 10);
        solution.AddRange(pp);//AddRange：添加实现了ICollection接口的一个集合的所有元素到指定集合的末尾
      }
      SvgUtils.AddSolution(svg, solution, false);

      solution.Clear();
      solution.Add(Clipper.MakePath(new double[] { 100, 0, 340, 0, 340, 200, 100, 200 }));
      
      solution.Add(Clipper.TranslatePath(solution[0], 60, 50));
      solution.Add(Clipper.TranslatePath(solution[1], 100, 50));
      SvgUtils.AddOpenSubject(svg, solution);

      ClipperOffset co = new();
      // because ClipperOffset only accepts Int64 paths, scale them 
      double scale = 10;//没影响，因为下面重新scale回来了
      Paths64 pp64 = Clipper.ScalePaths64(solution, scale);
      co.AddPath(pp64[0], JoinType.Bevel, EndType.Joined);
      co.AddPath(pp64[1], JoinType.Square, EndType.Joined);
      co.AddPath(pp64[2], JoinType.Round, EndType.Joined);
      co.Execute(10 * scale, pp64);
      // now de-scale the offset solution
      solution = Clipper.ScalePathsD(pp64, 1 / scale);

      const string filename = "../../../inflate.svg";
      SvgUtils.AddSolution(svg, solution, true);
      //SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 800, 600, 40);
      SvgUtils.SaveToFile(svg, filename, FillRule.Positive, 800, 600, 40);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoSimpleTest()
    {
      SvgWriter svg = new();

      //TRIANGLE OFFSET - WITH LARGE MITER

      PathsD pp = new() { Clipper.MakePath(new double[] { 30, 150, 160, 350, 0, 350 }) };
      PathsD solution = new();
      PathsD pRect = new() { Clipper.MakePath(new double[] { 20, 260, 40, 260, 40, 300, 20, 300 }) };
      solution.AddRange(pRect);
      PathsD pRect1 = new() { Clipper.MakePath(new double[] { 0, 260, 60, 260, 60, 300, 00, 300 }) };
      solution.AddRange(pRect1);
      for (int i = 0; i < 5; ++i)
      {
        //nb: the last parameter here (10) greatly increases miter limit
        pp = Clipper.InflatePaths(pp, -5, JoinType.Miter, EndType.Polygon, 10);
        solution.AddRange(pp);//AddRange：添加实现了ICollection接口的一个集合的所有元素到指定集合的末尾
      }
      //SvgUtils.AddSolution(svg, solution, false);
      SvgUtils.AddSolution(svg, solution, true);

      // RECTANGLE OFFSET - BEVEL, SQUARED AND ROUNDED

      solution.Clear();
      solution.Add(Clipper.MakePath(new double[] { 100, 0, 340, 0, 340, 200, 100, 200 }));
      PathsD pTri = new() { Clipper.MakePath(new double[] { 250, 50, 300, 150, 140, 150 }) };
      solution.Add(pTri[0]);

      //solution.Add(Clipper.TranslatePath(solution[0], 60, 50));
      //solution.Add(Clipper.TranslatePath(solution[1], 100, 50));
      //SvgUtils.AddOpenSubject(svg, solution);
      SvgUtils.AddSolution(svg, solution, true);

      // nb: rather than using InflatePaths(), we have to use the 
      // ClipperOffest class directly because we want to perform
      // different join types in a single offset operation
      ClipperOffset co = new();
      //co.ReverseSolution = false;
      // because ClipperOffset only accepts Int64 paths, scale them 
      // so the de-scaled offset result will have greater precision
      double scale = 100;
      Paths64 pp64 = Clipper.ScalePaths64(solution, scale);
      //co.AddPath(pp64[0], JoinType.Bevel, EndType.Joined);
      //co.AddPath(pp64[0], JoinType.Miter, EndType.Polygon);
      //co.AddPath(pp64[1], JoinType.Square, EndType.Joined);
      //co.AddPath(pp64[2], JoinType.Round, EndType.Joined);
      for (int i =0;i< pp64.Count; ++i)
      {
        //https://angusj.com/clipper2/Docs/Units/Clipper/Types/EndType.htm
        //co.AddPath(pp64[i], JoinType.Miter, EndType.Polygon);//有bug
        co.AddPath(pp64[i], JoinType.Square, EndType.Joined);//里面一圈，外面一圈
      }
      
      co.Execute(-10 * scale, pp64);
      // now de-scale the offset solution
      solution = Clipper.ScalePathsD(pp64, 1 / scale);

      const string filename = "../../../inflate_test.svg";
      SvgUtils.AddSolution(svg, solution, false);

      SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 800, 600, 40);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoSimpleInflatePaths()//平行方框和三角
    {
      SvgWriter svg = new();
      PathsD solution = new();

      //TRIANGLE OFFSET - WITH LARGE MITER//斜接
      PathsD pp = new() { Clipper.MakePath(new double[] { 100, 350, 200, 550, 10, 550 }) };
      PathsD pRect = new() { Clipper.MakePath(new double[] { 0, 300, 300, 300, 300, 600, 0, 600 }) };
      solution.AddRange(pRect);
      //PathsD pRect1 = new() { Clipper.MakePath(new double[] { 0, 260, 60, 260, 60, 300, 00, 300 }) };
      //solution.AddRange(pRect1);
      for (int i = 0; i < 5; ++i)
      {
        //nb: the last parameter here (10) greatly increases miter limit
        pp = Clipper.InflatePaths(pp, -10, JoinType.Miter, EndType.Polygon, 10);
        solution.AddRange(pp);//AddRange：添加实现了ICollection接口的一个集合的所有元素到指定集合的末尾
        //pRect = Clipper.InflatePaths(pRect, -10, JoinType.Miter, EndType.Polygon, 10);
        pRect = Clipper.InflatePaths(pRect, -10, JoinType.Miter, EndType.Joined, 15);
        solution.AddRange(pRect);
      }
      SvgUtils.AddSolution(svg, solution, false);
      //SvgUtils.AddSolution(svg, solution, true);

      const string filename = "../../../inflate_inflatePaths.svg";
      SvgUtils.AddSolution(svg, solution, false);

      SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 800, 600, 40);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoSimpleInflatePaths1()//只显示一个平行方框
    {
      SvgWriter svg = new();
      PathsD solution = new();

      //TRIANGLE OFFSET - WITH LARGE MITER//斜接
      PathsD pTri = new() { Clipper.MakePath(new double[] { 50, 350, 200, 550, 10, 550 }) };
      //PathsD pRect = new() { Clipper.MakePath(new double[] { 0, 300, 300, 300, 300, 600, 0, 600 }) };
      PathsD pRect = new() { Clipper.MakePath(new double[] { 0, 300, 0, 600, 300, 600, 300, 300 }) };//和上面的一样，都是外绿内白
      solution.AddRange(pRect);
      //PathsD pRect1 = new() { Clipper.MakePath(new double[] { 0, 260, 60, 260, 60, 300, 00, 300 }) };
      //solution.AddRange(pRect1);
      for (int i = 0; i < 1; ++i)
      {
        //nb: the last parameter here (10) greatly increases miter limit
        //pTri = Clipper.InflatePaths(pTri, -20, JoinType.Miter, EndType.Polygon, 10);
        //solution.AddRange(pTri);//AddRange：添加实现了ICollection接口的一个集合的所有元素到指定集合的末尾
        pRect = Clipper.InflatePaths(pRect, -10, JoinType.Miter, EndType.Polygon, 10);//单边的，外到内->绿，白
        //pRect = Clipper.InflatePaths(pRect, 10, JoinType.Square, EndType.Joined, 0);//双边的,外到内->绿白绿
        solution.AddRange(pRect);
      }
      SvgUtils.AddSolution(svg, solution, false);
      //SvgUtils.AddSolution(svg, solution, true);

      const string filename = "../../../inflate_inflatePaths.svg";
      SvgUtils.AddSolution(svg, solution, false);

      //SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 1000, 1000, 40);
      SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 0, 0, 0);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoSimpleInflatePaths2()//只显示一个平行方框和三角
    {
      SvgWriter svg = new();
      PathsD solution = new();

      //TRIANGLE OFFSET - WITH LARGE MITER//斜接
      PathsD pTri = new() { Clipper.MakePath(new double[] { 50, 350, 200, 550, 10, 550 }) };
      solution.AddRange(pTri);
      //PathsD pRect = new() { Clipper.MakePath(new double[] { 0, 300, 300, 300, 300, 600, 0, 600 }) };
      PathsD pRect = new() { Clipper.MakePath(new double[] { 0, 300, 0, 600, 300, 600, 300, 300 }) };//和上面的一样，都是外绿内白
      solution.AddRange(pRect);
      //PathsD pRect1 = new() { Clipper.MakePath(new double[] { 0, 260, 60, 260, 60, 300, 00, 300 }) };
      //solution.AddRange(pRect1);
      for (int i = 0; i < 1; ++i)
      {
        //nb: the last parameter here (10) greatly increases miter limit
        pTri = Clipper.InflatePaths(pTri, 20, JoinType.Miter, EndType.Polygon, 10);
        solution.AddRange(pTri);//AddRange：添加实现了ICollection接口的一个集合的所有元素到指定集合的末尾
        pRect = Clipper.InflatePaths(pRect, -10, JoinType.Miter, EndType.Polygon, 10);//单边的，外到内->绿，白
        //pRect = Clipper.InflatePaths(pRect, 10, JoinType.Square, EndType.Joined, 0);//双边的,外到内->绿白绿
        solution.AddRange(pRect);
      }
      SvgUtils.AddSolution(svg, solution, false);
      //SvgUtils.AddSolution(svg, solution, true);

      const string filename = "../../../inflate_inflatePaths.svg";
      SvgUtils.AddSolution(svg, solution, false);

      //SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 1000, 1000, 40);
      SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 0, 0, 0);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoSimpleClipperOffset()
    {
      SvgWriter svg = new();
      PathsD solution = new();
      // RECTANGLE OFFSET - BEVEL, SQUARED AND ROUNDED
      solution.Clear();
      PathsD pTri = new() { Clipper.MakePath(new double[] { 100, 0, 340, 0, 340, 200, 100, 200 }) };
      solution.Add(pTri[0]);
      //pTri = Clipper.InflatePaths(pTri, -50, JoinType.Miter, EndType.Joined, 15);
      //solution.Add(pTri[0]);
      //pTri = Clipper.InflatePaths(pTri, -10, JoinType.Miter, EndType.Joined, 5);
      //solution.Add(pTri[0]);
      //pTri = Clipper.InflatePaths(pTri, -20, JoinType.Miter, EndType.Joined, 2);
      //solution.Add(pTri[0]);
      //pTri = Clipper.InflatePaths(pTri, -40, JoinType.Miter, EndType.Joined, 2);
      //solution.Add(pTri[0]);
      //pTri = Clipper.InflatePaths(pTri, -20, JoinType.Miter, EndType.Joined, 2);
      //solution.Add(pTri[0]);
      //solution.Add(Clipper.MakePath(new double[] { 100, 0, 340, 0, 340, 200, 100, 200 }));
      solution.Add(Clipper.MakePath(new double[] { 300, 50, 380, 150, 40, 150 }));

            //solution.Add(Clipper.TranslatePath(solution[0], 60, 50));
            //solution.Add(Clipper.TranslatePath(solution[1], 100, 50));
            //SvgUtils.AddOpenSubject(svg, solution);
            //SvgUtils.AddSolution(svg, solution, true);

            // nb: rather than using InflatePaths(), we have to use the 
            // ClipperOffest class directly because we want to perform
            // different join types in a single offset operation
            //ClipperOffset co = new();
      //co.ReverseSolution = false;
      // because ClipperOffset only accepts Int64 paths, scale them 
      // so the de-scaled offset result will have greater precision
      double scale = 100;
      Paths64 pp64 = Clipper.ScalePaths64(solution, scale);
            //co.AddPath(pp64[0], JoinType.Bevel, EndType.Joined);
            //co.AddPath(pp64[0], JoinType.Miter, EndType.Polygon);
            //co.AddPath(pp64[1], JoinType.Square, EndType.Joined);
            //co.AddPath(pp64[2], JoinType.Round, EndType.Joined);

            //https://angusj.com/clipper2/Docs/Units/Clipper/Types/EndType.htm
            //http://www.angusj.com/clipper2/Docs/Units/Clipper.Offset/Classes/ClipperOffset/_Body.htm
            //co.AddPaths(pp64, JoinType.Miter, EndType.Polygon);//好像有bug
            //co.AddPaths(pp64, JoinType.Square, EndType.Joined);
            //co.AddPaths(pp64, JoinType.Square, EndType.Square);//可能不闭合

            //co.Execute(-10 * scale, pp64);
            //co.Execute(-20 * scale, pp64);
      // now de-scale the offset solution
      solution = Clipper.ScalePathsD(pp64, 1 / scale);//注释与否，可看到是否裁剪的效果

      const string filename = "../../../inflate_clippper.svg";
      SvgUtils.AddSolution(svg, solution, false);

      SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 800, 600, 40);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoSimpleClipperOffset1()
    {
      SvgWriter svg = new();
      PathsD solution = new();
      PathsD solution_new = new();
      // RECTANGLE OFFSET - BEVEL, SQUARED AND ROUNDED
      solution.Clear();
      PathsD pRect = new() { Clipper.MakePath(new double[] { 100, 0, 340, 0, 340, 200, 100, 200 }) };
      solution.AddRange(pRect);

      for (int i = 0; i < 3; ++i)
      {
        //solution.AddRange(pTri);//AddRange：添加实现了ICollection接口的一个集合的所有元素到指定集合的末尾
        pRect = Clipper.InflatePaths(pRect, -10, JoinType.Miter, EndType.Polygon, 10);//单边的，外到内->绿，白
        //pRect = Clipper.InflatePaths(pRect, 10, JoinType.Square, EndType.Joined, 0);//双边的,外到内->绿白绿
        solution.AddRange(pRect);
      }

      PathsD pTri = new() { Clipper.MakePath(new double[] { 100, 0, 340, 0, 340, 200, 100, 200 }) };
      pTri = Clipper.InflatePaths(pTri, 10, JoinType.Miter, EndType.Joined);
      solution.Add(pTri[0]);
      pTri = Clipper.InflatePaths(pTri, 10, JoinType.Miter, EndType.Joined);
      solution.Add(pTri[0]);
      //solution.Add(Clipper.MakePath(new double[] { 300, 50, 380, 150, 40, 150 }));

      //solution.Add(Clipper.TranslatePath(solution[0], 60, 50));
      //solution.Add(Clipper.TranslatePath(solution[1], 100, 50));
      //SvgUtils.AddOpenSubject(svg, solution);
      //SvgUtils.AddSolution(svg, solution, true);

      //ClipperOffset co = new();
      double scale = 100;
      Paths64 pp64_rect = Clipper.ScalePaths64(solution, scale);

      //co.AddPaths(pp64, JoinType.Miter, EndType.Polygon);//好像有bug
      //co.AddPaths(pp64_rect, JoinType.Square, EndType.Joined);
      //co.AddPaths(pp64, JoinType.Square, EndType.Square);//可能不闭合

      solution.Clear();
      solution.Add(Clipper.MakePath(new double[] { 300, 50, 380, 150, 40, 150 }));
      Paths64 pp64_tri = Clipper.ScalePaths64(solution, scale);
      //co.AddPaths(pp64_tri, JoinType.Square, EndType.Joined);

      //solution_new = Clipper.ScalePathsD(Clipper.Intersect(pp64_rect, pp64_tri, FillRule.EvenOdd), 1/scale);
      //solution_new = Clipper.ScalePathsD(Clipper.Intersect(pp64_tri, pp64_rect, FillRule.EvenOdd), 1/scale);//和上面一样
      //solution_new = Clipper.ScalePathsD(Clipper.Union(pp64_rect, pp64_tri, FillRule.EvenOdd), 1/scale);
      solution_new = Clipper.ScalePathsD(Clipper.Difference(pp64_rect, pp64_tri, FillRule.EvenOdd), 1 / scale);

      //co.Execute(-20 * scale, pp64_rect);
      // now de-scale the offset solution
      //solution = Clipper.ScalePathsD(pp64_rect, 1 / scale);//注释与否，可看到是否裁剪的效果

      const string filename = "../../../inflate_clippper.svg";
      //SvgUtils.AddSolution(svg, solution, false);
      SvgUtils.AddSolution(svg, solution_new, false);

      SvgUtils.SaveToFile(svg, filename, FillRule.NonZero, 0, 0, 0);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoSimpleClipperOffset2()
    {
      SvgWriter svg = new();
      PathsD solution = new();
      PathsD solution_new = new();
      // RECTANGLE OFFSET - BEVEL, SQUARED AND ROUNDED
      solution.Clear();
      PathsD pRect = new() { Clipper.MakePath(new double[] { 100, 0, 340, 0, 340, 200, 100, 200 }) };
      //SvgUtils.AddSubject(svg, pRect);
      solution.AddRange(pRect);

      for (int i = 0; i < 5; ++i)
      {
        //solution.AddRange(pTri);//AddRange：添加实现了ICollection接口的一个集合的所有元素到指定集合的末尾
        pRect = Clipper.InflatePaths(pRect, -10, JoinType.Miter, EndType.Polygon, 10);//单边的，外到内->绿，白
        //pRect = Clipper.InflatePaths(pRect, 10, JoinType.Square, EndType.Joined, 0);//双边的,外到内->绿白绿
        solution.AddRange(pRect);
        //SvgUtils.AddSubject(svg, pRect);
      }

      //ClipperOffset co = new();
      double scale = 100;
      Paths64 pp64_rect = Clipper.ScalePaths64(solution, scale);

      solution.Clear();
      PathsD pTri = new() { Clipper.MakePath(new double[] { 300, 50, 380, 150, 40, 150 }) };
      solution.AddRange(pTri);
      Paths64 pp64_tri = Clipper.ScalePaths64(solution, scale);
      //co.AddPaths(pp64_tri, JoinType.Square, EndType.Joined);

      //solution_new = Clipper.ScalePathsD(Clipper.Difference(pp64_rect, pp64_tri, FillRule.NonZero), 1 / scale);
      //solution_new = Clipper.ScalePathsD(Clipper.Difference(pp64_rect, pp64_tri, FillRule.Negative), 1 / scale);
      //solution_new = Clipper.ScalePathsD(Clipper.Difference(pp64_rect, pp64_tri, FillRule.Positive), 1 / scale);//只有外面的，没有绿白相间
      //solution_new = Clipper.ScalePathsD(Clipper.Union(pp64_tri, pp64_rect, FillRule.EvenOdd), 1 / scale);
      solution_new = Clipper.ScalePathsD(Clipper.Difference(pp64_rect, pp64_tri, FillRule.EvenOdd), 1 / scale);//绿白相间
      //solution_new.AddRange(pTri);

      //co.Execute(-20 * scale, pp64_rect);
      // now de-scale the offset solution
      //solution = Clipper.ScalePathsD(pp64_rect, 1 / scale);//注释与否，可看到是否裁剪的效果

      const string filename = "../../../inflate_clippper.svg";
      //SvgUtils.AddSolution(svg, solution, false);
      SvgUtils.AddSolution(svg, solution_new, false);
      //SvgUtils.AddSubject(svg, pTri);//灰色三角
      SvgUtils.AddClip(svg, pTri);//红色三角,和上面只是三角的颜色不同

      //SvgUtils.SaveToFile(svg, filename, FillRule.NonZero, 0, 0, 0);
      SvgUtils.SaveToFile(svg, filename, FillRule.Positive, 0, 0, 0);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoSimpleClipperOffset3()
    {
      SvgWriter svg = new();
      PathsD solution = new();
      PathsD solution_tri = new();
      PathsD solution_line = new();
      // RECTANGLE OFFSET - BEVEL, SQUARED AND ROUNDED
      solution.Clear();
      PathsD pRect = new() { Clipper.MakePath(new double[] { 100, 0, 340, 0, 340, 200, 100, 200 }) };
      PathsD pRectLine = new() { Clipper.MakePath(new double[] { 100, 0, 340, 0, 340, 200, 100, 200,  100,0 }) };
      pRectLine = Clipper.InflatePaths(pRectLine, -5, JoinType.Miter, EndType.Polygon, 10);
      //SvgUtils.AddSubject(svg, pRect);
      //SvgUtils.AddSubject(svg, pRectLine);
      solution.AddRange(pRect);
      solution_line.AddRange(pRectLine);

      for (int i = 0; i < 0; ++i)
      {
        pRect = Clipper.InflatePaths(pRect, -10, JoinType.Miter, EndType.Polygon, 10);//单边的，外到内->绿，白
        pRectLine = Clipper.InflatePaths(pRectLine, -10, JoinType.Miter, EndType.Polygon, 10);
        solution.AddRange(pRect);
        solution_line.AddRange(pRectLine);
      }

      double scale = 100;
      Paths64 pp64_rect = Clipper.ScalePaths64(solution, scale);
      Paths64 pp64_rect_line = Clipper.ScalePaths64(solution_line, scale);

      solution_tri.Clear();
      PathsD pTri = new() { Clipper.MakePath(new double[] { 300, 50, 380, 150, 40, 150 }) };
      solution_tri.AddRange(pTri);
      Paths64 pp64_tri = Clipper.ScalePaths64(solution_tri, scale);


      solution = Clipper.ScalePathsD(Clipper.Difference(pp64_rect, pp64_tri, FillRule.EvenOdd), 1 / scale);//绿白相间
      //Paths64 pp64_diff = Clipper.ScalePaths64(solution, scale);
      //solution_line = Clipper.ScalePathsD(Clipper.Difference(pp64_rect, pp64_rect_line, FillRule.EvenOdd), 1 / scale);//绿白相间
      ////solution_line = Clipper.ScalePathsD(Clipper.Difference(pp64_diff, pp64_rect_line, FillRule.Positive), 1 / scale);//绿白相间
      const string filename = "../../../inflate_clippper.svg";
      SvgUtils.AddSolution(svg, solution, false);
      //SvgUtils.AddSubject(svg, solution_new);//灰色
      //SvgUtils.AddOpenSubject(svg, solution_line);
      //SvgUtils.AddOpenSolution(svg, solution_line, false);
      //SvgUtils.AddSolution(svg, solution_line, false);
      //SvgUtils.AddSubject(svg, solution_line);
      //SvgUtils.AddClip(svg, pTri);//红色三角,和上面只是三角的颜色不同
      SvgUtils.SaveToFile(svg, filename, FillRule.Positive, 0, 0, 0);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoSimpleClipperOffset4()
    {
      SvgWriter svg = new();
      PathsD solution = new();
      PathsD solution_tri = new();
      solution.Clear();
      PathsD pRect = new() { Clipper.MakePath(new double[] { 100, 0, 340, 50, 300, 200, 150, 250 }) };
      solution.AddRange(pRect);

      double scale = 100;
      Paths64 pp64_rect = Clipper.ScalePaths64(solution, scale);

      solution_tri.Clear();
      PathsD pTri = new() { Clipper.MakePath(new double[] { 300, 50, 380, 150, 40, 200 }) };
      solution_tri.AddRange(pTri);
      Paths64 pp64_tri = Clipper.ScalePaths64(solution_tri, scale);


      solution = Clipper.ScalePathsD(Clipper.Difference(pp64_rect, pp64_tri, FillRule.EvenOdd), 1 / scale);//绿白相间
      const string filename = "../../../inflate_clippper.svg";
      SvgUtils.AddSolution(svg, solution, false);
      SvgUtils.SaveToFile(svg, filename, FillRule.Positive, 0, 0, 0);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static void DoRabbit()
    {
      PathsD pd = LoadPathsFromResource("InflateDemo.rabbit.bin");

      PathsD solution = new (pd);
      while (pd.Count > 0)
      {
        // and don't forget to scale the delta offset
        //pd = Clipper.InflatePaths(pd, -2.5, JoinType.Round, EndType.Polygon);
        pd = Clipper.InflatePaths(pd, -20, JoinType.Round, EndType.Polygon);
        // SimplifyPaths - is not essential but it not only 
        // speeds up the loop but it also tidies the result
        pd = Clipper.SimplifyPaths(pd, 0.25);
        solution.AddRange(pd);
      }

      const string filename = "../../../rabbit.svg";
      SvgWriter svg = new ();
      SvgUtils.AddSolution(svg, solution, false);
      //SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 450, 720, 10);
      SvgUtils.SaveToFile(svg, filename, FillRule.Positive, 450, 720, 10);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

    public static PathsD LoadPathsFromResource(string resourceName)
    {
      using Stream stream = Assembly.GetExecutingAssembly().
        GetManifestResourceStream(resourceName);
      if (stream == null) return new PathsD();
      using BinaryReader reader = new (stream);
      int len = reader.ReadInt32();
      PathsD result = new (len);
      for (int i = 0; i < len; i++)
      {
        int len2 = reader.ReadInt32();
        PathD p = new (len2);
        for (int j = 0; j < len2; j++)
        {
          long X = reader.ReadInt64();
          long Y = reader.ReadInt64();
          p.Add(new PointD(X, Y));
        }
        result.Add(p);
      }
      return result;
    }

    public static void DoVariableOffset()
    {
      Paths64 p = new() { Clipper.MakePath(new int[] { 0,50, 20,50, 40,50, 60,50, 80,50, 100,50 }) };
      Paths64 solution = new();
      ClipperOffset co = new();
      co.AddPaths(p, JoinType.Square, EndType.Butt);
      co.Execute(
        (path, path_norms, currPt, prevPt) => currPt * currPt + 10, solution);
      //(path, path_norms, currPt, prevPt) => path_norms[currPt].y, solution);

      const string filename = "../../../variable_offset.svg";
      SvgWriter svg = new();
      SvgUtils.AddOpenSubject(svg, p);
      SvgUtils.AddSolution(svg, solution, true);
      SvgUtils.SaveToFile(svg, filename, FillRule.EvenOdd, 500, 500, 60);
      ClipperFileIO.OpenFileWithDefaultApp(filename);
    }

  } //end Application

} //namespace

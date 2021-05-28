using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GraphicsLab2
{
   // Rasterisation grid class
   class RasterGrid
   {
      public Color4 backgroundColor;
      public Color4[][] grid;
      private int _resolution = 0;

      private float _cellW, _cellH;
      private float _screenW, _screenH;

      private int _minRes = 2;
      private int _maxRes = 180;

      public RasterGrid(int resolution, float screenW, float screenH, Color4 basicColor)
      {
         this.backgroundColor = basicColor;
         AddResolution(Math.Min(Math.Max(resolution, _minRes), _maxRes));
         SetScreenSize(screenW, screenH);
      }

      // Adding resolution to current resolution
      public void AddResolution(int resolution)
      {
         int newResolution = _resolution + resolution;

         if (newResolution >= _minRes && newResolution <= _maxRes)
         {
            _resolution = newResolution;
            grid = new Color4[newResolution][];

            for (int i = 0; i < newResolution; i++)
               grid[i] = new Color4[newResolution];


            ResetColours();
            RecalcCellSize();
         }
      }

      // Setting all cells color to background color
      public void ResetColours()
      {
         for (int i = 0; i < _resolution; i++)
         {
            for (int j = 0; j < _resolution; j++)
            {
               grid[i][j] = backgroundColor;
            }
         }
      }

      public void SetScreenSize(float screenW, float screenH)
      {
         _screenW = screenW;
         _screenH = screenH;

         RecalcCellSize();
      }

      private void RecalcCellSize()
      {
         _cellW = _screenW / _resolution;
         _cellH = _screenH / _resolution;
      }

      public void DrawBorders(float lineWidth)
      {
         GL.Color3(0f, 0f, 0f);
         GL.LineWidth(lineWidth);
         GL.Begin(BeginMode.Lines);

         for (int i = 0; i < _resolution - 1; i++)
         {
            GL.Vertex2(0, _cellH * (i + 1));
            GL.Vertex2(_screenW, _cellH * (i + 1));
         }

         for (int i = 0; i < _resolution - 1; i++)
         {
            GL.Vertex2(_cellW * (i + 1), 0);
            GL.Vertex2(_cellW * (i + 1), _screenH);
         }

         GL.End();
      }

      public void DrawCells()
      {
         GL.PointSize(_cellW);

         for (int i = 0; i < _resolution; i++)
         {
            for (int j = 0; j < _resolution; j++)
            {
               GL.Color4(grid[i][j]);
               GL.Begin(BeginMode.Points);
               GL.Vertex2(_cellW * (j + 1) - _cellW / 2, (_cellH * (i + 1) - _cellH / 2));
               GL.End();
            }
         }
      }

      // Performing rasterising of stroke of figure
      public void RasterWithStroke(Figure figure)
      {
         for (int i = 0; i < figure.vertices.Length; i++)
         {
            Vector2 v0 = figure.vertices[i];
            Vector2 v1 = figure.vertices[(i + 1) % figure.vertices.Length];
            PlotLine(v0, v1);
         }
      }

      // Performong rasterising of line
      public void PlotLine(Vector2 point0, Vector2 point1)
      {
         int x0 = (int)Math.Floor(point0.X / _cellW);
         int x1 = (int)Math.Floor(point1.X / _cellW);

         int y0 = (int)Math.Floor(point0.Y / _cellH);
         int y1 = (int)Math.Floor(point1.Y / _cellH);

         if (Math.Abs(y1 - y0) < Math.Abs(x1 - x0))
            if (x0 > x1)
               PlotLineLow(x1, y1, x0, y0);
            else
               PlotLineLow(x0, y0, x1, y1);
         else
            if (y0 > y1)
            PlotLineHigh(x1, y1, x0, y0);
         else
            PlotLineHigh(x0, y0, x1, y1);
      }

      public void PlotLineLow(int x0, int y0, int x1, int y1)
      {
         int dx = x1 - x0;
         int dy = y1 - y0;
         int yi = 1;
         if (dy < 0)
         {
            yi = -1;
            dy = -dy;
         }
         int D = 2 * dy - dx;

         int y = y0;
         for (int x = x0; x < x1; x++)
         {
            if (x >= 0 && x < grid[0].Length &&
               y >= 0 && y < grid.Length)
               grid[y][x] = Color4.Blue;
            if (D > 0)
            {
               y += yi;
               D += 2 * (dy - dx);
            }
            else
               D += 2 * dy;
         }
      }

      public void PlotLineHigh(int x0, int y0, int x1, int y1)
      {

         int dx = x1 - x0;
         int dy = y1 - y0;
         int xi = 1;
         if (dx < 0)
         {
            xi = -1;
            dx = -dx;
         }
         int D = 2 * dx - dy;

         int x = x0;
         for (int y = y0; y < y1; y++)
         {

            if (x >= 0 && x < grid[0].Length &&
               y >= 0 && y < grid.Length)
               grid[y][x] = Color4.Blue;
            if (D > 0)
            {
               x += xi;
               D += 2 * (dx - dy);
            }
            else
               D += 2 * dx;
         }
      }

      // Performing rasterising whole figure
      public void RasterWithFilling(Figure figure)
      {
         for (int i = 0; i < grid.Length; i++)
         {
            for (int j = 0; j < grid[0].Length; j++)
            {
               bool doFill = true;
               Vector2 p = new Vector2(j, i);

               for (int v = 0; v < figure.vertices.Length; v++)
               {
                  Vector2 v0 = new Vector2((int)Math.Floor(figure.vertices[v].X / _cellW), (int)Math.Floor(figure.vertices[v].Y / _cellH));
                  Vector2 v1 = new Vector2((int)Math.Floor(figure.vertices[(v + 1) % figure.vertices.Length].X / _cellW), (int)Math.Floor(figure.vertices[(v + 1) % figure.vertices.Length].Y / _cellH));

                  float d = (p.X - v0.X) * (v1.Y - v0.Y) - (p.Y - v0.Y) * (v1.X - v0.X);

                  if (d >= 0)
                  {
                     doFill = false;
                     break;
                  }
               }

               if (doFill)
                  grid[i][j] = Color4.Blue;
            }
         }
      }

      public void RasterFigures(List<Figure> figures)
      {
         foreach (var f in figures)
         {
            if (f.rasterMode == 0)
               RasterWithStroke(f);
            else
               RasterWithFilling(f);
         }
      }

      private Vector2 LineLineIntersection(Vector2 point0, Vector2 point1, Vector2 point2, Vector2 point3)
      {
         float a1 = point1.Y - point0.Y;
         float b1 = point0.X - point1.X;
         float c1 = a1 * point0.X + b1 * point0.Y;

         float a2 = point3.Y - point2.Y;
         float b2 = point2.X - point3.X;
         float c2 = a2 * point2.X + b2 * point2.Y;

         float det = a1 * b2 - a2 * b1;

         return new Vector2((b2 * c1 - b1 * c2) / det, (a1 * c2 - a2 * c1) / det);
      }

   }
}

//public void RasterWithFilling(List<Figure> figures)
//{
//   foreach (var f in figures)
//   {
//      for (int i = 0; i < grid.Length; i++)
//      {
//         for (int j = 0; j < grid[0].Length; j++)
//         {
//            bool doFill = true;
//            Vector2 p = new Vector2(j, i);

//            if (grid[i][j] != Color4.Blue)
//            {

//               for (int v = 0; v < f.vertices.Length; v++)
//               {
//                  Vector2 v0 = new Vector2((int)Math.Floor(f.vertices[v].X / _cellW), (int)Math.Floor(f.vertices[v].Y / _cellH));
//                  Vector2 v1 = new Vector2((int)Math.Floor(f.vertices[(v + 1) % f.vertices.Length].X / _cellW), (int)Math.Floor(f.vertices[(v + 1) % f.vertices.Length].Y / _cellH));

//                  float d = (p.X - v0.X) * (v1.Y - v0.Y) - (p.Y - v0.Y) * (v1.X - v0.X);

//                  if (d >= 0)
//                  {
//                     doFill = false;
//                     break;
//                  }
//               }

//               if (doFill)
//                  grid[i][j] = Color4.Blue;
//            }
//         }
//      }


//   }
//}

//private void PlotLineLow(float fx0, float fy0, float fx1, float fy1)
//{
//   float dx = fx1 - fx0;
//   float dy = fy1 - fy0;
//   int yi = 1;
//   if (dy < 0)
//   {
//      yi = -1;
//      dy = -dy;
//   }
//   float D = 2 * dy - dx;

//   int x0 = (int)Math.Floor(fx0 / _cellW);
//   int x1 = (int)Math.Floor(fx1 / _cellW);

//   int y0 = (int)Math.Floor(fy0 / _cellH);

//   int y = y0;
//   for (int x = x0; x < x1; x++)
//   {
//      if (x >= 0 && x < grid[0].Length &&
//         y >= 0 && y < grid.Length)
//         grid[y][x] = Color4.Blue;
//      if (D > 0)
//      {
//         y += yi;
//         D += 2 * (dy - dx);
//      }
//      else
//         D += 2 * dy;
//   }
//}

//private void PlotLineHigh(float fx0, float fy0, float fx1, float fy1)
//{

//   float dx = fx1 - fx0;
//   float dy = fy1 - fy0;
//   int xi = 1;
//   if (dx < 0)
//   {
//      xi = -1;
//      dx = -dx;
//   }
//   float D = 2 * dx - dy;

//   int x0 = (int)Math.Floor(fx0 / _cellW);

//   int y0 = (int)Math.Floor(fy0 / _cellH);
//   int y1 = (int)Math.Floor(fy1 / _cellH);

//   int x = x0;
//   for (int y = y0; y < y1; y++)
//   {

//      if (x >= 0 && x < grid[0].Length &&
//         y >= 0 && y < grid.Length)
//         grid[y][x] = Color4.Blue;
//      if (D > 0)
//      {
//         x += xi;
//         D += 2 * (dx - dy);
//      }
//      else
//         D += 2 * dx;
//   }
//}

//private void PlotLine(float x0, float y0, float x1, float y1)
//{
//   if (Math.Abs(y1 - y0) < Math.Abs(x1 - x0))
//      if (x0 > x1)
//         PlotLineLow(x1, y1, x0, y0);
//      else
//         PlotLineLow(x0, y0, x1, y1);
//   else
//      if (y0 > y1)
//      PlotLineHigh(x1, y1, x0, y0);
//   else
//      PlotLineHigh(x0, y0, x1, y1);
//}
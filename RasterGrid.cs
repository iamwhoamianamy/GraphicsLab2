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
   enum BlendType
   {
      And = 0,
      Or = 1,
      Xor = 2,
      NotAnd = 3,
      NotOr = 4,
      NotXor = 5,
      No = 6
   }

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

      public BlendType mixType;

      public RasterGrid(int resolution, float screenW, float screenH, Color4 basicColor)
      {
         this.backgroundColor = basicColor;
         AddResolution(Math.Min(Math.Max(resolution, _minRes), _maxRes));
         SetScreenSize(screenW, screenH);
         mixType = BlendType.Or;
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
         GL.Color3(0.5f, 0.5f, 0.5f);
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
            PlotLine(v0, v1, figure.color);
         }
      }

      // Performong rasterising of line
      public void PlotLine(Vector2 point0, Vector2 point1, Color4 color)
      {
         int x0 = (int)Math.Floor(point0.X / _cellW);
         int x1 = (int)Math.Floor(point1.X / _cellW);

         int y0 = (int)Math.Floor(point0.Y / _cellH);
         int y1 = (int)Math.Floor(point1.Y / _cellH);

         if (Math.Abs(y1 - y0) < Math.Abs(x1 - x0))
            if (x0 > x1)
               PlotLineLow(x1, y1, x0, y0, color);
            else
               PlotLineLow(x0, y0, x1, y1, color);
         else
            if (y0 > y1)
            PlotLineHigh(x1, y1, x0, y0, color);
         else
            PlotLineHigh(x0, y0, x1, y1, color);
      }

      public void PlotLineLow(int x0, int y0, int x1, int y1, Color4 color)
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
               grid[y][x] = BlendColors(grid[y][x], color, mixType);
            if (D > 0)
            {
               y += yi;
               D += 2 * (dy - dx);
            }
            else
               D += 2 * dy;
         }
      }

      public void PlotLineHigh(int x0, int y0, int x1, int y1, Color4 color)
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
               grid[y][x] = BlendColors(grid[y][x], color, mixType);
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

                  if (d > 0)
                  {
                     doFill = false;
                     break;
                  }
               }

               if (doFill)
                  grid[i][j] = BlendColors(grid[i][j], figure.color, mixType);
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

      public static Color4 BlendColors(Color4 a, Color4 b, BlendType mixMode)
      {
         switch(mixMode)
         {
            case BlendType.And:
            {
               byte newAR = (byte)Math.Round(a.R * 255);
               byte newBR = (byte)Math.Round(b.R * 255);

               byte newR = (byte)((byte)Math.Round(a.R * 255) & (byte)Math.Round(b.R * 255));
               byte newG = (byte)((byte)Math.Round(a.G * 255) & (byte)Math.Round(b.G * 255));
               byte newB = (byte)((byte)Math.Round(a.B * 255) & (byte)Math.Round(b.B * 255));
               return new Color4(newR / 255.0f, newG / 255.0f, newB / 255.0f, 1f);
            }
            case BlendType.Or:
            {
               byte newR = (byte)((byte)Math.Round(a.R * 255) | (byte)Math.Round(b.R * 255));
               byte newG = (byte)((byte)Math.Round(a.G * 255) | (byte)Math.Round(b.G * 255));
               byte newB = (byte)((byte)Math.Round(a.B * 255) | (byte)Math.Round(b.B * 255));
               return new Color4(newR / 255.0f, newG / 255.0f, newB / 255.0f, 1f);
            }
            case BlendType.Xor:
            {
               byte newR = (byte)((byte)Math.Round(a.R * 255) ^ (byte)Math.Round(b.R * 255));
               byte newG = (byte)((byte)Math.Round(a.G * 255) ^ (byte)Math.Round(b.G * 255));
               byte newB = (byte)((byte)Math.Round(a.B * 255) ^ (byte)Math.Round(b.B * 255));
               return new Color4(newR / 255.0f, newG / 255.0f, newB / 255.0f, 1f);
            }
            case BlendType.NotAnd:
            {
               byte newR = (byte)((byte)Math.Round(a.R * 255) & (byte)Math.Round(b.R * 255));
               byte newG = (byte)((byte)Math.Round(a.G * 255) & (byte)Math.Round(b.G * 255));
               byte newB = (byte)((byte)Math.Round(a.B * 255) & (byte)Math.Round(b.B * 255));
               return new Color4((byte)~newR / 255.0f, (byte)~newG / 255.0f, (byte)~newB / 255.0f, 1f);
            }
            case BlendType.NotOr:
            {
               byte newR = (byte)((byte)Math.Round(a.R * 255) | (byte)Math.Round(b.R * 255));
               byte newG = (byte)((byte)Math.Round(a.G * 255) | (byte)Math.Round(b.G * 255));
               byte newB = (byte)((byte)Math.Round(a.B * 255) | (byte)Math.Round(b.B * 255));
               return new Color4((byte)~newR / 255.0f, (byte)~newG / 255.0f, (byte)~newB / 255.0f, 1f);
            }
            case BlendType.NotXor:
            {
               byte newR = (byte)((byte)Math.Round(a.R * 255) ^ (byte)Math.Round(b.R * 255));
               byte newG = (byte)((byte)Math.Round(a.G * 255) ^ (byte)Math.Round(b.G * 255));
               byte newB = (byte)((byte)Math.Round(a.B * 255) ^ (byte)Math.Round(b.B * 255));
               return new Color4((byte)~newR / 255.0f, (byte)~newG / 255.0f, (byte)~newB / 255.0f, 1f);
            }

            case BlendType.No:
            {
               return new Color4(b.R, b.G, b.B, 1f);
            }

            default: return Color4.White;
         }
      }
   }
}
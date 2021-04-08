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
   class RasterGrid
   {
      public Color4 basicColor;
      public Color4[][] grid;
      private int _resolution = 0;

      private float _cellW, _cellH;
      private float _screenW, _screenH;

      private int _minRes = 20;
      private int _maxRes = 120;

      public RasterGrid(int resolution, float screenW, float screenH, Color4 basicColor)
      {
         this.basicColor = basicColor;
         AddResolution(Math.Min(Math.Max(resolution, _minRes), _maxRes));
         SetScreenSize(screenW, screenH);
      }

      //public int GetResolution()
      //{
      //   return _resolution;
      //}

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

      public void ResetColours()
      {
         for (int i = 0; i < _resolution; i++)
         {
            for (int j = 0; j < _resolution; j++)
            {
               grid[i][j] = basicColor;
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

      public void DrawBorders()
      {
         GL.Color3(0f, 0f, 0f);
         GL.LineWidth(2);
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

      public void RasterFigures(List<Figure> figures)
      {
         foreach (var f in figures)
         {
            for (int i = 0; i < f.vertices.Length; i++)
            {
               Vector2 v0 = f.vertices[i];
               Vector2 v1 = f.vertices[(i + 1) % f.vertices.Length];

               PlotLine(v0.X, v0.Y, v1.X, v1.Y);
            }
         }
      }

      private void PlotLineLow(int x0, int y0, int x1, int y1)
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

      private void PlotLineHigh(int x0, int y0, int x1, int y1)
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

      private void PlotLine(float fx0, float fy0, float fx1, float fy1)
      {
         int x0 = (int)(fx0 / _cellW);
         int x1 = (int)(fx1 / _cellW);

         int y0 = (int)(fy0 / _cellH);
         int y1 = (int)(fy1 / _cellH);

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

   }
}

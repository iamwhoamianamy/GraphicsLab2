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
      public Point[][] grid;
      private int _resolution = 0;

      private float _cellW, _cellH;
      private float _screenW, _screenH;

      public RasterGrid(int resolution, float screenW, float screenH)
      {
         AddResolution(Math.Min(Math.Max(resolution, 10), 40));
         SetScreenSize(screenW, screenH);
      }

      //public int GetResolution()
      //{
      //   return _resolution;
      //}

      public void AddResolution(int resolution)
      {
         int newResolution = _resolution + resolution;

         if (newResolution >= 10 && newResolution <= 40)
         {
            _resolution = newResolution;
            grid = new Point[newResolution][];

            for (int i = 0; i < newResolution; i++)
               grid[i] = new Point[newResolution];
         }

         RecalcCellSize();
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

      //private Vector2 GetCellCoord(int xi, int yi)
      //{

      //}

      public void DrawBorders()
      {
         GL.Color3(0f, 0f, 0f);
         GL.LineWidth(2);
         GL.Begin(BeginMode.Lines);



         for (int i = 0; i < _resolution - 1; i++)
         {
            for (int j = 0; j < _resolution - 1; j++)
            {
               GL.Vertex2(_cellW * (j + 1), 0);
               GL.Vertex2(_cellW * (j + 1), _screenH);
            }

            GL.Vertex2(0, _cellH * (i + 1));
            GL.Vertex2(_screenW, _cellH * (i + 1));

         }

         GL.End();
      }
   }
}

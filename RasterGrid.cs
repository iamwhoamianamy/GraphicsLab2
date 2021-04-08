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

      private float cellW, cellH;
      private float screenW, screenH;


      public RasterGrid(float screenW, float screenH)
      {
         grid = new Point[10][];

         for (int i = 0; i < 10; i++)
            grid[i] = new Point[10];

         SetScreenSize(screenW, screenH);
      }

      public void SetScreenSize(float screenW, float screenH)
      {
         this.screenW = screenW;
         this.screenH = screenH;

         cellW = screenW / 10;
         cellH = screenH / 10;
      }
      //private Vector2 GetCellCoord(int xi, int yi)
      //{

      //}

      public void DrawBorders()
      {
         GL.Color3(0f, 0f, 0f);
         GL.LineWidth(2);
         GL.Begin(BeginMode.Lines);



         for (int i = 0; i < grid.Length - 1; i++)
         {
            for (int j = 0; j < grid[i].Length - 1; j++)
            {
               GL.Vertex2(cellW * (j + 1), 0);
               GL.Vertex2(cellW * (j + 1), screenH);
            }

            GL.Vertex2(0, cellH * (i + 1));
            GL.Vertex2(screenW, cellH * (i + 1));

         }

         GL.End();
      }
   }
}

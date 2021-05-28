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
   public enum FigureState
   {
      Relaxing,
      Creation,
      Resizing,
      Moving,
      Rotation,
   }

   class Figure
   {
      public Vector2 pos;
      public float radius = 0;
      public Vector2[] vertices;
      public Color4 color;
      public int rasterMode = 0;
      public FigureState figureState;
      public float rotation;

      public Figure(Vector2 pos)
      {
         color = new Color4(1f, 1f, 1f, 0f);
         this.pos = pos;

         int count = 3;
         rotation = 0;

         radius = 0;
         vertices = new Vector2[count];
         for (int i = 0; i < count; i++)
            vertices[i] = Vector2.Zero;
      }

      public void SetRadius(float radius)
      {
         this.radius = Math.Max(radius, 2);
      }

      public void RecalcVertices(int newCount)
      {
         if(newCount > 2 && newCount != vertices.Length)
         {
            vertices = new Vector2[newCount];
            double angle = 2.0 / newCount * Math.PI;

            for (int i = 0; i < newCount; i++)
               vertices[i] = new Vector2(pos.X + radius * (float)Math.Cos(angle * i + rotation), pos.Y + radius * (float)Math.Sin(angle * i + rotation));
         }
      }

      public void RecalcVertices()
      {
         double angle = 360.0 / vertices.Length * Math.PI / 180.0;
         for (int i = 0; i < vertices.Length; i++)
            vertices[i] = new Vector2(pos.X + radius * (float)Math.Cos(angle * i + rotation), pos.Y + radius * (float)Math.Sin(angle * i + rotation));
      }

      public void Draw()
      {
         GL.Color3(color.R, color.G, color.B);

         GL.LineWidth(3);
         GL.Begin(BeginMode.LineLoop);

         foreach (var v in vertices)
            GL.Vertex2(v.X, v.Y);

         GL.End();
      }

      public void DrawCenter()
      {
         GL.Color3(1f, 1f, 1f);

         float size = 10f;

         GL.LineWidth(7.5f);
         GL.Begin(BeginMode.Lines);

         GL.Vertex2(pos.X + size, pos.Y);
         GL.Vertex2(pos.X - size, pos.Y);
         GL.Vertex2(pos.X, pos.Y + size);
         GL.Vertex2(pos.X, pos.Y - size);

         GL.End();
      }
   }
}

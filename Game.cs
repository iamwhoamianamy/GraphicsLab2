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
   class Game : GameWindow
   {

      List<Figure> figures;
      RasterGrid rasterGrid;

      bool isActive = false;                    // Есть ли активная фигура
      int active = 0;                           // Номер активной фигуры

      bool isInCreationProcess = false;         // Находится ли фигура в процессе создания
      Vector2 lastMousePress;

      float mouseX = 0, mouseY = 0;

      public Game(int width, int height, string title) :
           base(width, height, GraphicsMode.Default, title)
      {

      }

      protected override void OnLoad(EventArgs e)
      {
         figures = new List<Figure>();
         lastMousePress = Vector2.Zero;
         rasterGrid = new RasterGrid(80, Width, Height, Color4.White);


         //figures.Add(new Figure(new Vector2(300, 700)));
         //figures[0].radius = -100;
         //figures[0].RecalcVertices(5);

         base.OnLoad(e);
      }

      protected override void OnRenderFrame(FrameEventArgs e)
      {
         GL.Clear(ClearBufferMask.ColorBufferBit);

         Title = mouseX.ToString() + " " + mouseY.ToString();

         UpdatePhysics();
         Render();

         Context.SwapBuffers();
         base.OnRenderFrame(e);
      }

      private void UpdatePhysics()
      {
         //rasterGrid.PlotLine(100, 250, 200, 200);
         rasterGrid.RasterFigures(figures);
      }

      private void Render()
      {
         GL.ClearColor(Color4.DarkGray);

         rasterGrid.DrawCells();
         rasterGrid.DrawBorders(1f);

         if (isActive)
         {
            figures[active].DrawCenter();
         }
         rasterGrid.ResetColours();
      }

      protected override void OnKeyDown(KeyboardKeyEventArgs e)
      {
         if(e.Shift)
         {
            switch(e.Key)
            {
               case Key.A:
               {
                  if (!isInCreationProcess)
                  {
                     figures.Add(new Figure(new Vector2(mouseX, mouseY)));
                     isActive = true;
                     isInCreationProcess = true;
                     active = figures.Count - 1;
                     lastMousePress = new Vector2(mouseX, mouseY);
                  }
                  break;
               }
            }
         }

         switch(e.Key)
         {
            case Key.Plus:
            {
               rasterGrid.AddResolution(1);
               break;
            }
            case Key.Minus:
            {
               rasterGrid.AddResolution(-1);
               break;
            }
         }

         base.OnKeyDown(e);
      }

      protected override void OnMouseDown(MouseButtonEventArgs e)
      {
         switch(e.Button)
         {
            case MouseButton.Left:
            {
               if(isInCreationProcess)
               {
                  isInCreationProcess = false;
               }
               break;
            }
            case MouseButton.Right:
            {
               if(!isInCreationProcess && figures.Count > 0)
               {
                  float minDist = float.MaxValue;
                  int minInd = 0;
                  bool isFoundAny = false;

                  for (int i = 0; i < figures.Count; i++)
                  {
                     float dist = Vector2.Distance(figures[i].GetPos(), new Vector2(e.X, e.Y));
                     if (dist < minDist && dist < Math.Abs(figures[i].radius) / 2)
                     {
                        minDist = dist;
                        minInd = i;
                        isFoundAny = true;
                     }
                  }

                  if (isFoundAny)
                  {
                     active = minInd;
                     isActive = true;
                  }
               }

               if(isInCreationProcess)
               {
                  figures.RemoveAt(active);
                  isInCreationProcess = false;
                  isActive = false;
               }

               break;
            }
         }

         base.OnMouseDown(e);
      }

      protected override void OnMouseMove(MouseMoveEventArgs e)
      {
         if (isInCreationProcess)
         {
            float newRadius = e.X - lastMousePress.X;

            figures[active].radius = e.X - lastMousePress.X;
            figures[active].RecalcVertices();
         }

         mouseX = e.X;
         mouseY = e.Y;

         base.OnMouseMove(e);
      }

      protected override void OnMouseWheel(MouseWheelEventArgs e)
      {
         if (isActive)
         {
            figures[active].RecalcVertices(figures[active].vertices.Length + e.Delta);
         }

         base.OnMouseWheel(e);
      }

      protected override void OnResize(EventArgs e)
      {
         GL.Disable(EnableCap.DepthTest);
         GL.Viewport(0, 0, Width, Height);
         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadIdentity();
         GL.Ortho(0, Width, Height, 0, -1.0, 1.0);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadIdentity();

         rasterGrid.SetScreenSize(Width, Height);

         base.OnResize(e);
      }
   }
}

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
   public partial class Game : GameWindow
   {

      List<Figure> figures;
      RasterGrid rasterGrid;

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

         rasterGrid.RasterFigures(figures);
         //rasterGrid.RasterWithFilling(figures);
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

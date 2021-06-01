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
         rasterGrid = new RasterGrid(80, Width, Height, new Color4(0.1f, 0.3f, 0.5f, 1f));


         figures.Add(new Figure(new Vector2(100, 100)));
         figures[0].radius = 100;
         figures[0].RecalcVertices(4);
         figures[0].color = new Color4(0.6f, 0.7f, 0.8f, 1f);

         figures.Add(new Figure(new Vector2(100, 300)));
         figures[1].radius = 100;
         figures[1].RecalcVertices(4);
         figures[1].color = new Color4(10, 100, 1, 255);

         base.OnLoad(e);
      }

      protected override void OnRenderFrame(FrameEventArgs e)
      {
         GL.Clear(ClearBufferMask.ColorBufferBit);

         string newTitle = "Color mixing type = ";

         switch(rasterGrid.mixType)
         {
            case BlendType.And: newTitle += "And"; break;
            case BlendType.Or: newTitle += "Or"; break;
            case BlendType.Xor: newTitle += "Xor"; break;
            case BlendType.NotAnd: newTitle += "NotAnd"; break;
            case BlendType.NotOr: newTitle += "NotOr"; break;
            case BlendType.NotXor: newTitle += "NotXor"; break;
            case BlendType.No: newTitle += "No"; break;
         }

         Title = newTitle;

         UpdatePhysics();
         Render();

         Context.SwapBuffers();
         base.OnRenderFrame(e);
      }

      private void UpdatePhysics()
      {

         rasterGrid.RasterFigures(figures);
         Title += $" {rasterGrid.grid[0][0].R} {rasterGrid.grid[0][0].G} {rasterGrid.grid[0][0].B}";
         //rasterGrid.RasterWithFilling(figures);
      }

      private void Render()
      {
         GL.ClearColor(Color4.DarkGray);

         rasterGrid.DrawCells();
         //rasterGrid.DrawBorders(1f);

         if (isActive)
         {
            figures[active].DrawCenter();
         }

         foreach (var f in figures)
         {
            if(f.doDrawOutline)
            {
               f.DrawOutline();
            }
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

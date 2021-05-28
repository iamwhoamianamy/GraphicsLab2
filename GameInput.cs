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
   public partial class Game
   {

      bool isActive = false;     // Есть ли активная фигура
      int active = 0;            // Номер активной фигуры

      Vector2 lastMousePress;

      float mouseX = 0, mouseY = 0;
      
      protected override void OnKeyDown(KeyboardKeyEventArgs e)
      {
         if (e.Shift)
         {
            switch (e.Key)
            {
               case Key.A:
               {
                  if ((isActive && figures[active].figureState != FigureState.Creation) || !isActive)
                  {
                     figures.Add(new Figure(new Vector2(mouseX, mouseY)));
                     isActive = true;
                     active = figures.Count - 1;
                     figures[active].figureState = FigureState.Creation;
                     lastMousePress = new Vector2(mouseX, mouseY);
                  }
                  break;
               }
            }
         }

         switch (e.Key)
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
         switch (e.Button)
         {
            case MouseButton.Left:
            {
               if (isActive && figures[active].figureState == FigureState.Creation)
               {
                  figures[active].figureState = FigureState.Relaxing;
               }
               break;
            }
            case MouseButton.Right:
            {
               if (figures.Count > 0 && isActive && figures[active].figureState != FigureState.Creation)
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

               if (isActive && figures[active].figureState == FigureState.Creation)
               {
                  figures.RemoveAt(active);
                  isActive = false;
               }

               break;
            }
         }

         base.OnMouseDown(e);
      }

      protected override void OnMouseMove(MouseMoveEventArgs e)
      {
         if (isActive && figures[active].figureState == FigureState.Creation)
         {
            float newRadius = e.X - lastMousePress.X;

            figures[active].radius = e.X - lastMousePress.X;
            //figures[active].SetRadius(e.X - lastMousePress.X);
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
   }
}

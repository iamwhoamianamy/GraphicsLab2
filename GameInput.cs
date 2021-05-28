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
      float lastRadius = 0;
      float lastRotation = 0;
      Vector2 lastPosition;
      Vector2 lastMousePress;

      float mouseX = 0, mouseY = 0;
      
      protected override void OnKeyDown(KeyboardKeyEventArgs e)
      {
         if (e.Shift)
         {
            switch (e.Key)
            {
               // Создание новой фигуры
               case Key.A:
               {
                  if ((isActive && figures[active].figureState == FigureState.Relaxing) || !isActive)
                  {
                     figures.Add(new Figure(new Vector2(mouseX, mouseY)));
                     isActive = true;
                     active = figures.Count - 1;
                     figures[active].figureState = FigureState.Creation;
                     lastMousePress = new Vector2(mouseX, mouseY);
                  }
                  break;
               }
               case Key.Number1:
               {
                  if(isActive)
                     figures[active].color.R = Math.Max(0f, figures[active].color.R - 0.1f);
                  break;
               }
               case Key.Number2:
               {
                  if (isActive)
                     figures[active].color.G = Math.Max(0f, figures[active].color.G - 0.1f);
                  break;
               }
               case Key.Number3:
               {
                  if (isActive)
                     figures[active].color.B = Math.Max(0f, figures[active].color.B - 0.1f);
                  break;
               }
               
            }
         }

         switch (e.Key)
         {
            // Умельчение сетки
            case Key.Plus:
            {
               rasterGrid.AddResolution(1);
               break;
            }
            // Укрупнение сетки
            case Key.Minus:
            {
               rasterGrid.AddResolution(-1);
               break;
            }
            case Key.M:
            {
               rasterGrid.mixType = (MixType)(((int)(rasterGrid.mixType + 1)) % 6);
               break;
            }
         }

         if (isActive && figures[active].figureState == FigureState.Relaxing)
         {
            switch (e.Key)
            {
               // Изменение размера активной фигуры
               case Key.S:
               {
                  figures[active].figureState = FigureState.Resizing;
                  lastRadius = figures[active].radius;
                  lastMousePress = new Vector2(mouseX, mouseY);
                  break;
               }
               // Передвижение фиуры
               case Key.G:
               {
                  figures[active].figureState = FigureState.Moving;
                  lastPosition = figures[active].pos;
                  lastMousePress = new Vector2(mouseX, mouseY);
                  break;
               }
               // Вращение фигуры
               case Key.R:
               {
                  figures[active].figureState = FigureState.Rotation;
                  lastRotation = figures[active].rotation;
                  lastMousePress = new Vector2(mouseX, mouseY);
                  break;
               }
               // Изменение режима заполнения фигуры
               case Key.F:
               {
                  figures[active].rasterMode = (figures[active].rasterMode + 1) % 2;
                  break;
               }
               // Удаление фигуры
               case Key.X:
               {
                  figures.RemoveAt(active);
                  isActive = false;
                  break;
               }

               case Key.Number1:
               {
                  figures[active].color.R = Math.Min(1f, figures[active].color.R + 0.05f);
                  break;
               }
               case Key.Number2:
               {
                  figures[active].color.G = Math.Min(1f, figures[active].color.G + 0.05f);
                  break;
               }
               case Key.Number3:
               {
                  figures[active].color.B = Math.Min(1f, figures[active].color.B + 0.05f);
                  break;
               }
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
               // Применение текущей операции над фигурой
               if (isActive && figures[active].figureState != FigureState.Relaxing)
               {
                  figures[active].figureState = FigureState.Relaxing;
               }

               break;
            }
            case MouseButton.Right:
            {
               // Выбор новой активной фигуры
               if (isActive && figures[active].figureState == FigureState.Relaxing || !isActive)
               {
                  float minDist = float.MaxValue;
                  int minInd = 0;
                  bool isFoundAny = false;

                  for (int i = 0; i < figures.Count; i++)
                  {
                     float dist = Vector2.Distance(figures[i].pos, new Vector2(e.X, e.Y));
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

               if(isActive)
               {
                  switch(figures[active].figureState)
                  {
                     // Отмена процедуры создания новой фигуры
                     case FigureState.Creation:
                     {
                        figures.RemoveAt(active);
                        isActive = false;
                        break;
                     }
                     // Отмена процедуры изменения размера фигуры
                     case FigureState.Resizing:
                     {
                        figures[active].figureState = FigureState.Relaxing;
                        figures[active].radius = lastRadius;
                        figures[active].RecalcVertices();
                        break;
                     }
                     // Отмена процедуры передвижения фигуры
                     case FigureState.Moving:
                     {
                        figures[active].figureState = FigureState.Relaxing;
                        figures[active].pos = lastPosition;
                        figures[active].RecalcVertices();
                        break;
                     }
                     // Отмена процедуры вращения фигуры
                     case FigureState.Rotation:
                     {
                        figures[active].figureState = FigureState.Relaxing;
                        figures[active].rotation = lastRotation;
                        figures[active].RecalcVertices();
                        break;
                     }
                  }
               }
               break;
            }
         }

         base.OnMouseDown(e);
      }

      protected override void OnMouseMove(MouseMoveEventArgs e)
      {
         if (isActive)
         {
            switch (figures[active].figureState)
            {
               // Изменение размера фигуры во время процедуры создания новой фигуры
               case FigureState.Creation:
               {
                  figures[active].radius = Vector2.Distance(lastMousePress, figures[active].pos) + Vector2.Distance(new Vector2(e.X, e.Y), figures[active].pos);
                  figures[active].RecalcVertices();
                  break;
               }
               // Изменение размера фигуры во время процедуры изменения размера фигуры
               case FigureState.Resizing:
               {
                  figures[active].radius = lastRadius - Vector2.Distance(lastMousePress, figures[active].pos) + Vector2.Distance(new Vector2(e.X, e.Y), figures[active].pos);
                  figures[active].RecalcVertices();
                  break;
               }
               // Изменение положения фигуры во время процедуры передвижения фигуры
               case FigureState.Moving:
               {
                  figures[active].pos = lastPosition + new Vector2(e.X, e.Y) - lastMousePress;
                  figures[active].RecalcVertices();
                  break;
               }
               // Изменение угла поворота фигуры во время процедуры вращения фигуры
               case FigureState.Rotation:
               {
                  if(lastMousePress.Y < figures[active].pos.Y)
                     figures[active].rotation = lastRotation + (e.X - lastMousePress.X) * 0.01f;
                  else
                     figures[active].rotation = lastRotation - (e.X - lastMousePress.X) * 0.01f;

                  figures[active].RecalcVertices();
                  break;
               }
            }
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

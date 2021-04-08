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
   class Point
   {
      Color4 color;

      public Point(Color4 color)
      {
         this.color = new Color4(color.R, color.G, color.B, color.A);
      }
   }
}

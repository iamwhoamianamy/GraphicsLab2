using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsLab2
{
   class Program
   {
      public static void Main()
      {
         using (Game game = new Game(800, 800, "LearnOpenTK"))
         {

            game.Run(60.0);
         }
      }
   }
}

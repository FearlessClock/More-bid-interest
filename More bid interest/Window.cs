using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace More_bid_interest
{
    class Window
    {
        public Vector2 pos;
        public int width;
        public int height;
        public string[] texts;
        TextToScreen.TextWriter tw = new TextToScreen.TextWriter("Alphabet\\");

        public Window(Vector2 posistion, int W, int H, string[] text)
        {
            pos = posistion;
            width = W;
            height = H;
            texts = text;
        }

        /*public void Draw()
        {
            
            for(int i = 0; i < texts.Length && i < height; i++)
            {
                Console.SetCursorPosition((int)pos.X, (int)pos.Y+i);
                string conc;
                if (texts[i].Length > width)
                    conc = texts[i].Remove(width, texts[i].Length - width);
                else
                    conc = texts[i];
                Console.WriteLine(conc);
            }
        }*/
        public void Draw(string title)
        {
            //this isn't good! I am redrawing the title multiple times
            tw.WriteToScreen(new Vector2(0, 0), title, width, 10);

            for (int i = 0; i < texts.Length; i++)
            {
                tw.WriteToScreen(new Vector2(pos.X, pos.Y+i*20), texts[i], width, 10);
            }
        }
    }
}

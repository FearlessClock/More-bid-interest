using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace More_bid_interest
{
    class InteractiveWindow : Window
    {
        int selected;
        public ConsoleColor color = ConsoleColor.Green;
        List<Func<bool>> callBacks = new List<Func<bool>>();
        TextToScreen.TextWriter tw = new TextToScreen.TextWriter("Alphabet\\");

        public InteractiveWindow(Vector2 position, int W, int H, string[] text, List<Func<bool>> calls, int sel = 0) : base(position, W, H, text)
        {
            selected = sel;
            callBacks = calls;
        }

        public int Update()
        {
            selected += Camera.GetDirButtonPress;
            if (selected >= texts.Length)
                selected = 0;
            else if (selected < 0)
                selected = texts.Length - 1;
            if (selected < callBacks.Count && Camera.HasItemBeenSelected == 1)
                callBacks[selected]();
            return -1;
        }

        /*public new void Draw()
        {
            
            for(int i = 0; i < texts.Length; i++)
            {
                Console.SetCursorPosition((int)pos.X, (int)pos.Y+i);
                if (i == selected)
                {
                    Console.BackgroundColor = color;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                string conc;
                if (texts[i].Length > width)
                    conc = texts[i].Remove(width, texts[i].Length - width - 1);
                else
                    conc = texts[i];
                Console.WriteLine(conc);
                if (i == selected)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
        */
        public new void Draw()
        {
            for (int i = 0; i < texts.Length; i++)
            {
                tw.WriteToScreen(new Vector2(pos.X, pos.Y + i * 20), texts[i], width, 10, i == selected);
            }
        }
    }
}

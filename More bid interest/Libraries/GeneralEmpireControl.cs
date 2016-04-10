using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace More_bid_interest
{
    class GeneralEmpireControl
    {
        static int palaceLevel = 0;
        static Stopwatch st;
        static int step = 5000;
        static public void Setup()
        {
            st = new Stopwatch();
            st.Start();
        }
        static public int GetPalaceLevel
        {
            get { return palaceLevel; }
        }

        static public bool LevelUpPalace()
        {
            int levelCost = 1000;
            if (Player.Money >= levelCost)    //Take away the hard coded cost
            {
                if(Player.TakeAwayMoney(levelCost))
                {
                    palaceLevel++;
                }
                return true;
            }
            return false;
        }

        static public bool BackgroundUpdate()
        {
            if (st.ElapsedMilliseconds > step)
            {
                st.Restart();
                Player.GiveMoney += palaceLevel * 5;
            }
            return true;
        }
    }
}

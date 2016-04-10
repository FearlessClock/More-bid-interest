using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace More_bid_interest
{
    class Player
    {
        static List<Item> inventory = new List<Item>();
        static public List<Item> GetPlayerInventory
        {
            get { return inventory; }
        }
        static public void AddInventory(Item i)
        {
            inventory.Add(i);
        }                                          
        static double mash = 0;
        static string playerName = "Player";
        static int money = 4000;

        static public int Money
        {
            get { return money; }
        }
        static public int GiveMoney
        {
            get { return money; }
            set { money = value; }
        }

        static public bool TakeAwayMoney(int value)
        {
            if (money - value >= 0)
            {
                money -= value;
                return true;
            }
            else
            {
                return false;
            }
        }

        static public void RemoveMoney(int amount)
        {
            money -= amount;
        }

        static public bool HasFinishedMashing(int max, int highestBid)
        {
            return (money > highestBid && Camera.HasFinishedMashing(max));
        }
    }
}

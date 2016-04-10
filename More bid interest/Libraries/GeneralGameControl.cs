using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace More_bid_interest
{
    class GeneralGameControl
    {
        static long timeTillNextRound = 0;
        static int timeBetweenRounds = 1500;
        //A list of all the items in the game and not owned by the player
        static List<Item> inventory = new List<Item>();
        static List<Item> CurrentlyOnSaleItems = new List<Item>();
        
        //A list of all the buyers in the game
        static List<Buyer> ListOfBuyers = new List<Buyer>();
        static List<Buyer> CurrentBuyers = new List<Buyer>();

        static Random rand;
        static public int GetTimeTillNextBid
        {
            get {
                timeTillNextRound = 1500 - stopWatch.ElapsedMilliseconds;
                if (timeTillNextRound < 0) timeTillNextRound = 0;
                return (int)timeTillNextRound; }
        }

        static Stopwatch stopWatch;

        /// <summary>
        /// Launch the stopwatch to start the timer for the round start
        /// </summary>
        static public void StartStopwatch()
        {
            stopWatch = new Stopwatch();
            stopWatch.Restart();
            stopWatch.Start();
        }

        static public void setupItems(string filePath, Random random)
        {
            rand = random;
            XmlDocument ItemSettings = new XmlDocument();
            ItemSettings.Load(filePath);
            XmlNodeList nodes = ItemSettings.SelectNodes("//Items/Item");
            for(int i = 0; i < nodes.Count; i++)
            {
                if(nodes[i].Attributes["Owned"].Value.Equals("0"))
                {
                    inventory.Add(new Item(nodes[i].Attributes["Name"].Value, Convert.ToDouble(nodes[i].Attributes["BasePrice"].Value), Convert.ToInt32(nodes[i].Attributes["Owned"].Value)));
                }
                else
                {
                    Player.AddInventory(new Item(nodes[i].Attributes["Name"].Value, Convert.ToDouble(nodes[i].Attributes["BasePrice"].Value), 1));
                }
            }
        }
        static public void setupBuyers(string filePath)
        {
            XmlDocument buyersSettings = new XmlDocument();
            buyersSettings.Load(filePath);
            XmlNodeList nodes = buyersSettings.SelectNodes("//Buyers/Buyer");
            for (int i = 0; i < nodes.Count; i++)
            {
                if (ListOfBuyers != null && nodes[i] != null)
                    ListOfBuyers.Add(new Buyer(nodes[i].Attributes["Name"].Value, 
                        Convert.ToInt32(nodes[i].Attributes["Money"].Value), 
                        Convert.ToDouble(nodes[i].Attributes["MashSpeed"].Value)/100));
            }
        }
        static public void StartNewBid(List<Item> playerInven, GeneralBidControl bid)
        {
            bid.Setup(playerInven);
        }

        static public List<Item> FillListOfAuctionedItems(List<Item> ItemsPutOnSaleByPlayer)
        {
            CurrentlyOnSaleItems.Clear();
            int nmbrOfItems = rand.Next(1, 4);
            for(int i = 0; i < nmbrOfItems; i++)
            {
                int choice;
                if (Player.GetPlayerInventory.Count > 0)
                    choice = rand.Next(0, 2);
                else
                    choice = 1;
                if (ItemsPutOnSaleByPlayer.Count > 0 && choice == 0)
                {
                    int val = rand.Next(0, ItemsPutOnSaleByPlayer.Count);
                    CurrentlyOnSaleItems.Add(ItemsPutOnSaleByPlayer[val]);
                    ItemsPutOnSaleByPlayer.RemoveAt(val);
                }
                else if (inventory.Count > 0 && choice == 1)
                {
                    int val = rand.Next(0, inventory.Count);
                    CurrentlyOnSaleItems.Add(inventory[val]);
                    inventory.RemoveAt(val);
                }
            }
            return CurrentlyOnSaleItems;
        }

        static public void AddItem(Item i)
        {
            inventory.Add(i);
        }

        static public List<Buyer> FillListOfBuyers()
        {
            int maxNmbrOfBuyers = 4;
            int nmbrOfBuyers = rand.Next(2, maxNmbrOfBuyers);
            if (ListOfBuyers.Count > 0)
            {
                for (int i = 0; i < nmbrOfBuyers; i++)
                {
                    int val = rand.Next(0, ListOfBuyers.Count);
                    if (!CurrentBuyers.Contains(ListOfBuyers[val]))
                        CurrentBuyers.Add(ListOfBuyers[val]);
                    else
                        i--;
                }
            }
            return CurrentBuyers;
        }
    }
}

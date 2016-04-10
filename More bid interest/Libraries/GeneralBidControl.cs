using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace More_bid_interest
{
    class GeneralBidControl
    {
        List<Item> itemsOnSale = new List<Item>();
        int nextItemOnSale;
        List<Buyer> buyersInAuction = new List<Buyer>();
        Stopwatch stopWatch;
        TimeSpan LengthOfBid = new TimeSpan(0, 0, 20);
        bool roundActive = true;
        int mash = 0;
        string mashText = "";
        public string[] GetMashText
        {
            get { return new string[] { mashText }; }
        }
        public void SetRandomTime()
        {
            Random rand = new Random(DateTime.Now.Millisecond * 36);
            LengthOfBid = new TimeSpan(0, 0, rand.Next(15, 20));
        }
        public TimeSpan GetRemainingTime
        {
            get
            {
                TimeSpan time = new TimeSpan();
                if (stopWatch.Elapsed != null)
                    time = LengthOfBid - stopWatch.Elapsed;
                return time;
            }
        }
        //0 = player
        //1+ = buyers
        int[] lastBidValues;
        int highestBid;
        int HighestBidder = 0;
        public string GetHighestBidder
        {
            get {
                if (HighestBidder > -1 && HighestBidder < buyersInAuction.Count)
                    return buyersInAuction[HighestBidder].GetName;
                else if (HighestBidder == buyersInAuction.Count)
                    return "You";
                else
                    return "No-one";
            }
        }
        public GeneralBidControl()
        {
            nextItemOnSale = 0;
            stopWatch = new Stopwatch();
        }

        public string[] GetDebugText
        {
            get
            {
                return new string[2] { interRound.ToString(), "Highest Bid " + highestBid + " Highest bidder " + HighestBidder };
            }
        }

        /// <summary>
        /// Setup the controller for the current bidding round
        /// Get the items and the buyers and reset everything
        /// </summary>
        /// <param name="playerInven"></param>
        public void Setup(List<Item> playerInven)
        {
            itemsOnSale.Clear();
            buyersInAuction.Clear();
            highestBid = 0;
            HighestBidder = 0;
            roundActive = true;
            itemsOnSale = GeneralGameControl.FillListOfAuctionedItems(playerInven);
            buyersInAuction = GeneralGameControl.FillListOfBuyers();
            lastBidValues = new int[buyersInAuction.Count + 1];
            for (int i = 0; i < lastBidValues.Length; i++)
            {
                lastBidValues[i] = 0;
            }
            highestBid = 0;
            SetRandomTime();
            stopWatch.Restart();
            stopWatch.Start();
            roundActive = true;
        }

        public string PlayerBid
        {
            get { return lastBidValues[lastBidValues.Length - 1].ToString(); }
        }
        public string[] GetBuyerNames
        {
            get
            {
                if (buyersInAuction.Count > 0)
                {
                    string[] names = new string[buyersInAuction.Count];
                    for (int i = 0; i < buyersInAuction.Count; i++)
                    {
                        names[i] = buyersInAuction[i].GetName;
                        //names[i] = buyersInAuction[i].GetName;
                       // names[i] += " " + lastBidValues[i];
                    }
                    return names;
                }
                return new string[0];
            }
        }

        public string[] GetNextItemName
        {
            get
            {
                string[] res = new string[7];
                res[0] = "The item for sale is a(n): ";
                res[2] = "Who wants to bid more then ";
                res[3] = GetHighestBid.ToString();
                res[4] = "The current highest bidder is ";
                res[5] = GetHighestBidder;
                if (GetNextItem.GetOwner == 1)
                    res[6] = "The item is offered by you";
                else
                    res[6] = "The item is auctioned by an annonemouse seller"; 
                if (itemsOnSale.Count > 0 && nextItemOnSale < itemsOnSale.Count - 1)
                {
                    res[1] = itemsOnSale[nextItemOnSale].GetName;
                    return res;
                }
                return new string[] { "The current bidding session", "is over", "The item has been sold to " , GetHighestBidder};
            }
        }

        public Item GetNextItem
        {
            get
            {
                if (itemsOnSale.Count > 0 && nextItemOnSale < itemsOnSale.Count - 1)
                    return itemsOnSale[nextItemOnSale];
                return new Item("Nothing", 0, 0); ;
            }
        }

        public bool RemoveSoldItem()
        {
            //itemsOnSale.RemoveAt(nextItemOnSale);
            if (itemsOnSale.Count > 0)
                return true;
            return false;
        }
        //flag to pause the game between bid rounds
        bool interRound = false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns true if the bidding is finished</returns>
        public bool BidState()
        {
            int bidValue = 100;
            int maxMash = 10;
            int breakTime = 10000;
            //I have a list of Buyers, The current item, the last bids from each buyer and the player mashing his button
            if (!GetNextItem.GetName.Equals("Nothing") && roundActive)
            {
                for (int i = 0; i < buyersInAuction.Count; i++)
                {
                    buyersInAuction[i].Update(highestBid, maxMash);
                    if (buyersInAuction[i].CallBid && buyersInAuction[i].BankStatement > lastBidValues[i] + bidValue)
                    {
                        lastBidValues[i] = highestBid + bidValue;
                        highestBid += bidValue;
                        HighestBidder = i;
                    }
                }
                UpdateMash();
                if (Player.HasFinishedMashing(maxMash, highestBid))
                {
                    lastBidValues[lastBidValues.Length - 1] = highestBid + bidValue;   //The player lasstbid is the last value in the array
                    highestBid += bidValue;
                    HighestBidder = lastBidValues.Length - 1;
                }
                if (GetRemainingTime < TimeSpan.Zero || itemsOnSale.Count <= 0)
                {
                    roundActive = false;
                    stopWatch.Stop();
                    stopWatch.Reset();
                    //Give the item to the player if he won
                    if (GetHighestBidder.Equals("You"))
                    {
                        Player.AddInventory(GetNextItem);
                        Player.RemoveMoney(highestBid);
                    }
                    else
                    {
                        if(GetNextItem.GetOwner == 1)
                        {
                            Player.GiveMoney += highestBid;
                        }
                        GeneralGameControl.AddItem(GetNextItem);
                    }
                    RemoveSoldItem();
                    interRound = true;
                    if(itemsOnSale.Count <= 0)
                    {
                        //Add code to make the game tell 
                        //the player that the bid is over 
                        //and then make the player go back 
                        //to the main menu
                        stopWatch.Stop();
                    }
                }
            }
            else  if (!GetNextItem.GetName.Equals("Nothing") && interRound)
            {
                if (!stopWatch.IsRunning)
                {
                    nextItemOnSale++;
                    if (GetNextItem.GetName.Equals("Nothing"))
                        return true;
                    stopWatch.Start();
                }
                if (stopWatch.ElapsedMilliseconds > breakTime)
                    interRound = false;
            }
            else if (!GetNextItem.GetName.Equals("Nothing") && !interRound)
            {
                roundActive = true;
                highestBid = 0;
                HighestBidder = -1;
                lastBidValues = new int[buyersInAuction.Count + 1];
                stopWatch.Restart();
            }

            highestBid = GetHighestBid;
            return true;
        }



        public int GetHighestBid
        {
            get
            {
                int highestIndex = 0;
                int highestVal = 0;
                for (int i = 0; i < lastBidValues.Length; i++)
                {
                    if (lastBidValues[i] > highestVal)
                    {
                        highestVal = lastBidValues[i];
                        highestIndex = i;
                    }
                }
                return highestVal;
            }
        }

        private void UpdateMash()
        {

            if (mash != Camera.HasButtonBeenMashed)
            {
                mash = Camera.HasButtonBeenMashed;
            }
            else
            {
                Camera.NoChange();
                mash = Camera.HasButtonBeenMashed;
            }
            mashText = "";
            for (int i = 0; i < mash; i++)
            {
                mashText += "#";
            }
        }
    }
}

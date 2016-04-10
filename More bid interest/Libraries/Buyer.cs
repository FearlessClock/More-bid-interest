using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace More_bid_interest
{
    class Buyer
    {
        string name;
        int money;
        double mash;
        double mashSpeed;
        bool ImOut;
        int lastBid = 0;
        bool bid;
        public bool CallBid
        {
            get
            {
                bool call = bid;
                bid = false;
                return call;
            }
        }
        public string GetName
        {
            get { return name; }
        }

        public int BankStatement
        {
            get { return money; }
        }
        public Buyer(string name, int mon, double mashSpeed)
        {
            this.name = name;
            money = mon;
            this.mashSpeed = mashSpeed;
        }
        
        public void Update(int lastBid, int mashLimit)
        {
            if (money > lastBid + 100)
                mash += mashSpeed;
            else
                ImOut = false;
            if(mash > mashLimit)
            {
                bid = true;
                mash = 0;
            }
        }
    }
}

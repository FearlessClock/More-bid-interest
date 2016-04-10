using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace More_bid_interest
{
    struct Item
    {
        string name;
        int owner;
        public string GetName
        {
            get { return name; }
        }
        double price;
        public int GetOwner
        {
            get { return owner; }
        }

        public Item(string name, double price, int owner)
        {
            this.name = name;
            this.price = price;
            this.owner = owner;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ProfitLoss
{
    public class Deal
    {
        public Deal(decimal qty, decimal price)
        {
            Qty = qty;
            Price = price;
        }

        public decimal Qty { get; }

        public decimal Price { get; }
    }
}

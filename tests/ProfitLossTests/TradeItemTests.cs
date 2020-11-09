using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ProfitLoss;

namespace ProfitLossTests
{
    public class TradeItemTests
    {
        [Test]
        public void CanAddDeals()
        {
            var d1 = new TradeItem(10, 100);
            var d2 = new TradeItem(10, 300);

            var d3 = d1.Apply(d2);

            Assert.AreEqual(20, d3.Qty);
            Assert.AreEqual(200, d3.Price);
            Assert.True(d3.IsBuy);
        }

        [Test]
        public void CanCalcAvgByAddingDeal()
        {
            var deals = new TradeItem[]
            {
                new TradeItem(10, 100),
                new TradeItem(5, 120),
                new TradeItem(20, 140),
                new TradeItem(17, 147),
                new TradeItem(23, 169),
                new TradeItem(25, 109),
            };

            var d1 = deals.Aggregate((d1, d2) => d1.Apply(d2));

            var buyQty = deals.Sum(d => d.Qty);
            var avgBuyPrice = buyQty > 0 ? deals.Sum(d => d.Qty * d.Price) / buyQty : 0;
            var d2 = new TradeItem(buyQty, avgBuyPrice);

            Assert.True(d1.TheSame(d2));
        }
    }
}

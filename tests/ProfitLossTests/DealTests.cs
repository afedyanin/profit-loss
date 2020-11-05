using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ProfitLoss;

namespace ProfitLossTests
{
    public class DealTests
    {
        [Test]
        public void CanAddDeals()
        {
            var d1 = new Deal(10, 100);
            var d2 = new Deal(10, 300);

            var d3 = d1 + d2;

            Assert.AreEqual(20, d3.Qty);
            Assert.AreEqual(200, d3.Price);
            Assert.True(d3.IsBuy);
        }

        [Test]
        public void CanCalcAvgByAddingDeal()
        {
            var deals = new Deal[]
            {
                new Deal(10, 100),
                new Deal(5, 120),
                new Deal(20, 140),
                new Deal(17, 147),
                new Deal(23, 169),
                new Deal(25, 109),
            };

            var d1 = deals.Aggregate((d1, d2) => d1 + d2);

            var buyQty = deals.Sum(d => d.Qty);
            var avgBuyPrice = buyQty > 0 ? deals.Sum(d => d.Qty * d.Price) / buyQty : 0;
            var d2 = new Deal(buyQty, avgBuyPrice);

            Assert.AreEqual(d1, d2);
        }
    }
}

using System;
using NUnit.Framework;
using ProfitLoss;

namespace ProfitLossTests
{
    public class PositionTests
    {
        private static (decimal qty, decimal price)[] InitialDeals => new (decimal, decimal)[]
        {
            (12, 100),
            (17, 99),
            (-9, 101),
            (-4, 105),
            (3, 103),
        };

        private static (decimal qty, decimal price)[] ShortTrade => new (decimal, decimal)[]
        {
            (-20, 100), // -2000
            (10, 80),  // PNL = 200
        };

        [Test]
        public void CanAddSingleDeals()
        {
            var p = new Position();

            p.Append(InitialDeals[0].qty, InitialDeals[0].price);
            Assert.AreEqual(decimal.Zero, p.RealizedProfitLoss);
            Assert.AreEqual(100, p.AvgPrice);
            Assert.AreEqual(12, p.Qty);

            p.Append(InitialDeals[1].qty, InitialDeals[1].price);
            Assert.AreEqual(decimal.Zero, p.RealizedProfitLoss);
            var totalQty = 12m + 17m;
            var avgPrice = ((100m * 12m) + (99m * 17m)) / totalQty;
            Assert.AreEqual(avgPrice, p.AvgPrice);
            Assert.AreEqual(totalQty, p.Qty);

            p.Append(InitialDeals[2].qty, InitialDeals[2].price);
            var pnl = 9 * (InitialDeals[2].price - avgPrice);
            totalQty -= 9;
            Assert.AreEqual(pnl, p.RealizedProfitLoss);
            Assert.AreEqual(avgPrice, p.AvgPrice);
            Assert.AreEqual(totalQty, p.Qty);

            p.Append(InitialDeals[3].qty, InitialDeals[3].price);
            pnl += 4 * (InitialDeals[3].price - avgPrice);
            totalQty -= 4;
            Assert.AreEqual(pnl, p.RealizedProfitLoss);
            Assert.AreEqual(avgPrice, p.AvgPrice);
            Assert.AreEqual(totalQty, p.Qty);

            var amt = p.Amount;
            p.Append(InitialDeals[4].qty, InitialDeals[4].price);
            totalQty += InitialDeals[4].qty;
            var dealAmt = InitialDeals[4].qty * InitialDeals[4].price;
            avgPrice = (amt + dealAmt) / totalQty;
            Assert.AreEqual(pnl, p.RealizedProfitLoss);
            Assert.AreEqual(avgPrice, p.AvgPrice);
            Assert.AreEqual(totalQty, p.Qty);

            Console.WriteLine(p);

            var p2 = new Position();

            foreach (var d in InitialDeals)
            {
                p2.Append(d.qty, d.price);
            }

            Console.WriteLine(p2);

            var p3 = new Position(InitialDeals);
            Console.WriteLine(p3);
        }

        [Test]
        public void CanCalculateBasicValues()
        {
            var p = new Position(InitialDeals);

            Assert.AreEqual(19m, p.Qty);
            Assert.AreEqual(99.75m, p.AvgPrice);
            Assert.AreEqual(32.25m, Math.Round(p.RealizedProfitLoss, 2));
        }

        [TestCase(99, -14.25)]
        public void CanGetUnrealizedPnL(decimal exitPrice, decimal expectedPnL)
        {
            var p = new Position(InitialDeals);

            var unrealizedPnL = p.GetUnrealizedProfitLoss(exitPrice);

            Assert.AreEqual(expectedPnL, unrealizedPnL);
        }

        [Test]
        public void Scenario1FiilIncreasePosition()
        {
            var p = new Position(InitialDeals);

            p.Append(10m, 100m);

            Assert.AreEqual(29m, p.Qty);
            Assert.AreEqual(99.8362m, Math.Round(p.AvgPrice, 4));
            Assert.AreEqual(32.25m, Math.Round(p.RealizedProfitLoss, 2));
        }

        [Test]
        public void Scenario2FiilPartiallyDecreasePosition()
        {
            var p = new Position(InitialDeals);

            p.Append(-12m, 101m);

            Assert.AreEqual(7, p.Qty);
            Assert.AreEqual(99.75m, p.AvgPrice);
            Assert.AreEqual(47.25m, Math.Round(p.RealizedProfitLoss, 2));
        }

        [Test]
        public void Scenario3FiilFlattenPosition()
        {
            var p = new Position(InitialDeals);

            p.Append(-19m, 101m);

            Assert.AreEqual(decimal.Zero, p.Qty);
            Assert.AreEqual(decimal.Zero, p.AvgPrice);
            Assert.AreEqual(56m, Math.Round(p.RealizedProfitLoss, 2));
        }

        [Test]
        public void Scenario4FiilReversePosition()
        {
            var p = new Position(InitialDeals);

            p.Append(-22m, 101m);

            Assert.AreEqual(-3, p.Qty);
            Assert.AreEqual(101m, p.AvgPrice);
            Assert.AreEqual(56m, Math.Round(p.RealizedProfitLoss, 2));
        }

        [Test]
        public void CanGetPnLForShortTrade()
        {
            var p = new Position(ShortTrade);

            Assert.AreEqual(200m, p.RealizedProfitLoss);
        }
    }
}

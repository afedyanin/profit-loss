using System;
using System.Collections.Generic;
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

        private static (decimal qty, decimal price)[] FullTrades => new (decimal, decimal)[]
        {
            (12, 100),
            (17, 99),
            (-9, 101),
            (-4, 105),
            (3, 103),
            (10, 100),
            (-12, 101),
            (-19, 101),
            (-22, 101),
        };

        [Test]
        public void CanAddSingleDeals()
        {
            var p1 = new Position(InitialDeals[0].qty, InitialDeals[0].price);

            Assert.True(new TradeItem(12, 100).TheSame(p1.Current));
            Console.WriteLine(p1);

            var p2 = new Position(InitialDeals[1].qty, InitialDeals[1].price, p1);

            var totalQty = 12m + 17m;
            var avgPrice = ((100m * 12m) + (99m * 17m)) / totalQty;
            Assert.True(new TradeItem(totalQty, avgPrice).TheSame(p2.Current));
            Console.WriteLine(p2);

            var p3 = new Position(InitialDeals[2].qty, InitialDeals[2].price, p2);

            var pnl = 9 * (InitialDeals[2].price - avgPrice);
            totalQty -= 9;
            Assert.True(new TradeItem(totalQty, avgPrice, pnl).TheSame(p3.Current));
            Console.WriteLine(p3);

            var p4 = new Position(InitialDeals[3].qty, InitialDeals[3].price, p3);

            pnl = 4 * (InitialDeals[3].price - avgPrice);
            totalQty -= 4;
            Assert.True(new TradeItem(totalQty, avgPrice, pnl).TheSame(p4.Current));
            Console.WriteLine(p4);

            var amt = p4.Current.Amount;
            var p5 = new Position(InitialDeals[4].qty, InitialDeals[4].price, p4);

            totalQty += InitialDeals[4].qty;
            var dealAmt = InitialDeals[4].qty * InitialDeals[4].price;
            avgPrice = (amt + dealAmt) / totalQty;
            Assert.True(new TradeItem(totalQty, avgPrice).TheSame(p5.Current));
            Console.WriteLine(p5);
        }

        [Test]
        public void CanCalculateBasicValues()
        {
            var p = new Position(InitialDeals);
            var shouldBe = new TradeItem(19m, 99.75m, 32.25m);

            Console.WriteLine(shouldBe);
            Console.WriteLine(p.Current);

            Assert.True(shouldBe.TheSame(p.Current));
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
            var p0 = new Position(InitialDeals);
            var p = new Position(10, 100, p0);
            var shouldBe = new TradeItem(29m, 99.8362m);

            Assert.True(shouldBe.TheSame(p.Current));
            Assert.AreEqual(32.25m, Math.Round(p0.Current.RealizedProfitLoss, 2));
            Assert.AreEqual(decimal.Zero, p.Current.RealizedProfitLoss);
        }

        [Test]
        public void Scenario2FiilPartiallyDecreasePosition()
        {
            var p0 = new Position(InitialDeals);
            var p = new Position(-12, 101, p0);

            // 32.25m + 15 = 47.25m
            var pnl = p0.Current.RealizedProfitLoss + p.Current.RealizedProfitLoss;
            var shouldBe = new TradeItem(7, 99.75m, 15);

            Assert.True(shouldBe.TheSame(p.Current));
            Assert.AreEqual(47.25m, Math.Round(pnl, 2));
        }

        [Test]
        public void Scenario3FiilFlattenPosition()
        {
            var p0 = new Position(InitialDeals);
            var p = new Position(-19m, 101m, p0);

            // 32.25m + 23.75m = 56m
            var pnl = p0.Current.RealizedProfitLoss + p.Current.RealizedProfitLoss;
            var shouldBe = new TradeItem(decimal.Zero, decimal.Zero, 23.75m);

            Assert.True(shouldBe.TheSame(p.Current));
            Assert.AreEqual(56m, Math.Round(pnl, 2));
        }

        [Test]
        public void Scenario4FiilReversePosition()
        {
            var p0 = new Position(InitialDeals);
            var p = new Position(-22m, 101m, p0);

            // 32.25m + 23.75m = 56m
            var pnl = p0.Current.RealizedProfitLoss + p.Current.RealizedProfitLoss;
            var shouldBe = new TradeItem(-3, 101m, 23.75m);

            Assert.True(shouldBe.TheSame(p.Current));
            Assert.AreEqual(56m, Math.Round(pnl, 2));
        }

        [Test]
        public void CanGetPnLForShortTrade()
        {
            var p = new Position(ShortTrade);
            Assert.AreEqual(200m, p.Current.RealizedProfitLoss);
        }

        [Test]
        public void CanCalculateFullTrades()
        {
            var p = Position.Empty;
            var res = new List<Position>() { p };

            foreach (var (qty, price) in FullTrades)
            {
                p = new Position(qty, price, p);
                res.Add(p);
            }

            foreach (var pos in res)
            {
                Console.WriteLine(pos);
            }
        }
    }
}

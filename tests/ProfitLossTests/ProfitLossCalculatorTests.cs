using System;
using NUnit.Framework;
using ProfitLoss;

namespace ProfitLossTests
{
    public class ProfitLossCalculatorTests
    {
        private static Deal[] InitialDeals => new Deal[]
        {
            new Deal(12m, 100m),
            new Deal(17m, 99m),
            new Deal(-9m, 101m),
            new Deal(-4m, 105m),
            new Deal(3m, 103m),
        };

        [Test]
        public void CanCalculateBasicValues()
        {
            var pnl = new ProfitLossCalculator(InitialDeals);

            Assert.AreEqual(19m, pnl.Position);
            Assert.AreEqual(99.75m, pnl.AvgOpenPrice);
            Assert.AreEqual(32.25m, Math.Round(pnl.RealizedProfitLoss, 2));
        }

        [TestCase(99, -14.25)]
        public void CanGetUnrealizedPnL(decimal exitPrice, decimal expectedPnL)
        {
            var pnl = new ProfitLossCalculator(InitialDeals);

            var unrealizedPnL = pnl.GetUnrealizedProfitLoss(exitPrice);

            Assert.AreEqual(expectedPnL, unrealizedPnL);
        }

        [Test]
        public void Scenario1FiilIncreasePosition()
        {
            var pnl = new ProfitLossCalculator(InitialDeals);

            pnl.AddDeal(new Deal(10m, 100m));

            Assert.AreEqual(29m, pnl.Position);
            Assert.AreEqual(99.83620m, pnl.AvgOpenPrice);
            Assert.AreEqual(32.25m, Math.Round(pnl.RealizedProfitLoss, 2));
        }

        [Test]
        public void Scenario2FiilPartiallyDecreasePosition()
        {
            var pnl = new ProfitLossCalculator(InitialDeals);

            pnl.AddDeal(new Deal(-12m, 101m));

            Assert.AreEqual(7, pnl.Position);
            Assert.AreEqual(99.75m, pnl.AvgOpenPrice);
            Assert.AreEqual(47.25m, Math.Round(pnl.RealizedProfitLoss, 2));
        }

        [Test]
        public void Scenario3FiilFlattenPosition()
        {
            var pnl = new ProfitLossCalculator(InitialDeals);

            pnl.AddDeal(new Deal(-19m, 101m));

            Assert.AreEqual(decimal.Zero, pnl.Position);
            Assert.AreEqual(decimal.Zero, pnl.AvgOpenPrice);
            Assert.AreEqual(56m, Math.Round(pnl.RealizedProfitLoss, 2));
        }

        [Test]
        public void Scenario4FiilReversePosition()
        {
            var pnl = new ProfitLossCalculator(InitialDeals);

            pnl.AddDeal(new Deal(-22m, 101m));

            Assert.AreEqual(-3, pnl.Position);
            Assert.AreEqual(101m, pnl.AvgOpenPrice);
            Assert.AreEqual(56m, Math.Round(pnl.RealizedProfitLoss, 2));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace ProfitLoss
{
    public class ProfitLossCalculator
    {
        private readonly IList<Deal> _buyDeals;

        private readonly IList<Deal> _sellDeals;

        public ProfitLossCalculator(Deal[] deals = null)
        {
            deals ??= Array.Empty<Deal>();
            
            _sellDeals = new List<Deal>();
            _buyDeals = new List<Deal>();

            Init(deals);
        }

        public decimal RealizedProfitLoss { get; private set; }

        public decimal Position { get; private set; }

        public decimal AvgOpenPrice { get; private set; }

        protected decimal TotalBuyQty { get; private set; }

        protected decimal AvgBuyPrice { get; private set; }

        protected decimal TotalSellQty { get; private set; }

        protected decimal AvgSellPrice { get; private set; }

        public decimal GetUnrealizedProfitLoss(decimal exitPrice)
        {
            return (exitPrice - AvgOpenPrice) * Position;
        }

        public void AddDeal(Deal deal)
        {
            if (deal == null)
            {
                throw new ArgumentNullException(nameof(deal));
            }

            if (deal.Qty < 0)
            {
                _sellDeals.Add(deal);
            }
            else
            {
                _buyDeals.Add(deal);
            }

            CalculateBaseValues();
        }

        private void Init(Deal[] deals)
        {
            foreach (var deal in deals)
            {
                AddDeal(deal);
            }
        }

        private void CalculateBaseValues()
        {
            TotalBuyQty = _buyDeals.Sum(d => d.Qty);
            
            TotalSellQty = _sellDeals.Sum(d => d.Qty) * (-1);

            AvgBuyPrice = TotalBuyQty == decimal.Zero ? 
                decimal.Zero : _buyDeals.Sum(d => d.Qty * d.Price) / TotalBuyQty * 1.0m;

            AvgSellPrice = TotalSellQty == decimal.Zero ? 
                decimal.Zero : _sellDeals.Sum(d => d.Qty * d.Price) * (-1.0m) / TotalSellQty * 1.0m;

            Position = TotalBuyQty - TotalSellQty;

            AvgOpenPrice = Position > decimal.Zero ?
                AvgBuyPrice : Position < decimal.Zero ?
                AvgSellPrice : decimal.Zero;

            var realizedQty = Math.Min(TotalBuyQty, TotalSellQty);

            RealizedProfitLoss = realizedQty == decimal.Zero ? decimal.Zero : realizedQty * (AvgSellPrice - AvgBuyPrice);
        }
    }
}

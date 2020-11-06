using System;
using System.Linq;

namespace ProfitLoss
{
    public class Position
    {
        public Position((decimal qty, decimal price)[] deals = null)
        {
            if (deals == null || deals.Length <= 0)
            {
                return;
            }

            var typed = deals.Select(d => new Deal(d.qty, d.price)).ToArray();
            CalculateBaseValues(typed);
        }

        public decimal AvgPrice => Current.Price;

        public decimal Qty => Current.Qty * Current.Sign;

        public decimal Amount => Current.Amount;

        public decimal RealizedProfitLoss { get; private set; }

        internal Deal Current { get; private set; }

        public override string ToString()
        {
            return $"PnL={RealizedProfitLoss:N4} Qty={Qty:N4} AvgPrice={AvgPrice:N4} Amt={Amount:N4}";
        }

        public decimal GetUnrealizedProfitLoss(decimal exitPrice)
        {
            return Current.GetUnrealizedProfitLoss(exitPrice);
        }

        public void Append(decimal qty, decimal price)
        {
            Append(new Deal(qty, price));
        }

        internal void Append(Deal deal)
        {
            RealizedProfitLoss += Current.GetProfitLoss(deal);
            Current += deal;
        }

        private void CalculateBaseValues(Deal[] deals)
        {
            if (deals.Length <= 0)
            {
                return;
            }

            var buy = deals.Where(d => d.IsBuy).Aggregate((d1, d2) => d1 + d2);
            var sell = deals.Where(d => !d.IsBuy).Aggregate((d1, d2) => d1 + d2);

            Current = buy + sell;

            var realizedQty = Math.Min(buy.Qty, sell.Qty);
            RealizedProfitLoss = realizedQty == 0 ? 0 : realizedQty * (sell.Price - buy.Price);
        }
    }
}

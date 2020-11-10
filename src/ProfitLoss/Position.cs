using System.Linq;

namespace ProfitLoss
{
    public class Position
    {
        public static readonly Position Empty = new Position(decimal.Zero, decimal.Zero);

        public Position((decimal qty, decimal price)[] deals)
        {
            if (deals == null || deals.Length <= 0)
            {
                return;
            }

            var typed = deals.Select(d => new TradeItem(d.qty, d.price)).ToArray();

            Deal = typed.Last();

            var buy = typed.Where(d => d.IsBuy).Aggregate((d1, d2) => d1.Apply(d2));
            var sell = typed.Where(d => !d.IsBuy).Aggregate((d1, d2) => d1.Apply(d2));

            Current = buy.Apply(sell);
        }

        public Position(decimal qty, decimal price, Position previous = null)
        {
            Deal = new TradeItem(qty, price);
            Previous = previous;
            var currentPos = previous == null ? TradeItem.Empty : previous.Current;
            Current = currentPos.Apply(Deal);
        }

        public Position Previous { get; }

        public TradeItem Current { get; }

        public TradeItem Deal { get; }

        public decimal GetUnrealizedProfitLoss(decimal price)
            => Current.GetUnrealizedProfitLoss(price);

        public override string ToString()
        {
            return $"Deal: {Deal}\n\tPosition: {Current}";
        }
    }
}

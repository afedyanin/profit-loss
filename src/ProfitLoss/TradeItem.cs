using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("ProfitLossTests")]

namespace ProfitLoss
{
    public readonly struct TradeItem : IEquatable<TradeItem>
    {
        public static readonly TradeItem Empty = new TradeItem(decimal.Zero, decimal.Zero);

        public TradeItem(
            decimal qty,
            decimal price,
            decimal realizedProfitLoss = decimal.Zero,
            PositionState state = PositionState.Undefined)
        {
            Qty = Math.Abs(qty);
            Price = price;
            IsBuy = qty > decimal.Zero;
            State = state != PositionState.Undefined ?
                    state : Qty == decimal.Zero ?
                    PositionState.Closed : PositionState.Open; 
            RealizedProfitLoss = realizedProfitLoss;
        }

        public decimal Qty { get; }

        public decimal Price { get; }

        public bool IsBuy { get; }

        public decimal Sign => IsBuy ? 1 : (-1);

        public decimal Amount => Qty * Price;

        internal PositionState State { get; }

        internal decimal RealizedProfitLoss { get; }

        #region Equality

        public static bool operator ==(TradeItem left, TradeItem right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TradeItem left, TradeItem right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is TradeItem deal && Equals(deal);
        }

        public bool Equals(TradeItem other)
        {
            return (Qty, Price, IsBuy, State, RealizedProfitLoss) == (other.Qty, other.Price, other.IsBuy, other.State, other.RealizedProfitLoss);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Qty, Price, IsBuy, State, RealizedProfitLoss);
        }

        #endregion

        public override string ToString()
        {
            return $"Q={Qty * Sign:N4} P={Price:N4} A={Amount:N4} PnL={RealizedProfitLoss:N4} S={State}";
        }

        internal bool HasTheSameDirection(TradeItem other)
        {
            return !(IsBuy ^ other.IsBuy);
        }

        internal decimal GetUnrealizedProfitLoss(decimal price)
        {
            return (price - Price) * Qty * Sign;
        }

        internal TradeItem Apply(TradeItem deal)
        {
            if (deal == Empty)
            {
                return this;
            }

            if (this == Empty)
            {
                return deal;
            }

            if (HasTheSameDirection(deal))
            {
                var qty = Qty + deal.Qty;
                var price = (Amount + deal.Amount) / qty;

                return new TradeItem(qty * Sign, price, decimal.Zero, PositionState.Increased);
            }

            var priceDelta = (deal.Price - Price) * Sign;

            if (Qty == deal.Qty)
            {
                var closedPnL = priceDelta * Qty;

                return new TradeItem(decimal.Zero, decimal.Zero, closedPnL, PositionState.Closed);
            }

            if (Qty > deal.Qty)
            {
                var qty = Qty - deal.Qty;
                var price = Price;
                var decreasedPnL = priceDelta * deal.Qty;

                return new TradeItem(qty * Sign, price, decreasedPnL, PositionState.Decreased);
            }
            else
            {
                var qty = deal.Qty - Qty;
                var price = deal.Price;
                var reversePnL = priceDelta * Qty;

                return new TradeItem(qty * deal.Sign, price, reversePnL, PositionState.Reversed);
            }
        }
    }
}

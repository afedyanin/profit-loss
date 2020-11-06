using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("ProfitLossTests")]

namespace ProfitLoss
{
    internal readonly struct Deal : IEquatable<Deal>
    {
        public static readonly Deal NullDeal = new Deal(decimal.Zero, decimal.Zero);

        public Deal(decimal qty, decimal price)
        {
            Qty = Math.Abs(qty);
            Price = price;
            IsBuy = qty > 0;
        }

        public decimal Qty { get; }

        public decimal Price { get; }

        public bool IsBuy { get; }

        public decimal Amount => Qty * Price;

        internal decimal Sign => IsBuy ? 1 : (-1);

        public static Deal operator +(Deal left, Deal right)
        {
            return Add(left, right);
        }

        #region Equality

        public static bool operator ==(Deal left, Deal right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Deal left, Deal right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is Deal deal && Equals(deal);
        }

        public bool Equals(Deal other)
        {
            return (Qty, Price, IsBuy) == (other.Qty, other.Price, other.IsBuy);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Qty, Price, IsBuy);
        }

        #endregion

#pragma warning disable SA1204 // Static elements should appear before instance elements
        public static Deal Add(Deal left, Deal right)
#pragma warning restore SA1204 // Static elements should appear before instance elements
        {
            if (left == NullDeal)
            {
                return right;
            }

            if (right == NullDeal)
            {
                return left;
            }

            if (left.HasTheSameDirection(right))
            {
                var qty = left.Qty + right.Qty;
                var price = (left.Amount + right.Amount) / qty;

                return new Deal(qty * left.Sign, price);
            }

            if (left.Qty == right.Qty)
            {
                return NullDeal;
            }

            if (left.Qty > right.Qty)
            {
                var qty = left.Qty - right.Qty;
                var price = left.Price;

                return new Deal(qty * left.Sign, price);
            }
            else
            {
                var qty = right.Qty - left.Qty;
                var price = right.Price;

                return new Deal(qty * right.Sign, price);
            }
        }

        public decimal GetProfitLoss(Deal other)
        {
            if (other == NullDeal)
            {
                return decimal.Zero;
            }

            if (HasTheSameDirection(other))
            {
                return decimal.Zero;
            }

            var realizedQty = Math.Min(Qty, other.Qty);
            var priceDelta = (other.Price - Price) * Sign;

            return priceDelta * realizedQty;
        }

        public decimal GetUnrealizedProfitLoss(decimal price)
        {
            return (price - Price) * Qty * Sign;
        }

        public bool HasTheSameDirection(Deal other)
        {
            return !(IsBuy ^ other.IsBuy);
        }
    }
}

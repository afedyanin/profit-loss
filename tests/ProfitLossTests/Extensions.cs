using System;
using ProfitLoss;

namespace ProfitLossTests
{
    internal static class Extensions
    {
        public static bool TheSame(this TradeItem item, TradeItem other)
        {
            if (item == TradeItem.Empty)
            {
                return other == TradeItem.Empty;
            }

            if (other == TradeItem.Empty)
            {
                return item == TradeItem.Empty;
            }

            var q1 = Math.Round(item.Qty, 4);
            var q2 = Math.Round(other.Qty, 4);

            if (q1 != q2)
            {
                return false;
            }

            var p1 = Math.Round(item.Price, 4);
            var p2 = Math.Round(other.Price, 4);

            if (p1 != p2)
            {
                return false;
            }

            if (item.IsBuy != other.IsBuy)
            {
                return false;
            }

            var pnl1 = Math.Round(item.RealizedProfitLoss, 4);
            var pnl2 = Math.Round(other.RealizedProfitLoss, 4);

            if (pnl1 != pnl2)
            {
                return false;
            }

            /*
            if (item.State != other.State)
            {
                return false;
            }
            */

            return true;
        }
    }
}

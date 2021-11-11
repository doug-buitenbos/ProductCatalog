using System;

namespace ProductCatalogApi.Models
{
    public class PriceUpdate : IComparable<PriceUpdate>
    {
        public int Id { get; private set; }
        public DateTime UpdatedDateTime { get; set; }
        public Decimal Amount { get; set; }

        public PriceUpdate() { }

        public PriceUpdate(DateTime updateDateTime, decimal amount)
        {
            this.UpdatedDateTime = updateDateTime;
            this.Amount = amount;
        }  

        public int CompareTo(PriceUpdate other)
        {
            if (other == null) return 1;

            return this.UpdatedDateTime.CompareTo(other.UpdatedDateTime);
        }

        public static bool operator <(PriceUpdate left, PriceUpdate right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(PriceUpdate left, PriceUpdate right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(PriceUpdate left, PriceUpdate right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(PriceUpdate left, PriceUpdate right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}

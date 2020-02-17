using System;

namespace Web.API.Application.Models
{
    public class OutOfOffice
    {
        public int ResourceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Reason { get; set; }

        public bool Equals(OutOfOffice other)
        {
            if (other is null)
            {
                return false;
            }
            else
            {
                return this.FromDate == other.FromDate && this.ToDate == other.ToDate && this.Reason == other.Reason;
            }
        }

        public override bool Equals(object obj) => Equals(obj as OutOfOffice);
        public override int GetHashCode() => (ResourceId, FromDate, ToDate, Reason).GetHashCode();
    }
}

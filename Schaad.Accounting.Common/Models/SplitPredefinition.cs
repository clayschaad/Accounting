namespace Schaad.Accounting.Models
{
    public class SplitPredefinition
    {
        public string Id { get; set; }

        public string BookingText { get; set; }

        public decimal BookingValue { get; set; }

        public string AccountId { get; set; }


        /// <summary>
        /// Makes a copy
        /// </summary>
        public void Copy(SplitPredefinition target)
        {
            target.BookingText = BookingText;
            target.BookingValue = BookingValue;
            target.Id = Id;
            target.AccountId = AccountId;
        }
    }
}
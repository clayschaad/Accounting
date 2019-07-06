using Schaad.Accounting.Models;

namespace Schaad.Accounting.Datasets
{
    public class BookingRuleDataset : BookingRule
    {
        public string Account { get; set; }

        public BookingRuleDataset(BookingRule bookingRule, string account)
        {
            this.LookupText = bookingRule.LookupText;
            this.LookupValue = bookingRule.LookupValue;
            this.BookingText = bookingRule.BookingText;
            this.Id = bookingRule.Id;
            this.AccountId = bookingRule.AccountId;
            this.Account = account;
        }
    }
}
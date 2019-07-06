using System.Collections.Generic;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Interfaces
{
    public interface IBookingRuleRepository
    {
        List<BookingRule> GetBookingRuleList();

        void SaveBookingRule(BookingRule bookingRule);

        BookingRule GetBookingRule(string id);
    }
}
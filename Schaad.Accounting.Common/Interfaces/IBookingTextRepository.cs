using System.Collections.Generic;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Interfaces
{
    public interface IBookingTextRepository
    {
        List<BookingText> GetBookingTextList();

        void SaveBookingText(BookingText bookingText);

        BookingText GetBookingText(string id);
    }
}
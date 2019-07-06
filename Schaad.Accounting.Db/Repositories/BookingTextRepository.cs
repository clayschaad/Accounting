using System;
using System.Collections.Generic;
using System.Linq;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Repositories
{
    public class BookingTextRepository : BaseRepository, IBookingTextRepository
    {
        private readonly string BOOKING_TEXTS = "BookingTexts.xml";

        public BookingTextRepository(ISettingsService settingsService) : base(settingsService)
        {
            EnsureFileExisits(BOOKING_TEXTS);
        }

        /// <summary>
        /// Load bokking texts
        /// </summary>
        public List<BookingText> GetBookingTextList()
        {
            var bookingTexts = Load<List<BookingText>>(BOOKING_TEXTS);
            return bookingTexts ?? new List<BookingText>();
        }

        /// <summary>
        /// Save a booking text (insert/update)
        /// </summary>
        public void SaveBookingText(BookingText bookingText)
        {
            var bookingTexts = GetBookingTextList();
            var existingText = bookingTexts.FirstOrDefault(a => a.Id == bookingText.Id);

            if (existingText == null)
            {
                existingText = new BookingText();
                bookingTexts.Add(existingText);
                bookingText.Id = Guid.NewGuid().ToString();
            }
            bookingText.Copy(existingText);
            Save(bookingTexts, BOOKING_TEXTS);
        }

        /// <summary>
        /// Get booking text
        /// </summary>
        public BookingText GetBookingText(string id)
        {
            var bookingTexts = GetBookingTextList();
            return bookingTexts.FirstOrDefault(t => t.Id == id);
        }
    }
}
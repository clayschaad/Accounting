using System;
using System.Collections.Generic;
using System.Linq;
using Schaad.Accounting.Interfaces;
using Schaad.Accounting.Models;

namespace Schaad.Accounting.Repositories
{
    public class BookingRuleRepository : BaseRepository, IBookingRuleRepository
    {
        private readonly string BOOKING_RULES = "BookingRules.xml";

        public BookingRuleRepository(ISettingsService settingsService) : base(settingsService)
        {
            EnsureFileExisits(BOOKING_RULES);
        }

        /// <summary>
        /// Load booking rules
        /// </summary>
        public List<BookingRule> GetBookingRuleList()
        {
            var bookingTexts = Load<List<BookingRule>>(BOOKING_RULES);
            return bookingTexts ?? new List<BookingRule>();
        }

        /// <summary>
        /// Save a booking rule (insert/update)
        /// </summary>
        public void SaveBookingRule(BookingRule bookingRule)
        {
            var bookingRules = GetBookingRuleList();
            var existingRule = bookingRules.FirstOrDefault(a => a.Id == bookingRule.Id);

            if (existingRule == null)
            {
                existingRule = new BookingRule();
                bookingRules.Add(existingRule);
                bookingRule.Id = Guid.NewGuid().ToString();
            }
            bookingRule.Copy(existingRule);
            Save(bookingRules, BOOKING_RULES);
        }

        /// <summary>
        /// Get booking rule
        /// </summary>
        public BookingRule GetBookingRule(string id)
        {
            var bookingRules = GetBookingRuleList();
            return bookingRules.FirstOrDefault(t => t.Id == id);
        }
    }
}
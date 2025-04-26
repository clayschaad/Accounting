using Schaad.Finance.Api;
using System;
using System.Collections.Generic;

namespace Schaad.Accounting.Services
{
    public class DummyFxService : IFxService
    {
        private List<string> currencies = ["CHF", "EUR", "USD"];

        public decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency, string fixerIoApiKey)
        {
            if (currencies.Contains(fromCurrency) && currencies.Contains(fromCurrency))
            {
                return amount;
            }

            throw new NotImplementedException($"Cannot convert from {fromCurrency} to {toCurrency}");
        }

        public decimal GetFxRate(string fromCurrency, string toCurrency, DateTime date, string fixerIoApiKey)
        {
            throw new NotImplementedException();
        }
    }
}

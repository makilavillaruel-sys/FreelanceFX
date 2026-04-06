using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyApp
{
    public class ConversionRecords
    {
        public DateTime Timestamp { get; set; }
        public double AmountFrom { get; set; }
        public string CurrencyFrom { get; set; }
        public double AmountTo { get; set; }
        public string CurrencyTo { get; set; }
        public double Rate { get; set; }

        public override string ToString()
        {
            return $"{Timestamp:dd/MM/yyyy HH:mm}  |  {AmountFrom:N2} {CurrencyFrom}  →  {AmountTo:N2} {CurrencyTo}  (Rate: {Rate:N4})";
        }
    }
}





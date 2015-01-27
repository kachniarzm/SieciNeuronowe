using System;
using MLP_Data.Attributes;

namespace MLP_Data.Entity
{
    class StockExchangeListing4Params
    {
        public double Closing { get; set; }
        public double Volume { get; set; }

        [Oscilator]
        public double Rsi9 { get; set; }

        [Oscilator]
        public double StochasticOscilator14 { get; set; }

        public DateTime Date { get; set; }
    }
}

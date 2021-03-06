﻿using System;
using MLP_Data.Attributes;
using MLP_Data.Enums;

namespace MLP_Data.Entity
{
    class StockExchangeListingWithMacro
    {
        public double Opening { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public double Closing { get; set; }
        public double PrecentageChange { get; set; }
        public double Volume { get; set; }
        [Oscilator]
        public double Sma15 { get; set; }
        [Oscilator]
        public double Sma30 { get; set; }
        [Oscilator]
        public double Sma45 { get; set; }
        [Oscilator]
        public double Wma15 { get; set; }
        [Oscilator]
        public double Wma30 { get; set; }
        [Oscilator]
        public double Wma45 { get; set; }
        [Oscilator]
        public double Ema { get; set; }
        [Oscilator]
        public double Rsi7 { get; set; }
        [Oscilator]
        public double Rsi9 { get; set; }
        [Oscilator]
        public double Rsi14 { get; set; }
        [Oscilator]
        public double StochasticOscilator14 { get; set; }
        public DateTime Date { get; set; }
        public double EURO { get; set; }
        public double USD { get; set; }
        public double CHF { get; set; }
        public double GBP { get; set; }
        public double Zloto { get; set; }
        public IndexName IndexName { get; set; }
    }
}

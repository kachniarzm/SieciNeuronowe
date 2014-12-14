using System;
using MLP_Data.Enums;

namespace MLP_Data.Entity
{
    // polish: Notowanie giełdowe
    // warning: properties order must be the same as in csv file
    public class StockExchangeListing
    {
        public double Opening { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public double Closing { get; set; }
        public double PrecentageChange { get; set; }
        public double Volume { get; set; }
        public DateTime Date { get; set; }
        public IndexName IndexName { get; set; }
    }
}

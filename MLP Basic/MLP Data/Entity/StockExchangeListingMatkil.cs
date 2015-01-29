using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLP_Data.Attributes;
using MLP_Data.Enums;

namespace MLP_Data.Entity
{
    class StockExchangeListingMatkil
    {
        public double Closing { get; set; }
        public DateTime Date { get; set; }
        public double Opening0 { get; set; }
        public double Closing0 { get; set; }
        public double Max0 { get; set; }
        public double Min0 { get; set; }
        public double Volume0 { get; set; }
        [Oscilator]
        public double OpenMA15 { get; set; }
        [Oscilator]
        public double CloseMA15 { get; set; }
        [Oscilator]
        public double MaxMA15 { get; set; }
        [Oscilator]
        public double MinMA15 { get; set; }
        [Oscilator]
        public double VolumeMA15 { get; set; }
        [Oscilator]
        public double OpenMA30 { get; set; }
        [Oscilator]
        public double CloseMA30 { get; set; }
        [Oscilator]
        public double MaxMA30 { get; set; }
        [Oscilator]
        public double MinMA30 { get; set; }
        [Oscilator]
        public double VolumeMA30 { get; set; }
        [Oscilator]
        public double OpenMA45 { get; set; }
        [Oscilator]
        public double CloseMA45 { get; set; }
        [Oscilator]
        public double MaxMA45 { get; set; }
        [Oscilator]
        public double MinMA45 { get; set; }
        [Oscilator]
        public double VolumeMA45 { get; set; }
       [Oscilator]
        public double Williams_R_10 { get; set; }
       [Oscilator]
       public double MACD { get; set; }
        [Oscilator]
        public double Rsi14 { get; set; }
        [Oscilator]
        public double StochasticOscilator14 { get; set; }
        [Oscilator]
        public double InflacjaCPIMM { get; set; }
        [Oscilator]
        public double USD { get; set; }
        [Oscilator]
        public double WiborOn { get; set; }
        [Oscilator]
        public double WibidOn { get; set; }
        [Oscilator]
        public double BrentOil { get; set; }
        [Oscilator]
        public double Zloto { get; set; }
        public IndexName IndexName { get; set; }
    }
}

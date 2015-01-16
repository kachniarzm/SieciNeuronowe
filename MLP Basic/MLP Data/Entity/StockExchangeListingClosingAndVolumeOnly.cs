using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLP_Data.Attributes;
using MLP_Data.Enums;

namespace MLP_Data.Entity
{
    class StockExchangeListingClosingAndVolumeOnly
    {
        public double Closing { get; set; }
        public double Volume { get; set; }
        public DateTime Date { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricBillCompute
{
    public class HouseReading
    {
        public string HouseName { get; set; }
        public double PreviousReading { get; set; }
        public double PresentReading { get; set; }
        public double Consumption => PresentReading - PreviousReading;
        public double Bill { get; set; }
    }
}
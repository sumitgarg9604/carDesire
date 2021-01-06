using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment1_v9.Models
{
    public class ViewCarsLocations
    {
        public class ViewModel
        {
            public Cars car { get; set; }
            public  PickDropLoc loc { get; set; }

            
        }

        public class ChartCar
        {
            public string ChartCarMake { get; set; }
            public double ChartCarMileage { get; set; }
        }
    }
}
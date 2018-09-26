using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerializationTests.TestClasses
{
    public class Starship
    {
        private decimal _cost;

        public Starship(decimal cost)
        {
            _cost = cost;
        }

        public decimal Cost { get { return _cost; } }

        public string Name { get; set; }
    }
}

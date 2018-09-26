using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerializationTests.TestClasses
{
    public class Movie
    {
        public int Id { get; set; }
        public int Version;
        public long Length { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal MoneyMade;
    }
}

namespace SerializationTests.TestClasses2
{
    public class Movie
    {
        public int Id { get; set; }
        public int? Version;
        public long Length { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal MoneyMade;
        public DateTime DvdDate { get; set; }
        public bool? SeenIt;
    }
}

using System;

namespace TPL4.Models
{
    public class Drink
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Type { get; set; }
        public Manufacturer Manufacturer { get; set; }
    }
}

using System;

namespace Mynt.Models
{
    public class AssetValue
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public decimal Value { get; set; }
        public DateTime RecordedAt { get; set; }

        public Asset Asset { get; set; }
    }
} 
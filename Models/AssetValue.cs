using System;

namespace Mynt.Models
{
    public class AssetValue
    {
        public int Id { get; set; }
        public required int AssetId { get; set; }
        public required decimal Value { get; set; }
        public required DateTime RecordedAt { get; set; }

        public required Asset Asset { get; set; }
    }
} 
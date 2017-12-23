using System;

namespace FLive.Infrastructure.Models
{
    public class LiveWorkout
    {
        public long Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string StreamId { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public long PriceTierId { get; set; }
		public long TrainerId { get; set; }
    }
}
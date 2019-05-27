using System;

namespace TradeBot.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string ContentText { get; set; }
        public string AudiId { get; set; }
        public string AnimationId { get; set; }
        public string DocumentId { get; set; }
        public string StickerId { get; set; }
        public string VideoId { get; set; }
        public string[] PhotoIds { get; set; }
        public DateTime Time { get; set; }
        
        public int UserId { get; set; }
    }
}

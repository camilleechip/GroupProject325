
using System.Dynamic;

namespace TTL.Models
{
    public class MinorItem
    {
        //Decide if we want to have a campaign ID
        //public string CampaignId { get; set; } 
        public int Id { get; set; }
        public string CharacterId { get; set; }
        public ResourceType ResourceType { get; set; }
        public int Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string? Description { get; set; }
        public DateTime DateRecorded { get; set; } = DateTime.UtcNow;
    }

    public enum TransactionType
    {
        Add,
        Subtract
    }

    public enum ResourceType
    {
        Gold,
        Silver,
        Copper,
        Gems,
        XP
    }
}
namespace GroupProject325.Models;

/// <summary>
/// Join table between User and Campaign. Holds the user's role in that campaign.
/// </summary>
public class CampaignMember
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public int CampaignId { get; set; }
    public Campaign? Campaign { get; set; }

    public CampaignRole Role { get; set; } = CampaignRole.Member;
}

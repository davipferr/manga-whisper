namespace MangaWhisper.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool NotificationEnabled { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<MangaSubscription>? Subscriptions { get; set; }
}

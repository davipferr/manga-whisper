namespace MangaWhisper.Domain.Entities;

public class MangaSource
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string CheckerClassName { get; set; } = string.Empty;

    public virtual ICollection<MangaSubscription> Subscriptions { get; set; } = new List<MangaSubscription>();
}

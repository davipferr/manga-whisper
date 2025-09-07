namespace MangaWhisper.Domain.Entities;

public class Chapter
{
    public int Id { get; set; }
    public int MangaId { get; set; }
    public float Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public virtual Manga Manga { get; set; } = null!;
}

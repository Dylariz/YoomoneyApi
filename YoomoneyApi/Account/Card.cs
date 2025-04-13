namespace YoomoneyApi.Account;

public class Card
{
    /// <summary>
    /// Часть номера карты
    /// </summary>
    public string? PanFragment { get; set; }
    
    /// <summary>
    /// Тип карты
    /// </summary>
    public string? Type { get; set; }

    public Card(string? panFragment = null, string? type = null)
    {
        PanFragment = panFragment;
        Type = type;
    }
}
namespace GroupProject325.Models;

public class ItemPropertyValue
{
    public Guid Id { get; set; }

    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;

    // Restricted in LedgerContext: a CustomProperty that items still use can't be deleted.
    public Guid CustomPropertyId { get; set; }
    public CustomProperty CustomProperty { get; set; } = null!;

    public string? Value { get; set; }

    // Restricted in LedgerContext: a PropertyOption that items still use can't be deleted.
    public Guid? PropertyOptionId { get; set; }
    public PropertyOption? PropertyOption { get; set; }
}
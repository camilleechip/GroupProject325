using System.ComponentModel.DataAnnotations;

namespace GroupProject325.Models;

public class PropertyOption
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Value { get; set; } = string.Empty;

    public Guid CustomPropertyId { get; set; }
    public CustomProperty CustomProperty { get; set; } = null!;
}
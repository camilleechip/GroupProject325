using System.ComponentModel.DataAnnotations;

namespace GroupProject325.Models;

public class CustomProperty
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Description { get; set; }

    public PropertyValueType ValueType { get; set; } = PropertyValueType.Text;

    public bool AllowMultiple { get; set; } = false;

    public ICollection<PropertyOption> Options { get; set; } = new List<PropertyOption>();
}
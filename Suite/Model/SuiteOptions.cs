using System.ComponentModel.DataAnnotations;

namespace Model;

public sealed class SuiteOptions
{
    public const string SectionName = "Suite";

    [Required]
    public required Uri BaseUri { get; set; }
}

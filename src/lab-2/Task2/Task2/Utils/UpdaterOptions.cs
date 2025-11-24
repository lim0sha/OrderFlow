using System.ComponentModel.DataAnnotations;

namespace Task2.Utils;

public class UpdaterOptions
{
    [Required]
    public int RefreshIntervalSeconds { get; init; }
}

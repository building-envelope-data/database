using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class Numeration
{
    public Numeration(
        string? prefix,
        string mainNumber,
        string? suffix
    )
    {
        Prefix = prefix;
        MainNumber = mainNumber;
        Suffix = suffix;
    }

    [MinLength(1)] public string? Prefix { get; private set; }

    [Required] [MinLength(1)] public string MainNumber { get; private set; }

    [MinLength(1)] public string? Suffix { get; private set; }
}
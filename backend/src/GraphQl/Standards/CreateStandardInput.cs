using System;
using Database.GraphQl.Numerations;

namespace Database.GraphQl.Standards
{
    public sealed record CreateStandardInput(
          string? Title,
          string? Abstract,
          string? Section,
          int? Year,
          CreateNumerationInput Numeration,
          Enumerations.Standardizer[] Standardizers,
          Uri? Locator
        );
}
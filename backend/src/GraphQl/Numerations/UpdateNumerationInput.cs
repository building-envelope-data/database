namespace Database.GraphQl.Numerations
{
    public sealed record UpdateNumerationInput(
            string? Prefix,
            string MainNumber,
            string? Suffix
    );
}
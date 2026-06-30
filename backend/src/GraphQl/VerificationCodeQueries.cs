using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl;

[ExtendObjectType(nameof(Query))]
public sealed class VerificationCodeQueries
{
    public string GetVerificationCode(
        AppSettings appSettings
    )
    {
        return appSettings.VerificationCode;
    }
}
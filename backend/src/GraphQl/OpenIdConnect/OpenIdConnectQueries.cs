using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.ApiRequests;
using HotChocolate.Authorization;

namespace Database.GraphQl.OpenIdConnect;

[ExtendObjectType(nameof(Query))]
public sealed class OpenIdConnectQueries
{
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public Task<string?> GetCurrentOpenIdConnectTokenClientIdAsync(
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return authorization.SwitchUserOrApplicationAsync(
            (_, clientId) => Task.FromResult(clientId),
            (_, clientId) => Task.FromResult(clientId),
            cancellationToken
        );
    }

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public Task<QueryCurrentUserOrApplication.CurrentOpenIdConnectApplication?> GetCurrentOpenIdConnectApplicationAsync(
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return authorization.SwitchUserOrApplicationAsync(
            (user, _) => Task.FromResult<QueryCurrentUserOrApplication.CurrentOpenIdConnectApplication?>(null),
            (application, _) => Task.FromResult<QueryCurrentUserOrApplication.CurrentOpenIdConnectApplication?>(application),
            cancellationToken
        );
    }
}
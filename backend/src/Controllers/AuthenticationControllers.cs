using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Client.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Database.Controllers;

// Inspired by https://github.com/openiddict/openiddict-samples/blob/dev/samples/Velusia/Velusia.Client/Controllers/AuthenticationController.cs
// https://github.com/openiddict/openiddict-samples/blob/855c31f91d6bf5cde735ef3f96fcc3c015b51d79/samples/Velusia/Velusia.Client/Controllers/AuthenticationController.cs
public class AuthenticationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly string _issuer;

    public AuthenticationController(
        AppSettings appSettings,
        ApplicationDbContext context
    )
    {
        _issuer = appSettings.MetabaseHost;
        _context = context;
    }

    [HttpGet("~/connect/login")]
    public ActionResult LogIn(string? returnUrl)
    {
        // Ask the OpenIddict client middleware to redirect the user agent to the identity provider.
        return Challenge(
            new AuthenticationProperties(
                new Dictionary<string, string?>
                {
                    // Note: when only one client is registered in the client options,
                    // setting the issuer property is not required and can be omitted.
                    [OpenIddictClientAspNetCoreConstants.Properties.Issuer] = _issuer
                }
            )
            {
                // Only allow local return URLs to prevent open redirect attacks.
                RedirectUri = SanitizeReturnUrl(returnUrl)
            },
            OpenIddictClientAspNetCoreDefaults.AuthenticationScheme
        );
    }

    [HttpPost("~/connect/logout")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> LogOut(string? returnUrl)
    {
        // Retrieve the identity stored in the local authentication cookie. If it's not available,
        // this indicate that the user is already logged out locally (or has not logged in yet).
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (result is not { Succeeded: true })
            // Only allow local return URLs to prevent open redirect attacks.
            // https://learn.microsoft.com/en-us/aspnet/core/security/preventing-open-redirects
            return LocalRedirect(SanitizeReturnUrl(returnUrl));
        // Remove the local authentication cookie before triggering a redirection to the remote server.
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        // Ask the OpenIddict client middleware to redirect the user agent to the identity provider.
        return SignOut(
            new AuthenticationProperties(
                new Dictionary<string, string?>
                {
                    // Note: when only one client is registered in the client options,
                    // setting the issuer property is not required and can be omitted.
                    [OpenIddictClientAspNetCoreConstants.Properties.Issuer] = _issuer,
                    // While not required, the specification encourages sending an id_token_hint
                    // parameter containing an identity token returned by the server for this user.
                    [OpenIddictClientAspNetCoreConstants.Properties.IdentityTokenHint] =
                        result.Properties.GetTokenValue(OpenIddictClientAspNetCoreConstants.Tokens
                            .BackchannelIdentityToken)
                }
            )
            {
                // Only allow local return URLs to prevent open redirect attacks.
                RedirectUri = SanitizeReturnUrl(returnUrl)
            },
            OpenIddictClientAspNetCoreDefaults.AuthenticationScheme
        );
    }

    // Note: this controller uses the same callback action for all providers
    // but for users who prefer using a different action per provider,
    // the following action can be split into separate actions.
    [HttpGet("~/connect/callback/login/{provider}")]
    [HttpPost("~/connect/callback/login/{provider}")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> LogInCallback(string provider, CancellationToken cancellationToken)
    {
        // Retrieve the authorization data validated by OpenIddict as part of the callback handling.
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        // Multiple strategies exist to handle OAuth 2.0/OpenID Connect callbacks, each with their pros and cons:
        //
        //   * Directly using the tokens to perform the necessary action(s) on behalf of the user, which is suitable
        //     for applications that don't need a long-term access to the user's resources or don't want to store
        //     access/refresh tokens in a database or in an authentication cookie (which has security implications).
        //     It is also suitable for applications that don't need to authenticate users but only need to perform
        //     action(s) on their behalf by making API calls using the access token returned by the remote server.
        //
        //   * Storing the external claims/tokens in a database (and optionally keeping the essential claims in an
        //     authentication cookie so that cookie size limits are not hit). For the applications that use ASP.NET
        //     Core Identity, the UserManager.SetAuthenticationTokenAsync() API can be used to store external tokens.
        //
        //     Note: in this case, it's recommended to use column encryption to protect the tokens in the database.
        //
        //   * Storing the external claims/tokens in an authentication cookie, which doesn't require having
        //     a user database but may be affected by the cookie size limits enforced by most browser vendors
        //     (e.g Safari for macOS and Safari for iOS/iPadOS enforce a per-domain 4KB limit for all cookies).
        //
        //     Note: this is the approach used here, but the external claims are first filtered to only persist
        //     a few claims like the user identifier. The same approach is used to store the access/refresh tokens.
        //
        // Important: if the remote server doesn't support OpenID Connect and doesn't expose a userinfo endpoint,
        // result.Principal.Identity will represent an unauthenticated identity and won't contain any claim.
        //
        // Such identities cannot be used as-is to build an authentication cookie in ASP.NET Core (as the
        // antiforgery stack requires at least a name claim to bind CSRF cookies to the user's identity) but
        // the access/refresh tokens can be retrieved using result.Properties.GetTokens() to make API calls.
        if (result.Principal?.Identity is not ClaimsIdentity { IsAuthenticated: true })
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        // Build an identity based on the external claims and that will be used to create the authentication cookie.
        //
        // By default, all claims extracted during the authorization dance are available. The claims collection stored
        // in the cookie can be filtered out or mapped to different names depending on the claim name or its issuer.
        //
        // The claims are fetched from the userinfo endpoint of the
        // authorization provider.
        var claims = new List<Claim>(
            result.Principal.Claims
                .Select(claim => claim switch
                {
                    // https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
                    // Map the standard "sub" and custom "id" claims to ClaimTypes.NameIdentifier, which is
                    // the default claim type used by .NET and is required by the antiforgery components.
                    { Type: Claims.Subject }
                        => new Claim(ClaimTypes.NameIdentifier, claim.Value, claim.ValueType, claim.Issuer),
                    // Map the standard "name" claim to ClaimTypes.Name.
                    { Type: Claims.Name }
                        => new Claim(ClaimTypes.Name, claim.Value, claim.ValueType, claim.Issuer),
                    _ => claim
                })
                .Where(claim => claim switch
                {
                    // Preserve the basic claims that are necessary for the application to work correctly.
                    { Type: ClaimTypes.NameIdentifier or ClaimTypes.Name } => true,
                    // Don't preserve the other claims.
                    _ => false
                }));
        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimTypes.Name,
            ClaimTypes.Role
        );
        AuthenticationProperties properties;
        if (result.Properties is null)
        {
            properties = new AuthenticationProperties();
        }
        else
        {
            // Build the authentication properties based on the properties that were added when the challenge was triggered.
            properties = new AuthenticationProperties(result.Properties.Items);
            // If needed, the tokens returned by the authorization server can be stored in the authentication cookie.
            // To make cookies less heavy, tokens that are not used are filtered out before creating the cookie.
            properties.StoreTokens(result.Properties.GetTokens().Where(token => token switch
            {
                // Preserve the access, identity and refresh tokens returned in the token response, if available.
                {
                    Name: OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken or
                    OpenIddictClientAspNetCoreConstants.Tokens.BackchannelIdentityToken or
                    OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken
                } => true,
                // Ignore the other tokens.
                _ => false
            }));
        }

        // Add the user to the database if he/she does not exist.
        var subject =
            identity.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException(
                $"Impossible! The claim {ClaimTypes.NameIdentifier}, which is the subject of the token, is missing for the identity with name {identity.Name}.");
        var name =
            identity.FindFirst(c => c.Type == ClaimTypes.Name)?.Value
            ?? throw new InvalidOperationException(
                $"Impossible! The claim {ClaimTypes.Name} is missing for the identity with subject {subject}.");
        var user = await
            _context.Users.AsQueryable()
                .SingleOrDefaultAsync(
                    u => u.Subject == subject,
                    cancellationToken
                );
        if (user is null)
            _context.Users.Add(
                new User(
                    subject,
                    name
                )
            );
        else
            user.Update(name);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        // Ask the cookie authentication handler to return a new cookie and redirect
        // the user agent to the return URL stored in the authentication properties.
        return SignIn(
            new ClaimsPrincipal(identity),
            properties,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
    }

    // Note: this controller uses the same callback action for all providers
    // but for users who prefer using a different action per provider,
    // the following action can be split into separate actions.
    [HttpGet("~/connect/callback/logout/{provider}")]
    [HttpPost("~/connect/callback/logout/{provider}")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> LogOutCallback(string provider)
    {
        // Retrieve the data stored by OpenIddict in the state token created when the logout was triggered.
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);
        // In this sample, the local authentication cookie is always removed before the user agent is redirected
        // to the authorization server. Applications that prefer delaying the removal of the local cookie can
        // remove the corresponding code from the logout action and remove the authentication cookie in this action.
        //
        // Only allow local return URLs to prevent open redirect attacks.
        // https://learn.microsoft.com/en-us/aspnet/core/security/preventing-open-redirects
        return LocalRedirect(
            SanitizeReturnUrl(result.Properties?.RedirectUri)
        );
    }

    private string SanitizeReturnUrl(string? returnUrl)
    {
        return
            returnUrl is not null && Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : "/";
    }
}
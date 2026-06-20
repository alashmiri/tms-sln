using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

public class TmsAssessmentAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    // The ISystemClock parameter is removed completely from the constructor signatures
    public TmsAssessmentAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return Task.FromResult(AuthenticateResult.Fail("No user"));
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniCommerce_BackEnd_Luftborn.Authentication;

namespace MiniCommerce_BackEnd_Luftborn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private const string GoogleOpenIdConfigurationUrl = "https://accounts.google.com/.well-known/openid-configuration";

    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("openid-configuration")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOpenIdConfiguration(CancellationToken cancellationToken = default)
    {
        var sso = _configuration.GetSection(SsoConfiguration.SectionName).Get<SsoConfiguration>() ?? new SsoConfiguration();
        if (!sso.Enabled || string.IsNullOrEmpty(sso.Authority))
            return NotFound();

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(GoogleOpenIdConfigurationUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return Content(json, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(502, new { error = "Failed to fetch OpenID configuration", detail = ex.Message });
        }
    }

    [HttpGet("config")]
    [AllowAnonymous]
    public IActionResult GetSsoConfig()
    {
        var sso = _configuration.GetSection(SsoConfiguration.SectionName).Get<SsoConfiguration>() ?? new SsoConfiguration();
        return Ok(new
        {
            enabled = sso.Enabled,
            authority = sso.Enabled ? sso.Authority : null,
            clientId = sso.Enabled ? sso.ClientId : null,
            clientSecret = sso.Enabled ? sso.ClientSecret : null,
            audience = sso.Enabled ? sso.Audience : null
        });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        if (User.Identity?.IsAuthenticated != true)
            return Unauthorized();

        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(new
        {
            isAuthenticated = true,
            name = User.Identity.Name,
            authenticationType = User.Identity.AuthenticationType,
            claims
        });
    }
}

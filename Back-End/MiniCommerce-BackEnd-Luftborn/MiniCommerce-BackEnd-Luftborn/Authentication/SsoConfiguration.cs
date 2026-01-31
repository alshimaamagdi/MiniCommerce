namespace MiniCommerce_BackEnd_Luftborn.Authentication;

public class SsoConfiguration
{
    public const string SectionName = "SSO";

    public bool Enabled { get; set; }
    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string MetadataAddress { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; set; } = true;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

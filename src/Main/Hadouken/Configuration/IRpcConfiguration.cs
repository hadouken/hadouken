namespace Hadouken.Configuration
{
    public interface IRpcConfiguration
    {
        string GatewayUri { get; set; }

        string PluginUriTemplate { get; set; }
    }
}

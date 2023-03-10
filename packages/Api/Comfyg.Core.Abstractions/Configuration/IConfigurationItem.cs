namespace Comfyg.Core.Abstractions.Configuration;

public interface IConfigurationItem
{
    string Key { get; set; }

    string JsonValue { get; set; }

    string Label { get; set; }
}
using Comfyg.Contracts;

namespace Comfyg.Core;

internal interface ISerializableComfygValue : IComfygValue
{
    new string Key { get; set; }

    new string Value { get; set; }

    new string Version { get; set; }

    new DateTime CreatedAt { get; set; }
}
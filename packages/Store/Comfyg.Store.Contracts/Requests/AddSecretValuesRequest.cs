﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Store.Contracts.Secrets;

namespace Comfyg.Store.Contracts.Requests;

public class AddSecretValuesRequest : AddValuesRequest<ISecretValue>
{
    [Required]
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISecretValue>, IEnumerable<SecretValue>, IEnumerable<IComfygValue>>))]
    public override IEnumerable<ISecretValue> Values { get; init; } = null!;
}

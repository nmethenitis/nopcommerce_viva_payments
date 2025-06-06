﻿using System.Text.Json.Serialization;

namespace Nop.Plugin.Payments.VivaPayments.Models;
public class VivaIdentityResponse {
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
}

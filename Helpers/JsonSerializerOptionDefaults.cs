using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper.Execution;

namespace Nop.Plugin.Payments.VivaPayments.Helpers;
public static class JsonSerializerOptionDefaults {
    public static JsonSerializerOptions GetDefaultSettings() {
        var options = new JsonSerializerOptions {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}

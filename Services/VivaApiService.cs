using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nop.Plugin.Payments.VivaPayments.Helpers;
using Nop.Plugin.Payments.VivaPayments.Models;

namespace Nop.Plugin.Payments.VivaPayments.Services;
public class VivaApiService {

    private readonly VivaPaymentsSettings _vivaPaymentSettings;
    private readonly ILogger<VivaApiService> _logger;

    public VivaApiService(VivaPaymentsSettings vivaPaymentsSettings, ILogger<VivaApiService> logger) {
        _vivaPaymentSettings = vivaPaymentsSettings ?? throw new ArgumentNullException(nameof(vivaPaymentsSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<VivaPaymentOrderResponse> CreatePaymentOrderAsync(VivaPaymentOrderRequest request) {
        try {
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(GetApiBaseUrl());
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetTokenAsync()}");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var payload = JsonSerializer.Serialize(request);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var httpResponseMessage = await client.PostAsync(VivaPaymentsDefaults.OrderPath, content);
                if (httpResponseMessage.IsSuccessStatusCode) {
                    var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                    var response = JsonSerializer.Deserialize<VivaPaymentOrderResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
                    return response;
                } else {
                    throw new Exception($"Error: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}");
                }
            }
        } catch (Exception ex) {
            _logger.LogError($"Error creating payment order: {ex.Message}");
            throw new Exception($"Error creating payment order: {ex.Message}");
        }
    }

    public async Task<VivaTransactionResponse> GetTransactionDetailsAsync(string transactionId) {
        try {
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(GetApiBaseUrl());
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetTokenAsync()}");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var httpResponseMessage = await client.GetAsync(String.Format(VivaPaymentsDefaults.TransactionPath, transactionId));
                if (httpResponseMessage.IsSuccessStatusCode) {
                    var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                    var response = JsonSerializer.Deserialize<VivaTransactionResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
                    return response;
                } else {
                    throw new Exception($"Error: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}");
                }
            }
        } catch (Exception ex) {
            _logger.LogError($"Error fetching transaction details: {ex.Message}");
            throw new Exception($"Error fetching transaction details: {ex.Message}");
        }
    }

    public async Task<VivaTransactionCancelResponse> CancelTransaction(VivaTransactionCancelRequest vivaTransactionCancelRequest) {
        try {
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri(GetOldApiBaseUrl());
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {GetBasicAuth()}");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var httpResponseMessage = await client.DeleteAsync(String.Format(VivaPaymentsDefaults.CancelTransactionPath, vivaTransactionCancelRequest.TransactionId, vivaTransactionCancelRequest.Amount, vivaTransactionCancelRequest.SourceCode));
                if (httpResponseMessage.IsSuccessStatusCode) {
                    var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                    var response = JsonSerializer.Deserialize<VivaTransactionCancelResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
                    return response;
                } else {
                    throw new Exception($"Error: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}");
                }
            }
        } catch (Exception ex) {
            _logger.LogError($"Error canceling transaction: {ex.Message}");
            throw new Exception($"Error canceling transaction: {ex.Message}");
        }
    }

    public async Task<string> GetTokenAsync() {
        try {
            using (var client = new HttpClient()) {
                var base64Encoded = EncodeToBase64($"{_vivaPaymentSettings.ClientId}:{_vivaPaymentSettings.ClientSecret}");
                client.BaseAddress = new Uri(GetAccountBaseUrl());
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Encoded}");
                var requestContent = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var httpResponseMessage = await client.PostAsync(VivaPaymentsDefaults.AuthPath, requestContent);
                if (httpResponseMessage.IsSuccessStatusCode) {
                    var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                    var response = JsonSerializer.Deserialize<VivaIdentityResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
                    return response.AccessToken;
                } else {
                    throw new Exception($"Error: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}");
                }
            }
        } catch (Exception ex) {
            _logger.LogError($"Error fetching token: {ex.Message}");
            throw new Exception($"Error fetching token: {ex.Message}");
        }
    }

    public string GetBasicAuth() {
        return EncodeToBase64($"{_vivaPaymentSettings.MerchantId}:{_vivaPaymentSettings.ApiKey}");
    }

    private string EncodeToBase64(string input) {
        if (string.IsNullOrEmpty(input)) {
            throw new ArgumentException("Input cannot be null or empty", nameof(input));
        }
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        string base64String = Convert.ToBase64String(bytes);
        return base64String;
    }

    private string GetApiBaseUrl() {
        return _vivaPaymentSettings.IsSandbox ? VivaPaymentsDefaults.ApiUrl.Sandbox : VivaPaymentsDefaults.ApiUrl.Live;
    }

    private string GetOldApiBaseUrl() {
        return _vivaPaymentSettings.IsSandbox ? VivaPaymentsDefaults.OldApiUrl.Sandbox : VivaPaymentsDefaults.OldApiUrl.Live;
    }

    private string GetAccountBaseUrl() {
        return _vivaPaymentSettings.IsSandbox ? VivaPaymentsDefaults.AccountUrl.Sandbox : VivaPaymentsDefaults.AccountUrl.Live;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core.Domain.Directory;
using Nop.Core;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Services.Logging;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net.Http;
using Nop.Plugin.Payments.VivaPayments.Models;
using System.Text.Json;
using Nop.Plugin.Payments.VivaPayments.Helpers;
using System.Net.Mime;

namespace Nop.Plugin.Payments.VivaPayments.Services;
public class VivaApiService{

    private readonly VivaPaymentsSettings _vivaPaymentSettings;


    public VivaApiService(VivaPaymentsSettings vivaPaymentsSettings){
        _vivaPaymentSettings = vivaPaymentsSettings;
    }

    public async Task<VivaPaymentOrderResponse> CreatePaymentOrderAsync(VivaPaymentOrderRequest request) {
        using (var client = new HttpClient()) {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GetTokenAsync()}");
            client.DefaultRequestHeaders.Add("Content-Type", MediaTypeNames.Application.Json);
            var payload = JsonSerializer.Serialize(request);
            var content = new StringContent(payload, Encoding.UTF8, MediaTypeNames.Application.Json);
            var url = _vivaPaymentSettings.IsSandbox ? VivaPaymentsDefaults.ApiUrl.Sandbox : VivaPaymentsDefaults.ApiUrl.Live;
            var httpResponseMessage = await client.PostAsync($"{url}{VivaPaymentsDefaults.OrderPath}", content);
            if (httpResponseMessage.IsSuccessStatusCode) {
                var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<VivaPaymentOrderResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
                return response;
            } else {
                throw new Exception($"Error: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}");
            }
        }
    }

    public async Task<VivaTransactionResponse> GetTransactionDetailsAsync(string transactionId) {
        using (var client = new HttpClient()) {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GetTokenAsync()}");
            client.DefaultRequestHeaders.Add("Content-Type", MediaTypeNames.Application.Json);
            var content = new StringContent(string.Empty, Encoding.UTF8, MediaTypeNames.Application.Json);
            var url = _vivaPaymentSettings.IsSandbox ? VivaPaymentsDefaults.ApiUrl.Sandbox : VivaPaymentsDefaults.ApiUrl.Live;
            var httpResponseMessage = await client.PostAsync($"{url}{VivaPaymentsDefaults.TransactionPath}", content);
            if (httpResponseMessage.IsSuccessStatusCode) {
                var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<VivaTransactionResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
                return response;
            } else {
                throw new Exception($"Error: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}");
            }
        }
    }

    public async Task<string> GetTokenAsync(){
        using (var client = new HttpClient()){
            var base64Encoded = EncodeToBase64($"{_vivaPaymentSettings.ClientId}:{_vivaPaymentSettings.ClientSecret}");
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Encoded}");
            var requestContent = new StringContent("grant_type=client_credentials",Encoding.UTF8, "application/x-www-form-urlencoded");
            var url = _vivaPaymentSettings.IsSandbox ? VivaPaymentsDefaults.AccountUrl.Sandbox : VivaPaymentsDefaults.AccountUrl.Live;
            var httpResponseMessage = await client.PostAsync($"{url}{VivaPaymentsDefaults.AuthPath}", requestContent);
            if (httpResponseMessage.IsSuccessStatusCode) {
                var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<VivaIdentityResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
                return response.AccessToken;
            }else{
                throw new Exception($"Error: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}");
            }
        }
    }

    private string EncodeToBase64(string input){
        if (string.IsNullOrEmpty(input)){
            throw new ArgumentException("Input cannot be null or empty", nameof(input));
        }
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        string base64String = Convert.ToBase64String(bytes);
        return base64String;
    }
}

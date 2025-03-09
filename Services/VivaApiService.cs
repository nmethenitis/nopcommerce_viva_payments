﻿using System;
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
using System.Net.Http.Headers;
using System.Transactions;

namespace Nop.Plugin.Payments.VivaPayments.Services;
public class VivaApiService{

    private readonly VivaPaymentsSettings _vivaPaymentSettings;


    public VivaApiService(VivaPaymentsSettings vivaPaymentsSettings){
        _vivaPaymentSettings = vivaPaymentsSettings;
    }

    public async Task<VivaPaymentOrderResponse> CreatePaymentOrderAsync(VivaPaymentOrderRequest request) {
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
    }

    public async Task<VivaTransactionResponse> GetTransactionDetailsAsync(string transactionId) {
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
    }

    public async Task<VivaTransactionCancelResponse> CancelTransaction(VivaTransactionCancelRequest vivaTransactionCancelRequest) {
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
    }

    public async Task<string> GetTokenAsync(){
        using (var client = new HttpClient()){
            var base64Encoded = EncodeToBase64($"{_vivaPaymentSettings.ClientId}:{_vivaPaymentSettings.ClientSecret}");
            client.BaseAddress = new Uri(GetAccountBaseUrl());
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Encoded}");
            var requestContent = new StringContent("grant_type=client_credentials",Encoding.UTF8, "application/x-www-form-urlencoded");
            var httpResponseMessage = await client.PostAsync(VivaPaymentsDefaults.AuthPath, requestContent);
            if (httpResponseMessage.IsSuccessStatusCode) {
                var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                var response = JsonSerializer.Deserialize<VivaIdentityResponse>(httpResponseContent, JsonSerializerOptionDefaults.GetDefaultSettings());
                return response.AccessToken;
            }else{
                throw new Exception($"Error: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}");
            }
        }
    }

    public string GetBasicAuth() {
        return EncodeToBase64($"{_vivaPaymentSettings.MerchantId}:{_vivaPaymentSettings.ApiKey}");
    }

    private string EncodeToBase64(string input){
        if (string.IsNullOrEmpty(input)){
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

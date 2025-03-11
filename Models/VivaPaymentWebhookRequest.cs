using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.VivaPayments.Models;
public class VivaPaymentWebhookRequest {
    public string Url { get; set; }
    public EventData? EventData { get; set; }
    public DateTime? Created { get; set; }
    public string CorrelationId { get; set; }
    public int? EventTypeId { get; set; }
    public object? Delay { get; set; }
    public string MessageId { get; set; }
    public string RecipientId { get; set; }
    public int? MessageTypeId { get; set; }
}

public class EventData {
    public bool? Moto { get; set; }
    public int? BinId { get; set; }
    public bool? IsDcc { get; set; }
    public string Ucaf { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string BankId { get; set; }
    public bool? Systemic { get; set; }
    public bool? Switching { get; set; }
    public string ParentId { get; set; }
    public decimal? Amount { get; set; }
    public string ChannelId { get; set; }
    public int? TerminalId { get; set; }
    public string MerchantId { get; set; }
    public long? OrderCode { get; set; }
    public object? ProductId { get; set; }
    public string StatusId { get; set; }
    public string FullName { get; set; }
    public object? ResellerId { get; set; }
    public bool? DualMessage { get; set; }
    public DateTime? InsDate { get; set; }
    public decimal? TotalFee { get; set; }
    public string CardToken { get; set; }
    public string CardNumber { get; set; }
    public string Descriptor { get; set; }
    public decimal? TipAmount { get; set; }
    public string SourceCode { get; set; }
    public string SourceName { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string CompanyName { get; set; }
    public string TransactionId { get; set; }
    public string CompanyTitle { get; set; }
    public string PanEntryMode { get; set; }
    public int? ReferenceNumber { get; set; }
    public string ResponseCode { get; set; }
    public string CurrencyCode { get; set; }
    public string OrderCulture { get; set; }
    public string MerchantTrns { get; set; }
    public string CustomerTrns { get; set; }
    public bool? IsManualRefund { get; set; }
    public string TargetPersonId { get; set; }
    public string TargetWalletId { get; set; }
    public bool? AcquirerApproved { get; set; }
    public bool? LoyaltyTriggered { get; set; }
    public int? TransactionTypeId { get; set; }
    public string AuthorizationId { get; set; }
    public int? TotalInstallments { get; set; }
    public string CardCountryCode { get; set; }
    public string CardIssuingBank { get; set; }
    public decimal? RedeemedAmount { get; set; }
    public object? ClearanceDate { get; set; }
    public decimal? ConversionRate { get; set; }
    public int? CurrentInstallment { get; set; }
    public decimal? OriginalAmount { get; set; }
    public List<string>? Tags { get; set; }
    public object? BillId { get; set; }
    public object? ConnectedAccountId { get; set; }
    public object? ResellerSourceCode { get; set; }
    public object? ResellerSourceName { get; set; }
    public int? MerchantCategoryCode { get; set; }
    public object? ResellerCompanyName { get; set; }
    public string CardUniqueReference { get; set; }
    public string OriginalCurrencyCode { get; set; }
    public object? ExternalTransactionId { get; set; }
    public object? ResellerSourceAddress { get; set; }
    public DateTime? CardExpirationDate { get; set; }
    public object? ServiceId { get; set; }
    public string RetrievalReferenceNumber { get; set; }
    public List<object>? AssignedMerchantUsers { get; set; }
    public List<object>? AssignedResellerUsers { get; set; }
    public int? CardTypeId { get; set; }
    public object? ResponseEventId { get; set; }
    public string ElectronicCommerceIndicator { get; set; }
    public int? OrderServiceId { get; set; }
    public object? ApplicationIdentifierTerminal { get; set; }
    public object? IntegrationId { get; set; }
    public int? CardProductCategoryId { get; set; }
    public int? CardProductAccountTypeId { get; set; }
    public int? DigitalWalletId { get; set; }
    public object? DccSessionId { get; set; }
    public object? DccMarkup { get; set; }
    public object? DccDifferenceOverEcb { get; set; }
}
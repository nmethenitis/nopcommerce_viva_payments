namespace Nop.Plugin.Payments.VivaPayments.Models;
public class VivaTransactionCancelResponse {
    public string Emv { get; set; }
    public decimal? Amount { get; set; }
    public decimal? OriginalAmount { get; set; }
    public string StatusId { get; set; }
    public string CurrencyCode { get; set; }
    public Guid? TransactionId { get; set; }
    public int? ReferenceNumber { get; set; }
    public string AuthorizationId { get; set; }
    public string RetrievalReferenceNumber { get; set; }
    public int? ThreeDSecureStatusId { get; set; }
    public int? ErrorCode { get; set; }
    public string ErrorText { get; set; }
    public DateTime? TimeStamp { get; set; }
    public string CorrelationId { get; set; }
    public int? EventId { get; set; }
    public bool Success { get; set; }
}

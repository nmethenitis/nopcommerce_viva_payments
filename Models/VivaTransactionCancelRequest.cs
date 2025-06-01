namespace Nop.Plugin.Payments.VivaPayments.Models;
public class VivaTransactionCancelRequest {
    public string TransactionId { get; set; }
    public int Amount { get; set; }
    public string SourceCode { get; set; }
}

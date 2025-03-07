using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.VivaPayments.Models;
public class VivaTransactionResponse {
    public string Email { get; set; }
    public string BankId { get; set; }
    public double Amount { get; set; }
    public double ConversionRate { get; set; }
    public double OriginalAmount { get; set; }
    public string OriginalCurrencyCode { get; set; }
    public string SourceCode { get; set; }
    public bool Switching { get; set; }
    public long OrderCode { get; set; }
    public string StatusId { get; set; }
    public string FullName { get; set; }
    public DateTime InsDate { get; set; }
    public string CardNumber { get; set; }
    public string CurrencyCode { get; set; }
    public string CustomerTrns { get; set; }
    public string MerchantTrns { get; set; }
    public int TransactionTypeId { get; set; }
    public bool RecurringSupport { get; set; }
    public int TotalInstallments { get; set; }
    public string CardCountryCode { get; set; }
    public string CardUniqueReference { get; set; }
    public object CardIssuingBank { get; set; } // Using object since the value can be null
    public object EventId { get; set; } // Using object since the value can be null
    public int CurrentInstallment { get; set; }
    public int CardTypeId { get; set; }
    public DateTime CardExpirationDate { get; set; }
    public int DigitalWalletId { get; set; }
    public List<LoyaltyTransaction> LoyaltyTransactions { get; set; }
}

public class LoyaltyTransaction {
    public int PointsEarned { get; set; }
    public int ReferenceNumber { get; set; }
    public int PointsRedeemed { get; set; }
    public double? AmountEarned { get; set; } // Nullable type since it can be null
    public double AmountRedeemed { get; set; }
    public int ProgramId { get; set; }
    public string ExternalReferenceNumber { get; set; }
    public string RetrievalReferenceNumber { get; set; }
    public int LoyaltyTransactionTypeId { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.VivaPayments.Models;
public class VivaPaymentOrderRequest {
    public int Amount { get; set; }
    public string CustomerTrns { get; set; }
    public Customer Customer { get; set; }
    public string DynamicDescriptor { get; set; }
    public int CurrencyCode { get; set; }
    public int PaymentTimeout { get; set; }
    public bool Preauth { get; set; }
    public bool AllowRecurring { get; set; }
    public int MaxInstallments { get; set; }
    public bool ForceMaxInstallments { get; set; }
    public bool PaymentNotification { get; set; }
    public int TipAmount { get; set; }
    public bool DisableExactAmount { get; set; }
    public bool DisableCash { get; set; }
    public bool DisableWallet { get; set; }
    public string SourceCode { get; set; }
    public string MerchantTrns { get; set; }
    public List<string> Tags { get; set; }
    public PaymentMethodFees PaymentMethodFees { get; set; }
    public List<string> CardTokens { get; set; }
    public bool IsCardVerification { get; set; }
    public NbgLoanOrderOptions NbgLoanOrderOptions { get; set; }
    public KlarnaOrderOptions KlarnaOrderOptions { get; set; }
}

public class Customer {
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string CountryCode { get; set; }
    public string RequestLang { get; set; }
}

public class PaymentMethodFees {
    public string PaymentMethodId { get; set; }
    public int Fee { get; set; }
}

public class NbgLoanOrderOptions {
    public string Code { get; set; }
    public int ReceiptType { get; set; }
}

public class KlarnaOrderOptions {
    public Attachment Attachment { get; set; }
    public Address BillingAddress { get; set; }
    public Address ShippingAddress { get; set; }
    public List<OrderLine> OrderLines { get; set; }
}

public class Attachment {
    public string Body { get; set; }
    public string ContentType { get; set; }
}

public class Address {
    public string City { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Title { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string PostalCode { get; set; }
    public string StreetAddress { get; set; }
    public string StreetAddress2 { get; set; }
}

public class OrderLine {
    public string Name { get; set; }
    public string Type { get; set; }
    public int TaxRate { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }
    public string ImageUrl { get; set; }
    public string Reference { get; set; }
    public int TotalAmount { get; set; }
    public string ProductUrl { get; set; }
    public string MerchantData { get; set; }
    public string QuantityUnit { get; set; }
    public int TotalTaxAmount { get; set; }
    public int TotalDiscountAmount { get; set; }
    public Subscription Subscription { get; set; }
    public ProductIdentifiers ProductIdentifiers { get; set; }
}

public class Subscription {
    public string Name { get; set; }
    public string Interval { get; set; }
    public int IntervalCount { get; set; }
}

public class ProductIdentifiers {
    public string Size { get; set; }
    public string Brand { get; set; }
    public string Color { get; set; }
    public string CategoryPath { get; set; }
    public string GlobalTradeItemNumber { get; set; }
    public string ManufacturerPartNumber { get; set; }
}

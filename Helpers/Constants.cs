namespace Nop.Plugin.Payments.VivaPayments.Helpers;
public static class Constants {
    public static Dictionary<string, int> CurrencyCodeToNumeric = new Dictionary<string, int>{
        { "USD", 840 }, // United States Dollar
        { "EUR", 978 }, // Euro
        { "JPY", 392 }, // Japanese Yen
        { "GBP", 826 }, // British Pound Sterling
        { "AUD", 36 },  // Australian Dollar
        { "CAD", 124 }, // Canadian Dollar
        { "CHF", 756 }, // Swiss Franc
        { "CNY", 156 }, // Chinese Yuan Renminbi
        { "SEK", 752 }, // Swedish Krona
        { "NZD", 554 }  // New Zealand Dollar
    };

    public static Dictionary<int, string> CurrencyNumericToCode = new Dictionary<int, string>{
        { 840, "USD" }, // United States Dollar
        { 978, "EUR" }, // Euro
        { 392, "JPY" }, // Japanese Yen
        { 826, "GBP" }, // British Pound Sterling
        { 36, "AUD" },  // Australian Dollar
        { 124, "CAD" }, // Canadian Dollar
        { 756, "CHF" }, // Swiss Franc
        { 156, "CNY" }, // Chinese Yuan Renminbi
        { 752, "SEK" }, // Swedish Krona
        { 554, "NZD" }  // New Zealand Dollar
    };

    public static Dictionary<string, string> EventIds = new Dictionary<string, string>{
        { "0", "Undefined" },
        { "2061", "3DS flow incomplete" },
        { "2062", "3DS validation failed" },
        { "2108", "Payments Policy Acquiring Restriction" },
        { "3500", "DeviceNotFound" },
        { "6614", "IsoTransportChannelSendFailedTimeout" },
        { "6624", "IsoTransportChannelWriteFailed" },
        { "6650", "IsoTransportProxySendFailed" },
        { "7001", "TransactionFailed" },
        { "10001", "Refer to card issuer" },
        { "10003", "Invalid merchant number" },
        { "10004", "Pick up card" },
        { "10005", "Do not honor" },
        { "10006", "General error" },
        { "10007", "Pick up card, special condition (fraud account)" },
        { "10012", "Invalid transaction" },
        { "10013", "Invalid amount" },
        { "10014", "Invalid card number" },
        { "10015", "Invalid issuer" },
        { "10019", "IssuerResponseReenterTransaction" },
        { "10021", "IssuerResponseNoActionTaken" },
        { "10030", "Format error" },
        { "10039", "No credit account" },
        { "10041", "Lost card" },
        { "10043", "Stolen card" },
        { "10046", "Closed Account" },
        { "10051", "Insufficient funds" },
        { "10052", "No checking account" },
        { "10053", "No savings account" },
        { "10054", "Expired card" },
        { "10055", "IssuerResponseIncorrectPin" },
        { "10057", "Function not permitted to cardholder" },
        { "10058", "Function not permitted to terminal" },
        { "10059", "Suspected Fraud" },
        { "10061", "Withdrawal limit exceeded" },
        { "10062", "Restricted card" },
        { "10063", "Issuer response security violation" },
        { "10065", "Soft decline" },
        { "10070", "Call issuer" },
        { "10075", "PIN entry tries exceeded" },
        { "10076", "Invalid / non-existent 'to account' specified" },
        { "10077", "Invalid / non-existent 'from account' specified" },
        { "10078", "IssuerResponseCardBlockOrNotActivated" },
        { "10079", "Life Cycle" },
        { "10080", "No financial impact" },
        { "10081", "IssuerResponseDomesticDebitTransactionNotAllowed" },
        { "10083", "Fraud/Security" },
        { "10084", "Invalid Authorization Life Cycle" },
        { "10086", "IssuerResponsePinValidationNotPossible" },
        { "10088", "Cryptographic failure" },
        { "10089", "Unacceptable PIN - Transaction Declined - Retry" },
        { "10091", "IssuerResponseIssuerUnavailable" },
        { "10093", "Transaction cannot be completed; violation of law" },
        { "10094", "IssuerResponseDuplicateTransmissionDetected" },
        { "10096", "System malfunction" },
        { "10199", "IssuerResponseEmpty" },
        { "10200", "Generic error" },
        { "10201", "IssuerResponseForceReversal" },
        { "10210", "Invalid CVV" },
        { "10211", "Negative Online CAM, dCVV, iCVV, CVV, or CAVV results or Offline PIN Authentication" },
        { "10212", "Blocked Card" },
        { "10213", "Revocation of authorization order" },
        { "10214", "Verification Data Failed" },
        { "10215", "Policy" },
        { "10216", "Invalid/nonexistent account specified (general)" },
        { "10221", "IssuerResponseVisaNoSuchIssuerFirst8Digits" },
        { "10222", "IssuerResponseVisaLostCardPickUp" },
        { "10223", "IssuerResponseVisaStolenCardPickUp" },
        { "10224", "IssuerResponseVisaClosedAccount" },
        { "10300", "IssuerResponseScaRequestedOnlinePinSupported" },
        { "10301", "Soft decline" },
        { "10302", "IssuerResponseScaRequestedSecondTap" },
        { "10400", "IssuerResponseBancontactUseChip" },
        { "10401", "IssuerUnavailable" },
        { "18202", "AcquiringCartesBancairesVoidValidationFailed" },
        { "100918", "PayconiqPaymentStatusExpired" },
        { "100919", "PayconiqPaymentStatusCanceled" },
        { "101003", "PaymentMethodsParseResponseFailed" },
        { "101407", "KlarnaStatusCancelled" }
    };
}
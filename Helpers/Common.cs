using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Payments;

namespace Nop.Plugin.Payments.VivaPayments.Helpers;
public static class Common {
    public static PaymentStatus GetPaymentStatus(string paymentStatusId) {
        var result = PaymentStatus.Pending;
        paymentStatusId ??= string.Empty;
        switch (paymentStatusId.ToLowerInvariant()) {
            case "f":
                result = PaymentStatus.Paid;
                break;
            case "c":
                result = PaymentStatus.Authorized;
                break;
            case "a":
                result = PaymentStatus.Pending;
                break;
            case "e":
            case "x":
                result = PaymentStatus.Voided;
                break;
            case "r":
            case "m":
                result = PaymentStatus.Refunded;
                break;
            default:
                break;
        }

        return result;
    }
}

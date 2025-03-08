﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Payments.VivaPayments;
public class VivaPaymentsDefaults {
    public static string PluginName = "Payments.VivaPayments";
    public static string UserAgent => $"nopCommerce-{NopVersion.FULL_VERSION}";
    public static (string Sandbox, string Live) AccountUrl => ("https://demo-accounts.vivapayments.com/", "https://accounts.vivapayments.com/");
    public static string AuthPath = "connect/token";
    public static (string Sandbox, string Live) ApiUrl => ("https://demo-api.vivapayments.com/", "https://api.vivapayments.com/");
    public static string OrderPath = "checkout/v2/orders";
    public static (string Sandbox, string Live) RedirectUrl => ("https://demo.vivapayments.com/web/checkout?ref={OrderCode}", "https://www.vivapayments.com/web/checkout?ref={OrderCode}");
    public static string TransactionPath = "checkout/v2/transactions/{transactionId}";
}

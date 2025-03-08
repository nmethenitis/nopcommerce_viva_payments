using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

}

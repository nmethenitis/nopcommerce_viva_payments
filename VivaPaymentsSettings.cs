using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.VivaPayments;
public class VivaPaymentsSettings : ISettings
{
    public string SourceCode { get; set; }
    public string MerchantId { get; set; }
    public bool IsSandbox { get; set; }
    public bool PreAuth { get; set; } = false;
    public bool DisableExactAmount { get; set; } = false;
    public bool DisableCash { get; set; } = false;
    public bool DisableWallet { get; set; } = false;
    public string PaymentTitle { get; set; }
    public string PaymentDescription { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}

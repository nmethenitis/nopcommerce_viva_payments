using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.VivaPayments.Models;
public record ConfigurationModel : BaseNopModel{
    public int ActiveStoreScopeConfiguration { get; set; }

    public string SourceCode { get; set; }
    public bool SourceCode_OverrideForStore { get; set; }
    public string MerchantId { get; set; }
    public bool MerchantId_OverrideForStore { get; set; }
    public bool IsSandbox { get; set; } = false;
    public bool IsSandbox_OverrideForStore { get; set; }
    public string ClientId { get; set; }
    public bool ClientId_OverrideForStore { get; set; }
    public string ClientSecret { get; set; }
    public bool ClientSecret_OverrideForStore { get; set; }
    public bool PreAuth { get; set; } = false;
    public bool PreAuth_OverrideForStore { get; set; }
    public bool DisableExactAmount { get; set; } = false;
    public bool DisableExactAmount_OverrideForStore { get; set; }
    public bool DisableCash { get; set; } = false;
    public bool DisableCash_OverrideForStore { get; set; }
    public bool DisableWallet { get; set; } = false;
    public bool DisableWallet_OverrideForStore { get; set; }
    public string PaymentTitle { get; set; }
    public bool PaymentTitle_OverrideForStore { get; set; }
    public string PaymentDescription { get; set; }
    public bool PaymentDescription_OverrideForStore { get; set; }
}

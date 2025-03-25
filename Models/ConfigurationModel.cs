using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.VivaPayments.Models;
public record ConfigurationModel : BaseNopModel{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.SourceCode")] 
    public string SourceCode { get; set; }
    public bool SourceCode_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.MerchantId")]
    public string MerchantId { get; set; }
    public bool MerchantId_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.ApiKey")]
    public string ApiKey { get; set; }
    public bool ApiKey_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.IsSandbox")]
    public bool IsSandbox { get; set; } = false;
    public bool IsSandbox_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.ClientId")]
    public string ClientId { get; set; }
    public bool ClientId_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.ClientSecret")]
    public string ClientSecret { get; set; }
    public bool ClientSecret_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.PreAuth")]
    public bool PreAuth { get; set; } = false;
    public bool PreAuth_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.DisableExactAmount")]
    public bool DisableExactAmount { get; set; } = false;
    public bool DisableExactAmount_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.DisableCash")]
    public bool DisableCash { get; set; } = false;
    public bool DisableCash_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.DisableWallet")]
    public bool DisableWallet { get; set; } = false;
    public bool DisableWallet_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.PaymentTitle")]
    public string PaymentTitle { get; set; }
    public bool PaymentTitle_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.PaymentDescription")]
    public string PaymentDescription { get; set; }
    public bool PaymentDescription_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.EnableInstallments")]
    public bool EnableInstallments { get; set; } = false;
    public bool EnableInstallments_OverrideForStore { get; set; }
    [NopResourceDisplayName("Plugins.Payments.VivaPayments.Fields.MinInstallments")]
    public decimal MinInstallments { get; set; }
    public bool MinInstallments_OverrideForStore { get; set; }
}

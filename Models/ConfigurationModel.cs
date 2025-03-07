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
    public string AuthUrl { get; set; }
    public bool AuthUrl_OverrideForStore { get; set; }
    public string OrdersUrl { get; set; }
    public bool OrdersUrl_OverrideForStore { get; set; }
    public string RedirectUrl { get; set; }
    public bool RedirectUrl_OverrideForStore { get; set; }
    public string TransactionUrl { get; set; }
    public bool TransactionUrl_OverrideForStore { get; set; }
    public string ClientId { get; set; }
    public bool ClientId_OverrideForStore { get; set; }
    public string ClientSecret { get; set; }
    public bool ClientSecret_OverrideForStore { get; set; }
}

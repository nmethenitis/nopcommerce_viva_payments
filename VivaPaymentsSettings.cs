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
    public string AuthUrl { get; set; }
    public string OrdersUrl { get; set; }
    public string RedirectUrl { get; set; }
    public string TransactionUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}

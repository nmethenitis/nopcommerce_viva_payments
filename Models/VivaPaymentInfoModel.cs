using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.VivaPayments.Models;

public record VivaPaymentInfoModel : BaseNopModel {
    public string DescriptionText { get; set; }
}
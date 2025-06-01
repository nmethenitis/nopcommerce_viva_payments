using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.VivaPayments.Infrastracture;
public class RouteProvider : IRouteProvider {
    public int Priority => 100;

    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder) {
        endpointRouteBuilder.MapControllerRoute(
            name: VivaPaymentsDefaults.SuccessPaymentRouteName,
            pattern: "viva-payment/success",
            defaults: new { controller = "VivaPaymentsPublic", action = "PaymentSuccess" });
        endpointRouteBuilder.MapControllerRoute(
            name: VivaPaymentsDefaults.FailPaymentRouteName,
            pattern: "viva-payment/fail",
            defaults: new { controller = "VivaPaymentsPublic", action = "PaymentFail" });
        endpointRouteBuilder.MapControllerRoute(
            name: VivaPaymentsDefaults.WebhookPaymentRouteName,
            pattern: "viva-payment/webhook",
            defaults: new { controller = "VivaPaymentsPublic", action = "PaymentWebhook" });
        endpointRouteBuilder.MapControllerRoute(
            name: VivaPaymentsDefaults.OrderCompletedRouteName,
            pattern: "checkout/order-completed/{orderGuid:Guid}",
            defaults: new { controller = "VivaPaymentsPublic", action = "OrderCompleted" });
    }
}

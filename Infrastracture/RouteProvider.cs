using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}

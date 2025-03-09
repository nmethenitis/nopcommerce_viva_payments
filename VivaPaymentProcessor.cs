using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.VivaPayments.Components;
using Nop.Plugin.Payments.VivaPayments.Helpers;
using Nop.Plugin.Payments.VivaPayments.Models;
using Nop.Plugin.Payments.VivaPayments.Services;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.VivaPayments;
public class VivaPaymentProcessor : BasePlugin, IPaymentMethod {

    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly ISettingService _settingService;
    protected readonly IWebHelper _webHelper;
    protected readonly IPaymentService _paymentService;
    protected readonly VivaPaymentsSettings _vivaPaymentsSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IWorkContext _workContext;
    protected readonly IStoreContext _storeContext;
    protected readonly VivaApiService _vivaApiService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IOrderService _orderService;

    #endregion

    #region Ctor

    public VivaPaymentProcessor(ILocalizationService localizationService, IOrderTotalCalculationService orderTotalCalculationService, ISettingService settingService, IWebHelper webHelper, IPaymentService paymentService, ICustomerService customerService, IWorkContext workContext, IStoreContext storeContext, VivaPaymentsSettings vivaPaymentsSettings, VivaApiService vivaApiService, IHttpContextAccessor httpContextAccessor, IOrderService orderService) {
        _localizationService = localizationService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _settingService = settingService;
        _webHelper = webHelper;
        _paymentService = paymentService;
        _customerService = customerService;
        _workContext = workContext;
        _storeContext = storeContext;
        _vivaPaymentsSettings = vivaPaymentsSettings;
        _vivaApiService = vivaApiService;
        _httpContextAccessor = httpContextAccessor;
        _orderService = orderService;
    }

    #endregion
    public bool SupportCapture => false;

    public bool SupportPartiallyRefund => false;

    public bool SupportRefund => false;

    public bool SupportVoid => false;

    public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

    public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

    public bool SkipPaymentInfo => false;

    public override string GetConfigurationPageUrl() {
        return $"{_webHelper.GetStoreLocation()}Admin/VivaPayments/Configure";
    }

    public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest) {
        return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Cancel recurring method not supported" } });
    }

    public Task<bool> CanRePostProcessPaymentAsync(Order order) {
        return Task.FromResult(false);
    }

    public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest) {
        return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
    }

    public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart) {
        return Task.FromResult(decimal.Zero);
    }

    public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form) {
        return Task.FromResult(new ProcessPaymentRequest());
    }

    public Task<string> GetPaymentMethodDescriptionAsync() {
        return Task.FromResult(string.Empty);
    }

    public Type GetPublicViewComponent() {
        return typeof(VivaPaymentsInfoViewComponent);
    }

    public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart) {
        return Task.FromResult(false);
    }

    public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest) {
        try {
            var order = postProcessPaymentRequest.Order;
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            var langCode = _workContext.GetWorkingLanguageAsync().Result.LanguageCulture;
            var currencyCode = Constants.CurrencyCodeToNumeric[_workContext.GetWorkingCurrencyAsync().Result.CurrencyCode];
            var paymentOrderRequest = new VivaPaymentOrderRequest() {
                Amount = (int)order.OrderTotal * 100,
                Customer = new Customer() {
                    Email = customer.Email,
                    FullName = $"{customer.FirstName} {customer.LastName}",
                    Phone = customer.Phone,
                    RequestLang = langCode,
                },
                CustomerTrns = $"{_storeContext.GetCurrentStore().Name} order {order.Id}",
                MerchantTrns = $"Order {order.Id}",
                DynamicDescriptor = $"{_storeContext.GetCurrentStore().Name}",
                CurrencyCode = currencyCode,
                PaymentTimeout = VivaPaymentsDefaults.PaymentTimeout,
                SourceCode = _vivaPaymentsSettings.SourceCode
            };
            var paymentOrderResult = await _vivaApiService.CreatePaymentOrderAsync(paymentOrderRequest);
            if (paymentOrderResult != null) {
                order.AuthorizationTransactionCode = paymentOrderResult.OrderCode.ToString();
                await _orderService.UpdateOrderAsync(order);
                var redirectUrl = String.Format(_vivaPaymentsSettings.IsSandbox ? VivaPaymentsDefaults.RedirectUrl.Sandbox : VivaPaymentsDefaults.RedirectUrl.Live, order.AuthorizationTransactionCode);
                _httpContextAccessor.HttpContext.Response.Redirect(redirectUrl);
                return;
            } else {
                throw new NopException("Viva payment result is null");
            }
        } catch (Exception ex) {
            throw new NopException($"Error: {ex.Source} - {ex.Message}");
        }
    }

    public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest) {
        return Task.FromResult(new ProcessPaymentResult());
    }

    public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest) {
        return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Method not supported" } });
    }

    public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest) {
        var order = refundPaymentRequest.Order;
        return Task.FromResult(new RefundPaymentResult { Errors = new[] { "Method not supported" } });
    }

    public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form) {
        return Task.FromResult<IList<string>>(new List<string>());
    }

    public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest) {
        return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Method not supported" } });
    }
}
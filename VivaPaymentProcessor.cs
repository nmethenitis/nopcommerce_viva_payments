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
using Nop.Core.Domain.Payments;
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

    public override async Task InstallAsync() {
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string> {
            ["Plugins.Payments.VivaPayments.Payment.Status"] = "Payment status",
            ["Plugins.Payments.VivaPayments.Payment.Status.Success"] = "Success",
            ["Plugins.Payments.VivaPayments.Payment.Status.Fail"] = "Fail",
            ["Plugins.Payments.VivaPayments.Payment.Status.Message"] = "Message",
            ["Plugins.Payments.VivaPayments.Payment.Info.Description"] = "Pay by credit card / viva wallet / IRIS / Google pay / Apple pay through Viva Wallet payment gateway",
            ["Plugins.Payments.VivaPayments.Fields.SourceCode"] = "Source Code",
            ["Plugins.Payments.VivaPayments.Fields.MerchantId"] = "Merchant ID",
            ["Plugins.Payments.VivaPayments.Fields.ApiKey"] = "Api Key",
            ["Plugins.Payments.VivaPayments.Fields.IsSandbox"] = "Use Sandbox",
            ["Plugins.Payments.VivaPayments.Fields.ClientId"] = "Client ID",
            ["Plugins.Payments.VivaPayments.Fields.ClientSecret"] = "Client Secret",
            ["Plugins.Payments.VivaPayments.Fields.PreAuth"] = "Pre Auth",
            ["Plugins.Payments.VivaPayments.Fields.DisableExactAmount"] = "Disable Exact Amount",
            ["Plugins.Payments.VivaPayments.Fields.DisableCash"] = "Disable Cash",
            ["Plugins.Payments.VivaPayments.Fields.DisableWallet"] = "Disable Wallet",
            ["Plugins.Payments.VivaPayments.Fields.PaymentTitle"] = "Title",
            ["Plugins.Payments.VivaPayments.Fields.PaymentDescription"] = "Description",
            ["Plugins.Payments.VivaPayments.Fields.EnableInstallments"] = "Enable Installments",
            ["Plugins.Payments.VivaPayments.Fields.MinInstallments"] = "Min Installment Amount",
        });
        await base.InstallAsync();
    }

    public override async Task UninstallAsync() {
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.VivaPayments");
        await base.UninstallAsync();
    }

    public bool SupportCapture => false;

    public bool SupportPartiallyRefund => true;

    public bool SupportRefund => true;

    public bool SupportVoid => true;

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
        return Task.FromResult(true);
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

    public async Task<string> GetPaymentMethodDescriptionAsync() {
        return await _localizationService.GetResourceAsync("Plugins.Payments.VivaPayments.Payment.Info.Description");
    }

    public Type GetPublicViewComponent() {
        return typeof(PaymentInfoViewComponent);
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
                SourceCode = _vivaPaymentsSettings.SourceCode,
                MaxInstallments = _vivaPaymentsSettings.EnableInstallments && _vivaPaymentsSettings.MinInstallments > 0 ? CalculateMaxInstallments(order.OrderTotal, _vivaPaymentsSettings.MinInstallments) : 0
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

    public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest) {
        var order = refundPaymentRequest.Order;
        var vivaTransactionCancelRequest = new VivaTransactionCancelRequest() {
            Amount = (int)refundPaymentRequest.AmountToRefund*100,
            SourceCode = _vivaPaymentsSettings.SourceCode,
            TransactionId = order.CaptureTransactionId
        };
        var vivaTransactionCancelResponse = await _vivaApiService.CancelTransaction(vivaTransactionCancelRequest);
        if (vivaTransactionCancelResponse.Success.HasValue && vivaTransactionCancelResponse.Success.Value) {
            return new RefundPaymentResult { NewPaymentStatus = refundPaymentRequest.IsPartialRefund ? PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded};
        }
        return new RefundPaymentResult { Errors = new[] { $"Reversal failed with code: {vivaTransactionCancelResponse.ErrorCode} and message: {vivaTransactionCancelResponse.ErrorText}" } };
    }

    public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form) {
        return Task.FromResult<IList<string>>(new List<string>());
    }

    public async Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest) {
        var order = voidPaymentRequest.Order;
        var vivaTransactionCancelRequest = new VivaTransactionCancelRequest() {
            Amount = (int)order.OrderTotal * 100,
            SourceCode = _vivaPaymentsSettings.SourceCode,
            TransactionId = order.CaptureTransactionId
        };
        var vivaTransactionCancelResponse = await _vivaApiService.CancelTransaction(vivaTransactionCancelRequest);
        if (vivaTransactionCancelResponse.Success.HasValue && vivaTransactionCancelResponse.Success.Value) {
            return new VoidPaymentResult { NewPaymentStatus = PaymentStatus.Voided };
        }
        return new VoidPaymentResult { Errors = new[] { "Something went wrong" } };
    }

    private int CalculateMaxInstallments(decimal orderTotal, decimal minInstallmentAmount) {
        var installments = Math.Floor(orderTotal / minInstallmentAmount);
        if(installments >= 3 && installments <= 12) {
            return (int)installments;
        }else if (installments > 12) {
            return 12;
        }
        return 0;
    }
}
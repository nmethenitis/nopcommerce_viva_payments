using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
    protected readonly ILogger<VivaPaymentProcessor> _logger;

    #endregion

    #region Ctor

    public VivaPaymentProcessor(ILocalizationService localizationService, IOrderTotalCalculationService orderTotalCalculationService, ISettingService settingService, IWebHelper webHelper, IPaymentService paymentService, ICustomerService customerService, IWorkContext workContext, IStoreContext storeContext, VivaPaymentsSettings vivaPaymentsSettings, VivaApiService vivaApiService, IHttpContextAccessor httpContextAccessor, IOrderService orderService, ILogger<VivaPaymentProcessor> logger) {
        _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
        _orderTotalCalculationService = orderTotalCalculationService ?? throw new ArgumentNullException(nameof(orderTotalCalculationService));
        _settingService = settingService ?? throw new ArgumentNullException(nameof(settingService));
        _webHelper = webHelper ?? throw new ArgumentNullException(nameof(webHelper));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        _workContext = workContext ?? throw new ArgumentNullException(nameof(workContext));
        _storeContext = storeContext ?? throw new ArgumentNullException(nameof(storeContext));
        _vivaPaymentsSettings = vivaPaymentsSettings ?? throw new ArgumentNullException(nameof(vivaPaymentsSettings));
        _vivaApiService = vivaApiService ?? throw new ArgumentNullException(nameof(vivaApiService));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    public override async Task InstallAsync() {
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string> {
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Payment.Status"] = "Payment status",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Payment.Status.Success"] = "Success",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Payment.Status.Fail"] = "Fail",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Payment.Status.Message"] = "Message",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Payment.Info.Description"] = "Pay by credit card / viva wallet / IRIS / Google pay / Apple pay through Viva Wallet payment gateway",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.SourceCode"] = "Source Code",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.MerchantId"] = "Merchant ID",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.ApiKey"] = "Api Key",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.IsSandbox"] = "Use Sandbox",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.ClientId"] = "Client ID",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.ClientSecret"] = "Client Secret",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.PreAuth"] = "Pre Auth",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.DisableExactAmount"] = "Disable Exact Amount",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.DisableCash"] = "Disable Cash",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.DisableWallet"] = "Disable Wallet",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.PaymentTitle"] = "Title",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.PaymentDescription"] = "Description",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.EnableInstallments"] = "Enable Installments",
            [$"Plugins.{VivaPaymentsDefaults.PluginName}.Fields.MinInstallments"] = "Min Installment Amount",
        });
        await base.InstallAsync();
    }

    public override async Task UninstallAsync() {
        await _localizationService.DeleteLocaleResourcesAsync($"Plugins.{VivaPaymentsDefaults.PluginName}");
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
        return await _localizationService.GetResourceAsync($"Plugins.{VivaPaymentsDefaults.PluginName}.Payment.Info.Description");
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
                Amount = (int)(order.OrderTotal * 100),
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
                throw new Exception("Viva payment result is null");
            }
        } catch (Exception ex) {
            _logger.LogError(ex, "Error during post process payment");
            throw new Exception($"Error: {ex.Source} - {ex.Message}");
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
            Amount = (int)(refundPaymentRequest.AmountToRefund * 100),
            SourceCode = _vivaPaymentsSettings.SourceCode,
            TransactionId = order.CaptureTransactionId
        };
        var vivaTransactionCancelResponse = await _vivaApiService.CancelTransaction(vivaTransactionCancelRequest);
        if (vivaTransactionCancelResponse.Success) {
            return new RefundPaymentResult { NewPaymentStatus = refundPaymentRequest.IsPartialRefund ? PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded };
        }
        return new RefundPaymentResult { Errors = new[] { $"Reversal failed with code: {vivaTransactionCancelResponse.ErrorCode} and message: {vivaTransactionCancelResponse.ErrorText}" } };
    }

    public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form) {
        return Task.FromResult<IList<string>>(new List<string>());
    }

    public async Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest) {
        var order = voidPaymentRequest.Order;
        var vivaTransactionCancelRequest = new VivaTransactionCancelRequest() {
            Amount = (int)(order.OrderTotal * 100),
            SourceCode = _vivaPaymentsSettings.SourceCode,
            TransactionId = order.CaptureTransactionId
        };
        var vivaTransactionCancelResponse = await _vivaApiService.CancelTransaction(vivaTransactionCancelRequest);
        if (vivaTransactionCancelResponse.Success) {
            return new VoidPaymentResult { NewPaymentStatus = PaymentStatus.Voided };
        }
        return new VoidPaymentResult { Errors = new[] { "Something went wrong" } };
    }

    private int CalculateMaxInstallments(decimal orderTotal, decimal minInstallmentAmount) {
        var installments = Math.Floor(orderTotal / minInstallmentAmount);
        if (installments >= 3 && installments <= 12) {
            return (int)installments;
        } else if (installments > 12) {
            return 12;
        }
        return 0;
    }
}
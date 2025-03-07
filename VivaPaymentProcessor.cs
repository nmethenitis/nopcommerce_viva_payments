using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.VivaPayments.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.VivaPayments {
    public class VivaPaymentProcessor : BasePlugin, IPaymentMethod {

        #region Fields

        protected readonly ILocalizationService _localizationService;
        protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
        protected readonly ISettingService _settingService;
        protected readonly IWebHelper _webHelper;
        protected readonly IPaymentService _paymentService;
        protected readonly VivaPaymentsSettings _vivaPaymentsSettings;
        protected readonly CustomerService _customerService;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public VivaPaymentProcessor(ILocalizationService localizationService, IOrderTotalCalculationService orderTotalCalculationService, ISettingService settingService, IWebHelper webHelper, IPaymentService paymentService, VivaPaymentsSettings vivaPaymentsSettings, CustomerService customerService, IWorkContext workContext) {
            _localizationService = localizationService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _paymentService = paymentService;
            _vivaPaymentsSettings = vivaPaymentsSettings;
            _customerService = customerService;
            _workContext = workContext; 
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
            return Task.FromResult(new CancelRecurringPaymentResult());
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order) {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Type GetPublicViewComponent() {
            throw new NotImplementedException();
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart) {
            throw new NotImplementedException();
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest) {
            try {
                var order = postProcessPaymentRequest.Order;
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                var paymentOrderRequest = new VivaPaymentOrderRequest() {
                    Amount = (int)order.OrderTotal * 100,
                    Customer = new Customer() {
                        Email = customer.Email,
                        //CountryCode = customer.County
                        FullName = $"{customer.FirstName} {customer.LastName}",
                        Phone = customer.Phone,
                        RequestLang = _workContext.GetWorkingLanguageAsync().ToString(),
                    }
                };
            } catch (Exception ex) {
                throw ex;
            }
        }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest) {
            return Task.FromResult(new ProcessPaymentResult());
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest) {
            throw new NotImplementedException();
        }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest) {
            throw new NotImplementedException();
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form) {
            throw new NotImplementedException();
        }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest) {
            throw new NotImplementedException();
        }
    }
}

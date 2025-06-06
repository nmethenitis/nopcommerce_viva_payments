﻿using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.VivaPayments.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.VivaPayments.Components;
public class PaymentInfoViewComponent : NopViewComponent {
    protected readonly VivaPaymentsSettings _vivaPaymentsSettings;
    protected readonly ILocalizationService _localizationService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;

    public PaymentInfoViewComponent(VivaPaymentsSettings vivaPaymentsSettings,
        ILocalizationService localizationService,
        IStoreContext storeContext,
        IWorkContext workContext) {
        _vivaPaymentsSettings = vivaPaymentsSettings;
        _localizationService = localizationService;
        _storeContext = storeContext;
        _workContext = workContext;
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData) {
        var store = await _storeContext.GetCurrentStoreAsync();

        var model = new VivaPaymentInfoModel {
            DescriptionText = await _localizationService.GetLocalizedSettingAsync(_vivaPaymentsSettings,
                x => x.PaymentDescription, (await _workContext.GetWorkingLanguageAsync()).Id, store.Id)
        };

        return View("~/Plugins/Payments.VivaPayments/Views/PaymentInfo.cshtml", model);
    }
}
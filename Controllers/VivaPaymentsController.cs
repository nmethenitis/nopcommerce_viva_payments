﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.VivaPayments.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.VivaPayments.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class VivaPaymentsController : BasePaymentController
{
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;

    #endregion
    #region Ctor

    public VivaPaymentsController(ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStoreContext storeContext)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
    }

    #endregion
    #region Methods
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure(){
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var vivaPaymentSettings = await _settingService.LoadSettingAsync<VivaPaymentsSettings>(storeScope);

        var model = new ConfigurationModel{
            SourceCode = vivaPaymentSettings.SourceCode,
            MerchantId = vivaPaymentSettings.MerchantId,
            AuthUrl = vivaPaymentSettings.AuthUrl,
            OrdersUrl = vivaPaymentSettings.OrdersUrl,
            RedirectUrl = vivaPaymentSettings.RedirectUrl,
            TransactionUrl = vivaPaymentSettings.TransactionUrl,
            ClientId = vivaPaymentSettings.ClientId,
            ClientSecret = vivaPaymentSettings.ClientSecret
        };
        if (storeScope > 0){
            model.SourceCode_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.SourceCode, storeScope);
            model.MerchantId_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.MerchantId, storeScope);
            model.AuthUrl_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.AuthUrl, storeScope);
            model.OrdersUrl_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.OrdersUrl, storeScope);
            model.TransactionUrl_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.TransactionUrl, storeScope);
            model.RedirectUrl_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.RedirectUrl, storeScope);
            model.ClientId_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.ClientId, storeScope);
            model.ClientSecret_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.ClientSecret, storeScope);
        }

        return View("~/Plugins/Payments.VivaPayments/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure(ConfigurationModel model){
        if (!ModelState.IsValid)
            return await Configure();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var vivaPaymentSettings = _settingService.LoadSetting<VivaPaymentsSettings>(storeScope);

        //save settings
        vivaPaymentSettings.SourceCode = model.SourceCode;
        vivaPaymentSettings.MerchantId = model.MerchantId;
        vivaPaymentSettings.AuthUrl = model.AuthUrl;
        vivaPaymentSettings.OrdersUrl = model.OrdersUrl;
        vivaPaymentSettings.RedirectUrl = model.RedirectUrl;
        vivaPaymentSettings.TransactionUrl = model.TransactionUrl
        vivaPaymentSettings.ClientId = model.ClientId;
        vivaPaymentSettings.ClientSecret = model.ClientSecret;

        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */

        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.SourceCode, model.SourceCode_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.MerchantId, model.MerchantId_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.AuthUrl, model.AuthUrl_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.OrdersUrl, model.OrdersUrl_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.TransactionUrl, model.TransactionUrl_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.RedirectUrl, model.RedirectUrl_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.ClientId, model.ClientId_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.ClientSecret, model.ClientSecret_OverrideForStore, storeScope, true);
        //now clear settings cache
        _settingService.ClearCache();
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
        return await Configure();
    }
    #endregion
}

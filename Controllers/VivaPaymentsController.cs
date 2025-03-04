using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        var manualPaymentSettings = await _settingService.LoadSettingAsync<VivaPaymentsSettings>(storeScope);

        var model = new ConfigurationModel{
            VivaCheckoutUrl = manualPaymentSettings.VivaCheckoutUrl,
            ApiKey = manualPaymentSettings.ApiKey,
            ApiPassword = manualPaymentSettings.ApiPassword,
            SourceCode = manualPaymentSettings.SourceCode,
            VivaWalletEndPoint = manualPaymentSettings.VivaWalletEndPoint,
            ActiveStoreScopeConfiguration = storeScope,
            BaseApiUrl = manualPaymentSettings.BaseApiUrl,
            MerchantId = manualPaymentSettings.MerchantId,
            PaymentsCreateOrderUrl = manualPaymentSettings.PaymentsCreateOrderUrl,
            PaymentsUrl = manualPaymentSettings.PaymentsUrl,
            PublicKey = manualPaymentSettings.PublicKey,

        };
        if (storeScope > 0){
            model.ApiPassword_OvverideForStore = _settingService.SettingExists(manualPaymentSettings, x => x.ApiPassword, storeScope);
            model.SourceCode_OvverideForStore = _settingService.SettingExists(manualPaymentSettings, x => x.SourceCode, storeScope);
            model.VivaCheckoutUrl_OvverideForStore = _settingService.SettingExists(manualPaymentSettings, x => x.VivaCheckoutUrl, storeScope);
            model.VivaWalletEndPoint_OvverideForStore = _settingService.SettingExists(manualPaymentSettings, x => x.VivaWalletEndPoint, storeScope);
            model.ApiPassword_OvverideForStore = _settingService.SettingExists(manualPaymentSettings, x => x.ApiPassword, storeScope);
            model.BaseApiUrl_OvverideForStore = _settingService.SettingExists(manualPaymentSettings, x => x.BaseApiUrl, storeScope);
            model.MerchantId_OvverideForStore = _settingService.SettingExists(manualPaymentSettings, x => x.MerchantId, storeScope);
            model.PaymentsCreateOrderUrl_OvverideForStore = _settingService.SettingExists(manualPaymentSettings, x => x.PaymentsCreateOrderUrl, storeScope);
            model.PaymentsUrl_OvverideForStore = _settingService.SettingExists(manualPaymentSettings, x => x.PaymentsUrl, storeScope);
            model.ApiKey_OvverideForStore = _settingService.SettingExists(manualPaymentSettings, x => x.ApiKey, storeScope);
            model.PublicKey_OvverideForStore = _settingService.SettingExists(manualPaymentSettings, x => x.PublicKey, storeScope);
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
        var manualPaymentSettings = _settingService.LoadSetting<VivaPaymentsSettings>(storeScope);

        //save settings
        manualPaymentSettings.ApiKey = model.ApiKey;
        manualPaymentSettings.ApiPassword = model.ApiPassword;
        manualPaymentSettings.SourceCode = model.SourceCode;
        manualPaymentSettings.VivaCheckoutUrl = model.VivaCheckoutUrl;
        manualPaymentSettings.VivaWalletEndPoint = model.VivaWalletEndPoint;
        manualPaymentSettings.BaseApiUrl = model.BaseApiUrl;
        manualPaymentSettings.MerchantId = model.MerchantId;
        manualPaymentSettings.PaymentsCreateOrderUrl = model.PaymentsCreateOrderUrl;
        manualPaymentSettings.PaymentsUrl = model.PaymentsUrl;
        manualPaymentSettings.PublicKey = model.PublicKey;

        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */

        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.ApiKey, model.ApiKey_OvverideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.ApiPassword, model.ApiPassword_OvverideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.SourceCode, model.SourceCode_OvverideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.VivaCheckoutUrl, model.VivaCheckoutUrl_OvverideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.VivaWalletEndPoint, model.VivaWalletEndPoint_OvverideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.BaseApiUrl, model.BaseApiUrl_OvverideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.MerchantId, model.MerchantId_OvverideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.PaymentsCreateOrderUrl, model.PaymentsCreateOrderUrl_OvverideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.PaymentsUrl, model.PaymentsUrl_OvverideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(manualPaymentSettings, x => x.PublicKey, model.PublicKey_OvverideForStore, storeScope, true);
        //now clear settings cache
        _settingService.ClearCache();
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
        return await Configure();
    }
    #endregion
}

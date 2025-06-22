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
public class VivaPaymentsController : BasePaymentController {
    #region Fields

    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly ILanguageService _languageService;

    #endregion
    #region Ctor

    public VivaPaymentsController(ILocalizationService localizationService, INotificationService notificationService, IPermissionService permissionService, ISettingService settingService, IStoreContext storeContext, ILanguageService languageService) {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _storeContext = storeContext;
        _languageService = languageService;
    }

    #endregion
    #region Methods
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure() {
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var vivaPaymentSettings = await _settingService.LoadSettingAsync<VivaPaymentsSettings>(storeScope);

        var model = new ConfigurationModel {
            SourceCode = vivaPaymentSettings.SourceCode,
            MerchantId = vivaPaymentSettings.MerchantId,
            ApiKey = vivaPaymentSettings?.ApiKey,
            IsSandbox = vivaPaymentSettings.IsSandbox,
            PaymentTitle = vivaPaymentSettings.PaymentTitle,
            PaymentDescription = vivaPaymentSettings.PaymentDescription,
            ClientId = vivaPaymentSettings.ClientId,
            ClientSecret = vivaPaymentSettings.ClientSecret,
            EnableInstallments = vivaPaymentSettings.EnableInstallments,
            MinInstallments = vivaPaymentSettings.MinInstallments
        };
        await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) => {
            locale.PaymentDescription = await _localizationService
                .GetLocalizedSettingAsync(vivaPaymentSettings, x => x.PaymentDescription, languageId, 0, false, false);
        });
        if (storeScope > 0) {
            model.SourceCode_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.SourceCode, storeScope);
            model.MerchantId_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.MerchantId, storeScope);
            model.ApiKey_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.ApiKey, storeScope);
            model.IsSandbox_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.IsSandbox, storeScope);
            model.PaymentDescription_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.PaymentDescription, storeScope);
            model.PaymentTitle_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.PaymentTitle, storeScope);
            model.ClientId_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.ClientId, storeScope);
            model.ClientSecret_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.ClientSecret, storeScope);
            model.EnableInstallments_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.EnableInstallments, storeScope);
            model.MinInstallments_OverrideForStore = _settingService.SettingExists(vivaPaymentSettings, x => x.MinInstallments, storeScope);
        }
        return View("~/Plugins/Payments.VivaPayments/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> Configure(ConfigurationModel model) {
        if (!ModelState.IsValid)
            return await Configure();

        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var vivaPaymentSettings = _settingService.LoadSetting<VivaPaymentsSettings>(storeScope);

        //save settings
        vivaPaymentSettings.SourceCode = model.SourceCode;
        vivaPaymentSettings.MerchantId = model.MerchantId;
        vivaPaymentSettings.ApiKey = model.ApiKey;
        vivaPaymentSettings.IsSandbox = model.IsSandbox;
        vivaPaymentSettings.PaymentTitle = model.PaymentTitle;
        vivaPaymentSettings.PaymentDescription = model.PaymentDescription;
        vivaPaymentSettings.ClientId = model.ClientId;
        vivaPaymentSettings.ClientSecret = model.ClientSecret;
        vivaPaymentSettings.EnableInstallments = model.EnableInstallments;
        vivaPaymentSettings.MinInstallments = model.MinInstallments;

        /* We do not clear cache after each setting update.
         * This behavior can increase performance because cached settings will not be cleared 
         * and loaded from database after each update */

        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.SourceCode, model.SourceCode_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.MerchantId, model.MerchantId_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.ApiKey, model.ApiKey_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.IsSandbox, model.IsSandbox_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.PaymentTitle, model.PaymentTitle_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.PaymentDescription, model.PaymentDescription_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.ClientId, model.ClientId_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.ClientSecret, model.ClientSecret_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.EnableInstallments, model.EnableInstallments_OverrideForStore, storeScope, true);
        await _settingService.SaveSettingOverridablePerStoreAsync(vivaPaymentSettings, x => x.MinInstallments, model.MinInstallments_OverrideForStore, storeScope, true);
        //now clear settings cache
        await _settingService.ClearCacheAsync();
        foreach (var localized in model.Locales) {
            await _localizationService.SaveLocalizedSettingAsync(vivaPaymentSettings,
                x => x.PaymentDescription, localized.LanguageId, localized.PaymentDescription);
        }
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
        return await Configure();
    }
    #endregion
}

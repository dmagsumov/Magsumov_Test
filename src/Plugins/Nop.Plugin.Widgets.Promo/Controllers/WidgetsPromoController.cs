using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.Promo.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.Promo.Controllers
{
    [Area(AreaNames.Admin)]
    public class WidgetsPromoController : BasePluginController
    {
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public WidgetsPromoController(IStoreContext storeContext,
            IPermissionService permissionService, 
            IPictureService pictureService,
            ISettingService settingService,
            ICacheManager cacheManager,
            ILocalizationService localizationService)
        {
            this._storeContext = storeContext;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._localizationService = localizationService;
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var promoSettings = _settingService.LoadSetting<PromoSettings>(storeScope);
            var model = new ConfigurationModel
            {
                Text = promoSettings.Text,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Text_OverrideForStore = _settingService.SettingExists(promoSettings, x => x.Text, storeScope);
               
            }

            return View("~/Plugins/Widgets.Promo/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var promoSettings = _settingService.LoadSetting<PromoSettings>(storeScope);

            ////get previous picture identifiers
            //var previousText = new[] 
            //{
            //    promoSettings.Text
            //};

            promoSettings.Text = model.Text;
           

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(promoSettings, x => x.Text, model.Text_OverrideForStore, storeScope, false);
            

            //now clear settings cache
            _settingService.ClearCache();

            ////get current picture identifiers
            //var currentText = new[]
            //{
            //    promoSettings.Text
            //};



            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }
    }
}
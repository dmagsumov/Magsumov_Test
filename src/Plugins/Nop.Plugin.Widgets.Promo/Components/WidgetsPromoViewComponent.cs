using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.Promo.Infrastructure.Cache;
using Nop.Plugin.Widgets.Promo.Models;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Promo.Components
{
    [ViewComponent(Name = "WidgetsPromo")]
    public class WidgetsPromoViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ISettingService _settingService;
        

        public WidgetsPromoViewComponent(IStoreContext storeContext, 
            IStaticCacheManager cacheManager, 
            ISettingService settingService, 
            IPictureService pictureService)
        {
            this._storeContext = storeContext;
            this._cacheManager = cacheManager;
            this._settingService = settingService;
           
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var promoSettings = _settingService.LoadSetting<PromoSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel
            {
                Text = promoSettings.Text
            };

            if (string.IsNullOrEmpty(model.Text))
                //no text entered
                return Content("");

            return View("~/Plugins/Widgets.Promo/Views/PublicInfo.cshtml", model);
        }

    }
}

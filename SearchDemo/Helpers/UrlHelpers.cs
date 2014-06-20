using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using EPiServer;

namespace SearchDemo.Helpers
{
    public static class UrlHelpers
    {
        /// <summary>
        /// Returns the target URL for a PageReference. Respects the page's shortcut setting
        /// so if the page is set as a shortcut to another page or an external URL that URL
        /// will be returned.
        /// </summary>
        public static IHtmlString PageLinkUrl(this UrlHelper urlHelper, PageReference pageLink)
        {
            if(ContentReference.IsNullOrEmpty(pageLink))
            {
                return MvcHtmlString.Empty;
            }

            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var page = contentLoader.Get<PageData>(pageLink);

            return PageLinkUrl(urlHelper, page);
        }

        /// <summary>
        /// Returns the target URL for a page. Respects the page's shortcut setting
        /// so if the page is set as a shortcut to another page or an external URL that URL
        /// will be returned.
        /// </summary>
        public static IHtmlString PageLinkUrl(this UrlHelper urlHelper, PageData page)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            switch (page.LinkType)
            {
                case PageShortcutType.Normal:
                case PageShortcutType.FetchData:
                    return new MvcHtmlString(urlResolver.GetUrl(page.PageLink));

                case PageShortcutType.Shortcut:
                    var shortcutProperty = page.Property["PageShortcutLink"] as PropertyPageReference;
                    if (shortcutProperty != null && !ContentReference.IsNullOrEmpty(shortcutProperty.PageLink))
                    {
                        return urlHelper.PageLinkUrl(shortcutProperty.PageLink);
                    }
                    break;

                case PageShortcutType.External:
                    return new MvcHtmlString(page.LinkURL);
            }
            return MvcHtmlString.Empty;
        }

        public static RouteValueDictionary GetPageRoute(this RequestContext requestContext, PageReference pageLink)
        {
            var values = new RouteValueDictionary();
            values[RoutingConstants.NodeKey] = pageLink;
            values[RoutingConstants.LanguageKey] = ContentLanguage.PreferredCulture.Name;
            return values;
        }
    }
}

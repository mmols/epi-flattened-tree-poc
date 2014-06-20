using System;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Search;
using EPiServer.ServiceLocation;

namespace SearchDemo.Search
{
    /// <summary>
    ///     Overridden version of ContentSearchHandler to support for indexing unpublished content. 
    ///     This allows the indexcontent tool (/episerver/cms/admin/indexcontent.aspx) to work properly
    /// </summary>
    [ServiceConfiguration(ServiceType = typeof(GlobalContentSearchHandler))]
    [ServiceConfiguration(ServiceType = typeof(IReIndexable))]
    public class GlobalContentSearchHandler : ContentSearchHandler
    {
        private IContentRepository _contentRepository;
        private LanguageSelectorFactory _languageSelectorFactory;

        public GlobalContentSearchHandler(SearchHandler searchHandler, IContentRepository contentRepository,
            IContentTypeRepository contentTypeRepository, LanguageSelectorFactory languageSelectorFactory,
            SearchIndexConfig searchIndexConfig)
            : base(searchHandler, contentRepository, contentTypeRepository, languageSelectorFactory, searchIndexConfig)
        {
            _contentRepository = contentRepository;
            _languageSelectorFactory = languageSelectorFactory;
        }

        public override void IndexPublishedContent()
        {
            if (!this.ServiceActive)
                return;
            var slimContentReader = new SlimContentReader(this._contentRepository, this._languageSelectorFactory, (ContentReference)ContentReference.RootPage, (Func<IContent, bool>)(c =>
            {
                var local_0 = c as ISearchable;
                if (local_0 != null)
                    return local_0.AllowReIndexChildren;
                else
                    return true;
            }));
            while (slimContentReader.Next())
            {
                if (!slimContentReader.Current.ContentLink.CompareToIgnoreWorkID((ContentReference)ContentReference.RootPage))
                {
                    this.UpdateItem(slimContentReader.Current);
                }
            }
        }
    }
}

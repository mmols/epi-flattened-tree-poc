using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Search;
using EPiServer.ServiceLocation;
using System;
using EPiServer.Core;
using EPiServer;
using EPiServer.Search.Configuration;
using EPiServer.Search.IndexingService;
using Lucene.Net.Documents;
using SearchDemo.Models.Pages;
using SearchDemo.Search.Queries;

namespace SearchDemo.Search
{
    /// <summary>
    ///     This Initializer allows the tree hiding function to work, as well as indexing unpublished content and custom fields on that content.
    /// 
    ///     Credit to Ted Gustaf for Indexing Custom Fields
    ///     http://tedgustaf.com/blog/2013/4/add-custom-fields-to-the-episerver-search-index-with-episerver-7/
    /// </summary>
    [InitializableModule]
    public class SearchInitializer : IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            // Attach event handlers here
            IndexingService.DocumentAdding += CustomizeIndexing;
            DataFactory.Instance.CreatedPage += Instance_CreatedPage;
            EPiServer.DataFactory.Instance.FinishedLoadingChildren += new EPiServer.ChildrenEventHandler(Instance_FinishedLoadingChildren);
        }


        public void Instance_FinishedLoadingChildren(object sender, EPiServer.ChildrenEventArgs e)
        {
            var parent = ServiceLocator.Current.GetInstance<IContentRepository>().Get<IContent>(e.ContentLink) as SitePageData;
            if (parent != null && parent.FlattenTree)
            {
                e.ChildrenItems.Clear();
            }
        }


        public void Uninitialize(InitializationEngine context)
        {
            // Detach any event handlers here
            IndexingService.DocumentAdding -= CustomizeIndexing;
            DataFactory.Instance.CreatedPage -= Instance_CreatedPage;

        }
 

        private static void Instance_CreatedPage(object sender, PageEventArgs e)
        {
            //Add this page to search index
            if (e != null)
            {
                //Update this page in the index since it was Saved
                PageData page = DataFactory.Instance.GetPage(e.Page.PageLink);
                var searchHandler = ServiceLocator.Current.GetInstance<ContentSearchHandler>();
                searchHandler.UpdateItem(page);
            }
        }

        private void CustomizeIndexing(object sender, EventArgs e)
        {
            var addUpdateEventArgs = e as AddUpdateEventArgs;

            if (addUpdateEventArgs == null)
            {
                return; // Document is not being added/updated
            }

            // Get the document being indexed
            var document = addUpdateEventArgs.Document;

            if (document.IsUnifiedFileDocument())
            {
                return; // We don't customize VPP file indexing
            }

            var content = document.GetContent<IContent>();

            var page = content as PageData;

            if (page == null || PageReference.IsNullOrEmpty(page.ParentLink))
            {
                return;
            }

            //Index the Parent Link
            document.Add(new Field(ParentLinkQuery.Field, page.ParentLink.ID.ToString(), Field.Store.YES, Field.Index.ANALYZED));

            //Index the Page Status
            document.Add(new Field(VersionStatusQuery.Field, page.Status.ToString(), Field.Store.YES, Field.Index.ANALYZED));

        }

        public void Preload(string[] parameters)
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            var container = context.Container;
            container.Configure(x => x.For<ContentSearchHandler>().EnrichAllWith(
                y => new GlobalContentSearchHandler(container.GetInstance<SearchHandler>(),
                        container.GetInstance<IContentRepository>(), container.GetInstance<IContentTypeRepository>(),
                        container.GetInstance<LanguageSelectorFactory>(), container.GetInstance<SearchIndexConfig>())
            {
                ServiceActive = true
            }));
        }

    }
}


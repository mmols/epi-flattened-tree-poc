using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Core;
using EPiServer.Search;
using EPiServer.Search.Queries;
using EPiServer.Search.Queries.Lucene;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ContentQuery;
using EPiServer.Shell.Search;
using SearchDemo.Models.Pages;

namespace SearchDemo.Search.Queries
{
    /// <summary>
    ///  Query used in the FlattenedSearchComponent in Dojo
    /// </summary>
    [ServiceConfiguration(typeof(IContentQuery))]
    public class FlattenedSearchComponentQuery : ContentQueryBase
    {
        private readonly IContentRepository _contentRepository;
        private readonly SearchProvidersManager _searchProvidersManager;
        private readonly LanguageSelectorFactory _languageSelectorFactory;
        private readonly SearchIndexConfig _searchIndexConfig;

        public FlattenedSearchComponentQuery(
            IContentQueryHelper queryHelper,
            IContentRepository contentRepository,
            SearchProvidersManager searchProvidersManager,
            LanguageSelectorFactory languageSelectorFactory,
            SearchIndexConfig searchIndexConfig)
            : base(contentRepository, queryHelper)
        {
            _contentRepository = contentRepository;
            _searchProvidersManager = searchProvidersManager;
            _languageSelectorFactory = languageSelectorFactory;
            _searchIndexConfig = searchIndexConfig;
        }

        /// <summary>
        /// The key to trigger this query.
        /// </summary>
        public override string Name
        {
            get { return "FlattenedSearchComponentQuery"; }
        }

        protected override IEnumerable<IContent> GetContent(ContentQueryParameters parameters)
        {
            var queryText = HttpUtility.HtmlDecode(parameters.AllParameters["queryText"]);
            var contentId = HttpUtility.HtmlDecode(parameters.AllParameters["contentId"]);

            var query = new GroupQuery(LuceneOperator.AND);


            //Add Query Text Filter
            if (!String.IsNullOrWhiteSpace(queryText))
            {
                query.QueryExpressions.Add((IQueryExpression)new GroupQuery(LuceneOperator.OR)
                {
                    QueryExpressions = {
                      (IQueryExpression) new FieldQuery(this.AddTrailingWildcards(queryText)),
                      (IQueryExpression) new TermBoostQuery(this.AddTrailingWildcards(queryText), Field.Title, 5f)
                    }
                });
            }

            if (!String.IsNullOrEmpty(contentId))
            {
                PageReference parentReference = null;
                if (PageReference.TryParse(contentId, out parentReference))
                {
                    query.QueryExpressions.Add(new ParentLinkQuery(parentReference));
                }
            }

            //Only Pages
            query.QueryExpressions.Add(new ContentQuery<SitePageData>());

            //Access Control
            var accessQuery = new AccessControlListQuery();
            accessQuery.AddAclForUser(PrincipalInfo.Current, HttpContext.Current);
            query.QueryExpressions.Add(accessQuery);

            var searchHandler = ServiceLocator.Current.GetInstance<SearchHandler>();

            // Perform search
            var results = searchHandler.GetSearchResults(query, 1, int.MaxValue);

            // Convert search result to pages
            return results.IndexResponseItems.Select(x => x.GetContent<PageData>());
        }

        public virtual string AddTrailingWildcards(string query)
        {
            if (query.IndexOfAny(new char[2]
      {
        '*',
        ' '
      }) >= 0)
                return query;
            else
                return query + "*";
        }
    }
}

using System.Globalization;
using EPiServer.Core;
using EPiServer.Search.Queries;
using EPiServer.Search.Queries.Lucene;

namespace SearchDemo.Search.Queries
{
    /// <summary>
    ///  Query by Parent Link in the EPiServer Search Index
    /// </summary>
    public class ParentLinkQuery : IQueryExpression
    {
        public ParentLinkQuery(PageReference parentLink)
        {
            Expression = parentLink.ID.ToString();
            Boost = null;
        }

        public ParentLinkQuery(PageReference parentLink, float boost)
        {
            Expression = parentLink.ID.ToString();
            Boost = boost;
        }

        public string GetQueryExpression()
        {
            return string.Format("{0}:({1}{2})",
                Field,
                LuceneHelpers.EscapeParenthesis(Expression),
                Boost.HasValue ? string.Concat("^", Boost.Value.ToString(CultureInfo.InvariantCulture).Replace(",", ".")) : string.Empty);
        }

        public static string Field { get { return "PARENT_LINK"; } }

        public string Expression { get; set; }

        public float? Boost { get; set; }
    }
}

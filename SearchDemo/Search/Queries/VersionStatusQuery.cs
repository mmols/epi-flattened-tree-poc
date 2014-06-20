using System.Globalization;
using EPiServer.Core;
using EPiServer.Search.Queries;
using EPiServer.Search.Queries.Lucene;

namespace SearchDemo.Search.Queries
{
    /// <summary>
    ///  Query by Version Status in the EPiServer Search Index
    /// </summary>
    public class VersionStatusQuery : IQueryExpression
    {
        public VersionStatusQuery(VersionStatus status)
        {
            Expression = status.ToString();
            Boost = null;
        }

        public VersionStatusQuery(VersionStatus status, float boost)
        {
            Expression = status.ToString();
            Boost = boost;
        }

        public string GetQueryExpression()
        {
            return string.Format("{0}:({1}{2})",
                Field,
                LuceneHelpers.EscapeParenthesis(Expression),
                Boost.HasValue ? string.Concat("^", Boost.Value.ToString(CultureInfo.InvariantCulture).Replace(",", ".")) : string.Empty);
        }

        public static string Field { get { return "STATUS"; } }

        public string Expression { get; set; }

        public float? Boost { get; set; }
    }
}

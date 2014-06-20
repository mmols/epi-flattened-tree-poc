using System;
using EPiServer;
using EPiServer.Core;
using EPiServer.Search;
using EPiServer.ServiceLocation;
using Lucene.Net.Documents;


namespace SearchDemo.Search
{
    /// <summary>
    ///     Credit to Ted Gustaf for Search Extensions
    ///     http://tedgustaf.com/blog/2013/4/add-custom-fields-to-the-episerver-search-index-with-episerver-7/
    /// </summary>
    public static class SearchExtensions
    {
        public static T GetContent<T>(this IndexResponseItem document) where T : IContent
        {

            var fieldValueFragments = document.Id.Split('|'); // Field value is either 'GUID|language' or just a GUID

            Guid contentGuid;

            if (!Guid.TryParse(fieldValueFragments[0], out contentGuid))
            {
                throw new NotSupportedException("Expected first part of ID field to be a valid GUID");
            }

            return ServiceLocator.Current.GetInstance<IContentRepository>().Get<T>(contentGuid);
        }

        public static T GetContent<T>(this Document document) where T : IContent
        {

            // EPiServer Search adds a field called 'EPISERVER_SEARCH_ID' which contains the content GUID
            const string fieldName = "EPISERVER_SEARCH_ID";

            var fieldValue = document.Get(fieldName);

            if (string.IsNullOrWhiteSpace(fieldValue))
            {
                throw new NotSupportedException(string.Format("Specified document did not have a '{0}' field value", fieldName));
            }

            var fieldValueFragments = fieldValue.Split('|'); // Field value is either 'GUID|language' or just a GUID

            Guid contentGuid;

            if (!Guid.TryParse(fieldValueFragments[0], out contentGuid))
            {
                throw new NotSupportedException("Expected first part of ID field to be a valid GUID");
            }

            return ServiceLocator.Current.GetInstance<IContentRepository>().Get<T>(contentGuid);
        }

        public static bool IsUnifiedFileDocument(this Document document)
        {
            var underlyingTypes = document.Get("EPISERVER_SEARCH_TYPE");

            return !string.IsNullOrWhiteSpace(underlyingTypes) && underlyingTypes.Contains("UnifiedFile");
        }
    }
}

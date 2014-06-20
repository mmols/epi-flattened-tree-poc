using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace SearchDemo.Search
{
    /// <summary>
    ///      Register our view with EPiServer
    /// </summary>
    [ServiceConfiguration(typeof(ViewConfiguration))]
    public class SearchView : ViewConfiguration<IContentData>
    {
        public static string KeyName = "searchView";

        public SearchView()
        {
            Key = KeyName;
            Name = "Search";
            Description = "Searches for Content";
            IconClass = "epi-iconPreview";
            ControllerType = "searchdemo/components/FlattenedSearchComponent";
        }
    }
}

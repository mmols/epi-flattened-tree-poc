EPi Flattened Tree POC
=========

Inspired by Umbraco 7's collapsed tree feature - this proof of concept allows certain sections of the tree to be collapsed within EPiServer. These sections of the tree are then surfaced via a Search View, created with Dojo & EPiServer 7.5's Views feature.

In practice, this concept could be used to create a container of News Articles, which would be hidden from editors in the page tree. Editors would find articles by clicking on the container page, searching for a particular article using this View, and then utilizing the standard editor interfaces from that point onward. 

**Notes**

  - Built on Alloy MVC
  - Each page has a "Flatten Tree" property under Settings which will collapse all child pages in the tree
  - A Search view which will show all direct children of the current page can be accessed by clicking the Views dropdown in the upper right corner in Edit Mode
  - EPiServer Search has been modified to index unpublished content
  - This has not been tested at scale
  - Relevant C# code are located in the SearchDemo.Search namespace
  - Relevant Dojo scripts are located in the ClientResources folder

**Next Steps**

- Implement context menus in the Search Grid to allow page deletion
- Set the default view to Search if the Flatten Tree property is checked (currently this is only possible per content type)
- Swap out EPiServer Search and properly handle page versions in the index

**Built / Inspired from:**

[Add custom fields to the EPiServer Search index with EPiServer 7](http://tedgustaf.com/blog/2013/4/add-custom-fields-to-the-episerver-search-index-with-episerver-7/)

[Custom views and plugin areas in EPiServer 7.5](http://world.episerver.com/Blogs/Duong-Nguyen/Dates/2013/12/Custom-views-and-plugin-areas-in-EPiServer-75/)

[Creating a component that searches for content](http://world.episerver.com/Blogs/Linus-Ekstrom/Dates/2012/11/Creating-a-component-that-searches-for-content/)

[PowerSlice](http://joelabrahamsson.com/powerslice/ "PowerSlice")

[Umbraco](http://umbraco.com/ "Umbraco")

License
----

MIT

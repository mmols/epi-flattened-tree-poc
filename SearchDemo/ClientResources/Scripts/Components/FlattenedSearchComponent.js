define([
// Dojo
    "dojo",
    "dojo/_base/declare",
    "dojo/dom-geometry",
    "dojo/dom-class",

// Dijit
    "dijit/_TemplatedMixin",
    "dijit/_Container",
    "dijit/layout/_LayoutWidget",
    "dijit/_WidgetsInTemplateMixin",

// EPi CMS
    "dojo/text!./templates/FlattenedSearchComponent.html",
    "epi-cms/_ContentContextMixin",
    "epi-cms/contentediting/EditToolbar",
    "epi/shell/widget/SearchBox",

 //Encore
    "searchdemo/components/SearchViewGrid"
], function (
// Dojo
    dojo,
    declare,
    domGeometry,
    domClass,

// Dijit
    _TemplatedMixin,
    _Container,
    _LayoutWidget,
    _WidgetsInTemplateMixin,

// EPi CMS
    template,
    _ContentContextMixin,
    EditToolbar,
    SearchBox,

    //Encore
    SearchViewGrid
) {

    return declare("searchdemo.components.FlattenedSearchComponent", [_Container, _LayoutWidget, _TemplatedMixin, _WidgetsInTemplateMixin, _ContentContextMixin], {
        // summary: This component enabled searching of content where the results will be displayed in a grid.

        templateString: template,

        postCreate: function () {

            this.inherited(arguments);
            this._reloadQuery();
            domClass.add(this.ceToolbar.domNode, "epi-localToolbar epi-viewHeaderContainer");
        },

        resize: function (newSize) {
            // summary:
            //      Customize the default resize method.
            // newSize: object
            //      The new size of the custom query component.
            // tags:
            //      Public

            this.inherited(arguments);

            var toolbarSize = domGeometry.getMarginBox(this.toolbar);
            var gridSize = { w: newSize.w, h: newSize.h - toolbarSize.h }

            this.contentQuery.resize(gridSize);
        },

        updateView: function (callerData, ctx, additionalParams) {

            this.ceToolbar.update({
                currentContext: ctx,
                viewConfigurations: {
                    availableViews: callerData.availableViews,
                    viewName: callerData.viewName
                }
            });

        },

        _reloadQuery: function () {
            //If we have no query text, then just sort by created
            if (!this.queryText.value.length) {
                this.contentQuery.set("sortKey", "created");
                this.contentQuery.set("descending", "true");
            } else {
                this.contentQuery.set("sortKey", null);
                this.contentQuery.set("descending", "false");
            }

            this.contentQuery.set("queryParameters", { queryText: this.queryText.value, contentId: this._currentContext.id });
            this.contentQuery.set("queryName", "FlattenedSearchComponentQuery");
        },

        contextChanged: function (context, callerData) {
            this.inherited(arguments);

            this._reloadQuery();

        }


    });
});
define([
    "dojo",
    "dojo/_base/lang",
    "epi",
    "epi-cms/widget/_GridWidgetBase",
    "dojo/aspect",
    "epi-cms/contentediting/ContentActionSupport",
    "epi/datetime"
], function (dojo, lang, epi, _GridWidgetBase, aspect, ContentActionSupport, epiDate) {
    return dojo.declare("searchdemo.components.SearchViewGrid", [_GridWidgetBase], {
        queryName: null,

        queryParameters: null,

        dndTypes: ['epi.cms.contentreference'],

        postMixInProperties: function () {
            // summary:
            //
            // tags:
            //    protected
            this.storeKeyName = "epi.cms.content.search";

            this.inherited(arguments);
        },
        sortKey: null,
        descending: false,
        buildRendering: function () {
            // summary:
            //		Construct the UI for this widget with this.domNode initialized as a dgrid.
            // tags:
            //		protecteds

            this.inherited(arguments);

            var gridSettings = lang.mixin({
                showHeader: true,
                columns: {
                    name: {
                        label: epi.resources.header.name,
                        renderCell: lang.hitch(this, this._renderContentItem)
                    },
                    created: {
                        label: epi.resources.header.created,
                        renderCell: function (item, value, node, options) {
                            node.innerHTML = epiDate.toUserFriendlyHtml(value);
                        }
                    },
                    status: {
                        label: epi.resources.header.status,
                        className: "epi-width35",
                        renderCell: function (item, value, node, options) {
                            node.innerHTML = ContentActionSupport.getVersionStatus(value);
                        }
                    },
                    menu: {
                        label: '',
                        sortable: false,
                        renderCell: function (item, value, node, options) {
                            node.innerHTML = "<a class=\"epi-gridAction epi-visibleLink\" title=\"View\">View</a>";
                        }
                    }
                },
                store: this.store,
                dndSourceType: this.dndTypes
            }, this.defaultGridMixin);

            this.grid = new this._gridClass(gridSettings, this.domNode);

            this.on(".epi-gridAction:click", lang.hitch(this, function (evt) { // on delete action clicking
                var row = this.grid.row(evt);
                if (row && row.data) {
                    // Change context to that Item
                    var contextParameters = { uri: row.data.uri };
                    dojo.publish("/epi/shell/context/request", [contextParameters]);
                }
            }));


            this.own(
                aspect.around(this.grid, "insertRow", lang.hitch(this, this._aroundInsertRow))
            );
        },

        fetchData: function () {

            var queryOptions = { ignore: ["query"] };
            if (this.sortKey) {
                queryOptions.sort = [{ attribute: this.sortKey, descending: this.descending }];
            }
            this.grid.set("queryOptions", queryOptions);

            var queryParameters = this.queryParameters || {};
            queryParameters.query = this.queryName;
            this.grid.set("query", queryParameters);
        },
        _setQueryNameAttr: function (value) {
            this.queryName = value;
            this.fetchData();
        }
    });
});
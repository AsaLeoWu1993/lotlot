(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-7a13df94"],{"119c":function(t,a,e){"use strict";var i=e("b6f1");t.exports=function(t,a){return!!t&&i(function(){a?t.call(null,function(){},1):t.call(null)})}},7013:function(t,a,e){"use strict";var i=e("a1b2"),n=e.n(i);n.a},"76fd":function(t,a,e){"use strict";e.r(a);var i=function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"agent-report"},[e("div",{staticClass:"content"},[e("div",{staticClass:"list-header"},[e("div",{staticClass:"list-title"},[e("span",[t._v("代理列表")]),e("div",{staticClass:"list-operate"},[e("DatePicker",{staticClass:"columns-item-input",attrs:{type:"date",options:t.forbiddenTime,format:"yyyy-MM-dd",placeholder:"选择搜索的时间"},on:{"on-change":t.handleSearch},model:{value:t.searchDate,callback:function(a){t.searchDate=a},expression:"searchDate"}}),e("div",{staticClass:"search",on:{click:t.handleBackwater}},[t._v("一键回水")]),t._m(0)],1)])]),e("div",{staticClass:"list-data"},[t._m(1),e("div",{staticClass:"data-con"},t._l(t.agentReportData,function(a,i){return e("div",{key:i,staticClass:"data-item"},[e("div",{staticClass:"item-info"},[t._v(t._s(a.agentName))]),e("div",{staticClass:"item-info"},[t._v(t._s(a.allScroll))]),e("div",{staticClass:"item-info"},[t._v(t._s(a.backwater))]),e("div",{staticClass:"item-info"},[t._v(t._s(a.status))]),e("div",{staticClass:"item-info operate"},[e("div",{staticClass:"item-button",on:{click:function(e){return t.handleQueryOffline(a)}}},[t._v("查看下级数据")])])])}),0)]),e("Page",{staticClass:"footer-page",attrs:{total:t.page.total,"page-size":t.page.pageSize,current:t.page.pageNum,"show-sizer":""},on:{"on-page-size-change":t.handleChangeSize,"on-change":t.handleChangePage}})],1),e("div",{directives:[{name:"show",rawName:"v-show",value:t.offlineData.show,expression:"offlineData.show"}],staticClass:"offline-data-mask"},[e("div",{directives:[{name:"show",rawName:"v-show",value:t.offlineData.show,expression:"offlineData.show"}],staticClass:"offline-data"},[e("div",{staticClass:"offline-data-header"},[e("span",[t._v(t._s(t.offlineData.header))]),e("span",{staticClass:"modal-close",on:{click:t.handleCloseModal}},[t._v("X")])]),e("div",{staticClass:"account-setting-list"},[e("i-table",{attrs:{loading:t.isOfflining,columns:t.offlineData.cols,data:t.offlineData.list}})],1)])])])},n=[function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"poptip"},[e("p",[t._v("1.代理回水只能一天处理一次。并且只能在06:00后选择前一天回水处理。")]),e("p",[t._v("2.选定时间统计周期：当天06:00 - 次日06:00")])])},function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"data-header"},[e("p",{staticClass:"item-info"},[t._v("用户名")]),e("p",{staticClass:"item-info"},[t._v("流水汇总")]),e("p",{staticClass:"item-info"},[t._v("回水汇总")]),e("p",{staticClass:"item-info"},[t._v("状态")]),e("p",{staticClass:"item-info"},[t._v("操作")])])}],s=(e("f763"),e("b745"),{data:function(){return{page:{total:0,pageSize:10,pageNum:1},showLoading:!1,searchDate:(new Date).pattern("yyyy-MM-dd"),isOfflining:!1,gameList:[],offlineData:{show:!1,header:"",list:[],cols:[{title:"返水",align:"center",key:"backwater"},{title:"状态",align:"center",key:"status"},{title:"时间",align:"center",width:120,key:"time"}]},agentReportData:[],forbiddenTime:{disabledDate:function(t){return t&&t.valueOf()>=Date.now()}},isLoading:!1,isBackWater:!1,auto:null}},beforeRouteEnter:function(t,a,e){e(function(t){t.auto=function(){t.handleGetData().then(function(){setTimeout(function(){t.auto&&t.auto()},3e4)})},t.getGameList().then(function(){t.auto()})})},beforeRouteLeave:function(t,a,e){var i=this;i.auto=null,e()},methods:{getGameList:function(){var t=this;return new Promise(function(a,e){t.$axios({url:"/api/Merchant/GetGameList"}).then(function(i){if(100===i.data.Status){var n=i.data.Data;t.gameList=n.map(function(t){return{title:t.GameName,type:t.GameType,key:t.NickName}}).sort(function(t,a){return a.type-t.type}),t.gameList.forEach(function(a){t.offlineData.cols.unshift({title:a.title,align:"center",key:a.key})}),t.offlineData.cols.unshift({title:"下线昵称",align:"center",key:"offlineName"}),a()}e()})})},handleChangeSize:function(t){var a=this;a.page.pageSize=t,a.page.pageNum=1,a.handleGetData()},handleChangePage:function(t){var a=this;a.page.pageNum=t,a.handleGetData()},handleSearch:function(){var t=this;t.handleGetData()},handleBackwater:function(){var t=this;t.isBackWater||(t.isBackWater=!0,t.$axios({url:"/api/Monito/GhostUserReport",params:{time:t.searchDate}}).then(function(a){t.isBackWater=!1,100===a.data.Status?t.$Message.success(a.data.Message):t.$Message.error(a.data.Message)}))},handleQueryOffline:function(t){var a=this;a.offlineData.header="下线列表",a.handlegetOfflineData(t),a.offlineData.show=!0},handleCloseModal:function(){var t=this;t.offlineData.show=!1},handlegetOfflineData:function(t){var a=this;a.searchDate?a.isOfflining||(a.isOfflining=!0,a.offlineData.list=[],a.$axios({url:"/api/Monito/GetOfflineReport",params:{time:a.searchDate,agentID:t.agentId}}).then(function(t){if(a.isOfflining=!1,100===t.data.Status){var e=t.data.Data;a.offlineData.list=[],e.forEach(function(t){var e={offlineName:t.NickName+"("+t.OnlyCode+")",showId:t.OnlyCode,backwater:t.Ascent,status:1===t.BackStatus?"已回水":"未回水",time:t.Time};a.gameList.forEach(function(a){e[a.key]=t[a.key]}),a.offlineData.list.push(e)})}})):a.$Message.error("请选择查询日期")},handleGetData:function(){var t=this,a={};return new Promise(function(e,i){a["start"]=t.page.pageNum,a["pageSize"]=t.page.pageSize,t.searchDate?(a["time"]=t.searchDate,t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/Monito/SearchAgentReport",params:a}).then(function(a){if(t.isLoading=!1,100===a.data.Status){var i=a.data.Data;t.page.total=a.data.Total,t.agentReportData=[],i.forEach(function(a){var e=0,i={agentName:a.NickName+"("+a.OnlyCode+")",showId:a.OnlyCode,backwater:a.Ascent,status:1===a.BackStatus?"已回水":"未回水",agentId:a.AgentID};t.gameList.forEach(function(t){e+=a[t.key]}),i["allScroll"]=e,t.agentReportData.push(i)})}e()}))):t.$Message.error("请选择查询日期")})}}}),o=s,l=(e("7013"),e("6691")),r=Object(l["a"])(o,i,n,!1,null,"5dc354a2",null);a["default"]=r.exports},a1b2:function(t,a,e){},b745:function(t,a,e){"use strict";var i=e("b2f5"),n=e("648a"),s=e("db4b"),o=e("b6f1"),l=[].sort,r=[1,2,3];i(i.P+i.F*(o(function(){r.sort(void 0)})||!o(function(){r.sort(null)})||!e("119c")(l)),"Array",{sort:function(t){return void 0===t?l.call(s(this)):l.call(s(this),n(t))}})},f763:function(t,a,e){for(var i=e("dac5"),n=e("cfc7"),s=e("e5ef"),o=e("3754"),l=e("743d"),r=e("14fc"),c=e("8b37"),f=c("iterator"),u=c("toStringTag"),d=r.Array,h={CSSRuleList:!0,CSSStyleDeclaration:!1,CSSValueList:!1,ClientRectList:!1,DOMRectList:!1,DOMStringList:!1,DOMTokenList:!0,DataTransferItemList:!1,FileList:!1,HTMLAllCollection:!1,HTMLCollection:!1,HTMLFormElement:!1,HTMLSelectElement:!1,MediaList:!0,MimeTypeArray:!1,NamedNodeMap:!1,NodeList:!0,PaintRequestList:!1,Plugin:!1,PluginArray:!1,SVGLengthList:!1,SVGNumberList:!1,SVGPathSegList:!1,SVGPointList:!1,SVGStringList:!1,SVGTransformList:!1,SourceBufferList:!1,StyleSheetList:!0,TextTrackCueList:!1,TextTrackList:!1,TouchList:!1},g=n(h),p=0;p<g.length;p++){var m,v=g[p],D=h[v],C=o[v],L=C&&C.prototype;if(L&&(L[f]||l(L,f,d),L[u]||l(L,u,v),r[v]=d,D))for(m in i)L[m]||s(L,m,i[m],!0)}}}]);
//# sourceMappingURL=chunk-7a13df94.1587985104498.js.map
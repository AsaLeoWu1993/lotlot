(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-4a28ebb0"],{"119c":function(e,t,a){"use strict";var s=a("b6f1");e.exports=function(e,t){return!!e&&s(function(){t?e.call(null,function(){},1):e.call(null)})}},"2d43":function(e,t,a){var s=a("01f5"),i=a("6462"),n=a("db4b"),r=a("b146"),o=a("5824");e.exports=function(e,t){var a=1==e,c=2==e,l=3==e,u=4==e,d=6==e,y=5==e||d,p=t||o;return function(t,o,m){for(var h,f,v=n(t),g=i(v),T=s(o,m,3),D=r(g.length),q=0,w=a?p(t,D):c?p(t,0):void 0;D>q;q++)if((y||q in g)&&(h=g[q],f=T(h,q,v),e))if(a)w[q]=f;else if(f)switch(e){case 3:return!0;case 5:return h;case 6:return q;case 2:w.push(h)}else if(u)return!1;return d?-1:l||u?u:w}}},3083:function(e,t,a){"use strict";var s=a("e00b"),i=a.n(s);i.a},5824:function(e,t,a){var s=a("f691");e.exports=function(e,t){return new(s(e))(t)}},"608b":function(e,t,a){"use strict";var s=a("b2f5"),i=a("2d43")(5),n="find",r=!0;n in[]&&Array(1)[n](function(){r=!1}),s(s.P+s.F*r,"Array",{find:function(e){return i(this,e,arguments.length>1?arguments[1]:void 0)}}),a("644a")(n)},7364:function(e,t,a){var s=a("ddf7").f,i=Function.prototype,n=/^\s*function ([^ (]*)/,r="name";r in i||a("dad2")&&s(i,r,{configurable:!0,get:function(){try{return(""+this).match(n)[1]}catch(e){return""}}})},b5b8:function(e,t,a){var s=a("94ac");e.exports=Array.isArray||function(e){return"Array"==s(e)}},b745:function(e,t,a){"use strict";var s=a("b2f5"),i=a("648a"),n=a("db4b"),r=a("b6f1"),o=[].sort,c=[1,2,3];s(s.P+s.F*(r(function(){c.sort(void 0)})||!r(function(){c.sort(null)})||!a("119c")(o)),"Array",{sort:function(e){return void 0===e?o.call(n(this)):o.call(n(this),i(e))}})},c83a:function(e,t,a){"use strict";a.r(t);var s=function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"report-form"},[a("div",{staticClass:"content"},[a("div",{staticClass:"query-list"},[a("div",{staticClass:"query-list-t"},[a("div",{staticClass:"query-item"},[a("span",[e._v("快捷：")]),a("div",{staticClass:"quick-method"},[a("span",{class:1===e.quickSelect?"active":"",on:{click:function(t){return e.handleQueryByDate(1)}}},[e._v("今天")]),a("span",{class:2===e.quickSelect?"active":"",on:{click:function(t){return e.handleQueryByDate(2)}}},[e._v("昨天")]),a("span",{class:3===e.quickSelect?"active":"",on:{click:function(t){return e.handleQueryByDate(3)}}},[e._v("近一周")]),a("span",{class:4===e.quickSelect?"active":"",on:{click:function(t){return e.handleQueryByDate(4)}}},[e._v("自定义")])])]),a("i-select",{staticStyle:{width:"124px"},on:{"on-change":e.handleChangeType},model:{value:e.query.gameType,callback:function(t){e.$set(e.query,"gameType",t)},expression:"query.gameType"}},e._l(e.gameList,function(t,s){return a("i-option",{key:s,attrs:{value:t.type}},[e._v(e._s(t.name))])}),1),a("i-select",{staticStyle:{width:"124px"},on:{"on-change":e.handleChangeType},model:{value:e.query.selectUserType,callback:function(t){e.$set(e.query,"selectUserType",t)},expression:"query.selectUserType"}},e._l(e.userTypeList,function(t,s){return a("i-option",{key:s,attrs:{value:t.value}},[e._v(e._s(t.title))])}),1),a("div",{staticClass:"query-button",on:{click:e.handleChangeType}},[e._v("查询")])],1),a("div",{staticClass:"query-list-t"},[a("div",{staticClass:"quick-date"},[a("span",[e._v("区间：")]),a("DatePicker",{staticClass:"date-item",attrs:{disabled:e.canEdit,type:"date"},model:{value:e.query.startTime,callback:function(t){e.$set(e.query,"startTime",t)},expression:"query.startTime"}}),a("span",[e._v("06:00 --")]),a("DatePicker",{staticClass:"date-item",attrs:{disabled:e.canEdit,type:"date"},model:{value:e.query.endTime,callback:function(t){e.$set(e.query,"endTime",t)},expression:"query.endTime"}}),a("span",[e._v("06:00")])],1),a("i-input",{staticClass:"query-input-item",attrs:{placeholder:"查询用户ID/用户名"},model:{value:e.query.searchUsername,callback:function(t){e.$set(e.query,"searchUsername",t)},expression:"query.searchUsername"}}),a("div",{staticClass:"query-button delete",on:{click:function(t){e.showDelete=!0}}},[e._v("删除记录")])],1)]),a("div",{staticClass:"query-operate"},[a("i-select",{on:{"on-change":e.handleChangeMode},model:{value:e.query.gameModeType,callback:function(t){e.$set(e.query,"gameModeType",t)},expression:"query.gameModeType"}},e._l(e.gameTypeList,function(t,s){return a("i-option",{key:s,attrs:{value:t.value}},[e._v(e._s(t.title))])}),1),a("span",{on:{click:e.handleBackwater}},[e._v("一键回水")])],1),e._m(0)]),a("div",{staticClass:"content"},[a("div",{staticClass:"list-header"},[a("div",{staticClass:"list-title"},[a("span",[e._v("用户报表")]),a("div",{staticClass:"list-poptip"},[a("span",[e._v("玩家总数")]),a("span",[e._v(e._s(e.allGamer))]),a("span",[e._v("盈亏合计：")]),a("span",[e._v(e._s(e.lossAndWinnerAll))]),a("span",[e._v("流水合计：")]),a("span",[e._v(e._s(e.flowWaterAll))]),a("span",[e._v("回水合计：")]),a("span",[e._v(e._s(e.backwaterAll))])])])]),a("div",{staticClass:"list-data"},[a("div",{staticClass:"data-header"},[a("p",{staticClass:"item-info item-1"},[e._v("用户名")]),a("p",{staticClass:"item-info item-2"},[e._v("ID")]),a("p",{staticClass:"item-info item-3"},[e._v("用户类型")]),a("p",{staticClass:"item-info item-4"},[e._v("游戏")]),a("p",{staticClass:"item-info item-5"},[e._v("流水")]),a("p",{staticClass:"item-info item-6"},[e._v("盈亏")]),a("p",{staticClass:"item-info item-7"},[e._v("回水方案")]),a("p",{staticClass:"item-info item-8"},[e._v("回水金额")]),e.showDataList.length&&null!==e.showDataList[0].reversible?a("p",{staticClass:"item-info item-9"},[e._v("可返流水")]):e._e(),e.showDataList.length&&null!==e.showDataList[0].reversible?a("p",{staticClass:"item-info item-10"},[e._v("状态")]):e._e()]),a("div",{staticClass:"data-con"},e._l(e.showDataList,function(t,s){return a("div",{key:s,staticClass:"data-item"},[a("p",{staticClass:"item-info item-1"},[e._v(e._s(t.username))]),a("p",{staticClass:"item-info item-2"},[e._v(e._s(t.showId))]),a("p",{staticClass:"item-info item-3"},[e._v(e._s(t.userType))]),a("p",{staticClass:"item-info item-4"},[e._v(e._s(t.gamaName))]),a("p",{staticClass:"item-info item-5"},[e._v(e._s(t.flowingWater))]),a("p",{staticClass:"item-info item-6"},[e._v(e._s(t.profitLoss))]),a("p",{staticClass:"item-info item-6"},[e._v(e._s(t.backType))]),a("p",{staticClass:"item-info item-8"},[e._v(e._s(t.backwater))]),null!==t.reversible?a("p",{staticClass:"item-info item-9"},[e._v(e._s(t.reversible))]):e._e(),null!==t.reversible?a("p",{staticClass:"item-info item-10",style:{width:e.showDataList.length<=12?"97px":"auto"}},[e._v(e._s(t.status))]):e._e()])}),0)])]),a("Modal",{attrs:{styles:{top:"200px"},width:"300"},model:{value:e.showDelete,callback:function(t){e.showDelete=t},expression:"showDelete"}},[a("p",{staticStyle:{color:"#0d1941","font-size":"16px","text-align":"center"},attrs:{slot:"header"},slot:"header"},[a("span",[e._v("确认删除记录")])]),a("p",{staticStyle:{"text-align":"center"}},[e._v("是否确认删除记录？")]),a("div",{staticClass:"modal-footer",attrs:{slot:"footer"},slot:"footer"},[a("div",{staticClass:"enter-cancel",on:{click:function(t){e.showDelete=!1}}},[e._v("取消")]),a("div",{staticClass:"enter-ok",on:{click:e.handleEnterCancel}},[e._v("确定")])])])],1)},i=[function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"query-poptip"},[a("p",[e._v("1、一键回水流程是先根据左侧条件查询和回水模式筛选的选择在列表呈现需回水用户，然后点击一键回水执行列表用户的回水操作。")]),a("p",[e._v("2、点击一键回水会根据预设方案执行回水操作。需要注意的是回水只能计算某一天，如选择多天则无法成功回水。")])])}],n=(a("608b"),a("7364"),a("b745"),a("cde0"),a("f763"),{data:function(){return{activeGameType:"lottery",gameBigList:[{title:"彩票游戏",key:"lottery"},{title:"视讯游戏",key:"video"}],showLoading:!1,quickSelect:1,canEdit:!0,showDelete:!1,query:{startTime:(new Date).getHours()>=6?new Date:new Date((new Date).setDate((new Date).getDate()-1)),endTime:new Date((new Date).setDate((new Date).getDate()+1)),searchUsername:"",gameType:"0",selectUserType:"1",gameModeType:0},gameList:[],selectGameActive:"lottery",gameType:[{value:"lottery",title:"彩票游戏"},{value:"video",title:"视讯游戏"}],userTypeList:[{title:"正式用户",value:"1"},{title:"虚假用户",value:"4"}],gameTypeList:[{title:"所有玩家",value:0},{title:"按流水回水玩家",value:1},{title:"按输赢回水玩家",value:2}],page:{total:0,pageSize:10,pageNum:1},dataList:[],showDataList:[],forbiddenTime:{disabledDate:function(e){return e&&e.valueOf()>=Date.now()}},isLoading:!1,loginName:"",isDeleting:!1,selectItem:{}}},beforeRouteEnter:function(e,t,a){a(function(e){var t=localStorage.getItem("loginName");t&&(e.loginName=t,e.getGameList().then(function(){e.handleGetData()}))})},computed:{allGamer:function(){var e=this,t=[];return e.showDataList.forEach(function(e){-1===t.findIndex(function(t){return t===e.showId})&&"正常"===e.userType&&t.push(e.showId)}),t.length},lossAndWinnerAll:function(){var e=this;return e.showDataList.reduce(function(e,t){return e+t.profitLoss},0).toFixed(2)},flowWaterAll:function(){var e=this;return e.showDataList.reduce(function(e,t){return e+t.flowingWater},0).toFixed(2)},backwaterAll:function(){var e=this;return e.showDataList.reduce(function(e,t){return e+t.backwater},0).toFixed(2)}},methods:{handleSelectGameBigType:function(e){var t=this;t.activeGameType=e.key,t.page.pageNum=1,t.getGameList().then(function(){t.handleGetData()})},getGameList:function(){var e=this;return new Promise(function(t,a){"lottery"==e.activeGameType?e.$axios({url:"/api/Merchant/GetGameList"}).then(function(s){if(100===s.data.Status){var i=s.data.Data;e.gameList=i.map(function(e){return{name:e.GameName,type:e.GameType,key:e.NickName}}).sort(function(e,t){return e.type-t.type}),e.gameList.unshift({name:"所有游戏",type:"0",key:"all"}),t()}a()}):(e.gameList=[],e.gameList.push({name:"所有游戏",type:"0",key:"all"}),e.gameList.push({name:"百家乐",type:"1",key:"bjl"}),t())})},handleEnterCancel:function(){var e=this;e.showDelete=!1,e.handleDelete()},handleQueryByDate:function(e){var t=this,a=(new Date).getHours()>=6?new Date:new Date((new Date).setDate((new Date).getDate()-1)),s=new Date((new Date).setDate((new Date).getDate()+1)),i=new Date((new Date).setDate((new Date).getDate()-1)),n=new Date((new Date).setDate((new Date).getDate()-6));switch(t.quickSelect=e,e){case 1:t.canEdit=!0,t.query.startTime=a,t.query.endTime=s,t.handleGetData(a.pattern("yyyy-MM-dd")+" 06:00",s.pattern("yyyy-MM-dd")+" 06:00");break;case 2:t.canEdit=!0,t.query.startTime=i,t.query.endTime=a,t.handleGetData(i.pattern("yyyy-MM-dd")+" 06:00",a.pattern("yyyy-MM-dd")+" 06:00");break;case 3:t.canEdit=!0,t.query.startTime=n,t.query.endTime=s,t.handleGetData(n.pattern("yyyy-MM-dd")+" 06:00",s.pattern("yyyy-MM-dd")+" 06:00");break;case 4:t.canEdit=!1;break}},handleChangeType:function(){var e=this;e.handleGetData()},handleChangeSize:function(e){var t=this;t.page.pageSize=e,t.page.pageNum=1,t.handleGetData()},handleChangePage:function(e){var t=this;t.page.pageNum=e,t.handleGetData()},handlechangeColl:function(e){var t=this;e.value.length>1&&t.handleGetData(e.type)},handleBackwater:function(){var e=this,t={},a="";e.query.startTime||startTime?e.query.endTime||endTime?(t["startTime"]=e.query.startTime.pattern("yyyy-MM-dd")+" 06:00",t["endTime"]=e.query.endTime.pattern("yyyy-MM-dd")+" 06:00",e.query.gameModeType&&(t["pattern"]=e.query.gameModeType),e.query.searchUsername&&(t["userKeyword"]=e.query.searchUsername),e.query.selectUserType&&(t["userType"]=e.query.selectUserType),"0"!=e.query.gameType&&(t["gameType"]=e.query.gameType),a="lottery"==e.activeGameType?"/api/Monito/GhostReport":"/api/Monito/GhostVideoReport",e.isLoading||(e.isLoading=!0,e.showLoading=!0,e.$axios({url:a,method:"post",params:t}).then(function(t){e.isLoading=!1,100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.showLoading=!1,e.handleGetData()}))):e.$Message.success("请选择结束时间"):e.$Message.success("请选择开始时间")},handleDelete:function(){var e=this,t={},a="";e.query.startTime||startTime?e.query.endTime||endTime?(t["startTime"]=e.query.startTime.pattern("yyyy-MM-dd")+" 06:00",t["endTime"]=e.query.endTime.pattern("yyyy-MM-dd")+" 06:00",e.query.gameModeType&&(t["pattern"]=e.query.gameModeType),e.query.searchUsername&&(t["userKeyword"]=e.query.searchUsername),e.query.selectUserType&&(t["userType"]=e.query.selectUserType),"0"!=e.query.gameType&&(t["gameType"]=e.query.gameType),a="lottery"==e.activeGameType?"/api/Monito/DeleteRecords":"/api/Monito/DeleteVideoRecords",e.isDeleting||(e.isDeleting=!0,e.showLoading=!0,e.$axios({url:a,method:"post",params:t}).then(function(t){e.isDeleting=!1,100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.showLoading=!1,e.handleGetData()}))):e.$Message.success("请选择结束时间"):e.$Message.success("请选择开始时间")},handleBackwaterType:function(e){var t=this,a={};t.query.startTime&&t.query.endTime?(t.query.searchUsername&&(a["userCode"]=t.query.searchUsername),a["startTime"]=t.query.startTime.pattern("yyyy-MM-dd")+" 06:00",a["endTime"]=t.query.endTime.pattern("yyyy-MM-dd")+" 06:00",a["pattern"]=t.query.gameModeType,a["gameType"]=e.type,e.isLoading||(e.isLoading=!0,t.$axios({url:"/api/Monito/GhostReportByGameType",method:"post",params:a}).then(function(a){e.isLoading=!1,100===a.data.Status?t.$Message.success(a.data.Message):t.$Message.error(a.data.Message),t.handleGetData(e.type)}))):t.$Message.success("请选择查询日期")},handleGetData:function(e,t){var a=this,s={},i="";a.query.startTime||e?a.query.endTime||t?(s["startTime"]=e||a.query.startTime.pattern("yyyy-MM-dd")+" 06:00",s["endTime"]=t||a.query.endTime.pattern("yyyy-MM-dd")+" 06:00",a.query.gameModeType=0,a.query.searchUsername&&(s["userKeyword"]=a.query.searchUsername),a.query.selectUserType&&(s["userType"]=a.query.selectUserType),0!=a.query.gameType&&(s["gameType"]=a.query.gameType),i="lottery"==a.activeGameType?"/api/Monito/SearchReport":"/api/Monito/SearchVideoReport",a.showLoading||(a.showLoading=!0,a.$axios({url:i,params:s}).then(function(e){if(100===e.data.Status){var t=e.data.Data;a.dataList=t.map(function(e){return{username:e.NickName,showId:e.OnlyCode,userType:1==e.UserStatus?"正常":"虚拟",gamaName:a.gameList.find(function(t){return t.type==e.GameType}).name,flowingWater:e.InputAmount,profitLoss:e.ProLoss,backType:e.SchemeName,backwater:e.Ascent,status:1==e.BackStatus?"已返水":2==e.BackStatus?"未返水":3==e.BackStatus?"可返水":"",mode:e.Pattern,reversible:e.Reversible}}),a.handleChangeMode()}a.showLoading=!1}))):a.$Message.success("请选择结束时间"):a.$Message.success("请选择开始时间")},handleChangeMode:function(){var e=this;switch(e.query.gameModeType){case 0:e.showDataList=e.dataList;break;case 1:e.showDataList=e.dataList.filter(function(t){return t.mode===e.query.gameModeType});break;case 2:e.showDataList=e.dataList.filter(function(t){return t.mode===e.query.gameModeType});break}}}}),r=n,o=(a("3083"),a("6691")),c=Object(o["a"])(r,s,i,!1,null,"cb645a74",null);t["default"]=c.exports},cde0:function(e,t,a){"use strict";var s=a("b2f5"),i=a("2d43")(6),n="findIndex",r=!0;n in[]&&Array(1)[n](function(){r=!1}),s(s.P+s.F*r,"Array",{findIndex:function(e){return i(this,e,arguments.length>1?arguments[1]:void 0)}}),a("644a")(n)},e00b:function(e,t,a){},f691:function(e,t,a){var s=a("88dd"),i=a("b5b8"),n=a("8b37")("species");e.exports=function(e){var t;return i(e)&&(t=e.constructor,"function"!=typeof t||t!==Array&&!i(t.prototype)||(t=void 0),s(t)&&(t=t[n],null===t&&(t=void 0))),void 0===t?Array:t}},f763:function(e,t,a){for(var s=a("dac5"),i=a("cfc7"),n=a("e5ef"),r=a("3754"),o=a("743d"),c=a("14fc"),l=a("8b37"),u=l("iterator"),d=l("toStringTag"),y=c.Array,p={CSSRuleList:!0,CSSStyleDeclaration:!1,CSSValueList:!1,ClientRectList:!1,DOMRectList:!1,DOMStringList:!1,DOMTokenList:!0,DataTransferItemList:!1,FileList:!1,HTMLAllCollection:!1,HTMLCollection:!1,HTMLFormElement:!1,HTMLSelectElement:!1,MediaList:!0,MimeTypeArray:!1,NamedNodeMap:!1,NodeList:!0,PaintRequestList:!1,Plugin:!1,PluginArray:!1,SVGLengthList:!1,SVGNumberList:!1,SVGPathSegList:!1,SVGPointList:!1,SVGStringList:!1,SVGTransformList:!1,SourceBufferList:!1,StyleSheetList:!0,TextTrackCueList:!1,TextTrackList:!1,TouchList:!1},m=i(p),h=0;h<m.length;h++){var f,v=m[h],g=p[v],T=r[v],D=T&&T.prototype;if(D&&(D[u]||o(D,u,y),D[d]||o(D,d,v),c[v]=y,g))for(f in s)D[f]||n(D,f,s[f],!0)}}}]);
//# sourceMappingURL=chunk-4a28ebb0.1588757178371.js.map
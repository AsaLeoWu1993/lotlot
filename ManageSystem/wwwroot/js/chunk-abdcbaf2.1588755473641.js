(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-abdcbaf2"],{"119c":function(t,a,e){"use strict";var i=e("b6f1");t.exports=function(t,a){return!!t&&i(function(){a?t.call(null,function(){},1):t.call(null)})}},"2d43":function(t,a,e){var i=e("01f5"),s=e("6462"),n=e("db4b"),o=e("b146"),r=e("5824");t.exports=function(t,a){var e=1==t,l=2==t,c=3==t,d=4==t,f=6==t,u=5==t||f,h=a||r;return function(a,r,v){for(var g,p,m=n(a),w=s(m),C=i(r,v,3),k=o(w.length),L=0,b=e?h(a,k):l?h(a,0):void 0;k>L;L++)if((u||L in w)&&(g=w[L],p=C(g,L,m),t))if(e)b[L]=p;else if(p)switch(t){case 3:return!0;case 5:return g;case 6:return L;case 2:b.push(g)}else if(d)return!1;return f?-1:c||d?d:b}}},4921:function(t,a,e){"use strict";e.r(a);var i=function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"agent"},[e("div",{staticClass:"content"},[e("div",{staticClass:"list-header"},[e("div",{staticClass:"list-title"},[e("span",[t._v("代理列表")]),e("div",{staticClass:"list-operate"},[e("i-input",{staticClass:"columns-item-input",attrs:{placeholder:"用户ID/用户名"},model:{value:t.searchKey,callback:function(a){t.searchKey=a},expression:"searchKey"}}),e("div",{staticClass:"search",on:{click:t.getData}},[t._v("搜索")]),t._m(0)],1)])]),e("div",{staticClass:"list-data"},[t._m(1),e("div",{staticClass:"data-con"},t._l(t.agentData,function(a,i){return e("div",{key:i,staticClass:"data-item"},[e("div",{staticClass:"item-info"},[e("img",{attrs:{src:a.header,alt:"",title:""}})]),e("div",{staticClass:"item-info"},[t._v(t._s(a.username))]),e("div",{staticClass:"item-info"},[t._v(t._s(a.showId))]),e("div",{staticClass:"item-info"},[t._v(t._s(a.balance))]),e("div",{staticClass:"item-info"},[t._v(t._s(a.roomNum))]),e("div",{staticClass:"item-info operate"},[e("div",{staticClass:"item-button",on:{click:function(e){return t.handleQueryOffline(a)}}},[t._v("下线管理")]),e("div",{staticClass:"item-button",on:{click:function(e){return t.handleAddOffline(a)}}},[t._v("添加下线")]),e("div",{staticClass:"item-button",on:{click:function(e){return t.handleShowCancel(a,"agent")}}},[t._v("取消代理")]),e("div",{staticClass:"item-button",on:{click:function(e){return t.handleModifyBackwater(a)}}},[t._v("回水管理")])])])}),0)]),e("Page",{staticClass:"footer-page",attrs:{total:t.page.total,"page-size":t.page.pageSize,current:t.page.pageNum,"show-sizer":""},on:{"on-page-size-change":t.handleChangeSize,"on-change":t.handleChangePage}})],1),e("div",{directives:[{name:"show",rawName:"v-show",value:t.backwaterShow||t.addOfflineShow||t.offlineListShow,expression:"backwaterShow || addOfflineShow || offlineListShow"}],staticClass:"agent-mask"},[e("div",{directives:[{name:"show",rawName:"v-show",value:t.backwaterShow,expression:"backwaterShow"}],staticClass:"agent-back-water"},[e("div",{staticClass:"back-water-header"},[e("span",[t._v("设置代理回水比例")]),e("span",{staticClass:"modal-close",on:{click:function(a){return t.handleCloseModal("backwaterShow")}}},[t._v("X")])]),e("div",{staticClass:"back-water-list"},[t._l(t.gameList,function(a,i){return e("div",{key:i,staticClass:"back-water-item"},[e("p",{staticClass:"back-water-item-h"},[t._v(t._s(a.title+"回水"))]),e("InputNumber",{staticClass:"columns-item-input",attrs:{max:100,min:0},model:{value:a.value,callback:function(e){t.$set(a,"value",e)},expression:"item.value"}}),e("span",[t._v("%")])],1)}),e("div",{staticClass:"back-water-item"},[e("i-button",{on:{click:t.handleSubmit}},[t._v("提交修改")])],1)],2)]),e("div",{directives:[{name:"show",rawName:"v-show",value:t.addOfflineShow,expression:"addOfflineShow"}],staticClass:"add-offline"},[e("div",{staticClass:"add-offline-header"},[e("span",[t._v("添加下线")]),e("span",{staticClass:"modal-close",on:{click:function(a){return t.handleCloseModal("addOfflineShow")}}},[t._v("X")])]),e("div",{staticClass:"add-offline-list"},[e("div",{staticClass:"add-offline-item"},[e("p",{staticClass:"add-offline-item-h"},[t._v("登录账号")]),e("Select",{staticClass:"columns-item-input",attrs:{filterable:""},model:{value:t.addOffline.username,callback:function(a){t.$set(t.addOffline,"username",a)},expression:"addOffline.username"}},t._l(t.showUserList,function(a,i){return e("Option",{key:i,attrs:{value:a.id}},[t._v(t._s(a.nickName))])}),1)],1),e("div",{staticClass:"add-offline-item"},[e("i-button",{on:{click:t.handleAddOfflineSub}},[t._v("添加")])],1)])]),e("div",{directives:[{name:"show",rawName:"v-show",value:t.offlineListShow,expression:"offlineListShow"}],staticClass:"offline-list"},[e("div",{staticClass:"offline-list-header"},[e("span",[t._v("下线列表")]),e("span",{staticClass:"modal-close",on:{click:function(a){return t.handleCloseModal("offlineListShow")}}},[t._v("X")])]),e("div",{staticClass:"offline-list-table"},[e("i-table",{attrs:{columns:t.offlineListCols,data:t.offlineListData,height:"370"}})],1)])]),e("Modal",{attrs:{styles:{top:"200px"},width:"300"},model:{value:t.showDelete,callback:function(a){t.showDelete=a},expression:"showDelete"}},[e("p",{staticStyle:{color:"#0d1941","font-size":"16px","text-align":"center"},attrs:{slot:"header"},slot:"header"},[e("span",[t._v("确认取消")])]),e("p",{staticStyle:{"text-align":"center"}},[t._v("是否确认取消？")]),e("div",{staticClass:"modal-footer",attrs:{slot:"footer"},slot:"footer"},[e("div",{staticClass:"enter-cancel",on:{click:function(a){t.showDelete=!1}}},[t._v("取消")]),e("div",{staticClass:"enter-ok",on:{click:t.handleEnterCancel}},[t._v("确定")])])])],1)},s=[function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"poptip"},[e("p",[t._v("1.玩家输入房号的时候填入代理推荐码则自动绑定上下级关系")])])},function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"data-header"},[e("p",{staticClass:"item-info"},[t._v("头像")]),e("p",{staticClass:"item-info"},[t._v("用户名")]),e("p",{staticClass:"item-info"},[t._v("ID")]),e("p",{staticClass:"item-info"},[t._v("账户余额")]),e("p",{staticClass:"item-info"},[e("span",[t._v("专属房间号")]),e("span",[t._v("（代理推荐码）")])]),e("p",{staticClass:"item-info"},[t._v("操作")])])}],n=(e("608b"),e("f763"),e("b745"),{data:function(){var t=this;return{page:{total:0,pageSize:10,pageNum:1},showLoading:!1,searchKey:"",backwaterShow:!1,addOfflineShow:!1,offlineListShow:!1,showDelete:!1,addOffline:{username:""},gameList:[],offlineListCols:[{title:"用户信息",align:"center",key:"userInfo"},{title:"操作",align:"center",render:function(a,e){return a("div",[a("Button",{props:{type:"primary",size:"small"},style:{marginRight:"5px"},on:{click:function(){t.handleShowCancel(e.row,"offline")}}},"取消下线")])}}],offlineListData:[],backwaterId:"",backwater:{id:"",userId:"",type:""},agentData:[],isLoading:!1,userList:[],showUserList:[]}},mounted:function(){var t=this;t.getGameList().then(function(){t.getData(),t.getUserList()})},methods:{getGameList:function(){var t=this;return new Promise(function(a,e){t.$axios({url:"/api/Merchant/GetGameList"}).then(function(i){if(100===i.data.Status){var s=i.data.Data;t.gameList=s.map(function(t){return{title:t.GameName,type:t.GameType,key:t.NickName,value:0}}).sort(function(t,a){return t.type-a.type}),a()}e()})})},handleChangeSize:function(t){var a=this;a.page.pageSize=t,a.page.pageNum=1,a.getData()},handleChangePage:function(t){var a=this;a.page.pageNum=t,a.getData()},handleEnterCancel:function(){var t=this;switch(t.backwater.type){case"agent":t.handleCancelAgent();break;case"offline":t.handleCancelOffline();break}},handleShowCancel:function(t,a){var e=this;e.backwater.id=t.id,e.backwater.userId=t.userId,e.backwater.type=a,e.showDelete=!0},handleCancelAgent:function(){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/User/AvatarOperation",params:{userID:t.backwater.userId}}).then(function(a){100===a.data.Status?t.$Message.success(a.data.Message):t.$Message.error(a.data.Message),t.isLoading=!1,t.showDelete=!1,t.page.total%t.page.pageSize===1&&1!==t.page.total&&(t.page.pageNum=parseInt(t.page.total/t.page.pageSize)),1===t.page.total&&(t.page.pageNum=1),t.getData()}))},handleModifyBackwater:function(t){var a=this;a.backwaterShow=!0,a.getBackwaterById(t.id)},getBackwaterById:function(t){var a=this;a.$axios({url:"/api/AgentBackwater/GetAgentInfo",params:{id:t}}).then(function(t){if(100===t.data.Status){var e=t.data.Model;a.backwaterId=e.ID,a.gameList.forEach(function(t){t.value=e[t.key]})}})},handleSubmit:function(){var t=this,a={};t.isLoading||(t.isLoading=!0,a["ID"]=t.backwaterId,t.gameList.forEach(function(t){a[t.key]=t.value}),t.$axios({url:"/api/AgentBackwater/UpdateAgentInfo",method:"post",data:a}).then(function(a){100===a.data.Status?t.$Message.success(a.data.Message):t.$Message.error(a.data.Message),t.isLoading=!1,t.backwaterShow=!1,t.getData()}))},handleAddOffline:function(t){var a=this;a.backwater.id=t.id,a.showUserList=a.userList.filter(function(a){return a.id!==t.userId}),a.addOfflineShow=!0},handleAddOfflineSub:function(){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/AgentBackwater/AddOfflineUser",method:"post",params:{id:t.backwater.id,userID:t.addOffline.username}}).then(function(a){100===a.data.Status?t.$Message.success(a.data.Message):t.$Message.error(a.data.Message),t.isLoading=!1,t.addOfflineShow=!1,t.getData()}))},handleQueryOffline:function(t){var a=this;a.backwater.id=t.id,a.getOfflineList(),a.offlineListShow=!0},getOfflineList:function(){var t=this;t.$axios({url:"/api/AgentBackwater/GetOfflineUsers",params:{id:t.backwater.id}}).then(function(a){if(100===a.data.Status){var e=a.data.Data;t.offlineListData=e.map(function(a){var e=t.userList.find(function(t){return t.id===a.UserID});return{id:a.ID,userId:a.UserID,userInfo:a.NickName+(e?"("+e.onlyCode+")":"")}})}})},handleCloseModal:function(t){var a=this;a[t]=!1},handleCancelOffline:function(t){var a=this;a.isLoading||(a.isLoading=!0,a.$axios({url:"/api/AgentBackwater/DeleteOfflineUser",method:"post",params:{id:a.backwater.id,userID:a.backwater.userId}}).then(function(t){100===t.data.Status?a.$Message.success(t.data.Message):a.$Message.error(t.data.Message),a.isLoading=!1,a.showDelete=!1,a.offlineListShow=!1,a.getData()}))},getData:function(){var t=this,a={};t.searchKey&&(a["keyword"]=t.searchKey),a["start"]=t.page.pageNum,a["pageSize"]=t.page.pageSize,t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/AgentBackwater/GetAgentUserList",params:a}).then(function(a){if(100===a.data.Status){var e=a.data.Data;t.page.total=a.data.Total,t.agentData=e.map(function(t){return{id:t.ID,header:t.Avatar?-1===t.Avatar.indexOf("://")?"/"+t.Avatar:t.Avatar:"",username:t.NickName,loginName:t.LoginName,balance:t.UserMoney,racingRatio:t.Racing+"%",airshipRatio:t.Airship+"%",timeHonoredRatio:t.TimeHonored+"%",speedRacerRatio:t.ExtremeSpeed+"%",austrTenRatio:t.Aus10+"%",austrFiveRatio:t.Aus5+"%",roomNum:t.RoomNum,userId:t.UserID,isAgent:t.IsAgent,showId:t.OnlyCode}})}else t.$Message.error(a.data.Message);t.isLoading=!1}))},getUserList:function(){var t=this;t.$axios({url:"/api/Setup/GetUpDownUserList",params:{status:!1}}).then(function(a){if(100===a.data.Status){var e=a.data.Data;t.userList=e.map(function(t){return{id:t.UserID,onlyCode:t.OnlyCode,nickName:t.NickName+"("+t.OnlyCode+")"}})}t.isLoading=!1})}}}),o=n,r=(e("d8c7"),e("6691")),l=Object(r["a"])(o,i,s,!1,null,"1bc72777",null);a["default"]=l.exports},5824:function(t,a,e){var i=e("f691");t.exports=function(t,a){return new(i(t))(a)}},6052:function(t,a,e){},"608b":function(t,a,e){"use strict";var i=e("b2f5"),s=e("2d43")(5),n="find",o=!0;n in[]&&Array(1)[n](function(){o=!1}),i(i.P+i.F*o,"Array",{find:function(t){return s(this,t,arguments.length>1?arguments[1]:void 0)}}),e("644a")(n)},b5b8:function(t,a,e){var i=e("94ac");t.exports=Array.isArray||function(t){return"Array"==i(t)}},b745:function(t,a,e){"use strict";var i=e("b2f5"),s=e("648a"),n=e("db4b"),o=e("b6f1"),r=[].sort,l=[1,2,3];i(i.P+i.F*(o(function(){l.sort(void 0)})||!o(function(){l.sort(null)})||!e("119c")(r)),"Array",{sort:function(t){return void 0===t?r.call(n(this)):r.call(n(this),s(t))}})},d8c7:function(t,a,e){"use strict";var i=e("6052"),s=e.n(i);s.a},f691:function(t,a,e){var i=e("88dd"),s=e("b5b8"),n=e("8b37")("species");t.exports=function(t){var a;return s(t)&&(a=t.constructor,"function"!=typeof a||a!==Array&&!s(a.prototype)||(a=void 0),i(a)&&(a=a[n],null===a&&(a=void 0))),void 0===a?Array:a}},f763:function(t,a,e){for(var i=e("dac5"),s=e("cfc7"),n=e("e5ef"),o=e("3754"),r=e("743d"),l=e("14fc"),c=e("8b37"),d=c("iterator"),f=c("toStringTag"),u=l.Array,h={CSSRuleList:!0,CSSStyleDeclaration:!1,CSSValueList:!1,ClientRectList:!1,DOMRectList:!1,DOMStringList:!1,DOMTokenList:!0,DataTransferItemList:!1,FileList:!1,HTMLAllCollection:!1,HTMLCollection:!1,HTMLFormElement:!1,HTMLSelectElement:!1,MediaList:!0,MimeTypeArray:!1,NamedNodeMap:!1,NodeList:!0,PaintRequestList:!1,Plugin:!1,PluginArray:!1,SVGLengthList:!1,SVGNumberList:!1,SVGPathSegList:!1,SVGPointList:!1,SVGStringList:!1,SVGTransformList:!1,SourceBufferList:!1,StyleSheetList:!0,TextTrackCueList:!1,TextTrackList:!1,TouchList:!1},v=s(h),g=0;g<v.length;g++){var p,m=v[g],w=h[m],C=o[m],k=C&&C.prototype;if(k&&(k[d]||r(k,d,u),k[f]||r(k,f,m),l[m]=u,w))for(p in i)k[p]||n(k,p,i[p],!0)}}}]);
//# sourceMappingURL=chunk-abdcbaf2.1588755473641.js.map
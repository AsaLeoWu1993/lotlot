(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-6d0552ea"],{"013f":function(a,t,e){var i=e("4ce5"),s=e("224c"),n=e("008a"),o=e("eafa"),r=e("5dd2");a.exports=function(a,t){var e=1==a,l=2==a,c=3==a,d=4==a,u=6==a,f=5==a||u,h=t||r;return function(t,r,v){for(var g,p,m=n(t),w=s(m),C=i(r,v,3),k=o(w.length),L=0,S=e?h(t,k):l?h(t,0):void 0;k>L;L++)if((f||L in w)&&(g=w[L],p=C(g,L,m),a))if(e)S[L]=p;else if(p)switch(a){case 3:return!0;case 5:return g;case 6:return L;case 2:S.push(g)}else if(d)return!1;return u?-1:c||d?d:S}}},2346:function(a,t,e){var i=e("75c4");a.exports=Array.isArray||function(a){return"Array"==i(a)}},4921:function(a,t,e){"use strict";e.r(t);var i=function(){var a=this,t=a.$createElement,e=a._self._c||t;return e("div",{staticClass:"agent"},[e("div",{staticClass:"content"},[e("div",{staticClass:"list-header"},[e("div",{staticClass:"list-title"},[e("span",[a._v("代理列表")]),e("div",{staticClass:"list-operate"},[e("i-input",{staticClass:"columns-item-input",attrs:{placeholder:"用户ID/用户名"},model:{value:a.searchKey,callback:function(t){a.searchKey=t},expression:"searchKey"}}),e("div",{staticClass:"search",on:{click:a.getData}},[a._v("搜索")]),a._m(0)],1)])]),e("div",{staticClass:"list-data"},[a._m(1),e("div",{staticClass:"data-con"},a._l(a.agentData,(function(t,i){return e("div",{key:i,staticClass:"data-item"},[e("div",{staticClass:"item-info"},[e("img",{attrs:{src:t.header,alt:"",title:""}})]),e("div",{staticClass:"item-info"},[a._v(a._s(t.username))]),e("div",{staticClass:"item-info"},[a._v(a._s(t.showId))]),e("div",{staticClass:"item-info"},[a._v(a._s(t.balance))]),e("div",{staticClass:"item-info"},[a._v(a._s(t.roomNum))]),e("div",{staticClass:"item-info operate"},[e("div",{staticClass:"item-button",on:{click:function(e){return a.handleQueryOffline(t)}}},[a._v("下线管理")]),e("div",{staticClass:"item-button",on:{click:function(e){return a.handleAddOffline(t)}}},[a._v("添加下线")]),e("div",{staticClass:"item-button",on:{click:function(e){return a.handleShowCancel(t,"agent")}}},[a._v("取消代理")]),e("div",{staticClass:"item-button",on:{click:function(e){return a.handleModifyBackwater(t)}}},[a._v("回水管理")])])])})),0)]),e("Page",{staticClass:"footer-page",attrs:{total:a.page.total,"page-size":a.page.pageSize,current:a.page.pageNum,"show-sizer":""},on:{"on-page-size-change":a.handleChangeSize,"on-change":a.handleChangePage}})],1),e("div",{directives:[{name:"show",rawName:"v-show",value:a.backwaterShow||a.addOfflineShow||a.offlineListShow,expression:"backwaterShow || addOfflineShow || offlineListShow"}],staticClass:"agent-mask"},[e("div",{directives:[{name:"show",rawName:"v-show",value:a.backwaterShow,expression:"backwaterShow"}],staticClass:"agent-back-water"},[e("div",{staticClass:"back-water-header"},[e("span",[a._v("设置代理回水比例")]),e("span",{staticClass:"modal-close",on:{click:function(t){return a.handleCloseModal("backwaterShow")}}},[a._v("X")])]),e("div",{staticClass:"back-water-list"},[a._l(a.gameList,(function(t,i){return e("div",{key:i,staticClass:"back-water-item"},[e("p",{staticClass:"back-water-item-h"},[a._v(a._s(t.title+"回水"))]),e("InputNumber",{staticClass:"columns-item-input",attrs:{max:100,min:0},model:{value:t.value,callback:function(e){a.$set(t,"value",e)},expression:"item.value"}}),e("span",[a._v("%")])],1)})),e("div",{staticClass:"back-water-item"},[e("i-button",{on:{click:a.handleSubmit}},[a._v("提交修改")])],1)],2)]),e("div",{directives:[{name:"show",rawName:"v-show",value:a.addOfflineShow,expression:"addOfflineShow"}],staticClass:"add-offline"},[e("div",{staticClass:"add-offline-header"},[e("span",[a._v("添加下线")]),e("span",{staticClass:"modal-close",on:{click:function(t){return a.handleCloseModal("addOfflineShow")}}},[a._v("X")])]),e("div",{staticClass:"add-offline-list"},[e("div",{staticClass:"add-offline-item"},[e("p",{staticClass:"add-offline-item-h"},[a._v("登录账号")]),e("Select",{staticClass:"columns-item-input",attrs:{filterable:""},model:{value:a.addOffline.username,callback:function(t){a.$set(a.addOffline,"username",t)},expression:"addOffline.username"}},a._l(a.showUserList,(function(t,i){return e("Option",{key:i,attrs:{value:t.id}},[a._v(a._s(t.nickName))])})),1)],1),e("div",{staticClass:"add-offline-item"},[e("i-button",{on:{click:a.handleAddOfflineSub}},[a._v("添加")])],1)])]),e("div",{directives:[{name:"show",rawName:"v-show",value:a.offlineListShow,expression:"offlineListShow"}],staticClass:"offline-list"},[e("div",{staticClass:"offline-list-header"},[e("span",[a._v("下线列表")]),e("span",{staticClass:"modal-close",on:{click:function(t){return a.handleCloseModal("offlineListShow")}}},[a._v("X")])]),e("div",{staticClass:"offline-list-table"},[e("i-table",{attrs:{columns:a.offlineListCols,data:a.offlineListData,height:"370"}})],1)])]),e("Modal",{attrs:{styles:{top:"200px"},width:"300"},model:{value:a.showDelete,callback:function(t){a.showDelete=t},expression:"showDelete"}},[e("p",{staticStyle:{color:"#0d1941","font-size":"16px","text-align":"center"},attrs:{slot:"header"},slot:"header"},[e("span",[a._v("确认取消")])]),e("p",{staticStyle:{"text-align":"center"}},[a._v("是否确认取消？")]),e("div",{staticClass:"modal-footer",attrs:{slot:"footer"},slot:"footer"},[e("div",{staticClass:"enter-cancel",on:{click:function(t){a.showDelete=!1}}},[a._v("取消")]),e("div",{staticClass:"enter-ok",on:{click:a.handleEnterCancel}},[a._v("确定")])])])],1)},s=[function(){var a=this,t=a.$createElement,e=a._self._c||t;return e("div",{staticClass:"poptip"},[e("p",[a._v("1.玩家输入房号的时候填入代理推荐码则自动绑定上下级关系")])])},function(){var a=this,t=a.$createElement,e=a._self._c||t;return e("div",{staticClass:"data-header"},[e("p",{staticClass:"item-info"},[a._v("头像")]),e("p",{staticClass:"item-info"},[a._v("用户名")]),e("p",{staticClass:"item-info"},[a._v("ID")]),e("p",{staticClass:"item-info"},[a._v("账户余额")]),e("p",{staticClass:"item-info"},[e("span",[a._v("专属房间号")]),e("span",[a._v("（代理推荐码）")])]),e("p",{staticClass:"item-info"},[a._v("操作")])])}],n=(e("e697"),e("6d57"),e("c904"),{data:function(){var a=this;return{page:{total:0,pageSize:10,pageNum:1},showLoading:!1,searchKey:"",backwaterShow:!1,addOfflineShow:!1,offlineListShow:!1,showDelete:!1,addOffline:{username:""},gameList:[],offlineListCols:[{title:"用户信息",align:"center",key:"userInfo"},{title:"操作",align:"center",render:function(t,e){return t("div",[t("Button",{props:{type:"primary",size:"small"},style:{marginRight:"5px"},on:{click:function(){a.handleShowCancel(e.row,"offline")}}},"取消下线")])}}],offlineListData:[],backwaterId:"",backwater:{id:"",userId:"",type:""},agentData:[],isLoading:!1,userList:[],showUserList:[]}},mounted:function(){var a=this;a.getGameList().then((function(){a.getData(),a.getUserList()}))},methods:{getGameList:function(){var a=this;return new Promise((function(t,e){a.$axios({url:"/api/Merchant/GetGameList"}).then((function(i){if(100===i.data.Status){var s=i.data.Data;a.gameList=s.map((function(a){return{title:a.GameName,type:a.GameType,key:a.NickName,value:0}})).sort((function(a,t){return a.type-t.type})),t()}e()}))}))},handleChangeSize:function(a){var t=this;t.page.pageSize=a,t.page.pageNum=1,t.getData()},handleChangePage:function(a){var t=this;t.page.pageNum=a,t.getData()},handleEnterCancel:function(){var a=this;switch(a.backwater.type){case"agent":a.handleCancelAgent();break;case"offline":a.handleCancelOffline();break}},handleShowCancel:function(a,t){var e=this;e.backwater.id=a.id,e.backwater.userId=a.userId,e.backwater.type=t,e.showDelete=!0},handleCancelAgent:function(){var a=this;a.isLoading||(a.isLoading=!0,a.$axios({url:"/api/User/AvatarOperation",params:{userID:a.backwater.userId}}).then((function(t){100===t.data.Status?a.$Message.success(t.data.Message):a.$Message.error(t.data.Message),a.isLoading=!1,a.showDelete=!1,a.page.total%a.page.pageSize===1&&1!==a.page.total&&(a.page.pageNum=parseInt(a.page.total/a.page.pageSize)),1===a.page.total&&(a.page.pageNum=1),a.getData()})))},handleModifyBackwater:function(a){var t=this;t.backwaterShow=!0,t.getBackwaterById(a.id)},getBackwaterById:function(a){var t=this;t.$axios({url:"/api/AgentBackwater/GetAgentInfo",params:{id:a}}).then((function(a){if(100===a.data.Status){var e=a.data.Model;t.backwaterId=e.ID,t.gameList.forEach((function(a){a.value=e[a.key]}))}}))},handleSubmit:function(){var a=this,t={};a.isLoading||(a.isLoading=!0,t["ID"]=a.backwaterId,a.gameList.forEach((function(a){t[a.key]=a.value})),a.$axios({url:"/api/AgentBackwater/UpdateAgentInfo",method:"post",data:t}).then((function(t){100===t.data.Status?a.$Message.success(t.data.Message):a.$Message.error(t.data.Message),a.isLoading=!1,a.backwaterShow=!1,a.getData()})))},handleAddOffline:function(a){var t=this;t.backwater.id=a.id,t.showUserList=t.userList.filter((function(t){return t.id!==a.userId})),t.addOfflineShow=!0},handleAddOfflineSub:function(){var a=this;a.isLoading||(a.isLoading=!0,a.$axios({url:"/api/AgentBackwater/AddOfflineUser",method:"post",params:{id:a.backwater.id,userID:a.addOffline.username}}).then((function(t){100===t.data.Status?a.$Message.success(t.data.Message):a.$Message.error(t.data.Message),a.isLoading=!1,a.addOfflineShow=!1,a.getData()})))},handleQueryOffline:function(a){var t=this;t.backwater.id=a.id,t.getOfflineList(),t.offlineListShow=!0},getOfflineList:function(){var a=this;a.$axios({url:"/api/AgentBackwater/GetOfflineUsers",params:{id:a.backwater.id}}).then((function(t){if(100===t.data.Status){var e=t.data.Data;a.offlineListData=e.map((function(t){var e=a.userList.find((function(a){return a.id===t.UserID}));return{id:t.ID,userId:t.UserID,userInfo:t.NickName+(e?"("+e.onlyCode+")":"")}}))}}))},handleCloseModal:function(a){var t=this;t[a]=!1},handleCancelOffline:function(a){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/AgentBackwater/DeleteOfflineUser",method:"post",params:{id:t.backwater.id,userID:t.backwater.userId}}).then((function(a){100===a.data.Status?t.$Message.success(a.data.Message):t.$Message.error(a.data.Message),t.isLoading=!1,t.showDelete=!1,t.offlineListShow=!1,t.getData()})))},getData:function(){var a=this,t={};a.searchKey&&(t["keyword"]=a.searchKey),t["start"]=a.page.pageNum,t["pageSize"]=a.page.pageSize,a.isLoading||(a.isLoading=!0,a.$axios({url:"/api/AgentBackwater/GetAgentUserList",params:t}).then((function(t){if(100===t.data.Status){var e=t.data.Data;a.page.total=t.data.Total,a.agentData=e.map((function(a){return{id:a.ID,header:a.Avatar?-1===a.Avatar.indexOf("://")?"/"+a.Avatar:a.Avatar:"",username:a.NickName,loginName:a.LoginName,balance:a.UserMoney,racingRatio:a.Racing+"%",airshipRatio:a.Airship+"%",timeHonoredRatio:a.TimeHonored+"%",speedRacerRatio:a.ExtremeSpeed+"%",austrTenRatio:a.Aus10+"%",austrFiveRatio:a.Aus5+"%",roomNum:a.RoomNum,userId:a.UserID,isAgent:a.IsAgent,showId:a.OnlyCode}}))}else a.$Message.error(t.data.Message);a.isLoading=!1})))},getUserList:function(){var a=this;a.$axios({url:"/api/Setup/GetUpDownUserList",params:{status:!1}}).then((function(t){if(100===t.data.Status){var e=t.data.Data;a.userList=e.map((function(a){return{id:a.UserID,onlyCode:a.OnlyCode,nickName:a.NickName+"("+a.OnlyCode+")"}}))}a.isLoading=!1}))}}}),o=n,r=(e("d8c7"),e("4023")),l=Object(r["a"])(o,i,s,!1,null,"1bc72777",null);t["default"]=l.exports},"5dd2":function(a,t,e){var i=e("81dc");a.exports=function(a,t){return new(i(a))(t)}},"6d57":function(a,t,e){for(var i=e("e44b"),s=e("80a9"),n=e("bf16"),o=e("e7ad"),r=e("86d4"),l=e("da6d"),c=e("cb3d"),d=c("iterator"),u=c("toStringTag"),f=l.Array,h={CSSRuleList:!0,CSSStyleDeclaration:!1,CSSValueList:!1,ClientRectList:!1,DOMRectList:!1,DOMStringList:!1,DOMTokenList:!0,DataTransferItemList:!1,FileList:!1,HTMLAllCollection:!1,HTMLCollection:!1,HTMLFormElement:!1,HTMLSelectElement:!1,MediaList:!0,MimeTypeArray:!1,NamedNodeMap:!1,NodeList:!0,PaintRequestList:!1,Plugin:!1,PluginArray:!1,SVGLengthList:!1,SVGNumberList:!1,SVGPathSegList:!1,SVGPointList:!1,SVGStringList:!1,SVGTransformList:!1,SourceBufferList:!1,StyleSheetList:!0,TextTrackCueList:!1,TextTrackList:!1,TouchList:!1},v=s(h),g=0;g<v.length;g++){var p,m=v[g],w=h[m],C=o[m],k=C&&C.prototype;if(k&&(k[d]||r(k,d,f),k[u]||r(k,u,m),l[m]=f,w))for(p in i)k[p]||n(k,p,i[p],!0)}},"81dc":function(a,t,e){var i=e("fb68"),s=e("2346"),n=e("cb3d")("species");a.exports=function(a){var t;return s(a)&&(t=a.constructor,"function"!=typeof t||t!==Array&&!s(t.prototype)||(t=void 0),i(t)&&(t=t[n],null===t&&(t=void 0))),void 0===t?Array:t}},a2cd:function(a,t,e){"use strict";var i=e("238a");a.exports=function(a,t){return!!a&&i((function(){t?a.call(null,(function(){}),1):a.call(null)}))}},a8f9:function(a,t,e){},c904:function(a,t,e){"use strict";var i=e("e46b"),s=e("5daa"),n=e("008a"),o=e("238a"),r=[].sort,l=[1,2,3];i(i.P+i.F*(o((function(){l.sort(void 0)}))||!o((function(){l.sort(null)}))||!e("a2cd")(r)),"Array",{sort:function(a){return void 0===a?r.call(n(this)):r.call(n(this),s(a))}})},d8c7:function(a,t,e){"use strict";var i=e("a8f9"),s=e.n(i);s.a},e697:function(a,t,e){"use strict";var i=e("e46b"),s=e("013f")(5),n="find",o=!0;n in[]&&Array(1)[n]((function(){o=!1})),i(i.P+i.F*o,"Array",{find:function(a){return s(this,a,arguments.length>1?arguments[1]:void 0)}}),e("0e8b")(n)}}]);
//# sourceMappingURL=chunk-6d0552ea.1589621121187.js.map
(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-039588e1"],{"119c":function(t,e,s){"use strict";var i=s("b6f1");t.exports=function(t,e){return!!t&&i(function(){e?t.call(null,function(){},1):t.call(null)})}},"2d43":function(t,e,s){var i=s("01f5"),a=s("6462"),n=s("db4b"),o=s("b146"),c=s("5824");t.exports=function(t,e){var s=1==t,r=2==t,l=3==t,d=4==t,u=6==t,f=5==t||u,v=e||c;return function(e,c,p){for(var m,h,g=n(e),C=a(g),L=i(c,p,3),S=o(C.length),w=0,y=s?v(e,S):r?v(e,0):void 0;S>w;w++)if((f||w in C)&&(m=C[w],h=L(m,w,g),t))if(s)y[w]=h;else if(h)switch(t){case 3:return!0;case 5:return m;case 6:return w;case 2:y.push(m)}else if(d)return!1;return u?-1:l||d?d:y}}},5824:function(t,e,s){var i=s("f691");t.exports=function(t,e){return new(i(t))(e)}},"873a":function(t,e,s){"use strict";var i=function(){var t=this,e=t.$createElement,s=t._self._c||e;return s("div",{class:["switch",t.size,t.open?t.size+"-open":t.size+"-close"],on:{click:t.handleSwitch}},[s("div",{staticClass:"switch-item"})])},a=[],n={props:{open:{type:Boolean,default:!1},size:{type:String,default:"small"}},methods:{handleSwitch:function(){var t=this;t.$emit("switch")}}},o=n,c=(s("e14d"),s("6691")),r=Object(c["a"])(o,i,a,!1,null,null,null);e["a"]=r.exports},a1d2:function(t,e,s){"use strict";var i=s("e256"),a=s.n(i);a.a},b5b8:function(t,e,s){var i=s("94ac");t.exports=Array.isArray||function(t){return"Array"==i(t)}},b745:function(t,e,s){"use strict";var i=s("b2f5"),a=s("648a"),n=s("db4b"),o=s("b6f1"),c=[].sort,r=[1,2,3];i(i.P+i.F*(o(function(){r.sort(void 0)})||!o(function(){r.sort(null)})||!s("119c")(c)),"Array",{sort:function(t){return void 0===t?c.call(n(this)):c.call(n(this),a(t))}})},b78b:function(t,e,s){"use strict";s.r(e);var i=function(){var t=this,e=t.$createElement,s=t._self._c||e;return s("div",{staticClass:"admin-info"},[s("div",{staticClass:"content"},[s("div",{staticClass:"list-header"},[s("div",{staticClass:"list-title"},[s("span",[t._v("群内管理员消息/弹窗推送")]),s("div",{staticClass:"list-operate"},[s("div",{staticClass:"send",on:{click:t.handleSubmit}},[s("span",[t._v("发送")])])])])]),s("div",{staticClass:"data-con"},[s("div",{staticClass:"data-list"},[s("div",{staticClass:"data-item"},[s("p",[t._v("消息类型")]),s("i-select",{staticClass:"item-select",model:{value:t.messageType,callback:function(e){t.messageType=e},expression:"messageType"}},t._l(t.messageList,function(e,i){return s("i-option",{key:i,attrs:{value:e.key}},[t._v(t._s(e.title))])}),1)],1),s("div",{directives:[{name:"show",rawName:"v-show",value:1==t.messageType,expression:"messageType == 1"}],staticClass:"data-item"},[s("p",[t._v("选择彩种")]),s("CheckboxGroup",{model:{value:t.selectedGame,callback:function(e){t.selectedGame=e},expression:"selectedGame"}},t._l(t.gameList,function(e,i){return s("Checkbox",{key:i,attrs:{label:e.type}},[s("span",[t._v(t._s(e.name))])])}),1)],1),s("div",{staticClass:"data-item"},[s("p",[t._v("消息内容")]),s("Input",{staticClass:"column-textarea",attrs:{type:"textarea",rows:5,autosize:{minRows:5,maxRows:5}},model:{value:t.sendText,callback:function(e){t.sendText=e},expression:"sendText"}})],1)])])]),s("div",{staticClass:"content"},[s("div",{staticClass:"list-header"},[s("div",{staticClass:"list-title"},[s("span",[t._v("滚动公告")]),s("div",{staticClass:"list-operate"},[s("div",{staticClass:"send",on:{click:function(e){return t.handleSend("1")}}},[s("span",[t._v("发布/修改")])])])])]),s("div",{staticClass:"data-con"},[s("div",{staticClass:"data-list"},[s("div",{staticClass:"data-item"},[s("p",[t._v("公告内容")]),s("Input",{staticClass:"column-textarea",attrs:{type:"textarea",rows:4,autosize:{minRows:4,maxRows:4}},model:{value:t.sendNotice,callback:function(e){t.sendNotice=e},expression:"sendNotice"}})],1)]),s("div",{staticClass:"data-list"},[t._m(0),t._m(1),t._l(t.noticeList,function(e,i){return s("div",{key:i,staticClass:"data-item"},[s("div",{staticClass:"item-info"},[s("switchs",{attrs:{open:e.status},on:{switch:function(s){return t.handleChangeStatus(e)}}})],1),s("div",{staticClass:"item-info"},[s("p",{attrs:{title:e.content}},[t._v(t._s(e.content))])]),s("div",{staticClass:"item-info"},[s("span",[t._v(t._s(e.time))])]),s("div",{staticClass:"item-info"},[s("div",{staticClass:"item-button",on:{click:function(s){return t.handleShowEdit(e)}}},[t._v("修改")]),s("div",{staticClass:"item-button",on:{click:function(s){return t.handleDelete(e)}}},[t._v("删除")])])])})],2)])]),s("div",{directives:[{name:"show",rawName:"v-show",value:t.editShow,expression:"editShow"}],staticClass:"article-mask"},[s("div",{directives:[{name:"show",rawName:"v-show",value:t.editShow,expression:"editShow"}],staticClass:"article-content"},[s("div",{staticClass:"article-header"},[s("span",[t._v("修改公告")]),s("span",{staticClass:"modal-close",on:{click:t.handleCloseModal}},[t._v("X")])]),s("div",{staticClass:"article-list"},[s("div",{staticClass:"article-item"},[s("p",{staticClass:"article-item-t"},[t._v("公告内容")]),s("i-input",{staticClass:"column-textarea",attrs:{type:"textarea",rows:8,autosize:{minRows:8,maxRows:8}},model:{value:t.editNotice,callback:function(e){t.editNotice=e},expression:"editNotice"}})],1),s("div",{staticClass:"article-item"},[s("i-button",{on:{click:function(e){return t.handleSend("2")}}},[t._v("提交")])],1)])])]),s("Modal",{attrs:{"class-name":"vertical-center-modal",width:"300"},model:{value:t.showDelete,callback:function(e){t.showDelete=e},expression:"showDelete"}},[s("p",{staticStyle:{color:"#0d1941","font-size":"16px","text-align":"center"},attrs:{slot:"header"},slot:"header"},[s("span",[t._v("确认删除")])]),s("p",{staticStyle:{"text-align":"center"}},[t._v("是否确认删除？")]),s("div",{staticClass:"modal-footer",attrs:{slot:"footer"},slot:"footer"},[s("div",{staticClass:"enter-cancel",on:{click:function(e){t.showDelete=!1}}},[t._v("取消")]),s("div",{staticClass:"enter-ok",on:{click:t.handleEnterDelete}},[t._v("确定")])])])],1)},a=[function(){var t=this,e=t.$createElement,s=t._self._c||e;return s("div",{staticClass:"data-item"},[s("p",[t._v("公告列表")])])},function(){var t=this,e=t.$createElement,s=t._self._c||e;return s("div",{staticClass:"data-item"},[s("p",{staticClass:"item-info"},[t._v("是否启用")]),s("p",{staticClass:"item-info"},[t._v("公告内容")]),s("p",{staticClass:"item-info"},[t._v("发布时间")]),s("p",{staticClass:"item-info"},[t._v("操作")])])}],n=(s("cde0"),s("f763"),s("b745"),s("873a")),o={components:{switchs:n["a"]},data:function(){return{showLoading:!1,id:"",showDelete:!1,editShow:!1,sendNotice:"",editNotice:"",noticeList:[],selectedGame:[],sendText:"",messageType:1,messageList:[{key:1,title:"群内管理员消息"},{key:2,title:"房间弹窗推送消息"}],gameList:[],isLoading:!1}},mounted:function(){var t=this;t.getGameList().then(function(){t.getData()})},methods:{getGameList:function(){var t=this;return new Promise(function(e,s){t.$axios({url:"/api/Merchant/GetGameList"}).then(function(i){if(100===i.data.Status){var a=i.data.Data;t.gameList=a.map(function(t){return{name:t.GameName,type:t.GameType,key:t.NickName}}).sort(function(t,e){return t.type-e.type}),e()}s()})})},handleDelete:function(t){var e=this;e.id=t.id,e.showDelete=!0},handleShowEdit:function(t){var e=this;e.editNotice=t.content,e.id=t.id,e.editShow=!0},handleChangeStatus:function(t){var e=this,s="";s=t.status?"/api/Setup/CloseArticle":"/api/Setup/OpenArticle",e.isLoading||(e.isLoading=!0,e.$axios({url:s,params:{articleID:t.id}}).then(function(t){e.isLoading=!1,100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.getData()}))},handleEnterDelete:function(){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/Setup/DeleteArticleByID",method:"post",params:{ID:t.id}}).then(function(e){100===e.data.Status?t.$Message.success(e.data.Message):t.$Message.error(e.data.Message),t.showDelete=!1,t.isLoading=!1,t.getData()}))},handleSend:function(t){var e=this,s={},i="",a={},n="";(e.sendNotice||"1"!==t)&&(e.editNotice||"2"!==t)?("1"===t?(i="/api/Setup/AddArticle",n="post",s["Content"]=e.sendNotice,s["ArticleType"]=1):(a["id"]=e.id,i="/api/Setup/UpdateArticle",n="post",s["Content"]=e.editNotice,s["ArticleType"]=1),e.isLoading||(e.isLoading=!0,e.$axios({url:i,method:n,data:s,params:a}).then(function(t){100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.sendNotice="",e.editNotice="",e.editShow=!1,e.isLoading=!1,e.getData()}))):e.$Message.error("请填写公告内容")},getData:function(){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/Setup/GetArticleList"}).then(function(e){if(100===e.data.Status){var s=e.data.Data;t.noticeList=s.map(function(t){return{id:t.ID,type:t.Type,content:t.Content,time:t.Time.substr(0,16),status:t.Open}})}t.isLoading=!1}))},handleCloseModal:function(){var t=this;t.editShow=!1,t.editNotice=""},handleSubmit:function(){var t=this,e={},s="";if(t.sendText.trim()){if(e["Content"]=t.sendText,1===t.messageType){if(!t.selectedGame.length)return void t.$Message.error("请选择游戏类型");t.gameList.forEach(function(s){e[s.key]=-1!==t.selectedGame.findIndex(function(t){return t===s.type})}),s="/api/Setup/AddMeaage"}else s="/api/Setup/AddArticle",e["ArticleType"]=2;t.isLoading||(t.isLoading=!0,t.$axios({url:s,method:"post",data:e}).then(function(e){100===e.data.Status?t.$Message.success(e.data.Message):t.$Message.error(e.data.Message),t.sendText="",t.selectedGame=[],t.isLoading=!1}))}else t.$Message.error("请输入发送内容")}}},c=o,r=(s("a1d2"),s("6691")),l=Object(r["a"])(c,i,a,!1,null,"123c54df",null);e["default"]=l.exports},c233:function(t,e,s){},cde0:function(t,e,s){"use strict";var i=s("b2f5"),a=s("2d43")(6),n="findIndex",o=!0;n in[]&&Array(1)[n](function(){o=!1}),i(i.P+i.F*o,"Array",{findIndex:function(t){return a(this,t,arguments.length>1?arguments[1]:void 0)}}),s("644a")(n)},e14d:function(t,e,s){"use strict";var i=s("c233"),a=s.n(i);a.a},e256:function(t,e,s){},f691:function(t,e,s){var i=s("88dd"),a=s("b5b8"),n=s("8b37")("species");t.exports=function(t){var e;return a(t)&&(e=t.constructor,"function"!=typeof e||e!==Array&&!a(e.prototype)||(e=void 0),i(e)&&(e=e[n],null===e&&(e=void 0))),void 0===e?Array:e}},f763:function(t,e,s){for(var i=s("dac5"),a=s("cfc7"),n=s("e5ef"),o=s("3754"),c=s("743d"),r=s("14fc"),l=s("8b37"),d=l("iterator"),u=l("toStringTag"),f=r.Array,v={CSSRuleList:!0,CSSStyleDeclaration:!1,CSSValueList:!1,ClientRectList:!1,DOMRectList:!1,DOMStringList:!1,DOMTokenList:!0,DataTransferItemList:!1,FileList:!1,HTMLAllCollection:!1,HTMLCollection:!1,HTMLFormElement:!1,HTMLSelectElement:!1,MediaList:!0,MimeTypeArray:!1,NamedNodeMap:!1,NodeList:!0,PaintRequestList:!1,Plugin:!1,PluginArray:!1,SVGLengthList:!1,SVGNumberList:!1,SVGPathSegList:!1,SVGPointList:!1,SVGStringList:!1,SVGTransformList:!1,SourceBufferList:!1,StyleSheetList:!0,TextTrackCueList:!1,TextTrackList:!1,TouchList:!1},p=a(v),m=0;m<p.length;m++){var h,g=p[m],C=v[g],L=o[g],S=L&&L.prototype;if(S&&(S[d]||c(S,d,f),S[u]||c(S,u,g),r[g]=f,C))for(h in i)S[h]||n(S,h,i[h],!0)}}}]);
//# sourceMappingURL=chunk-039588e1.1589421487082.js.map
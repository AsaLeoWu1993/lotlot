(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-1b5e51a3"],{"119c":function(e,t,a){"use strict";var i=a("b6f1");e.exports=function(e,t){return!!e&&i(function(){t?e.call(null,function(){},1):e.call(null)})}},2616:function(e,t,a){"use strict";a.r(t);var i=function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"base-setting"},[a("div",{staticClass:"content"},[a("div",{staticClass:"list-header"},[a("div",{staticClass:"list-title"},[a("span",[e._v("各游戏封盘时间设置（以开奖时间为参照）")]),a("div",{staticClass:"operate"},[a("span",{staticStyle:{"vertical-align":"middle"},on:{click:function(t){return e.handleOperate(1)}}},[e._v("修改/保存")])])])]),a("div",{staticClass:"list"},[e._m(0),e._l(e.gameList,function(t,i){return[t.gameType==e.selectGameTimeType?a("div",{key:i,staticClass:"list-item"},[a("div",[e._v(e._s(t.name))]),a("div",[a("InputNumber",{staticClass:"base-item-input",attrs:{max:120,min:1},model:{value:t.value,callback:function(a){e.$set(t,"value",a)},expression:"item.value"}})],1)]):e._e()]})],2)]),a("div",{staticClass:"content"},[a("div",{staticClass:"list-header"},[a("div",{staticClass:"list-title"},[a("span",[e._v("系统消息设置")]),a("div",{staticClass:"operate"},[a("div",{staticStyle:{display:"inline-block",position:"relative",width:"190px",height:"32px","vertical-align":"middle"}},[a("i-select",{staticStyle:{position:"absolute","margin-right":"10px",width:"180px"},on:{"on-change":e.handleChangeMesType},model:{value:e.selectGameMesType,callback:function(t){e.selectGameMesType=t},expression:"selectGameMesType"}},e._l(e.gameType,function(t,i){return a("i-option",{key:i,attrs:{value:t.value}},[e._v(e._s(t.title))])}),1)],1),a("span",{on:{click:function(t){return e.handleOperate(2)}}},[e._v("修改/保存")])])])]),a("div",{staticClass:"list-1"},[a("div",{staticClass:"base-list-item"},[a("div",{staticClass:"base-item-second"},[a("span",[e._v("封盘前")]),a("InputNumber",{staticClass:"base-item-input",attrs:{max:120,min:1},model:{value:e.beforeSealingSecond,callback:function(t){e.beforeSealingSecond=t},expression:"beforeSealingSecond"}}),a("span",[e._v("秒")]),a("Checkbox",{staticClass:"system-operate-item",model:{value:e.before50.show,callback:function(t){e.$set(e.before50,"show",t)},expression:"before50.show"}},[a("span",[e._v(e._s(e.before50.title))])])],1),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.beforeSealing,expression:"beforeSealing"}],domProps:{value:e.beforeSealing},on:{input:function(t){t.target.composing||(e.beforeSealing=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[e._m(1),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.afterSealing1,expression:"afterSealing1"}],domProps:{value:e.afterSealing1},on:{input:function(t){t.target.composing||(e.afterSealing1=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[a("div",{staticClass:"base-item-second"},[a("span",[e._v("封盘后")]),a("InputNumber",{staticClass:"base-item-input",attrs:{max:120,min:1},model:{value:e.afterSealing2Second,callback:function(t){e.afterSealing2Second=t},expression:"afterSealing2Second"}}),a("span",[e._v("秒自定义")])],1),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.afterSealing2,expression:"afterSealing2"}],domProps:{value:e.afterSealing2},on:{input:function(t){t.target.composing||(e.afterSealing2=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[a("div",{staticClass:"base-item-second space-between"},[a("span",[e._v("中奖明细")]),a("div",{staticClass:"space-right"},[a("div",{staticClass:"right-item",on:{click:function(t){e.editShow=!0}}},[e._v("{结算时中奖格式}")]),a("div",{staticClass:"right-item",on:{click:function(t){e.editShowNot=!0}}},[e._v("{结算时未中奖格式}")])])]),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.winningDetails,expression:"winningDetails"}],domProps:{value:e.winningDetails},on:{input:function(t){t.target.composing||(e.winningDetails=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[a("div",{staticClass:"base-item-second space-between"},[a("span",[e._v("会员积分")]),a("Checkbox",{staticClass:"system-operate-item",model:{value:e.showBillTable,callback:function(t){e.showBillTable=t},expression:"showBillTable"}},[a("span",[e._v("使用表单显示余额")])])],1),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.membershipScore,expression:"membershipScore"}],domProps:{value:e.membershipScore},on:{input:function(t){t.target.composing||(e.membershipScore=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[a("p",{staticClass:"base-item-second"},[e._v("封盘提示")]),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.sealingTips,expression:"sealingTips"}],domProps:{value:e.sealingTips},on:{input:function(t){t.target.composing||(e.sealingTips=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[a("p",{staticClass:"base-item-second"},[e._v("系统消息可用变量说明")]),e._l(e.poptips,function(t,i){return a("div",{key:i,staticClass:"poptip-item"},[a("span",{domProps:{textContent:e._s(t.title)}}),a("span",{domProps:{textContent:e._s(t.desc)}})])})],2)])]),a("div",{directives:[{name:"show",rawName:"v-show",value:e.editShow||e.editShowNot,expression:"editShow || editShowNot"}],staticClass:"article-mask"},[a("div",{directives:[{name:"show",rawName:"v-show",value:e.editShow||e.editShowNot,expression:"editShow || editShowNot"}],staticClass:"article-content"},[a("div",{staticClass:"article-header"},[a("span",[e._v("{结算} 格式内容设置")]),a("span",{staticClass:"modal-close",on:{click:function(t){e.editShow=!1,e.editShowNot=!1}}},[e._v("X")])]),a("div",{staticClass:"article-list"},[e.editShow?a("i-input",{staticClass:"column-textarea",attrs:{type:"textarea",rows:4,autosize:{minRows:4,maxRows:4}},model:{value:e.Settlement,callback:function(t){e.Settlement=t},expression:"Settlement"}}):e._e(),e.editShowNot?a("i-input",{staticClass:"column-textarea",attrs:{type:"textarea",rows:4,autosize:{minRows:4,maxRows:4}},model:{value:e.NotSettlement,callback:function(t){e.NotSettlement=t},expression:"NotSettlement"}}):e._e(),a("p",{staticStyle:{padding:"4px 0"},domProps:{textContent:e._s(e.editShowNot?"支持变量： {玩家}{当期盈亏}":"支持变量： {玩家}{当期盈亏}{中奖详细}")}}),a("div",{staticClass:"article-item",on:{click:function(t){return e.handleSave(e.editShow?"3":"4")}}},[e._v("确定")])],1)])]),a("loading",{attrs:{loading:e.isLoading}}),a("div",{staticStyle:{margin:"20px 0"}},[a("router-view")],1)],1)},s=[function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"list-item"},[a("div",[e._v("彩种")]),a("div",[e._v("距开奖*秒")])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"base-item-second"},[a("span",[e._v("封盘信息核对")])])}],n=(a("608b"),a("7364"),a("f763"),a("b745"),{data:function(){return{poptips:[{title:"封盘前提示可用变量",desc:"{期号}{游戏}"},{title:"封盘核对可用变量",desc:"{期号}{游戏}{玩家数量}{玩家总分}{核对}"},{title:"中奖明细可用变量",desc:"{期号}{游戏}{开奖信息}{在线人数}{玩家数量}{玩家总分}{当期期号}{结算记录}{中奖记录}{中奖}{结算}"},{title:"会员积分可用变量",desc:"{微群名字}{在线人数}{玩家数量}{玩家总分}{账单内容}"},{title:"封盘后自定义消息",desc:"无可用变量"}],selectGameTimeType:"lottery",selectGameMesType:"lottery",editShow:!1,editShowNot:!1,gameType:[{value:"lottery",title:"彩票游戏设置"},{value:"video",title:"视讯游戏设置"}],gameList:[],sealingTips:"",winningDetails:"",membershipScore:"",beforeSealing:"",beforeSealingSecond:0,afterSealing1:"",afterSealing1Second:0,afterSealing2:"",afterSealing2Second:0,Settlement:"",NotSettlement:"",isLoading:!1,showBillTable:!1,id:"",before50:{show:!1,title:"禁止撤单"}}},mounted:function(){var e=this;e.getGameList().then(function(){e.getData()})},methods:{handleChangeTimeType:function(e){console.log(e)},handleChangeMesType:function(e){var t=this;console.log(e,t.selectGameMesType),t.getData()},getGameList:function(){var e=this;return new Promise(function(t,a){e.$axios({url:"/api/Merchant/GetGameList"}).then(function(i){if(100===i.data.Status){var s=i.data.Data;e.gameList=s.map(function(e){return{name:e.GameName,type:e.GameType,key:e.NickName,value:0,gameType:"lottery"}}).sort(function(e,t){return e.type-t.type}),e.gameList.push({name:"百家乐",type:"1",key:"bjl",value:0,gameType:"video"}),t()}a()})})},handleOperate:function(e){var t=this;switch(e){case 1:t.handleSave(e);break;case 2:t.handleSave(e);break}},handleSave:function(e){var t=this,a=!1,i={},s="";t.afterSealing2||2!==e?t.afterSealing2Second||2!==e?t.afterSealing1||2!==e?t.beforeSealing||2!==e?t.beforeSealingSecond||2!==e?t.sealingTips||2!==e?t.winningDetails||2!==e?t.membershipScore||2!==e?t.Settlement?t.NotSettlement?(t.gameList.forEach(function(i){i.value||1!==e||a||(a=!0,t.$Message.error("请填写".concat(i.name,"封盘时间")))}),a||(i["ID"]=t.id,i["CustomMsg"]=t.afterSealing2,i["CustomTime"]=t.afterSealing2Second,i["EntertainedAfterMsg"]=t.afterSealing1,i["EntertainedFrontMsg"]=t.beforeSealing,i["EntertainedFrontTime"]=t.beforeSealingSecond,i["LotteryFrontMsg"]=t.sealingTips,i["MembershipScore"]=t.membershipScore,i["WinningDetails"]=t.winningDetails,i["ProhibitChe"]=t.before50.show,i["ShowBillTable"]=t.showBillTable,i["Settlement"]=t.Settlement,i["NotSettlement"]=t.NotSettlement,1==e&&(i["LotteryFrontTime"]=[],t.gameList.forEach(function(e){i["LotteryFrontTime"].push({Type:e.type,LotteryTime:e.value})})),t.isLoading||(t.isLoading=!0,s="lottery"==t.selectGameMesType?"/api/Merchant/UpdateSetupInfo":"/api/Merchant/UpdateVideoSetupInfo",t.$axios({url:s,method:"put",data:i}).then(function(e){t.isLoading=!1,t.editShow=!1,t.editShowNot=!1,100===e.data.Status?(t.$Message.success(e.data.Message),t.getData()):t.$Message.error(e.data.Message)})))):t.$Message.error("请填写{未中奖格式}内容"):t.$Message.error("请填写{中奖格式}内容"):t.$Message.error("请填写会员积分消息"):t.$Message.error("请填写中奖提示消息"):t.$Message.error("请填写封盘提示"):t.$Message.error("请填写自定义封盘前时间"):t.$Message.error("请填写自定义封盘前消息"):t.$Message.error("请填写封盘后核对消息"):t.$Message.error("请填写自定义封盘后时间"):t.$Message.error("请填写自定义封盘后消息")},getData:function(){var e=this,t="";e.isLoading||(e.isLoading=!0,t="lottery"==e.selectGameMesType?"/api/Merchant/GetSetupInfo":"/api/Merchant/GetVideoSetupInfo",e.$axios({url:t}).then(function(t){if(100===t.data.Status){var a=t.data.Model;e.afterSealing2=a.CustomMsg,e.afterSealing2Second=a.CustomTime,e.Settlement=a.Settlement,e.NotSettlement=a.NotSettlement,e.afterSealing1=a.EntertainedAfterMsg,e.beforeSealing=a.EntertainedFrontMsg,e.beforeSealingSecond=a.EntertainedFrontTime,e.sealingTips=a.LotteryFrontMsg,e.winningDetails=a.WinningDetails,e.membershipScore=a.MembershipScore,e.before50.show=a.ProhibitChe,e.showBillTable=a.ShowBillTable,a.LotteryFrontTime&&e.gameList.forEach(function(e){var t=a.LotteryFrontTime.find(function(t){return t.Type===e.type});t&&(e.value=t.LotteryTime)}),e.id=a.ID,e.canEdit1=!1,e.canEdit2=!1}e.isLoading=!1}))}}}),o=n,r=(a("89b4"),a("6691")),l=Object(r["a"])(o,i,s,!1,null,"aa3294be",null);t["default"]=l.exports},"2d43":function(e,t,a){var i=a("01f5"),s=a("6462"),n=a("db4b"),o=a("b146"),r=a("5824");e.exports=function(e,t){var a=1==e,l=2==e,c=3==e,d=4==e,u=6==e,m=5==e||u,p=t||r;return function(t,r,f){for(var v,g,S=n(t),h=s(S),b=i(r,f,3),w=o(h.length),y=0,C=a?p(t,w):l?p(t,0):void 0;w>y;y++)if((m||y in h)&&(v=h[y],g=b(v,y,S),e))if(a)C[y]=g;else if(g)switch(e){case 3:return!0;case 5:return v;case 6:return y;case 2:C.push(v)}else if(d)return!1;return u?-1:c||d?d:C}}},5824:function(e,t,a){var i=a("f691");e.exports=function(e,t){return new(i(e))(t)}},"608b":function(e,t,a){"use strict";var i=a("b2f5"),s=a("2d43")(5),n="find",o=!0;n in[]&&Array(1)[n](function(){o=!1}),i(i.P+i.F*o,"Array",{find:function(e){return s(this,e,arguments.length>1?arguments[1]:void 0)}}),a("644a")(n)},7364:function(e,t,a){var i=a("ddf7").f,s=Function.prototype,n=/^\s*function ([^ (]*)/,o="name";o in s||a("dad2")&&i(s,o,{configurable:!0,get:function(){try{return(""+this).match(n)[1]}catch(e){return""}}})},"89b4":function(e,t,a){"use strict";var i=a("b8a7"),s=a.n(i);s.a},b5b8:function(e,t,a){var i=a("94ac");e.exports=Array.isArray||function(e){return"Array"==i(e)}},b745:function(e,t,a){"use strict";var i=a("b2f5"),s=a("648a"),n=a("db4b"),o=a("b6f1"),r=[].sort,l=[1,2,3];i(i.P+i.F*(o(function(){l.sort(void 0)})||!o(function(){l.sort(null)})||!a("119c")(r)),"Array",{sort:function(e){return void 0===e?r.call(n(this)):r.call(n(this),s(e))}})},b8a7:function(e,t,a){},f691:function(e,t,a){var i=a("88dd"),s=a("b5b8"),n=a("8b37")("species");e.exports=function(e){var t;return s(e)&&(t=e.constructor,"function"!=typeof t||t!==Array&&!s(t.prototype)||(t=void 0),i(t)&&(t=t[n],null===t&&(t=void 0))),void 0===t?Array:t}},f763:function(e,t,a){for(var i=a("dac5"),s=a("cfc7"),n=a("e5ef"),o=a("3754"),r=a("743d"),l=a("14fc"),c=a("8b37"),d=c("iterator"),u=c("toStringTag"),m=l.Array,p={CSSRuleList:!0,CSSStyleDeclaration:!1,CSSValueList:!1,ClientRectList:!1,DOMRectList:!1,DOMStringList:!1,DOMTokenList:!0,DataTransferItemList:!1,FileList:!1,HTMLAllCollection:!1,HTMLCollection:!1,HTMLFormElement:!1,HTMLSelectElement:!1,MediaList:!0,MimeTypeArray:!1,NamedNodeMap:!1,NodeList:!0,PaintRequestList:!1,Plugin:!1,PluginArray:!1,SVGLengthList:!1,SVGNumberList:!1,SVGPathSegList:!1,SVGPointList:!1,SVGStringList:!1,SVGTransformList:!1,SourceBufferList:!1,StyleSheetList:!0,TextTrackCueList:!1,TextTrackList:!1,TouchList:!1},f=s(p),v=0;v<f.length;v++){var g,S=f[v],h=p[S],b=o[S],w=b&&b.prototype;if(w&&(w[d]||r(w,d,m),w[u]||r(w,u,S),l[S]=m,h))for(g in i)w[g]||n(w,g,i[g],!0)}}}]);
//# sourceMappingURL=chunk-1b5e51a3.1588140222220.js.map
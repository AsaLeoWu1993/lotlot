(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-4b2914c9"],{"013f":function(e,t,a){var i=a("4ce5"),n=a("224c"),s=a("008a"),o=a("eafa"),r=a("5dd2");e.exports=function(e,t){var a=1==e,l=2==e,c=3==e,u=4==e,d=6==e,m=5==e||d,f=t||r;return function(t,r,p){for(var v,g,S=s(t),h=n(S),b=i(r,p,3),w=o(h.length),y=0,T=a?f(t,w):l?f(t,0):void 0;w>y;y++)if((m||y in h)&&(v=h[y],g=b(v,y,S),e))if(a)T[y]=g;else if(g)switch(e){case 3:return!0;case 5:return v;case 6:return y;case 2:T.push(v)}else if(u)return!1;return d?-1:c||u?u:T}}},2346:function(e,t,a){var i=a("75c4");e.exports=Array.isArray||function(e){return"Array"==i(e)}},2616:function(e,t,a){"use strict";a.r(t);var i=function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"base-setting"},[a("div",{staticClass:"content"},[a("div",{staticClass:"list-header"},[a("div",{staticClass:"list-title"},[a("span",[e._v("各游戏封盘时间设置（以开奖时间为参照）")]),a("div",{staticClass:"operate"},[a("div",{staticClass:"button",on:{click:e.handleFlushNow}},[e._v("立即刷新")]),a("span",{staticStyle:{"vertical-align":"middle"},on:{click:function(t){return e.handleOperate(1)}}},[e._v("修改/保存")])])])]),a("div",{staticClass:"list"},[e._m(0),e._l(e.gameList,(function(t,i){return[t.gameType==e.selectGameTimeType?a("div",{key:i,staticClass:"list-item"},[a("div",[e._v(e._s(t.name))]),a("div",[a("InputNumber",{staticClass:"base-item-input",attrs:{max:0==i||2==i?600:1==i||4==i||5==i||6==i||7==i||8==i?200:120,min:1,"active-change":!1},model:{value:t.value,callback:function(a){e.$set(t,"value",a)},expression:"item.value"}})],1),a("div",{staticStyle:{display:"flex","align-items":"center","justify-content":"center","background-color":"#e4e4e4"},domProps:{textContent:e._s(t.countDownText)}})]):e._e()]}))],2)]),a("div",{staticClass:"content"},[a("div",{staticClass:"list-header"},[a("div",{staticClass:"list-title"},[a("span",[e._v("系统消息设置")]),a("div",{staticClass:"operate"},[a("span",{on:{click:function(t){return e.handleOperate(2)}}},[e._v("修改/保存")])])])]),a("div",{staticClass:"list-1"},[a("div",{staticClass:"base-list-item"},[a("div",{staticClass:"base-item-second"},[a("span",[e._v("封盘前")]),a("InputNumber",{staticClass:"base-item-input",attrs:{max:120,min:1},model:{value:e.beforeSealingSecond,callback:function(t){e.beforeSealingSecond=t},expression:"beforeSealingSecond"}}),a("span",[e._v("秒")]),a("Checkbox",{staticClass:"system-operate-item",model:{value:e.before50.show,callback:function(t){e.$set(e.before50,"show",t)},expression:"before50.show"}},[a("span",[e._v(e._s(e.before50.title))])])],1),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.beforeSealing,expression:"beforeSealing"}],domProps:{value:e.beforeSealing},on:{input:function(t){t.target.composing||(e.beforeSealing=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[e._m(1),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.afterSealing1,expression:"afterSealing1"}],domProps:{value:e.afterSealing1},on:{input:function(t){t.target.composing||(e.afterSealing1=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[a("div",{staticClass:"base-item-second"},[a("span",[e._v("封盘后")]),a("InputNumber",{staticClass:"base-item-input",attrs:{max:120,min:1},model:{value:e.afterSealing2Second,callback:function(t){e.afterSealing2Second=t},expression:"afterSealing2Second"}}),a("span",[e._v("秒自定义")])],1),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.afterSealing2,expression:"afterSealing2"}],domProps:{value:e.afterSealing2},on:{input:function(t){t.target.composing||(e.afterSealing2=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[a("div",{staticClass:"base-item-second space-between"},[a("span",[e._v("中奖明细")]),a("div",{staticClass:"space-right"},[a("div",{staticClass:"right-item",on:{click:function(t){e.editShow=!0}}},[e._v("{结算时中奖格式}")]),a("div",{staticClass:"right-item",on:{click:function(t){e.editShowNot=!0}}},[e._v("{结算时未中奖格式}")])])]),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.winningDetails,expression:"winningDetails"}],domProps:{value:e.winningDetails},on:{input:function(t){t.target.composing||(e.winningDetails=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[a("div",{staticClass:"base-item-second space-between"},[a("span",[e._v("会员积分")]),a("Checkbox",{staticClass:"system-operate-item",model:{value:e.showBillTable,callback:function(t){e.showBillTable=t},expression:"showBillTable"}},[a("span",[e._v("使用表单显示余额")])])],1),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.membershipScore,expression:"membershipScore"}],domProps:{value:e.membershipScore},on:{input:function(t){t.target.composing||(e.membershipScore=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[a("p",{staticClass:"base-item-second"},[e._v("封盘提示")]),a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.sealingTips,expression:"sealingTips"}],domProps:{value:e.sealingTips},on:{input:function(t){t.target.composing||(e.sealingTips=t.target.value)}}})]),a("div",{staticClass:"base-list-item"},[a("p",{staticClass:"base-item-second"},[e._v("系统消息可用变量说明")]),e._l(e.poptips,(function(t,i){return a("div",{key:i,staticClass:"poptip-item"},[a("span",{domProps:{textContent:e._s(t.title)}}),a("span",{domProps:{textContent:e._s(t.desc)}})])}))],2)])]),a("div",{directives:[{name:"show",rawName:"v-show",value:e.editShow||e.editShowNot,expression:"editShow || editShowNot"}],staticClass:"article-mask"},[a("div",{directives:[{name:"show",rawName:"v-show",value:e.editShow||e.editShowNot,expression:"editShow || editShowNot"}],staticClass:"article-content"},[a("div",{staticClass:"article-header"},[a("span",[e._v("{结算} 格式内容设置")]),a("span",{staticClass:"modal-close",on:{click:function(t){e.editShow=!1,e.editShowNot=!1}}},[e._v("X")])]),a("div",{staticClass:"article-list"},[e.editShow?a("i-input",{staticClass:"column-textarea",attrs:{type:"textarea",rows:4,autosize:{minRows:4,maxRows:4}},model:{value:e.Settlement,callback:function(t){e.Settlement=t},expression:"Settlement"}}):e._e(),e.editShowNot?a("i-input",{staticClass:"column-textarea",attrs:{type:"textarea",rows:4,autosize:{minRows:4,maxRows:4}},model:{value:e.NotSettlement,callback:function(t){e.NotSettlement=t},expression:"NotSettlement"}}):e._e(),a("p",{staticStyle:{padding:"4px 0"},domProps:{textContent:e._s(e.editShowNot?"支持变量： {玩家}{当期盈亏}":"支持变量： {玩家}{当期盈亏}{中奖详细}")}}),a("div",{staticClass:"article-item",on:{click:function(t){return e.handleSave(e.editShow?"3":"4")}}},[e._v("确定")])],1)])]),a("loading",{attrs:{loading:e.isLoading}}),a("div",{staticStyle:{margin:"20px 0"}},[a("router-view")],1)],1)},n=[function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"list-item"},[a("div",[e._v("彩种")]),a("div",[e._v("距开奖*秒")]),a("div",{staticStyle:{"background-color":"#e4e4e4","font-size":"13px","word-break":"keep-all"}},[e._v("当前封盘倒计时")])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"base-item-second"},[a("span",[e._v("封盘信息核对")])])}],s=(a("e697"),a("cc57"),a("c904"),a("e204"),a("6d57"),a("702a"),{data:function(){return{poptips:[{title:"封盘前提示可用变量",desc:"{期号}{游戏}"},{title:"封盘核对可用变量",desc:"{期号}{游戏}{玩家数量}{玩家总分}{核对}"},{title:"中奖明细可用变量",desc:"{期号}{游戏}{开奖信息}{在线人数}{玩家数量}{玩家总分}{当期期号}{结算记录}{中奖记录}{中奖}{结算}"},{title:"会员积分可用变量",desc:"{微群名字}{在线人数}{玩家数量}{玩家总分}{账单内容}"},{title:"封盘后自定义消息",desc:"无可用变量"}],selectGameTimeType:"lottery",selectGameMesType:"lottery",editShow:!1,editShowNot:!1,gameType:[{value:"lottery",title:"彩票游戏设置"},{value:"video",title:"视讯游戏设置"}],gameList:[],sealingTips:"",winningDetails:"",membershipScore:"",beforeSealing:"",beforeSealingSecond:0,afterSealing1:"",afterSealing1Second:0,afterSealing2:"",afterSealing2Second:0,Settlement:"",NotSettlement:"",isLoading:!1,showBillTable:!1,id:"",before50:{show:!1,title:"禁止撤单"},interval2:null,startTime:0,endTime:0,lastTime:0}},mounted:function(){var e=this;e.getGameList().then((function(){e.handleGetGameInfo(),e.getData()})),e.interval2||(e.interval2=setInterval((function(){e.countDown()}),1e3-(e.endTime-e.startTime)))},methods:{handleFlushNow:function(){var e=this,t=+new Date;t-e.lastTime>5e3?e.handleGetGameInfo().then((function(){e.$Message.success("刷新成功")})):e.$Message.error("刷新间隔需要大于5秒")},handleChangeTimeType:function(e){console.log(e)},countDown:function(){var e=this;e.startTime=+new Date,e.gameList.forEach((function(e){e.countDown>0?(e.countDown--,e.countDownText=("00"+parseInt(e.countDown/60)).slice(-2)+":"+("00"+(e.countDown%60).toFixed(0)).slice(-2)):e.countDownText="0"})),e.endTime=+new Date},handleChangeMesType:function(e){var t=this;console.log(e,t.selectGameMesType),t.getData()},handleGetGameInfo:function(){var e=this;return new Promise((function(t,a){e.$axios({url:"/api/Merchant/GetGameInfos"}).then((function(a){var i=a.data.Data;i.forEach((function(t){var a=e.gameList.findIndex((function(e){return e.type==t.GameType&&"lottery"==e.gameType})),i=e.gameList[a];e.gameList.splice(a,1,Object.assign({},i,{countDown:t.Surplus}))})),e.lastTime=+new Date,t()}))}))},getGameList:function(){var e=this;return new Promise((function(t,a){e.$axios({url:"/api/Merchant/GetGameList"}).then((function(i){if(100===i.data.Status){var n=i.data.Data;e.gameList=n.map((function(e){return{name:e.GameName,type:e.GameType,key:e.NickName,value:0,gameType:"lottery",countDown:0,countDownText:""}})).sort((function(e,t){return e.type-t.type})),t()}a()}))}))},handleOperate:function(e){var t=this;switch(e){case 1:t.handleSave(e);break;case 2:t.handleSave(e);break}},handleSave:function(e){var t=this,a=!1,i={},n="";t.afterSealing2||2!==e?t.afterSealing2Second||2!==e?t.afterSealing1||2!==e?t.beforeSealing||2!==e?t.beforeSealingSecond||2!==e?t.sealingTips||2!==e?t.winningDetails||2!==e?t.membershipScore||2!==e?t.Settlement?t.NotSettlement?(t.gameList.forEach((function(i){i.value||1!==e||a||"lottery"!=i.gameType||(a=!0,t.$Message.error("请填写".concat(i.name,"封盘时间")))})),a||(i["ID"]=t.id,i["CustomMsg"]=t.afterSealing2,i["CustomTime"]=t.afterSealing2Second,i["EntertainedAfterMsg"]=t.afterSealing1,i["EntertainedFrontMsg"]=t.beforeSealing,i["EntertainedFrontTime"]=t.beforeSealingSecond,i["LotteryFrontMsg"]=t.sealingTips,i["MembershipScore"]=t.membershipScore,i["WinningDetails"]=t.winningDetails,i["ProhibitChe"]=t.before50.show,i["ShowBillTable"]=t.showBillTable,i["Settlement"]=t.Settlement,i["NotSettlement"]=t.NotSettlement,i["LotteryFrontTime"]=[],t.gameList.forEach((function(e){"bjl"!=e.key&&i["LotteryFrontTime"].push({Type:e.type,LotteryTime:e.value})})),t.isLoading||(t.isLoading=!0,n="lottery"==t.selectGameMesType?"/api/Merchant/UpdateSetupInfo":"/api/Merchant/UpdateVideoSetupInfo",t.$axios({url:n,method:"post",data:i}).then((function(e){t.isLoading=!1,t.editShow=!1,t.editShowNot=!1,100===e.data.Status?(t.$Message.success(e.data.Message),t.getData()):t.$Message.error(e.data.Message)}))))):t.$Message.error("请填写{未中奖格式}内容"):t.$Message.error("请填写{中奖格式}内容"):t.$Message.error("请填写会员积分消息"):t.$Message.error("请填写中奖提示消息"):t.$Message.error("请填写封盘提示"):t.$Message.error("请填写自定义封盘前时间"):t.$Message.error("请填写自定义封盘前消息"):t.$Message.error("请填写封盘后核对消息"):t.$Message.error("请填写自定义封盘后时间"):t.$Message.error("请填写自定义封盘后消息")},getData:function(){var e=this,t="";e.isLoading||(e.isLoading=!0,t="lottery"==e.selectGameMesType?"/api/Merchant/GetSetupInfo":"/api/Merchant/GetVideoSetupInfo",e.$axios({url:t}).then((function(t){if(100===t.data.Status){var a=t.data.Model;e.afterSealing2=a.CustomMsg,e.afterSealing2Second=a.CustomTime,e.Settlement=a.Settlement,e.NotSettlement=a.NotSettlement,e.afterSealing1=a.EntertainedAfterMsg,e.beforeSealing=a.EntertainedFrontMsg,e.beforeSealingSecond=a.EntertainedFrontTime,e.sealingTips=a.LotteryFrontMsg,e.winningDetails=a.WinningDetails,e.membershipScore=a.MembershipScore,e.before50.show=a.ProhibitChe,e.showBillTable=a.ShowBillTable,a.LotteryFrontTime&&e.gameList.forEach((function(e){var t=a.LotteryFrontTime.find((function(t){return t.Type==e.type}));t&&(e.value=t.LotteryTime)})),e.id=a.ID,e.canEdit1=!1,e.canEdit2=!1}e.isLoading=!1})))}}}),o=s,r=(a("52ec"),a("4023")),l=Object(r["a"])(o,i,n,!1,null,"56d47ba8",null);t["default"]=l.exports},"52ec":function(e,t,a){"use strict";var i=a("e9a3"),n=a.n(i);n.a},"5dd2":function(e,t,a){var i=a("81dc");e.exports=function(e,t){return new(i(e))(t)}},"6d57":function(e,t,a){for(var i=a("e44b"),n=a("80a9"),s=a("bf16"),o=a("e7ad"),r=a("86d4"),l=a("da6d"),c=a("cb3d"),u=c("iterator"),d=c("toStringTag"),m=l.Array,f={CSSRuleList:!0,CSSStyleDeclaration:!1,CSSValueList:!1,ClientRectList:!1,DOMRectList:!1,DOMStringList:!1,DOMTokenList:!0,DataTransferItemList:!1,FileList:!1,HTMLAllCollection:!1,HTMLCollection:!1,HTMLFormElement:!1,HTMLSelectElement:!1,MediaList:!0,MimeTypeArray:!1,NamedNodeMap:!1,NodeList:!0,PaintRequestList:!1,Plugin:!1,PluginArray:!1,SVGLengthList:!1,SVGNumberList:!1,SVGPathSegList:!1,SVGPointList:!1,SVGStringList:!1,SVGTransformList:!1,SourceBufferList:!1,StyleSheetList:!0,TextTrackCueList:!1,TextTrackList:!1,TouchList:!1},p=n(f),v=0;v<p.length;v++){var g,S=p[v],h=f[S],b=o[S],w=b&&b.prototype;if(w&&(w[u]||r(w,u,m),w[d]||r(w,d,S),l[S]=m,h))for(g in i)w[g]||s(w,g,i[g],!0)}},"81dc":function(e,t,a){var i=a("fb68"),n=a("2346"),s=a("cb3d")("species");e.exports=function(e){var t;return n(e)&&(t=e.constructor,"function"!=typeof t||t!==Array&&!n(t.prototype)||(t=void 0),i(t)&&(t=t[s],null===t&&(t=void 0))),void 0===t?Array:t}},a2cd:function(e,t,a){"use strict";var i=a("238a");e.exports=function(e,t){return!!e&&i((function(){t?e.call(null,(function(){}),1):e.call(null)}))}},c904:function(e,t,a){"use strict";var i=a("e46b"),n=a("5daa"),s=a("008a"),o=a("238a"),r=[].sort,l=[1,2,3];i(i.P+i.F*(o((function(){l.sort(void 0)}))||!o((function(){l.sort(null)}))||!a("a2cd")(r)),"Array",{sort:function(e){return void 0===e?r.call(s(this)):r.call(s(this),n(e))}})},cc57:function(e,t,a){var i=a("064e").f,n=Function.prototype,s=/^\s*function ([^ (]*)/,o="name";o in n||a("149f")&&i(n,o,{configurable:!0,get:function(){try{return(""+this).match(s)[1]}catch(e){return""}}})},e204:function(e,t,a){"use strict";var i=a("e46b"),n=a("013f")(6),s="findIndex",o=!0;s in[]&&Array(1)[s]((function(){o=!1})),i(i.P+i.F*o,"Array",{findIndex:function(e){return n(this,e,arguments.length>1?arguments[1]:void 0)}}),a("0e8b")(s)},e697:function(e,t,a){"use strict";var i=a("e46b"),n=a("013f")(5),s="find",o=!0;s in[]&&Array(1)[s]((function(){o=!1})),i(i.P+i.F*o,"Array",{find:function(e){return n(this,e,arguments.length>1?arguments[1]:void 0)}}),a("0e8b")(s)},e9a3:function(e,t,a){}}]);
//# sourceMappingURL=chunk-4b2914c9.1589626175857.js.map
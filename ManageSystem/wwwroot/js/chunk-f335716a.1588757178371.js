(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-f335716a"],{"26f0":function(e,t,s){},"2d43":function(e,t,s){var a=s("01f5"),i=s("6462"),n=s("db4b"),l=s("b146"),c=s("5824");e.exports=function(e,t){var s=1==e,r=2==e,o=3==e,u=4==e,p=6==e,m=5==e||p,y=t||c;return function(t,c,d){for(var v,f,R=n(t),C=i(R),g=a(c,d,3),h=l(C.length),k=0,b=s?y(t,h):r?y(t,0):void 0;h>k;k++)if((m||k in C)&&(v=C[k],f=g(v,k,R),e))if(s)b[k]=f;else if(f)switch(e){case 3:return!0;case 5:return v;case 6:return k;case 2:b.push(v)}else if(u)return!1;return p?-1:o||u?u:b}}},5824:function(e,t,s){var a=s("f691");e.exports=function(e,t){return new(a(e))(t)}},"769f":function(e,t,s){"use strict";var a=s("26f0"),i=s.n(a);i.a},ae77:function(e,t,s){"use strict";s.r(t);var a=function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("div",{staticClass:"game-setting"},[s("div",{staticClass:"content"},[s("div",{staticClass:"list-header"},[s("div",{staticClass:"list-title"},[s("span",[e._v("回复消息设置")]),s("div",{staticClass:"operate"},[s("span",{on:{click:function(t){return e.handleOperate(1)}}},[e._v("修改/保存")])])])]),s("div",{staticClass:"list"},[s("div",{staticClass:"base-list-item"},[s("p",{staticClass:"list-item-title"},[e._v("游戏成功时回复")]),s("textarea",{directives:[{name:"model",rawName:"v-model",value:e.gameSuccReply,expression:"gameSuccReply"}],domProps:{value:e.gameSuccReply},on:{input:function(t){t.target.composing||(e.gameSuccReply=t.target.value)}}})]),s("div",{staticClass:"base-list-item"},[s("p",{staticClass:"list-item-title"},[e._v("查分时回复")]),s("textarea",{directives:[{name:"model",rawName:"v-model",value:e.queryIntegReply,expression:"queryIntegReply"}],domProps:{value:e.queryIntegReply},on:{input:function(t){t.target.composing||(e.queryIntegReply=t.target.value)}}})]),s("div",{staticClass:"base-list-item"},[s("p",{staticClass:"list-item-title"},[e._v("查流水统计时回复")]),s("textarea",{directives:[{name:"model",rawName:"v-model",value:e.queryCountReply,expression:"queryCountReply"}],domProps:{value:e.queryCountReply},on:{input:function(t){t.target.composing||(e.queryCountReply=t.target.value)}}})]),s("div",{staticClass:"base-list-item"},[s("p",{staticClass:"list-item-title"},[e._v("调整余额时回复")]),s("textarea",{directives:[{name:"model",rawName:"v-model",value:e.changeBalanceReply,expression:"changeBalanceReply"}],domProps:{value:e.changeBalanceReply},on:{input:function(t){t.target.composing||(e.changeBalanceReply=t.target.value)}}})]),s("div",{staticClass:"base-list-item"},[s("p",{staticClass:"list-item-title"},[e._v("自动返水时回复")]),s("textarea",{directives:[{name:"model",rawName:"v-model",value:e.autoReturnReply,expression:"autoReturnReply"}],domProps:{value:e.autoReturnReply},on:{input:function(t){t.target.composing||(e.autoReturnReply=t.target.value)}}})]),e._m(0),s("div",{staticClass:"list-item"},[s("div",[s("CheckboxGroup",{staticClass:"system-operate-list",model:{value:e.systemOperateCheck,callback:function(t){e.systemOperateCheck=t},expression:"systemOperateCheck"}},e._l(e.systemOperate,function(t,a){return s("Checkbox",{key:a,staticClass:"system-operate-item",attrs:{label:t.label}},[s("span",[e._v(e._s(t.title))])])}),1)],1),s("div",{staticClass:"system-reply-list"},[s("div",{staticClass:"system-reply-item"},[s("span",[e._v("取消订单时回复")]),s("i-input",{staticClass:"columns-item-input",model:{value:e.systemReply.cancelOrder,callback:function(t){e.$set(e.systemReply,"cancelOrder",t)},expression:"systemReply.cancelOrder"}})],1),s("div",{staticClass:"system-reply-item"},[s("span",[e._v("收到请求时回复")]),s("i-input",{staticClass:"columns-item-input",model:{value:e.systemReply.receiveReq,callback:function(t){e.$set(e.systemReply,"receiveReq",t)},expression:"systemReply.receiveReq"}})],1),s("div",{staticClass:"system-reply-item"},[s("span",[e._v("禁止取消时回复")]),s("i-input",{staticClass:"columns-item-input",model:{value:e.systemReply.notCancelOrder,callback:function(t){e.$set(e.systemReply,"notCancelOrder",t)},expression:"systemReply.notCancelOrder"}})],1),s("div",{staticClass:"system-reply-item"},[s("span",[e._v("命令错误时回复")]),s("i-input",{staticClass:"columns-item-input",model:{value:e.systemReply.commandError,callback:function(t){e.$set(e.systemReply,"commandError",t)},expression:"systemReply.commandError"}})],1),s("div",{staticClass:"system-reply-item"},[s("span",[e._v("余额不足时回复")]),s("i-input",{staticClass:"columns-item-input",model:{value:e.systemReply.insuffBalance,callback:function(t){e.$set(e.systemReply,"insuffBalance",t)},expression:"systemReply.insuffBalance"}})],1),s("div",{staticClass:"system-reply-item"},[s("span",[e._v("下注封盘时回复")]),s("i-input",{staticClass:"columns-item-input",model:{value:e.systemReply.sealing,callback:function(t){e.$set(e.systemReply,"sealing",t)},expression:"systemReply.sealing"}})],1)])])])]),s("div",{staticClass:"system-reply"},[s("p",{staticClass:"base-setting-save"},[e._v("可用变量说明")]),s("div",{staticClass:"sys-variable-list"},e._l(e.sysVariables,function(t,a){return s("div",{key:a,staticClass:"sys-item"},[s("span",[e._v(e._s(t.title))]),s("span",[e._v(e._s(t.desc))])])}),0)])])},i=[function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("div",{staticClass:"base-list-item"},[s("p",{staticClass:"list-item-title"},[e._v("回复消息设置可用变量")]),s("div",{staticClass:"poptip"},[s("p",[e._v("{游戏}{当前玩法}{当期玩法}{当前玩法明细}{当期玩法明细}{期号}{简写期号}{剩余}{当期得分}{当日盈亏}{当期盈亏}{昵称}")]),s("p",[e._v("{当日流水}{当前使用分数}{当期使用分数}{当前多行玩法}")]),s("p",[e._v("{未开奖简写期号}{未开奖玩法}{未开奖玩法明细}{未开奖使用分数}")])])])}],n=(s("cde0"),{data:function(){return{showLoading:!1,canEdit1:!1,canEdit2:!1,autoBackWater:!1,sysVariables:[{title:"{游戏}",desc:"当前游戏类型， 如：北京赛车"},{title:"{期号}",desc:"完整期号， 如重庆时时彩 第20150101120期"},{title:"{剩余}",desc:"玩家剩余分数，默认保留2位小数。可在封盘设定中全局修改"},{title:"{当前玩法}",desc:"玩家单次输入的命令生成的玩法内容，可以是一条或多条订单"},{title:"{当期玩法}",desc:"当前期所有玩法内容汇总"},{title:"{当前玩法明细}",desc:"如1/12/10，显示为“第一球1-10，第一球2-10"},{title:"{当期玩法明细}",desc:"当期所有玩法明细汇总"},{title:"{简写期号}",desc:"简写期号，如重庆时时彩 第120期"},{title:"{当期得分}",desc:"当期中奖分数"},{title:"{当日得分}",desc:"当日所有中奖分数总和"},{title:"{当期盈亏}",desc:"当期盈利分数"},{title:"{当日盈亏}",desc:"当日盈利分数总和"},{title:"{当日流水}",desc:"当日截止当前期所有有效订单分数总和"},{title:"{当前使用分数}",desc:"当前玩法的使用分数"},{title:"{当期使用分数}",desc:"当前期合计使用分数"},{title:"{当前多行玩法}",desc:"自动换行显示当前所有玩法内容"},{title:"{未开奖简写期号}",desc:"未开奖结算简写期号"},{title:"{未开奖玩法}",desc:"未开奖结算玩法"},{title:"{未开奖玩法明细}",desc:"未开奖结算玩法明细"},{title:"{未开奖使用分数}",desc:"未开奖结算使用分数"}],gameSuccReply:"",queryIntegReply:"",queryCountReply:"",changeBalanceReply:"",autoReturnReply:"",systemReply:{notAddGamer:"",notCancelOrder:"",returnMsg:"",lockGamer:"",notStartGame:"",moveGamer:"",startGame:"",stopGame:"",commandError:"",receiveReq:"",cancelOrder:"",insuffBalance:"",sealing:"",noBetting:""},systemOperateCheck:[],systemOperate:[{label:3,title:"提交无效指令后@对方"},{label:4,title:"下注时已封盘@对方"},{label:5,title:"下注成功后@对方"},{label:6,title:"取消玩法后@对方"},{label:7,title:"收到查回请求后@对方"},{label:8,title:"确认查回分数后@对方"},{label:9,title:"下注积分不足时@对方"},{label:10,title:"下注发生限额时@对方"}],isLoading:!1}},mounted:function(){var e=this;e.getData()},methods:{handleOperate:function(e){var t=this;t.handleSave()},handleSave:function(){var e=this,t={};t={GameSuccess:e.gameSuccReply,CheckScore:e.queryIntegReply,CheckStream:e.queryCountReply,Remainder:e.changeBalanceReply,Backwater:e.autoReturnReply,ProhibitionCancel:e.systemReply.notCancelOrder,UserLock:e.systemReply.lockGamer,GameNotStart:e.systemReply.notStartGame,GameStart:e.systemReply.startGame,GameStop:e.systemReply.stopGame,CommandError:e.systemReply.commandError,ReceivingRequests:e.systemReply.receiveReq,CancelOrder:e.systemReply.cancelOrder,NotEnough:e.systemReply.insuffBalance,Sealing:e.systemReply.sealing,NotOrders:e.systemReply.noBetting,NoticeInvalidSub:-1!==e.systemOperateCheck.findIndex(function(e){return 3===e}),NoticeSealing:-1!==e.systemOperateCheck.findIndex(function(e){return 4===e}),NoticeBetSuccess:-1!==e.systemOperateCheck.findIndex(function(e){return 5===e}),NoticeCancel:-1!==e.systemOperateCheck.findIndex(function(e){return 6===e}),NoticeCheckRequest:-1!==e.systemOperateCheck.findIndex(function(e){return 7===e}),NoticeConfirmRequest:-1!==e.systemOperateCheck.findIndex(function(e){return 8===e}),NoticeInsufficientIntegral:-1!==e.systemOperateCheck.findIndex(function(e){return 9===e}),NoticeQuota:-1!==e.systemOperateCheck.findIndex(function(e){return 10===e}),SwitchBackwater:e.autoBackWater},e.isLoading||(e.isLoading=!0,e.$axios({url:"/api/Setup/UpdateReplySetUp",method:"post",data:t}).then(function(t){e.isLoading=!1,100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.getData()}))},getData:function(){var e=this;e.isLoading||(e.isLoading=!0,e.showLoading=!0,e.axios({url:"/api/Setup/GetReplySetUp"}).then(function(t){if(e.isLoading=!1,100===t.data.Status){var s=t.data.Model;e.gameSuccReply=s.GameSuccess,e.queryIntegReply=s.CheckScore,e.queryCountReply=s.CheckStream,e.changeBalanceReply=s.Remainder,e.autoReturnReply=s.Backwater,e.systemReply={notCancelOrder:s.ProhibitionCancel,lockGamer:s.UserLock,notStartGame:s.GameNotStart,startGame:s.GameStart,stopGame:s.GameStop,commandError:s.CommandError,receiveReq:s.ReceivingRequests,cancelOrder:s.CancelOrder,insuffBalance:s.NotEnough,sealing:s.Sealing,noBetting:s.NotOrders},s.NoticeInvalidSub&&e.systemOperateCheck.push(3),s.NoticeSealing&&e.systemOperateCheck.push(4),s.NoticeBetSuccess&&e.systemOperateCheck.push(5),s.NoticeCancel&&e.systemOperateCheck.push(6),s.NoticeCheckRequest&&e.systemOperateCheck.push(7),s.NoticeConfirmRequest&&e.systemOperateCheck.push(8),s.NoticeInsufficientIntegral&&e.systemOperateCheck.push(9),s.NoticeQuota&&e.systemOperateCheck.push(10),e.autoBackWater=s.SwitchBackwater}e.showLoading=!1}))}}}),l=n,c=(s("769f"),s("6691")),r=Object(c["a"])(l,a,i,!1,null,null,null);t["default"]=r.exports},b5b8:function(e,t,s){var a=s("94ac");e.exports=Array.isArray||function(e){return"Array"==a(e)}},cde0:function(e,t,s){"use strict";var a=s("b2f5"),i=s("2d43")(6),n="findIndex",l=!0;n in[]&&Array(1)[n](function(){l=!1}),a(a.P+a.F*l,"Array",{findIndex:function(e){return i(this,e,arguments.length>1?arguments[1]:void 0)}}),s("644a")(n)},f691:function(e,t,s){var a=s("88dd"),i=s("b5b8"),n=s("8b37")("species");e.exports=function(e){var t;return i(e)&&(t=e.constructor,"function"!=typeof t||t!==Array&&!i(t.prototype)||(t=void 0),a(t)&&(t=t[n],null===t&&(t=void 0))),void 0===t?Array:t}}}]);
//# sourceMappingURL=chunk-f335716a.1588757178371.js.map
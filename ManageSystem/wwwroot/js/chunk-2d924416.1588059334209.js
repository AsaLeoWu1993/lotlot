(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-2d924416"],{"3ce3":function(t,a,e){"use strict";e.r(a);var n=function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"fly-alone"},[e("div",{staticClass:"content"},[e("div",{staticClass:"list-header"},[e("div",{staticClass:"list-title"},[e("span",[t._v("飞单同步账户管理")]),e("div",{staticClass:"list-poptip"},[e("Checkbox",{staticStyle:{"font-size":"14px"},on:{"on-change":t.hanleSyncTime},model:{value:t.flyInfo.AutoSyn,callback:function(a){t.$set(t.flyInfo,"AutoSyn",a)},expression:"flyInfo.AutoSyn"}},[t._v("打开同步时自动修正封盘时间，提前于收单盘口"),e("InputNumber",{staticClass:"column-input",staticStyle:{width:"60px"},attrs:{min:0},model:{value:t.flyInfo.AdvanceTime,callback:function(a){t.$set(t.flyInfo,"AdvanceTime",a)},expression:"flyInfo.AdvanceTime"}}),t._v("秒")],1),e("div",{staticStyle:{display:"inline-block","font-size":"14px"}},[t._v("\n                        飞单开启\n                        "),e("switchs",{staticStyle:{"verify-align":"middle"},attrs:{size:"big",open:t.flyInfo.OpenSheet},on:{switch:function(a){return t.handleSwitch(t.flyInfo.OpenSheet)}}})],1),e("div",{staticClass:"button",staticStyle:{"margin-left":"8px"},on:{click:t.handleSyncInfo}},[e("span",[t._v("飞单同步")])])],1)])]),e("div",{staticClass:"query-list"},[e("div",{staticClass:"connection-info"},[e("div",{staticClass:"connection-item"},[e("span",[t._v("连接安全码")]),e("i-input",{staticClass:"column-input",model:{value:t.flyInfo.SeurityNo,callback:function(a){t.$set(t.flyInfo,"SeurityNo",a)},expression:"flyInfo.SeurityNo"}})],1),e("div",{staticClass:"connection-item"},[e("span",[t._v("房间号")]),e("i-input",{staticClass:"column-input",model:{value:t.flyInfo.RoomNum,callback:function(a){t.$set(t.flyInfo,"RoomNum",a)},expression:"flyInfo.RoomNum"}})],1),e("div",{staticClass:"connection-item"},[e("span",[t._v("登录账号")]),e("i-input",{staticClass:"column-input",model:{value:t.flyInfo.MerchantName,callback:function(a){t.$set(t.flyInfo,"MerchantName",a)},expression:"flyInfo.MerchantName"}})],1),e("div",{staticClass:"connection-item"},[e("span",[t._v("登录密码")]),e("i-input",{staticClass:"column-input",attrs:{type:"password"},model:{value:t.flyInfo.Password,callback:function(a){t.$set(t.flyInfo,"Password",a)},expression:"flyInfo.Password"}})],1),e("div",{directives:[{name:"show",rawName:"v-show",value:!t.isLoginStatus,expression:"!isLoginStatus"}],staticClass:"connection-item button login",on:{click:t.handleVerifyConnect}},[e("span",{staticStyle:{"margin-right":"0"}},[t._v("登录")])]),e("div",{directives:[{name:"show",rawName:"v-show",value:t.isLoginStatus,expression:"isLoginStatus"}],staticClass:"connection-item button exit",on:{click:t.handleLongExit}},[e("span",{staticStyle:{"margin-right":"0"}},[t._v("退出")])])]),e("div",{staticClass:"sync-info"},[e("div",{staticClass:"sync-item"},[e("p",[t._v("收单盘口（高级账户）")]),e("p",{domProps:{textContent:t._s(t.sync.handicap)}})]),e("div",{staticClass:"sync-item"},[e("p",[t._v("飞单账号")]),e("p",{domProps:{textContent:t._s(t.sync.account)}})]),e("div",{staticClass:"sync-item"},[e("p",[t._v("账户余额")]),e("p",{domProps:{textContent:t._s(t.sync.balance)}})]),e("div",{staticClass:"sync-item"},[e("p",[t._v("今日已结算金额")]),e("p",{domProps:{textContent:t._s(t.sync.alreadyKnotBalance)}})]),e("div",{staticClass:"sync-item"},[e("p",[t._v("今日未结算金额")]),e("p",{domProps:{textContent:t._s(t.sync.unfinishedBalance)}})]),e("div",{staticClass:"sync-item"},[e("p",[t._v("今日盈亏")]),e("p",{domProps:{textContent:t._s(t.sync.profitAndLoss)}})])])])]),e("div",{staticClass:"other-nav"},[t._m(0),e("Checkbox",{staticStyle:{"font-size":"14px"},on:{"on-change":t.handleTipsStatus},model:{value:t.flyInfo.Remind,callback:function(a){t.$set(t.flyInfo,"Remind",a)},expression:"flyInfo.Remind"}},[t._v("飞单账户余额低于"),e("InputNumber",{staticClass:"column-input",staticStyle:{width:"120px"},attrs:{min:0},model:{value:t.flyInfo.LowFraction,callback:function(a){t.$set(t.flyInfo,"LowFraction",a)},expression:"flyInfo.LowFraction"}}),t._v("声音提示")],1)],1),e("div",{staticClass:"content"},[t._m(1),e("div",{staticClass:"list-data"},[t._m(2),e("div",{staticClass:"data-con"},t._l(t.showDataList,function(a,n){return e("div",{key:n,staticClass:"data-item"},[e("p",{staticClass:"item-info item-1"},[t._v(t._s(a.username))]),e("p",{staticClass:"item-info item-3"},[t._v(t._s(a.gameName))]),e("p",{staticClass:"item-info item-4"},[t._v(t._s(a.perids))]),e("p",{staticClass:"item-info item-5",attrs:{title:a.currentBet}},[t._v(t._s(a.currentBet))]),e("p",{staticClass:"item-info item-6"},[t._v(t._s(a.bet))]),e("p",{staticClass:"item-info item-7"},[t._v(t._s(a.time))]),e("p",{staticClass:"item-info item-8"},[t._v(t._s(a.status))])])}),0)]),e("Page",{directives:[{name:"show",rawName:"v-show",value:t.page.total,expression:"page.total"}],staticClass:"footer-page",attrs:{total:t.page.total,"page-size":t.page.pageSize,current:t.page.pageNum,"show-sizer":""},on:{"on-page-size-change":t.handleChangeSize,"on-change":t.handleChangePage}})],1)])},s=[function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"nav-item"},[e("span",[t._v("飞单列表")])])},function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"list-header"},[e("div",{staticClass:"list-title"},[e("span",[t._v("飞单同步列表")]),e("div",{staticClass:"list-poptip"},[e("span",[t._v("说明：开启飞单的玩家注单会进入列表，禁止取消后进行同步。")])])])])},function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"data-header"},[e("p",{staticClass:"item-info item-1"},[t._v("玩家")]),e("p",{staticClass:"item-info item-2"},[t._v("游戏")]),e("p",{staticClass:"item-info item-4"},[t._v("期号")]),e("p",{staticClass:"item-info item-5"},[t._v("注单内容")]),e("p",{staticClass:"item-info item-6"},[t._v("注单金额")]),e("p",{staticClass:"item-info item-7"},[t._v("注单时间")]),e("p",{staticClass:"item-info item-8"},[t._v("状态")])])}],i=e("873a"),o=e("702a"),c={components:{switchs:i["a"]},data:function(){return{flyAloneMana:{openUse:!0,useSecond:20,isOpenSync:!1},connectionInfo:{code:"",account:"",password:""},sync:{handicap:"",account:"",balance:"",alreadyKnotBalance:0,unfinishedBalance:0,profitAndLoss:0},dataList:[],isLoading:!1,isLoading1:!1,flyInfo:{userId:"",targetId:"",AdvanceTime:20,SeurityNo:"",AutoSyn:!1,Password:"",Remind:!1,OpenSheet:!1,LowFraction:20,MerchantName:"",RoomNum:""},page:{total:0,pageSize:10,pageNum:1},isLoginStatus:!1,poptipWav:e("d0ca"),poptipMp3:e("ff9c"),isLogining:!1}},beforeRouteLeave:function(t,a,e){var n=this;n.lotterySignalr.closeSignalr(),e()},computed:{showDataList:function(){var t=this,a=(t.page.pageNum-1)*t.page.pageSize,e=t.page.pageNum*t.page.pageSize-1;return t.dataList.slice(a,e)}},mounted:function(){var t=this;t.handleGetInfo(),t.lotterySignalr.reStart(100),o["a"].$on("LoginStatus",function(a){"string"==typeof a&&(a=JSON.parse(a)),console.log(a),t.$Message.success(a.Message)}),o["a"].$on("TargetInfo",function(a){"string"==typeof a&&(a=JSON.parse(a)),t.sync.handicap=a.MerchantName,t.sync.account=a.UserName,t.sync.balance=a.Balance,t.sync.alreadyKnotBalance=a.Already,t.sync.unfinishedBalance=a.NAlready,t.sync.profitAndLoss=a.Proloss,t.isLogining=!1}),o["a"].$on("MerchantBetSheet",function(a){"string"==typeof a&&(a=JSON.parse(a));var e={username:a.UserName,gameName:a.GameType,perids:a.Nper,currentBet:a.Remark,bet:a.UseMoney,time:a.Time,status:a.Status};t.dataList.unshift(e),t.page.total=t.dataList.length}),o["a"].$on("LowFraction",function(a){console.log(a);var e=document.createElement("audio");e.preload="auto",e.src=t.poptipWav,e.src=t.poptipMp3,e.play()}),o["a"].$on("exitStatus",function(){t.$Message.success("退出成功!"),t.isLoginStatus=!1})},methods:{handleChangeSize:function(t){var a=this;a.page.pageSize=t,a.page.pageNum=1,a.handleGetSheetInfos()},handleChangePage:function(t){var a=this;a.page.pageNum=t,a.periods="",a.handleGetSheetInfos()},handleTipsLimit:function(){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/Setup/TipsLimit",params:{num:t.flyInfo.LowFraction}}).then(function(a){t.isLoading=!1,100===a.data.Status?t.$Message.success(a.data.Message):t.$Message.error(a.data.Message)}).catch(function(a){t.isLoading=!1}))},handleTipsStatus:function(){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/Setup/TipsOpen",params:{open:t.flyInfo.Remind}}).then(function(a){t.isLoading=!1,100===a.data.Status?(t.$Message.success(a.data.Message),t.flyInfo.Remind&&t.handleTipsLimit()):t.$Message.error(a.data.Message)}).catch(function(a){t.isLoading=!1}))},hanleSyncTime:function(){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/Setup/SheetAutoCorrection",params:{islock:t.flyInfo.AutoSyn}}).then(function(a){t.isLoading=!1,100===a.data.Status?t.$Message.success(a.data.Message):t.$Message.error(a.data.Message)}).catch(function(a){t.isLoading=!1}))},handleSyncInfo:function(){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/Setup/SheetSynchronization",params:{time:t.flyInfo.AdvanceTime}}).then(function(a){t.isLoading=!1,100===a.data.Status?t.$Message.success(a.data.Message):t.$Message.error(a.data.Message)}).catch(function(a){t.isLoading=!1}))},handleGetInfo:function(){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/Setup/GetFlySheetInfo"}).then(function(a){t.isLoading=!1,100===a.data.Status&&(t.flyInfo.AdvanceTime=a.data.Model.AdvanceTime,t.flyInfo.SeurityNo=a.data.Model.SeurityNo||"",t.flyInfo.AutoSyn=a.data.Model.AutoSyn,t.flyInfo.Password=a.data.Model.Password||"",t.flyInfo.Remind=a.data.Model.Remind,t.flyInfo.OpenSheet=a.data.Model.OpenSheet,t.flyInfo.LowFraction=a.data.Model.LowFraction,t.flyInfo.MerchantName=a.data.Model.MerchantName||"",t.flyInfo.RoomNum=a.data.Model.RoomNum||"")}).catch(function(a){t.isLoading=!1}))},handleSwitch:function(t){var a=this;a.isLoading||(a.isLoading=!0,a.$axios({url:"/api/Setup/FlySingleOpen",params:{open:!t}}).then(function(e){a.isLoading=!1,100===e.data.Status?(a.$Message.success(e.data.Message),a.flyInfo.OpenSheet=!t):a.$Message.error(e.data.Message)}).catch(function(t){a.isLoading=!1}))},handleLongExit:function(){var t=this;t.lotterySignalr.exitMonitor()},handleVerifyConnect:function(){var t=this;t.flyInfo.SeurityNo.trim()?t.flyInfo.RoomNum.trim()?t.flyInfo.MerchantName.trim()?t.flyInfo.Password.trim()?t.isLoading||t.isLogining?t.isLogining&&t.$Message.error("登录中,请稍后..."):(t.isLoading=!0,t.isLogining=!0,t.$axios({url:"/api/Setup/AddSheetMerchant",params:{seurityNo:t.flyInfo.SeurityNo,merchantName:t.flyInfo.MerchantName,merchantPwd:t.flyInfo.Password,roomNum:t.flyInfo.RoomNum}}).then(function(a){t.isLoading=!1,100===a.data.Status?(t.$Message.success(a.data.Message),t.isLoginStatus=!0,t.flyInfo.userId=a.data.Model.UserID,t.flyInfo.targetId=a.data.Model.TargetID,t.lotterySignalr.joinMonitor(t.flyInfo.userId,t.flyInfo.targetId)):t.$Message.error(a.data.Message)}).catch(function(a){t.isLoading=!1})):t.$Message.error("请输入登录密码!"):t.$Message.error("请输入登录账号!"):t.$Message.error("请输入房间码!"):t.$Message.error("请输入安全码!")}}},l=c,r=(e("46c0"),e("6691")),d=Object(r["a"])(l,n,s,!1,null,null,null);a["default"]=d.exports},"46c0":function(t,a,e){"use strict";var n=e("5ef4"),s=e.n(n);s.a},"5ef4":function(t,a,e){},"873a":function(t,a,e){"use strict";var n=function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{class:["switch",t.size,t.open?t.size+"-open":t.size+"-close"],on:{click:t.handleSwitch}},[e("div",{staticClass:"switch-item"})])},s=[],i={props:{open:{type:Boolean,default:!1},size:{type:String,default:"small"}},methods:{handleSwitch:function(){var t=this;t.$emit("switch")}}},o=i,c=(e("e14d"),e("6691")),l=Object(c["a"])(o,n,s,!1,null,null,null);a["a"]=l.exports},c233:function(t,a,e){},e14d:function(t,a,e){"use strict";var n=e("c233"),s=e.n(n);s.a}}]);
//# sourceMappingURL=chunk-2d924416.1588059334209.js.map
(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-40468902"],{"119c":function(e,t,a){"use strict";var s=a("b6f1");e.exports=function(e,t){return!!e&&s(function(){t?e.call(null,function(){},1):e.call(null)})}},"2d43":function(e,t,a){var s=a("01f5"),i=a("6462"),n=a("db4b"),r=a("b146"),o=a("5824");e.exports=function(e,t){var a=1==e,u=2==e,l=3==e,c=4==e,y=6==e,d=5==e||y,p=t||o;return function(t,o,m){for(var f,h,v=n(t),g=i(v),T=s(o,m,3),q=r(g.length),w=0,_=a?p(t,q):u?p(t,0):void 0;q>w;w++)if((d||w in g)&&(f=g[w],h=T(f,w,v),e))if(a)_[w]=h;else if(h)switch(e){case 3:return!0;case 5:return f;case 6:return w;case 2:_.push(f)}else if(c)return!1;return y?-1:l||c?c:_}}},5824:function(e,t,a){var s=a("f691");e.exports=function(e,t){return new(s(e))(t)}},"608b":function(e,t,a){"use strict";var s=a("b2f5"),i=a("2d43")(5),n="find",r=!0;n in[]&&Array(1)[n](function(){r=!1}),s(s.P+s.F*r,"Array",{find:function(e){return i(this,e,arguments.length>1?arguments[1]:void 0)}}),a("644a")(n)},7364:function(e,t,a){var s=a("ddf7").f,i=Function.prototype,n=/^\s*function ([^ (]*)/,r="name";r in i||a("dad2")&&s(i,r,{configurable:!0,get:function(){try{return(""+this).match(n)[1]}catch(e){return""}}})},"7db9":function(e,t,a){},8027:function(e,t,a){"use strict";var s=a("7db9"),i=a.n(s);i.a},b5b8:function(e,t,a){var s=a("94ac");e.exports=Array.isArray||function(e){return"Array"==s(e)}},b745:function(e,t,a){"use strict";var s=a("b2f5"),i=a("648a"),n=a("db4b"),r=a("b6f1"),o=[].sort,u=[1,2,3];s(s.P+s.F*(r(function(){u.sort(void 0)})||!r(function(){u.sort(null)})||!a("119c")(o)),"Array",{sort:function(e){return void 0===e?o.call(n(this)):o.call(n(this),i(e))}})},b978:function(e,t,a){"use strict";a.r(t);var s=function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"field-monitor"},[a("div",{staticClass:"content"},[a("div",{staticClass:"query-list"},[a("div",{staticClass:"query-list-t"},[a("i-select",{on:{"on-change":e.handleChangeType},model:{value:e.query.gameType,callback:function(t){e.$set(e.query,"gameType",t)},expression:"query.gameType"}},[a("i-option",{attrs:{value:"0"}},[e._v("所有游戏")]),e._l(e.gameList,function(t,s){return a("i-option",{key:s,attrs:{value:t.type}},[e._v(e._s(t.name))])})],2),a("i-select",{on:{"on-change":e.handleChangeType},model:{value:e.query.orderStatus,callback:function(t){e.$set(e.query,"orderStatus",t)},expression:"query.orderStatus"}},e._l(e.orderList,function(t,s){return a("i-option",{key:s,attrs:{value:t.key}},[e._v(e._s(t.title))])}),1),a("i-select",{on:{"on-change":e.handleChangeType},model:{value:e.query.selectUserType,callback:function(t){e.$set(e.query,"selectUserType",t)},expression:"query.selectUserType"}},e._l(e.userTypeList,function(t,s){return a("i-option",{key:s,attrs:{value:t.value}},[e._v(e._s(t.title))])}),1),a("i-input",{staticClass:"query-input-item",attrs:{placeholder:"查询用户ID/用户名"},model:{value:e.query.searchUsername,callback:function(t){e.$set(e.query,"searchUsername",t)},expression:"query.searchUsername"}})],1),a("div",{staticClass:"query-list-t"},[a("div",{staticClass:"quick-method"},[a("span",{class:1===e.quickSelect?"active":"",on:{click:function(t){return e.handleQueryByDate(1)}}},[e._v("今天")]),a("span",{class:2===e.quickSelect?"active":"",on:{click:function(t){return e.handleQueryByDate(2)}}},[e._v("昨天")]),a("span",{class:3===e.quickSelect?"active":"",on:{click:function(t){return e.handleQueryByDate(3)}}},[e._v("近一周")]),a("span",{class:4===e.quickSelect?"active":"",on:{click:function(t){return e.handleQueryByDate(4)}}},[e._v("自定义")])]),a("div",{staticClass:"quick-date"},[a("span",[e._v("区间：")]),a("DatePicker",{staticClass:"date-item",attrs:{disabled:e.canEdit,type:"date"},model:{value:e.query.startTime,callback:function(t){e.$set(e.query,"startTime",t)},expression:"query.startTime"}}),a("span",[e._v("06:00 --")]),a("DatePicker",{staticClass:"date-item",attrs:{disabled:e.canEdit,type:"date"},model:{value:e.query.endTime,callback:function(t){e.$set(e.query,"endTime",t)},expression:"query.endTime"}}),a("span",[e._v("06:00")])],1),a("i-input",{staticClass:"query-input-item",attrs:{placeholder:"查询期号"},model:{value:e.query.perids,callback:function(t){e.$set(e.query,"perids",t)},expression:"query.perids"}})],1)]),a("div",{staticClass:"query-operate"},[a("span",{on:{click:e.handleChangeType}},[e._v("查询")]),a("span",{on:{click:function(t){e.agree.show=!0}}},[e._v("撤销未结注单")])]),e._m(0)]),a("div",{staticClass:"content"},[a("div",{staticClass:"list-header"},[a("div",{staticClass:"list-title"},[a("span",[e._v("注单列表")]),a("div",{staticClass:"list-p"},[a("div",{staticClass:"list-flush-time"},[a("span",{directives:[{name:"show",rawName:"v-show",value:e.flushTime>0,expression:"flushTime > 0"}],domProps:{textContent:e._s("下次注单刷新："+e.flushTime+"秒")}}),a("span",{directives:[{name:"show",rawName:"v-show",value:0==e.flushTime,expression:"flushTime == 0"}]},[e._v("刷新中")]),a("div",{staticClass:"button",on:{click:function(t){e.flush.show=!0}}},[e._v("立即刷新")])]),a("div",{staticClass:"list-poptip"},[a("span",[e._v("下注合计：")]),a("span",[e._v(e._s(e.allBet))]),a("span",[e._v("盈亏合计：")]),a("span",[e._v(e._s(e.lossAndWinner))])])])])]),a("div",{staticClass:"list-data"},[e._m(1),a("div",{staticClass:"data-con"},e._l(e.dataList,function(t,s){return a("div",{key:s,staticClass:"data-item"},[a("p",{staticClass:"item-info item-1"},[e._v(e._s(t.username))]),a("p",{staticClass:"item-info item-2"},[e._v(e._s(t.showId))]),a("p",{staticClass:"item-info item-3"},[e._v(e._s(t.gameName))]),a("p",{staticClass:"item-info item-4"},[e._v(e._s(t.perids))]),a("p",{staticClass:"item-info item-5",attrs:{title:t.currentBet}},[e._v(e._s(t.currentBet))]),a("p",{staticClass:"item-info item-6"},[e._v(e._s(t.bet))]),a("p",{staticClass:"item-info item-7"},[e._v(e._s(t.time))]),a("p",{staticClass:"item-info item-8"},[e._v(e._s(e.gameList.find(function(e){return e.name==t.gameName}).value>t.perids&&"未开奖"===t.status?"未结算":t.status))]),a("p",{staticClass:"item-info item-9"},[a("span",{directives:[{name:"show",rawName:"v-show",value:"未开奖"!==t.status,expression:"item.status !== '未开奖'"}]},[e._v(e._s(t.lastProLoss))])])])}),0)]),a("Page",{staticClass:"footer-page",attrs:{total:e.page.total,"page-size":e.page.pageSize,current:e.page.pageNum,"show-sizer":""},on:{"on-page-size-change":e.handleChangeSize,"on-change":e.handleChangePage}})],1),a("Modal",{attrs:{styles:{top:"200px"},width:"300"},model:{value:e.agree.show,callback:function(t){e.$set(e.agree,"show",t)},expression:"agree.show"}},[a("p",{staticStyle:{color:"#0d1941","font-size":"16px","text-align":"center"},attrs:{slot:"header"},slot:"header"},[a("span",[e._v("撤销未结注单")])]),a("p",{staticStyle:{"text-align":"center"}},[e._v("是否确认撤销未结注单？")]),a("div",{staticClass:"modal-footer",attrs:{slot:"footer"},slot:"footer"},[a("div",{staticClass:"enter-cancel",on:{click:function(t){e.agree.show=!1}}},[e._v("取消")]),a("div",{staticClass:"enter-ok",on:{click:e.handleEnterCancel}},[e._v("确定")])])]),a("Modal",{attrs:{styles:{top:"200px"},width:"300"},model:{value:e.flush.show,callback:function(t){e.$set(e.flush,"show",t)},expression:"flush.show"}},[a("p",{staticStyle:{color:"#0d1941","font-size":"16px","text-align":"center"},attrs:{slot:"header"},slot:"header"},[a("span",[e._v("立即刷新")])]),a("p",{staticStyle:{"text-align":"center"}},[e._v("是否确定刷新注单列表？")]),a("div",{staticClass:"modal-footer",attrs:{slot:"footer"},slot:"footer"},[a("div",{staticClass:"enter-cancel",on:{click:function(t){e.flush.show=!1}}},[e._v("取消")]),a("div",{staticClass:"enter-ok",on:{click:function(t){return e.handleChangeType("now")}}},[e._v("确定")])])])],1)},i=[function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"query-poptip"},[a("span",[e._v("说明：撤销未结注单流程为先条件查询再撤销，撤销的目标是以列表内显示的查询结果中未开奖的注单为准。")])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"data-header"},[a("p",{staticClass:"item-info item-1"},[e._v("用户名")]),a("p",{staticClass:"item-info item-2"},[e._v("ID")]),a("p",{staticClass:"item-info item-3"},[e._v("游戏")]),a("p",{staticClass:"item-info item-4"},[e._v("期号")]),a("p",{staticClass:"item-info item-5"},[e._v("下注内容")]),a("p",{staticClass:"item-info item-6"},[e._v("使用积分")]),a("p",{staticClass:"item-info item-7"},[e._v("下注时间")]),a("p",{staticClass:"item-info item-8"},[e._v("状态")]),a("p",{staticClass:"item-info item-9"},[e._v("盈亏")])])}],n=(a("7364"),a("608b"),a("34a3"),a("b745"),a("702a")),r={data:function(){return{activeGameType:"lottery",gameBigList:[{title:"彩票游戏",key:"lottery"},{title:"视讯游戏",key:"video"}],flush:{show:!1,timeout:null,reCreate:!1},agree:{show:!1},showLoading:!1,canEdit:!0,dataList:[],query:{startTime:(new Date).getHours()>=6?new Date:new Date((new Date).setDate((new Date).getDate()-1)),endTime:new Date((new Date).setDate((new Date).getDate()+1)),searchUsername:"",gameType:"0",orderStatus:1,selectUserType:"1",perids:""},quickSelect:1,allBet:0,lossAndWinner:0,gameList:[],orderList:[{key:1,title:"所有注单"},{key:2,title:"未开奖注单"},{key:3,title:"已开奖注单"}],userTypeList:[{title:"正式用户",value:"1"},{title:"虚假用户",value:"4"}],statusTextList:["","等待中","封盘中","开奖中","已停售","未开奖","已关闭"],page:{total:0,pageSize:10,pageNum:1},auto:null,isLoading:!1,flushTime:0,lastFlushTime:0}},beforeRouteEnter:function(e,t,a){a(function(e){e.getGameList(),e.autoFlush(),setInterval(function(){0!=e.flushTime&&e.flushTime--},1e3)})},beforeRouteLeave:function(e,t,a){var s=this;s.auto=null,a()},methods:{autoFlush:function(){var e=this;e.auto=function(){e.flushTime=0,e.handleAllInfo().then(function(){e.handleGetList().then(function(){e.flushTime=15,e.flush.timeout=setTimeout(function(){e.auto&&e.auto()},15e3)}).catch(function(){e.flushTime=15,e.flush.timeout=setTimeout(function(){e.auto&&e.auto()},15e3)})})},e.auto()},handleSelectGameBigType:function(e){var t=this,a=+new Date;t.flush.show=!1,!t.lastFlushTime||a-t.lastFlushTime>5e3?(clearTimeout(t.flush.timeout),t.flush.timeout=null,t.flushTime=0,t.auto=null,t.lastFlushTime=a,t.page.pageNum=1,t.query.gameType="0",t.activeGameType=e.key,t.page.pageNum=1,t.getGameList(),t.autoFlush()):t.$Message.error("刷新间隔需要大于5秒")},getGameList:function(){var e=this;e.gameList=[],"lottery"==e.activeGameType?e.$axios({url:"/api/Merchant/GetGameList"}).then(function(t){if(100===t.data.Status){var a=t.data.Data;e.gameList=a.map(function(e){return{name:e.GameName,type:e.GameType,key:e.NickName,nextPeriods:""}}).sort(function(e,t){return e.type-t.type})}}):e.gameList.push({name:"百家乐",type:1,key:"bjl",nextPeriods:""})},handleEnterCancel:function(){var e=this;e.agree.show=!1,e.handleRevokeOrder()},handleManualLottery:function(e,t){var a=this;n["a"].$emit("changeActive","other"),a.$router.replace({name:"manualLottery",params:{type:a.gameList.find(function(t){return t.name==e}).type,periods:t}})},handleQueryByDate:function(e){var t=this,a=(new Date).getHours()>=6?new Date:new Date((new Date).setDate((new Date).getDate()-1)),s=new Date((new Date).setDate((new Date).getDate()+1)),i=new Date((new Date).setDate((new Date).getDate()-1)),n=new Date((new Date).setDate((new Date).getDate()-6));switch(t.quickSelect=e,e){case 1:t.canEdit=!0,t.query.startTime=a,t.query.endTime=s,t.handleGetList(a.pattern("yyyy-MM-dd")+" 06:00",s.pattern("yyyy-MM-dd")+" 06:00"),t.handleAllInfo(a.pattern("yyyy-MM-dd")+" 06:00",s.pattern("yyyy-MM-dd")+" 06:00");break;case 2:t.canEdit=!0,t.query.startTime=i,t.query.endTime=a,t.handleGetList(i.pattern("yyyy-MM-dd")+" 06:00",a.pattern("yyyy-MM-dd")+" 06:00"),t.handleAllInfo(i.pattern("yyyy-MM-dd")+" 06:00",a.pattern("yyyy-MM-dd")+" 06:00");break;case 3:t.canEdit=!0,t.query.startTime=n,t.query.endTime=s,t.handleGetList(n.pattern("yyyy-MM-dd")+" 06:00",s.pattern("yyyy-MM-dd")+" 06:00"),t.handleAllInfo(n.pattern("yyyy-MM-dd")+" 06:00",s.pattern("yyyy-MM-dd")+" 06:00");break;case 4:t.canEdit=!1;break}},handleChangeSize:function(e){var t=this;t.page.pageSize=e,t.page.pageNum=1,t.handleGetList()},handleChangePage:function(e){var t=this;t.page.pageNum=e,t.handleGetList()},handleChangeType:function(e){var t=this,a=+new Date;t.flush.show=!1,!t.lastFlushTime||a-t.lastFlushTime>5e3?("now"==e&&(clearTimeout(t.flush.timeout),t.flush.timeout=null,t.flushTime=0,t.auto=null,t.flush.reCreate=!0),t.lastFlushTime=a,t.page.pageNum=1,t.handleAllInfo().then(function(){t.handleGetList().then(function(){t.flush.reCreate&&(t.flush.reCreate=!1,t.autoFlush())})})):t.$Message.error("刷新间隔需要大于5秒")},handleRevokeOrder:function(){var e=this,t={},a="";"0"!=e.query.gameType&&(t["gameType"]=e.query.gameType),t["betType"]=2,e.query.selectUserType&&(t["userType"]=e.query.selectUserType),e.query.searchUsername&&(t["userKeyword"]=e.query.searchUsername),e.query.startTime||startTime?e.query.endTime||endTime?(t["startTime"]=e.query.startTime.pattern("yyyy-MM-dd")+" 06:00",t["endTime"]=e.query.endTime.pattern("yyyy-MM-dd")+" 06:00",e.query.perids&&(t["nper"]=e.query.perids),a="lottery"==e.activeGameType?"/api/Monito/RevokeNoteList":"/api/Monito/RevokeVideoNoteList",e.isLoading||(e.isLoading=!0,e.$axios({url:a,method:"post",params:t}).then(function(t){100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.isLoading=!1,e.handleGetList()}))):e.$Message.error("请选择结束时间"):e.$Message.error("请选择开始时间")},handleAllInfo:function(e,t){var a=this,s={},i="";if("0"!=a.query.gameType&&(s["gameType"]=a.query.gameType),s["betType"]=a.query.orderStatus,a.query.selectUserType&&(s["userType"]=a.query.selectUserType),a.query.searchUsername&&(s["userKeyword"]=a.query.searchUsername),a.query.startTime||e){if(a.query.endTime||t)return s["startTime"]=e||a.query.startTime.pattern("yyyy-MM-dd")+" 06:00",s["endTime"]=t||a.query.endTime.pattern("yyyy-MM-dd")+" 06:00",a.query.perids&&(s["nper"]=a.query.perids),i="lottery"==a.activeGameType?"/api/Monito/GetProfitLossInfo":"/api/Monito/GetVideoProfitLossInfo",new Promise(function(e,t){a.$axios({url:i,params:s}).then(function(t){if(100===t.data.Status){var s=t.data.Model;a.allBet=s.Flow,a.lossAndWinner=s.ProLoss}e()})});a.$Message.error("请选择结束时间")}else a.$Message.error("请选择开始时间")},handleGetList:function(e,t){var a=this,s={},i="";if("0"!=a.query.gameType&&(s["gameType"]=a.query.gameType),s["betType"]=a.query.orderStatus,a.query.selectUserType&&(s["userType"]=a.query.selectUserType),a.query.searchUsername&&(s["userKeyword"]=a.query.searchUsername),a.query.startTime||e){if(a.query.endTime||t)return s["startTime"]=e||a.query.startTime.pattern("yyyy-MM-dd")+" 06:00",s["endTime"]=t||a.query.endTime.pattern("yyyy-MM-dd")+" 06:00",a.query.perids&&(s["nper"]=a.query.perids),s["start"]=a.page.pageNum,s["pageSize"]=a.page.pageSize,i="lottery"==a.activeGameType?"/api/Monito/GetProfitLossByType":"/api/Monito/GetVideoProfitLossByType",new Promise(function(e,t){a.showLoading?(a.$Message.error("自动刷新中请稍后"),t()):(a.showLoading=!0,a.$axios({url:i,params:s}).then(function(t){if(100===t.data.Status){var s=t.data.Data;a.page.total=t.data.Total,a.dataList=s.map(function(e){return{username:e.NickName,showId:e.OnlyCode,gameName:e.GameName,perids:e.Nper,currentBet:e.BetContent,bet:e.UserBetMoney,time:e.BetTime,status:e.Status,lastProLoss:"未开奖"!==e.Status?e.ProLoss:"-"}})}a.showLoading=!1,e()}))});a.$Message.error("请选择结束时间")}else a.$Message.error("请选择开始时间")}}},o=r,u=(a("8027"),a("6691")),l=Object(u["a"])(o,s,i,!1,null,"e6ba3cee",null);t["default"]=l.exports},f691:function(e,t,a){var s=a("88dd"),i=a("b5b8"),n=a("8b37")("species");e.exports=function(e){var t;return i(e)&&(t=e.constructor,"function"!=typeof t||t!==Array&&!i(t.prototype)||(t=void 0),s(t)&&(t=t[n],null===t&&(t=void 0))),void 0===t?Array:t}}}]);
//# sourceMappingURL=chunk-40468902.1588759372124.js.map
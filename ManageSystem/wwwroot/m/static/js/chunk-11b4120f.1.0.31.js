(window.webpackJsonp=window.webpackJsonp||[]).push([["chunk-11b4120f"],{1542:function(s,e,t){"use strict";t.r(e);var i=t("702a"),a={data:function(){return{userInfo:{nickname:"",onlyCode:"",integral:"",headerImg:"",loginName:"",winnerOrLose:"",flowMoney:"",isAgent:!1},showPeriods:!1,showExitRoom:!1,versionNum:"1.0.31",isLoading:!1,selectedPeriod:2,periods:[{text:"10期",key:10},{text:"20期",key:20},{text:"30期",key:30}]}},beforeRouteLeave:function(s,e,t){this.auto=null,t()},mounted:function(){var s=this;switch(s.getLocalData("user-periods")){case"10":s.selectedPeriod=0;break;case"20":s.selectedPeriod=1;break;case"30":s.selectedPeriod=2}i.a.$emit("navigateShow",!0),i.a.$emit("changeIndex",2);var e=s.getLocalData("userInfo");e&&("string"==typeof e&&(e=JSON.parse(e)),s.userInfo.headerImg=e.userHead,s.userInfo.integral=e.amount,s.userInfo.loginName=e.loginName,s.userInfo.nickname=e.nickname,s.userInfo.onlyCode=e.onlyCode,s.userInfo.isAgent="YES"==e.isAgent,s.auto=function(){s.handleGetUserInfo().then((function(){setTimeout((function(){s.auto&&s.auto()}),1e4)}))},s.auto()),i.a.$on("PointChange",(function(e){s.userInfo.integral=e})),i.a.$on("UserAvatarChange",(function(e){s.userInfo.headerImg=s.getPicUrl(e)}))},methods:{handleCheckedPeriods:function(s,e){var t=this;t.selectedPeriod=e,t.setLocalData("user-periods",s),setTimeout((function(){t.showPeriods=!1}),1e3)},handleError:function(){this.$refs.userHeader.src=t("4fa0")},changeStoreIntegral:function(s){var e=this.getLocalData("userInfo");e&&("string"==typeof e&&(e=JSON.parse(e)),e.amount=s,this.setLocalData("userInfo",JSON.stringify(e)))},handleGetUserInfo:function(){var s=this;return new Promise((function(e,t){s.isLoading||(s.isLoading=!0,s.$axios({url:"/api/SwUser/GetAccountChange"}).then((function(t){if(s.isLoading=!1,100===t.data.Status){var i=t.data.Model;s.userInfo.integral=i.Integral,s.userInfo.winnerOrLose=i.ProLoss,s.userInfo.flowMoney=i.Flow,s.changeStoreIntegral(s.userInfo.integral)}e()})).catch((function(e){s.isLoading=!1})))}))},hanldeExitRoom:function(){this.delLocalData("safetyCode");try{this.setLocalData("safetyCode","null")}catch(s){}this.hanldeExit()},hanldeExit:function(){var s=this;s.delLocalData("userInfo"),s.delLocalData("gameType"),s.lotterySignalr.closeSignalr(),s.$axios({url:"/api/SwUser/UserExit"}).then((function(e){var t={};try{api,t={browser:"app"}}catch(e){}s.$router.push({path:"login",query:t})}))},handleJump:function(s){i.a.$emit("navigateShow",!1),this.$router.push({name:s})}}},n=(t("3977"),t("5511")),A=Object(n.a)(a,(function(){var s=this,e=s.$createElement,i=s._self._c||e;return i("div",{staticClass:"personal-info"},[i("div",{staticClass:"user-info",on:{click:function(e){return s.handleJump("accountSetting")}}},[i("div",{staticClass:"header-item"},[i("div",{staticClass:"user-header"},[i("img",{ref:"userHeader",attrs:{src:s.userInfo.headerImg,alt:"user-header",title:"user-header"},on:{error:s.handleError}})])]),i("div",{staticClass:"header-item header-center"},[i("p",{staticClass:"header-item-p"},[i("span",[s._v(s._s(s.userInfo.nickname))])]),i("p",{staticClass:"header-item-p"},[i("span",[s._v("账号：")]),i("span",[s._v(s._s(s.userInfo.loginName||"-未设置-"))])]),i("p",{staticClass:"header-item-p"},[i("span",[s._v("ID：")]),i("span",[s._v(s._s(s.userInfo.onlyCode))])])])]),i("div",{staticClass:"integral-info"},[i("div",{staticClass:"integral-item"},[i("p",[s._v(s._s(s.userInfo.integral||0))])]),i("div",{staticClass:"integral-item"},[i("p",[s._v(s._s(s.userInfo.winnerOrLose||0))])]),i("div",{staticClass:"integral-item"},[i("p",[s._v(s._s(s.userInfo.flowMoney||0))])])]),i("div",{staticClass:"personal-content-box"},[i("div",{staticClass:"personal-content"},[i("div",{staticClass:"personal-item",on:{click:function(e){return s.handleJump("bindAccount")}}},[i("div",{staticClass:"personal-item-img bind-account"}),i("span",[s._v("提现收款设置")]),i("div",{staticClass:"p-turn-right"})]),i("div",{staticClass:"personal-item",on:{click:function(e){s.showPeriods=!0}}},[i("div",{staticClass:"personal-item-img periods"}),i("span",[s._v("开奖图显示期数")]),i("div",{staticClass:"p-turn-right"})]),i("div",{staticClass:"personal-item",on:{click:function(e){return s.handleJump("accountBalance")}}},[i("div",{staticClass:"personal-item-img account-balance"}),i("span",[s._v("账变记录")]),i("div",{staticClass:"p-turn-right"})]),i("div",{staticClass:"personal-item",on:{click:function(e){return s.handleJump("recharge")}}},[i("div",{staticClass:"personal-item-img recharge"}),i("span",[s._v("充值记录")]),i("div",{staticClass:"p-turn-right"})]),i("div",{staticClass:"personal-item",on:{click:function(e){return s.handleJump("cash")}}},[i("div",{staticClass:"personal-item-img cash"}),i("span",[s._v("提现记录")]),i("div",{staticClass:"p-turn-right"})]),i("div",{staticClass:"personal-item",on:{click:function(e){return s.handleJump("betting")}}},[i("div",{staticClass:"personal-item-img record"}),i("span",[s._v("投注记录")]),i("div",{staticClass:"p-turn-right"})]),s.userInfo.isAgent?i("div",{staticClass:"personal-item",on:{click:function(e){return s.handleJump("agent")}}},[i("div",{staticClass:"personal-item-img agent"}),i("span",[s._v("代理报表")]),i("div",{staticClass:"p-turn-right"})]):s._e(),i("div",{staticClass:"personal-item"},[i("div",{staticClass:"personal-item-img version"}),i("span",[s._v("版本号")]),i("span",[s._v(s._s(s.versionNum))])]),i("div",{staticClass:"personal-item",on:{click:function(e){s.showExitRoom=!0}}},[i("div",{staticClass:"personal-item-img change-safe-code"}),i("span",[s._v("退出房间")]),i("div",{staticClass:"p-turn-right"})]),i("div",{staticClass:"personal-item",on:{click:s.hanldeExit}},[i("div",{staticClass:"personal-item-img change-acc"}),i("span",[s._v("切换账号")]),i("div",{staticClass:"p-turn-right red-right"})])])]),i("div",{directives:[{name:"show",rawName:"v-show",value:s.showExitRoom,expression:"showExitRoom"}],staticClass:"big-img-shade"},[i("div",{staticClass:"exit-content"},[i("div",{staticClass:"content-item"},[i("p",[s._v("确定退出房间吗？")]),i("div",{staticClass:"button-list"},[i("span",{on:{click:s.hanldeExitRoom}},[s._v("退出")]),i("span",{on:{click:function(e){s.showExitRoom=!1}}},[s._v("继续游戏")])])])])]),i("div",{directives:[{name:"show",rawName:"v-show",value:s.showPeriods,expression:"showPeriods"}],staticClass:"account-type-shade",on:{click:function(e){s.showPeriods=!1}}},[i("div",{staticClass:"account-type"},s._l(s.periods,(function(e,a){return i("div",{key:a,staticClass:"account-item",on:{click:function(t){return t.stopPropagation(),s.handleCheckedPeriods(e.key,a)}}},[i("span",[s._v(s._s(e.text))]),i("img",{directives:[{name:"show",rawName:"v-show",value:s.selectedPeriod===a,expression:"selectedPeriod === index"}],attrs:{src:t("6551"),alt:"checked"}})])})),0)])])}),[],!1,null,null,null);e.default=A.exports},3977:function(s,e,t){"use strict";var i=t("7704");t.n(i).a},"4fa0":function(s,e){s.exports="data:image/png;base64,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"},6551:function(s,e){s.exports="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAMAAACdt4HsAAABv1BMVEVHcEwAywwA1AAAyw0A/wAAygwAzAAA/wAA0gAAygwAvwAAyAAAygwAyw0AywwAyg0AzA0AygwA2wAAygwAyw0A/wAAzwAAywwAzAoA1QAAygwAywwAywwAzQcAyg0AywwAyw0AywwAyg0Ayw0AzwgAywsAywwAygsAywwAzgoAyw0AywwA1AAAyw0AzAwAyw0Ayg0AywwA2AAAygwAygoAywwA3wAAyw0AzQoAygwAyw0AywwAygsAyw0AxgAAywsAywwAzAwAygsAyg0AzAsA0QAAyg0AywwAzAwAywwAyg0Ayw0AywwAygwAywsAygwAzA0AywsAywsAywsAyw0Ayg0AywwAzA0AywwAyw0Ayw0AygwAyw0AywwAyw0AywwAyg0AygwAyg0AygwAyw0AyQ0AzAsAyQgAyw0AygwAygwAywwAywwAywwAygwAywwAzAsAygwAygwAyAkAywwAygkAygwAygsAywsAygwAyg0AzQsAzAoAzgsAywsAygwAygsAzQkAyg0AywsAyw0AzAwAyg0AywwAzQ0AywwAywwAywwAywwAygwAywwAywwAzAwAzAsAywwAzAAAyw0EsEZOAAAAlHRSTlMAvwy0A88FARHUBA6u2LrKFOcHmqECEOFLEvavwCTfgJ6n3sQgWc5D5RrcUwbIaPR4/g38TbkIozO82+sYxQlEqIFb2nILoJNp4uzw+Fdx9yh1iHCJ7up3lPPdak+PsarLPsfMjRNaIe+W5IWm1n1rhlLQHNM6ZmFd42VHGS9FrYc4spydVbfNPdJsl/n7mbtBRtEPS1b9KgAAAlRJREFUWMPtl2dX2zAUhuUMGyhJIKEEMpoUSqG07E0ZbaF777333nvvve8PrskJkixb0lX6lfeTcnOe9ziO7iJkQVKtal+bOtHVsq3nSOuG2fZGQzrSlM2ARx3XXjho/G6hAwK0pNtG4VUjAyBR+uFiPb9yCyh0fVqDN64BtfpuKPn8Y9AqOyznl9UDQqeaZfyKzYBS7Lzk98cAqeXrAy9PK6D1JsjgEBjoiZ+/bcLDzrzIh+qNDMASDY6DodZ5+bpvaHK88KeUWt60uIXmY3Xkd+mwneftrQY8KZZOXZ4UxPLVLp8qn3dxBrVYvorxsI/xTk8FPNxhBqNo3klxn1lW3kTzDXzgGDXYWBEPvdRgrCIeGqjBvYp4yFGDNIpf6qsr1GC/jo8G8dBJDVbr+EUk4udhhhq0lCM/fv2U8UF39SM1iJYjo8SO43muqGTLkTE7wEHKQ5Ea7JkPxZM+B5evkeTaQWrwiD3VFLHDIh+WvNtB1tJYcHLKA6j4vhBLR64kT4Y4JKHgIcoVlK9cPMEc3KMt5eEVZ/Ca/yIx/9rm+Lj8el3ki+K4cHHn/jgNf8ZT1h+IqROpdfmkgof33qnwgFh8Iy5vKXihsZCj/vKv5OGC0Buds4LDxF/LqLmS59+FmVCZ44fz/gGh16Q3NwVMKI6F5wv/OWS9lY15aRx/UjoxT1Rj+MuKub9/SIsPfFFvGuc0/KVPunF/sFPFDz3TLww13dKJ73MbbuUZ3nElgM4U75ssXS+9j5H5sKnfdO9rfrp7xMrlwu+u7m07nVzYgxX6B0/XEIZDD/k4AAAAAElFTkSuQmCC"},7704:function(s,e,t){}}]);
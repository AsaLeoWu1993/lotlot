(window.webpackJsonp=window.webpackJsonp||[]).push([["chunk-69584390"],{"013f":function(t,e,s){var a=s("4ce5"),i=s("224c"),n=s("008a"),r=s("eafa"),o=s("5dd2");t.exports=function(t,e){var s=1==t,l=2==t,c=3==t,u=4==t,d=6==t,f=5==t||d,h=e||o;return function(e,o,v){for(var m,p,g=n(e),y=i(g),_=a(o,v,3),w=r(y.length),C=0,T=s?h(e,w):l?h(e,0):void 0;C<w;C++)if((f||C in y)&&(p=_(m=y[C],C,g),t))if(s)T[C]=p;else if(p)switch(t){case 3:return!0;case 5:return m;case 6:return C;case 2:T.push(m)}else if(u)return!1;return d?-1:c||u?u:T}}},"042f":function(t,e,s){"use strict";var a={props:{vShow:{type:Boolean,default:!1},loadingText:{type:String,default:""}}},i=(s("9ecf"),s("5511")),n=Object(i.a)(a,(function(){var t=this,e=t.$createElement,s=t._self._c||e;return s("div",{directives:[{name:"show",rawName:"v-show",value:t.vShow,expression:"vShow"}],staticClass:"loading"},[s("div",{staticClass:"loader"},[s("svg",{staticClass:"circular",attrs:{viewBox:"25 25 50 50"}},[s("circle",{staticClass:"path",attrs:{cx:"50",cy:"50",r:"20",fill:"none","stroke-width":"5","stroke-miterlimit":"10"}})]),s("p",{directives:[{name:"show",rawName:"v-show",value:t.loadingText,expression:"loadingText"}],staticClass:"loader-poptip"},[t._v(t._s(t.loadingText))])])])}),[],!1,null,"4df03536",null);e.a=n.exports},2346:function(t,e,s){var a=s("75c4");t.exports=Array.isArray||function(t){return"Array"==a(t)}},"3afd":function(t,e,s){},"4f46":function(t,e,s){"use strict";var a=s("69bd");s.n(a).a},"5dd2":function(t,e,s){var a=s("81dc");t.exports=function(t,e){return new(a(t))(e)}},"69bd":function(t,e,s){},"81dc":function(t,e,s){var a=s("fb68"),i=s("2346"),n=s("cb3d")("species");t.exports=function(t){var e;return i(t)&&("function"!=typeof(e=t.constructor)||e!==Array&&!i(e.prototype)||(e=void 0),a(e)&&null===(e=e[n])&&(e=void 0)),void 0===e?Array:e}},9095:function(t,e,s){"use strict";s.r(e),s("9a33"),s("6d57"),s("c904"),s("cc57"),s("e697");var a=s("a39e"),i=s("042f"),n=s("702a"),r={components:{gameHeader:a.a,vLoading:i.a},data:function(){return{lotteryList:[],gameName:"",isLoading:!1,page:0,gameType:"",type:1,noMoreData:!1,flush:{start:0,end:0,isFlushing:!1,isEnd:!1},textColor:[{value:"虎",color:"#639ffd"},{value:"龙",color:"#fa0000"},{value:"和",color:"#31d22f"},{value:"小",color:"#639ffd"},{value:"大",color:"#fa0000"},{value:"双",color:"#639ffd"},{value:"单",color:"#fa0000"}]}},beforeRouteEnter:function(t,e,s){s((function(t){var e=t.getLocalData("gameType");n.a.$emit("navigateShow",!1),e&&(t.gameType=e,t.handleGetGameList().then((function(){var s=t.gameList.find((function(t){return t.type==e}));t.type=s.gameType,t.gameName=s.name,t.getData()})))}))},methods:{handleGetGameList:function(){var t=this;return new Promise((function(e,s){t.$axios({url:"/api/SwUser/GetGameList"}).then((function(a){if(100===a.data.Status){var i=a.data.Data;t.gameList=i.map((function(t){return{name:t.GameName,type:t.GameType,key:t.NickName,gameType:t.Type}})).sort((function(t,e){return t.type-e.type})),e()}s()}))}))},goBack:function(){this.$router.push({name:"game"})},getData:function(){var t=this;t.isLoading||(t.isLoading=!0,t.page++,t.$axios({url:"/api/SwUser/GetGameHistory",params:{gameType:t.gameType,start:t.page,pageSize:30}}).then((function(e){if(t.isLoading=!1,100===e.data.Status){var s=e.data.Data;t.handleResult(s)}t.flush.end=t.flush.start,t.flush.isFlushing=!1})).catch((function(e){t.isLoading=!1})))},handleResult:function(t){var e=this;t.forEach((function(t){e.lotteryList.push({periods:t.IssueNum,nums:t.Number.split("|").map((function(t){return parseInt(t)})),mes:t.Message.split("|")})})),0===t.length&&(e.noMoreData=!0)},handleJudgmentType:function(t){var e={beforeThree:"",centerThree:"",afterThree:""},s={beforeThree:t.slice(0,3),centerThree:t.slice(1,4),afterThree:t.slice(2,5)};for(var a in e)s[a][0]===s[a][1]&&s[a][1]===s[a][2]?e[a]="豹子":s[a][0]===s[a][1]||s[a][1]===s[a][2]||s[a][0]===s[a][2]?e[a]="对子":s[a][0]+1===s[a][1]&&s[a][1]+1===s[a][2]||s[a][0]-1===s[a][1]&&s[a][1]-1===s[a][2]?e[a]="顺子":s[a][0]+1===s[a][1]||s[a][1]+1===s[a][2]||s[a][0]-1===s[a][1]||s[a][1]-1===s[a][2]?e[a]="半顺":e[a]="杂六";return e},handleScroll:function(t){var e=this;t.target.scrollTop+t.target.clientHeight===t.target.scrollHeight&&(e.moreData=!0,setTimeout((function(){e.$refs.list.scrollTop+=25,e.getData()}),20))},handleTouchEnd:function(t){var e=this;e.flush.isEnd=!0,100<e.flush.end-e.flush.start&&0===e.$refs.list.scrollTop?(e.lotteryList=[],e.noMoreData=!1,e.page=0,e.getData()):t.target.scrollTop+t.target.clientHeight===t.target.scrollHeight&&e.flush.end-e.flush.start<-100?setTimeout((function(){e.$refs.list.scrollTop+=25,e.getData()}),20):(e.flush.end=e.flush.start,e.flush.isFlushing=!1)},handleTouchMove:function(t){this.flush.end=t.changedTouches[0].pageY},handleTouchStart:function(t){var e=this;e.flush.start=t.targetTouches[0].pageY,e.flush.end=e.flush.start,e.flush.isFlushing=!0,e.flush.isEnd=!1}}},o=(s("9914"),s("5511")),l=Object(o.a)(r,(function(){var t=this,e=t.$createElement,s=t._self._c||e;return s("div",{staticClass:"lottery my-position"},[s("gameHeader",{attrs:{headerTitle:"开奖走势 ["+t.gameName+"]"},on:{goBack:t.goBack}}),s("div",{staticClass:"lottery-list"},[s("div",{staticClass:"lottery-head"},[s("span",{class:["header-info",2==t.type?"header-title1":"header-title"]},[t._v("期数")]),s("div",{class:["header-info",2==t.type?"five-result":"ten-result"]},[t._v("結菓")]),2==t.type?s("div",{staticClass:"header-info total"},[t._v("總啝")]):t._e(),1==t.type?s("div",{staticClass:"header-info ten-total"},[t._v("蒄亞")]):t._e(),s("div",{class:["header-info",2==t.type?"five-tiger":"ten-tiger"]},[t._v("龍唬")]),2==t.type?s("div",{staticClass:"header-info three"},[t._v("偂三")]):t._e(),2==t.type?s("div",{staticClass:"header-info three"},[t._v("仲三")]):t._e(),2==t.type?s("div",{staticClass:"header-info three"},[t._v("後三")]):t._e()]),s("div",{ref:"list",staticClass:"lottery-list-data",on:{touchstart:function(e){return e.stopPropagation(),t.handleTouchStart(e)},touchmove:function(e){return e.stopPropagation(),t.handleTouchMove(e)},touchend:function(e){return e.stopPropagation(),t.handleTouchEnd(e)}}},[s("div",{directives:[{name:"show",rawName:"v-show",value:t.flush.end>t.flush.start&&t.flush.isFlushing,expression:"flush.end > flush.start && flush.isFlushing"}],staticClass:"flush-item flush-down"},[s("div",{staticClass:"downwrap-content"},[s("p",{staticClass:"downwrap-progress",style:{transform:"rotate("+5*(t.flush.end-t.flush.start)+"deg)"}}),s("p",{staticClass:"downwrap-tips"},[t._v(t._s(t.flush.isEnd?"下拉刷新":"刷新中"))])])]),s("div",{staticClass:"list-data",style:{transitionDuration:t.flush.isFlushing?"0":"300ms",transform:"translate(0, "+(t.flush.end-t.flush.start)/5+"px) translateZ(0)"}},t._l(t.lotteryList,(function(e,a){return s("div",{key:a,staticClass:"lottery-item"},[s("div",{class:["lottery-title","3"===t.gameType?"small-title":""]},[t._v(t._s(e.periods))]),t._l(e.nums,(function(e,a){return s("div",{key:"num-"+a,class:["num-item",(2==t.type?"five-":"ten-")+"num-"+e]})})),1==t.type?s("div",{staticClass:"num-item first-second",staticStyle:{color:"#767e84"}},[t._v(t._s(e.mes[0]))]):t._e(),1==t.type?s("div",{staticClass:"num-item first-second",style:{color:"小"===e.mes[2]?"#2e3076":"#d5091d"}},[t._v(t._s(e.mes[2]))]):t._e(),1==t.type?s("div",{staticClass:"num-item first-second",style:{color:"单"===e.mes[1]?"#2e3076":"#d5091d"}},[t._v(t._s(e.mes[1]))]):t._e(),1==t.type?s("div",{staticClass:"num-item ten-tiger",style:{color:"龙"===e.mes[3]?"#2e3076":"#d5091d"}},[t._v(t._s(e.mes[3]))]):t._e(),1==t.type?s("div",{staticClass:"num-item ten-tiger",style:{color:"龙"===e.mes[4]?"#2e3076":"#d5091d"}},[t._v(t._s(e.mes[4]))]):t._e(),1==t.type?s("div",{staticClass:"num-item ten-tiger",style:{color:"龙"===e.mes[5]?"#2e3076":"#d5091d"}},[t._v(t._s(e.mes[5]))]):t._e(),1==t.type?s("div",{staticClass:"num-item ten-tiger",style:{color:"龙"===e.mes[6]?"#2e3076":"#d5091d"}},[t._v(t._s(e.mes[6]))]):t._e(),1==t.type?s("div",{staticClass:"num-item ten-tiger",style:{color:"龙"===e.mes[7]?"#2e3076":"#d5091d"}},[t._v(t._s(e.mes[7]))]):t._e(),2==t.type?s("div",{staticClass:"num-item total",staticStyle:{color:"#767e84"}},[t._v(t._s(e.mes[0]))]):t._e(),2==t.type?s("div",{staticClass:"num-item total",style:{color:"大"===e.mes[1]?"#d5091d":"#2e3076"}},[t._v(t._s(e.mes[1]))]):t._e(),2==t.type?s("div",{staticClass:"num-item total",style:{color:"双"===e.mes[2]?"#d5091d":"#2e3076"}},[t._v(t._s(e.mes[2]))]):t._e(),2==t.type?s("div",{staticClass:"num-item tiger",style:{color:"龙"===e.mes[3]?"#2e3076":"和"===e.mes[3]?"#56181a":"#d5091d"}},[t._v(t._s(e.mes[3]))]):t._e(),2==t.type?s("div",{staticClass:"num-item three",style:{color:"豹子"===e.mes[4]?"#8f1623":"#0c5920"}},[t._v(t._s(e.mes[4]))]):t._e(),2==t.type?s("div",{staticClass:"num-item three",style:{color:"豹子"===e.mes[5]?"#8f1623":"#0c5920"}},[t._v(t._s(e.mes[5]))]):t._e(),2==t.type?s("div",{staticClass:"num-item three",style:{color:"豹子"===e.mes[6]?"#8f1623":"#0c5920"}},[t._v(t._s(e.mes[6]))]):t._e()],2)})),0),s("div",{directives:[{name:"show",rawName:"v-show",value:t.flush.end<t.flush.start&&t.flush.isFlushing,expression:"flush.end < flush.start && flush.isFlushing"}],staticClass:"flush-item flush-up"},[s("div",{staticClass:"downwrap-content"},[s("p",{class:["downwrap-progress",t.noMoreData?"":"rotate-progress"]}),s("p",{staticClass:"downwrap-tips"},[t._v(t._s(t.noMoreData?"没有更多数据":"加载中"))])])])])]),s("vLoading",{attrs:{vShow:t.isLoading}})],1)}),[],!1,null,"24fae75c",null);e.default=l.exports},9914:function(t,e,s){"use strict";var a=s("3afd");s.n(a).a},"9ecf":function(t,e,s){"use strict";var a=s("b3b5");s.n(a).a},a39e:function(t,e,s){"use strict";var a={props:{headerTitle:{type:String,default:""},addInfo:{type:String,default:""}},data:function(){return{vWidth:"372px"}},mounted:function(){/Android|webOS|iPhone|iPod|BlackBerry/i.test(navigator.userAgent)?this.vWidth=window.innerWidth+"px":this.vWidth=Math.ceil(375*window.innerHeight/667)+"px"},methods:{handleGoBack:function(){this.$emit("goBack")},handleGoBackThis:function(t){return this.$emit("goBack"),t.stopPropagation(),t.preventDefault(),!1}}},i=(s("4f46"),s("5511")),n=Object(i.a)(a,(function(){var t=this,e=t.$createElement,s=t._self._c||e;return s("div",{staticClass:"game-header",style:{width:t.vWidth}},[s("div",{staticClass:"go-back",on:{click:t.handleGoBack,touchstart:t.handleGoBackThis}}),s("p",{staticClass:"header-title"},[s("span",[t._v(t._s(t.headerTitle))]),s("span",{staticClass:"add-info"},[t._v(t._s(t.addInfo))])]),s("div",{staticClass:"additional"},[t._t("add")],2)])}),[],!1,null,null,null);e.a=n.exports},b3b5:function(t,e,s){},cc57:function(t,e,s){var a=s("064e").f,i=Function.prototype,n=/^\s*function ([^ (]*)/;"name"in i||s("149f")&&a(i,"name",{configurable:!0,get:function(){try{return(""+this).match(n)[1]}catch(t){return""}}})},e697:function(t,e,s){"use strict";var a=s("e46b"),i=s("013f")(5),n="find",r=!0;n in[]&&Array(1)[n]((function(){r=!1})),a(a.P+a.F*r,"Array",{find:function(t,e){return i(this,t,1<arguments.length?e:void 0)}}),s("0e8b")(n)}}]);
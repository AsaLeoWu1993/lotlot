(window.webpackJsonp=window.webpackJsonp||[]).push([["chunk-4e843f52"],{"013f":function(e,t,s){var i=s("4ce5"),o=s("224c"),n=s("008a"),a=s("eafa"),r=s("5dd2");e.exports=function(e,t){var s=1==e,u=2==e,l=3==e,c=4==e,d=6==e,m=5==e||d,p=t||r;return function(t,r,y){for(var v,h,f=n(t),w=o(f),k=i(r,y,3),g=a(w.length),S=0,_=s?p(t,g):u?p(t,0):void 0;S<g;S++)if((m||S in w)&&(h=k(v=w[S],S,f),e))if(s)_[S]=h;else if(h)switch(e){case 3:return!0;case 5:return v;case 6:return S;case 2:_.push(v)}else if(c)return!1;return d?-1:l||c?c:_}}},"042f":function(e,t,s){"use strict";var i={props:{vShow:{type:Boolean,default:!1},loadingText:{type:String,default:""}}},o=(s("9ecf"),s("5511")),n=Object(o.a)(i,(function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("div",{directives:[{name:"show",rawName:"v-show",value:e.vShow,expression:"vShow"}],staticClass:"loading"},[s("div",{staticClass:"loader"},[s("svg",{staticClass:"circular",attrs:{viewBox:"25 25 50 50"}},[s("circle",{staticClass:"path",attrs:{cx:"50",cy:"50",r:"20",fill:"none","stroke-width":"5","stroke-miterlimit":"10"}})]),s("p",{directives:[{name:"show",rawName:"v-show",value:e.loadingText,expression:"loadingText"}],staticClass:"loader-poptip"},[e._v(e._s(e.loadingText))])])])}),[],!1,null,"4df03536",null);t.a=n.exports},2346:function(e,t,s){var i=s("75c4");e.exports=Array.isArray||function(e){return"Array"==i(e)}},"405e":function(e,t,s){"use strict";var i=s("7019");s.n(i).a},"4f46":function(e,t,s){"use strict";var i=s("69bd");s.n(i).a},"5dd2":function(e,t,s){var i=s("81dc");e.exports=function(e,t){return new(i(e))(t)}},"69bd":function(e,t,s){},7019:function(e,t,s){},"81dc":function(e,t,s){var i=s("fb68"),o=s("2346"),n=s("cb3d")("species");e.exports=function(e){var t;return o(e)&&("function"!=typeof(t=e.constructor)||t!==Array&&!o(t.prototype)||(t=void 0),i(t)&&null===(t=t[n])&&(t=void 0)),void 0===t?Array:t}},"9ecf":function(e,t,s){"use strict";var i=s("b3b5");s.n(i).a},a39e:function(e,t,s){"use strict";var i={props:{headerTitle:{type:String,default:""},addInfo:{type:String,default:""}},data:function(){return{vWidth:"372px"}},mounted:function(){/Android|webOS|iPhone|iPod|BlackBerry/i.test(navigator.userAgent)?this.vWidth=window.innerWidth+"px":this.vWidth=Math.ceil(375*window.innerHeight/667)+"px"},methods:{handleGoBack:function(){this.$emit("goBack")},handleGoBackThis:function(e){return this.$emit("goBack"),e.stopPropagation(),e.preventDefault(),!1}}},o=(s("4f46"),s("5511")),n=Object(o.a)(i,(function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("div",{staticClass:"game-header",style:{width:e.vWidth}},[s("div",{staticClass:"go-back",on:{click:e.handleGoBack,touchstart:e.handleGoBackThis}}),s("p",{staticClass:"header-title"},[s("span",[e._v(e._s(e.headerTitle))]),s("span",{staticClass:"add-info"},[e._v(e._s(e.addInfo))])]),s("div",{staticClass:"additional"},[e._t("add")],2)])}),[],!1,null,null,null);t.a=n.exports},b3b5:function(e,t,s){},bd86:function(e,t,s){"use strict";s.r(t),s("c904"),s("cc57"),s("e697");var i=s("a39e"),o=s("042f"),n=s("702a"),a={components:{gameHeader:i.a,vLoading:o.a},data:function(){return{isLoading:!1,gameType:"",gameName:"",colsHeaderList:{num:"数字",position:"大小单双",tigerOrLong:"龙虎",sumAll:"和值",sumPosition:"和值大小单双",backThree:"前中后三"},showList:[],colsList:[{desc:"1",key:"Num1",type:"num",isShow:!1,value:""},{desc:"2",key:"Num2",type:"num",isShow:!1,value:""},{desc:"3",key:"Num3",type:"num",isShow:!1,value:""},{desc:"4",key:"Num4",type:"num",isShow:!1,value:""},{desc:"5",key:"Num5",type:"num",isShow:!1,value:""},{desc:"6",key:"Num6",type:"num",isShow:!1,value:""},{desc:"7",key:"Num7",type:"num",isShow:!1,value:""},{desc:"8",key:"Num8",type:"num",isShow:!1,value:""},{desc:"9",key:"Num9",type:"num",isShow:!1,value:""},{desc:"0",key:"Num0",type:"num",isShow:!1,value:""},{desc:"10",key:"Num10",type:"num",isShow:!1,value:""},{desc:"大",key:"Da",type:"position",isShow:!1,value:"",order:1},{desc:"小",key:"Xiao",type:"position",isShow:!1,value:"",order:2},{desc:"单",key:"Dan",type:"position",isShow:!1,value:"",order:3},{desc:"双",key:"Shuang",type:"position",isShow:!1,value:"",order:4},{desc:"龙",key:"Long",type:"tigerOrLong",isShow:!1,value:"",order:1},{desc:"虎",key:"Hu",type:"tigerOrLong",isShow:!1,value:"",order:2},{desc:"和",key:"He",type:"tigerOrLong",isShow:!1,value:"",order:3},{desc:"3",key:"SNum3",type:"sumAll",isShow:!1,value:""},{desc:"4",key:"SNum4",type:"sumAll",isShow:!1,value:""},{desc:"5",key:"SNum5",type:"sumAll",isShow:!1,value:""},{desc:"6",key:"SNum6",type:"sumAll",isShow:!1,value:""},{desc:"7",key:"SNum7",type:"sumAll",isShow:!1,value:""},{desc:"8",key:"SNum8",type:"sumAll",isShow:!1,value:""},{desc:"9",key:"SNum9",type:"sumAll",isShow:!1,value:""},{desc:"10",key:"SNum10",type:"sumAll",isShow:!1,value:""},{desc:"11",key:"SNum11",type:"sumAll",isShow:!1,value:""},{desc:"12",key:"SNum12",type:"sumAll",isShow:!1,value:""},{desc:"13",key:"SNum13",type:"sumAll",isShow:!1,value:""},{desc:"14",key:"SNum14",type:"sumAll",isShow:!1,value:""},{desc:"15",key:"SNum15",type:"sumAll",isShow:!1,value:""},{desc:"16",key:"SNum16",type:"sumAll",isShow:!1,value:""},{desc:"17",key:"SNum17",type:"sumAll",isShow:!1,value:""},{desc:"18",key:"SNum18",type:"sumAll",isShow:!1,value:""},{desc:"19",key:"SNum19",type:"sumAll",isShow:!1,value:""},{desc:"大",key:"SDa",type:"sumPosition",isShow:!1,value:"",order:1},{desc:"小",key:"SXiao",type:"sumPosition",isShow:!1,value:"",order:2},{desc:"单",key:"SDan",type:"sumPosition",isShow:!1,value:"",order:3},{desc:"双",key:"SShuang",type:"sumPosition",isShow:!1,value:"",order:4},{desc:"大",key:"CDa",type:"sumPosition",isShow:!1,value:"",order:1},{desc:"小",key:"CXiao",type:"sumPosition",isShow:!1,value:"",order:2},{desc:"单",key:"CDan",type:"sumPosition",isShow:!1,value:"",order:3},{desc:"双",key:"CShuang",type:"sumPosition",isShow:!1,value:"",order:4},{desc:"豹子",key:"Baozi",type:"backThree",isShow:!1,value:""},{desc:"顺子",key:"Shunzi",type:"backThree",isShow:!1,value:""},{desc:"半顺",key:"Banshun",type:"backThree",isShow:!1,value:""},{desc:"对子",key:"Duizi",type:"backThree",isShow:!1,value:""},{desc:"杂六",key:"Zaliu",type:"backThree",isShow:!1,value:""},{desc:"庄",key:"Banker",type:"bjl",isShow:!1,value:"",order:1},{desc:"闲",key:"Player",type:"bjl",isShow:!1,value:"",order:2},{desc:"和",key:"He",type:"bjl",isShow:!1,value:"",order:3},{desc:"庄对",key:"BankerPair",type:"bjl",isShow:!1,value:"",order:4},{desc:"闲对",key:"PlayerPair",type:"bjl",isShow:!1,value:"",order:5},{desc:"任意对子",key:"AnyPair",type:"bjl",isShow:!1,value:"",order:6}],roomType:""}},beforeRouteEnter:function(e,t,s){s((function(e){var t=e.getLocalData("gameType");e.roomType=e.getLocalData("roomType"),n.a.$emit("navigateShow",!1),t&&(e.gameType=t,e.handleGetGameList().then((function(){var s=e.gameList.find((function(e){return e.type==t}));s?e.gameName=s.name:(s=e.gameList.find((function(e){return-1!=t.indexOf(e.key)})))&&(e.gameName=s.name)&&(e.gameType=s.type),e.getData()})))}))},methods:{handleGetGameList:function(){var e=this;return new Promise((function(t,s){e.$axios({url:"/api/SwUser/GetGameList"}).then((function(i){if(100===i.data.Status){var o=i.data.Data;e.gameList=o.map((function(e){return{name:e.GameName,type:e.GameType,key:e.NickName,gameType:e.Type}})).sort((function(e,t){return e.type-t.type})),e.gameList.push({name:"百家乐",type:1,key:"bjl",gameType:"video"}),t()}s()}))}))},goBack:function(){var e=this;"lottery"==e.getLocalData("roomType")?e.$router.push({name:"game",params:{id:e.gameType}}):e.$router.push({name:"video",params:{id:e.gameType}})},getData:function(){var e=this,t="";e.isLoading||(e.isLoading=!0,t="lottery"==e.roomType?"/api/SwUser/GetGameOdds":"/api/SwUser/GetVideoGameOdds",e.$axios({url:t,params:{gameType:e.gameType}}).then((function(t){if(e.isLoading=!1,100===t.data.Status){var s=t.data.Model,i=[],o=function(t){var o=void 0;(o="He"==t?"lottery"==e.roomType?e.colsList.find((function(e){return e.key===t})):e.colsList.find((function(e){return e.key===t&&"bjl"==e.type})):e.colsList.find((function(e){return e.key===t})))&&(o.value=s[t],i.push(o))};for(var n in s)o(n);e.showList=i}})).catch((function(t){e.isLoading=!1})))}}},r=(s("405e"),s("5511")),u=Object(r.a)(a,(function(){var e=this,t=e.$createElement,s=e._self._c||t;return s("div",{staticClass:"odds my-position"},[s("gameHeader",{attrs:{headerTitle:"赔率介绍 ["+e.gameName+"]"},on:{goBack:e.goBack}}),s("div",{staticClass:"odds-content"},[s("div",{directives:[{name:"show",rawName:"v-show",value:e.showList.filter((function(e){return"tigerOrLong"===e.type})).length&&"lottery"==e.roomType,expression:"showList.filter(item => item.type === 'tigerOrLong').length && roomType == 'lottery'"}],staticClass:"tigerOrLong"},[s("div",{staticClass:"num-item item-title"},[s("p",[e._v(e._s(e.colsHeaderList.tigerOrLong))]),s("p",[e._v("赔率")])]),e._l(e.showList.filter((function(e){return"tigerOrLong"===e.type})).sort((function(e,t){return e.order-t.order})),(function(t,i){return s("div",{key:i,staticClass:"num-item"},[s("p",[e._v(e._s(t.desc))]),s("p",[e._v(e._s(t.value))])])}))],2),s("div",{directives:[{name:"show",rawName:"v-show",value:e.showList.filter((function(e){return"sumPosition"===e.type})).length&&"lottery"==e.roomType,expression:"showList.filter(item => item.type === 'sumPosition').length && roomType == 'lottery'"}],staticClass:"sumPosition"},[s("div",{staticClass:"num-item item-title"},[s("p",[e._v(e._s(e.colsHeaderList.sumPosition))]),s("p",[e._v("赔率")])]),e._l(e.showList.filter((function(e){return"sumPosition"===e.type})).sort((function(e,t){return e.order-t.order})),(function(t,i){return s("div",{key:i,staticClass:"num-item"},[s("p",[e._v(e._s(t.desc))]),s("p",[e._v(e._s(t.value))])])}))],2),s("div",{directives:[{name:"show",rawName:"v-show",value:e.showList.filter((function(e){return"position"===e.type})).length&&"lottery"==e.roomType,expression:"showList.filter(item => item.type === 'position').length && roomType == 'lottery'"}],staticClass:"position"},[s("div",{staticClass:"num-item item-title"},[s("p",[e._v(e._s(e.colsHeaderList.position))]),s("p",[e._v("赔率")])]),e._l(e.showList.filter((function(e){return"position"===e.type})).sort((function(e,t){return e.order-t.order})),(function(t,i){return s("div",{key:i,staticClass:"num-item"},[s("p",[e._v(e._s(t.desc))]),s("p",[e._v(e._s(t.value))])])}))],2),s("div",{directives:[{name:"show",rawName:"v-show",value:e.showList.filter((function(e){return"backThree"===e.type})).length&&"lottery"==e.roomType,expression:"showList.filter(item => item.type === 'backThree').length && roomType == 'lottery'"}],staticClass:"backThree"},[s("div",{staticClass:"num-item item-title"},[s("p",[e._v(e._s(e.colsHeaderList.num))]),s("p",[e._v("赔率")])]),e._l(e.showList.filter((function(e){return"backThree"===e.type})),(function(t,i){return s("div",{key:i,staticClass:"num-item"},[s("p",[e._v(e._s(t.desc))]),s("p",[e._v(e._s(t.value))])])}))],2),s("div",{directives:[{name:"show",rawName:"v-show",value:e.showList.filter((function(e){return"num"===e.type})).length&&"lottery"==e.roomType,expression:"showList.filter(item => item.type === 'num').length && roomType == 'lottery'"}],staticClass:"num"},[s("div",{staticClass:"num-item item-title"},[s("p",[e._v(e._s(e.colsHeaderList.num))]),s("p",[e._v("赔率")])]),e._l(e.showList.filter((function(e){return"num"===e.type})).sort((function(e,t){return parseInt(e.desc)>=parseInt(t.desc)?1:-1})),(function(t,i){return s("div",{key:i,staticClass:"num-item"},[s("p",[e._v(e._s(t.desc))]),s("p",[e._v(e._s(t.value))])])}))],2),s("div",{directives:[{name:"show",rawName:"v-show",value:e.showList.filter((function(e){return"sumAll"===e.type})).length&&"lottery"==e.roomType,expression:"showList.filter(item => item.type === 'sumAll').length && roomType == 'lottery'"}],staticClass:"sumAll"},[s("div",{staticClass:"num-item item-title"},[s("p",[e._v(e._s(e.colsHeaderList.sumAll))]),s("p",[e._v("赔率")])]),e._l(e.showList.filter((function(e){return"sumAll"===e.type})).sort((function(e,t){return parseInt(e.desc)>=parseInt(t.desc)?1:-1})),(function(t,i){return s("div",{key:i,staticClass:"num-item"},[s("p",[e._v(e._s(t.desc))]),s("p",[e._v(e._s(t.value))])])}))],2),s("div",{directives:[{name:"show",rawName:"v-show",value:e.showList.filter((function(e){return"bjl"===e.type})).length&&"video"==e.roomType,expression:"showList.filter(item => item.type === 'bjl').length && roomType == 'video'"}],staticClass:"sumAll"},e._l(e.showList.filter((function(e){return"bjl"===e.type})).sort((function(e,t){return parseInt(e.order)>=parseInt(t.order)?1:-1})),(function(t,i){return s("div",{key:i,staticClass:"num-item"},[s("p",[e._v(e._s(t.desc))]),s("p",[e._v(e._s(t.value))])])})),0)]),s("vLoading",{attrs:{vShow:e.isLoading}})],1)}),[],!1,null,"da41165a",null);t.default=u.exports},cc57:function(e,t,s){var i=s("064e").f,o=Function.prototype,n=/^\s*function ([^ (]*)/;"name"in o||s("149f")&&i(o,"name",{configurable:!0,get:function(){try{return(""+this).match(n)[1]}catch(e){return""}}})},e697:function(e,t,s){"use strict";var i=s("e46b"),o=s("013f")(5),n="find",a=!0;n in[]&&Array(1)[n]((function(){a=!1})),i(i.P+i.F*a,"Array",{find:function(e,t){return o(this,e,1<arguments.length?t:void 0)}}),s("0e8b")(n)}}]);
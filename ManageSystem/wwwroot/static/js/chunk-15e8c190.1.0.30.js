(window.webpackJsonp=window.webpackJsonp||[]).push([["chunk-15e8c190"],{"084d":function(t,e,n){},"4f46":function(t,e,n){"use strict";var a=n("69bd");n.n(a).a},"69bd":function(t,e,n){},"891d":function(t,e,n){"use strict";n.r(e),n("cc57");var a=n("a39e"),c=n("702a"),i={components:{gameHeader:a.a},data:function(){return{gameName:"",gameType:"ten",list:{ten:[{con1:"玩法一：（大小单双）",con2:"",con3:"",con4:"",con5:"12大100",con6:"第一名大下注100元,第二名大下注100元",con7:"第一名开大中奖，第二名开大也中奖"},{con1:"玩法二：（定位）",con2:"",con3:"",con4:"",con5:"123/1/100",con6:"第一名 第二名 第三名都买了1号车100元",con7:"1号车跑到 第一名 第二名 第三名为中奖"},{con1:"玩法三：（和值）",con2:"和341819/100",con3:"和3下注100、和4下注100",con4:"和18下注100、和19下注100"}],five:[{con1:"玩法一：（龙虎和）",con2:"",con3:"",con4:"万位大于个位则中奖龙",con5:"",con6:"万位小于个位则中奖虎",con7:"万位等于个位则中奖和"},{con1:"玩法二：（1-5定位）",con2:"",con3:"2/012/100 千位012各下注100元",con4:"3/567/100 百位567各下注100元",con5:"",con6:"",con7:""},{con1:"玩法三：（1-5名大小单双）",con2:"",con3:"3大100 百位大下注100元",con4:"1大100 万位大下注100元"}]}}},beforeRouteEnter:function(t,e,n){n((function(e){var n=t.params.name,a=t.params.type;c.a.$emit("navigateShow",!1),n&&a?(e.gameName=n,e.gameType=1==a?"ten":"five"):e.goBack()}))},methods:{goBack:function(){var t=this.getLocalData("gameType");this.$router.push({name:"game",params:{id:t}})}}},o=(n("d7db"),n("5511")),s=Object(o.a)(i,(function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("div",{staticClass:"play-method my-position"},[n("gameHeader",{attrs:{headerTitle:"玩法介绍 ["+t.gameName+"]"},on:{goBack:t.goBack}}),n("div",{staticClass:"play-content"},[t._m(0),n("div",{staticClass:"rule-list"},t._l(t.list[t.gameType],(function(e,a){return n("div",{key:a,staticClass:"rule-item"},[n("div",{staticClass:"rule-item-box"},[e.con1?n("p",{class:["rule-item-header","rule-item-"+a]},[t._v(t._s(e.con1))]):t._e(),e.con2?n("p",{staticClass:"rule-item-i"},[t._v(t._s(e.con2))]):t._e(),e.con3?n("p",{staticClass:"rule-item-i"},[t._v(t._s(e.con3))]):t._e(),e.con4?n("p",{staticClass:"rule-item-i"},[t._v(t._s(e.con4))]):t._e(),e.con5?n("p",{staticClass:"rule-item-i"},[t._v(t._s(e.con5))]):t._e(),e.con6?n("p",{staticClass:"rule-item-i"},[t._v(t._s(e.con6))]):t._e(),e.con7?n("p",{staticClass:"rule-item-i"},[t._v(t._s(e.con7))]):t._e()])])})),0),t._m(1)])],1)}),[function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("div",{staticClass:"rule-header"},[n("p",[t._v("下注格式（通用）:位置/玩法/金额")])])},function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("div",{staticClass:"rule-footer"},[n("p",[t._v("说明：")]),n("p",[t._v("当下注内容包含汉字的，与其相邻的免去间隔符“/”")])])}],!1,null,"0fa938d7",null);e.default=s.exports},a39e:function(t,e,n){"use strict";var a={props:{headerTitle:{type:String,default:""},addInfo:{type:String,default:""}},data:function(){return{vWidth:"372px"}},mounted:function(){/Android|webOS|iPhone|iPod|BlackBerry/i.test(navigator.userAgent)?this.vWidth=window.innerWidth+"px":this.vWidth=Math.ceil(375*window.innerHeight/667)+"px"},methods:{handleGoBack:function(){this.$emit("goBack")},handleGoBackThis:function(t){return this.$emit("goBack"),t.stopPropagation(),t.preventDefault(),!1}}},c=(n("4f46"),n("5511")),i=Object(c.a)(a,(function(){var t=this,e=t.$createElement,n=t._self._c||e;return n("div",{staticClass:"game-header",style:{width:t.vWidth}},[n("div",{staticClass:"go-back",on:{click:t.handleGoBack,touchstart:t.handleGoBackThis}}),n("p",{staticClass:"header-title"},[n("span",[t._v(t._s(t.headerTitle))]),n("span",{staticClass:"add-info"},[t._v(t._s(t.addInfo))])]),n("div",{staticClass:"additional"},[t._t("add")],2)])}),[],!1,null,null,null);e.a=i.exports},cc57:function(t,e,n){var a=n("064e").f,c=Function.prototype,i=/^\s*function ([^ (]*)/;"name"in c||n("149f")&&a(c,"name",{configurable:!0,get:function(){try{return(""+this).match(i)[1]}catch(t){return""}}})},d7db:function(t,e,n){"use strict";var a=n("084d");n.n(a).a}}]);
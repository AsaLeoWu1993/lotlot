(window.webpackJsonp=window.webpackJsonp||[]).push([["chunk-21a92952"],{"042f":function(t,a,e){"use strict";var i={props:{vShow:{type:Boolean,default:!1},loadingText:{type:String,default:""}}},n=(e("9ecf"),e("5511")),s=Object(n.a)(i,(function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{directives:[{name:"show",rawName:"v-show",value:t.vShow,expression:"vShow"}],staticClass:"loading"},[e("div",{staticClass:"loader"},[e("svg",{staticClass:"circular",attrs:{viewBox:"25 25 50 50"}},[e("circle",{staticClass:"path",attrs:{cx:"50",cy:"50",r:"20",fill:"none","stroke-width":"5","stroke-miterlimit":"10"}})]),e("p",{directives:[{name:"show",rawName:"v-show",value:t.loadingText,expression:"loadingText"}],staticClass:"loader-poptip"},[t._v(t._s(t.loadingText))])])])}),[],!1,null,"4df03536",null);a.a=s.exports},"2c53":function(t,a,e){},"2e5f":function(t,a,e){"use strict";e.r(a),e("6491");var i=e("a39e"),n=e("042f"),s=e("702a"),o={components:{gameHeader:i.a,vLoading:n.a},data:function(){return{wechatImg:"",alipayImg:"",gameType:"",wechatNum:"",alipayNum:"",isLoading:!1,bigShow:!1,bigQrcode:""}},beforeRouteEnter:function(t,a,e){e((function(t){var a=t.getLocalData("gameType");s.a.$emit("navigateShow",!1),a&&(t.gameType=a,setTimeout((function(){t.getData()}),300))}))},methods:{handleSaveImg:function(a){var e=this;if(a){a.startsWith("http")||(a=encodeURI(location.origin+a));try{var i=function(){api.download({url:a,encode:!0,report:!0,cache:!0,allowResume:!0,savePath:"fs://temp".concat(a.slice(a.lastIndexOf("/")+1))},(function(t,a){2==t.state&&e.showPoptip("二维码下载失败","error",1200),1==t.state?api.saveMediaToAlbum({path:t.savePath},(function(t,a){t&&t.status?e.showPoptip("二维码已保存至相册","success",1200):e.showPoptip("二维码保存失败","error",1200)})):e.showPoptip("二维码保存失败","error",1200)}))};api.hasPermission({list:["photos"]})[0].granted?i():api.requestPermission({list:["photos"],code:1},(function(t,a){list.list[0].granted&&i()}))}catch(t){e.bigQrcode=a,e.bigShow=!0}}else e.showPoptip("没有设置收款二维码","error",1200)},handleShowAccount:function(t){this.showPoptip(t,"success",1200)},goBack:function(){this.$router.push({name:"game",params:{id:this.gameType}})},getData:function(){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/SwUser/GetQRCode"}).then((function(a){if(t.isLoading=!1,100===a.data.Status){var e=a.data.Model;e.WeChatQRcode||e.AlipayQRcode?(t.wechatImg=e.WeChatQRcode?t.getPicUrl(e.WeChatQRcode):"",t.alipayImg=e.AlipayQRcode?t.getPicUrl(e.AlipayQRcode):"",t.alipayNum=e.AlipayNum,t.wechatNum=e.WeChatNum):t.showPoptip("暂无数据","error",1200)}else t.getData()})).catch((function(a){t.isLoading=!1})))}}},c=(e("de01"),e("5511")),r=Object(c.a)(o,(function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"qrCode my-position"},[e("gameHeader",{attrs:{headerTitle:"扫码上分"},on:{goBack:t.goBack}}),e("div",{staticClass:"qr-content"},[e("img",{staticClass:"alipay-icon",attrs:{src:t.wechatImg,alt:""},on:{click:function(a){return t.handleSaveImg(t.wechatImg)}}}),e("p",{staticClass:"alipay-account",on:{click:function(a){return t.handleShowAccount(t.wechatNum)}}},[t._v("账号:"+t._s(t.wechatNum))]),e("img",{staticClass:"wechat-icon",attrs:{src:t.alipayImg,alt:""},on:{click:function(a){return t.handleSaveImg(t.alipayImg)}}}),e("p",{staticClass:"wechat-account",on:{click:function(a){return t.handleShowAccount(t.alipayNum)}}},[t._v("账号:"+t._s(t.alipayNum))])]),t._m(0),e("vLoading",{attrs:{vShow:t.isLoading}}),e("div",{directives:[{name:"show",rawName:"v-show",value:t.bigShow,expression:"bigShow"}],staticClass:"qrcode-png",on:{click:function(a){t.bigShow=!1}}},[e("div",[e("img",{staticStyle:{"max-width":"80%","max-height":"80%"},attrs:{src:t.bigQrcode,alt:""}}),e("p",[t._v("请使用手机截图保存")])])])],1)}),[function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"poptip"},[e("div",{staticClass:"poptip-header"},[t._v("操作步骤：")]),e("div",{staticClass:"poptip-item"},[t._v("1.点击二维码图片将自动为您保存到相册")]),e("div",{staticClass:"poptip-item"},[t._v("2.在支付宝/微信中打开扫一扫。")]),e("div",{staticClass:"poptip-item"},[t._v("3.在扫一扫中点击右上角，选择从相册中读取二维码。")]),e("div",{staticClass:"poptip-item"},[t._v("4.充值未到账请联系客服。")])])}],!1,null,"57e833c6",null);a.default=r.exports},"4f46":function(t,a,e){"use strict";var i=e("69bd");e.n(i).a},6491:function(t,a,e){"use strict";var i=e("e46b"),n=e("eafa"),s=e("7c0a"),o="startsWith",c=""[o];i(i.P+i.F*e("bc96")(o),"String",{startsWith:function(t,a){var e=s(this,t,o),i=n(Math.min(1<arguments.length?a:void 0,e.length)),r=String(t);return c?c.call(e,r,i):e.slice(i,i+r.length)===r}})},"69bd":function(t,a,e){},"7c0a":function(t,a,e){var i=e("2fd4"),n=e("f6b4");t.exports=function(t,a,e){if(i(a))throw TypeError("String#"+e+" doesn't accept regex!");return String(n(t))}},"9ecf":function(t,a,e){"use strict";var i=e("b3b5");e.n(i).a},a39e:function(t,a,e){"use strict";var i={props:{headerTitle:{type:String,default:""},addInfo:{type:String,default:""}},data:function(){return{vWidth:"372px"}},mounted:function(){/Android|webOS|iPhone|iPod|BlackBerry/i.test(navigator.userAgent)?this.vWidth=window.innerWidth+"px":this.vWidth=Math.ceil(375*window.innerHeight/667)+"px"},methods:{handleGoBack:function(){this.$emit("goBack")},handleGoBackThis:function(t){return this.$emit("goBack"),t.stopPropagation(),t.preventDefault(),!1}}},n=(e("4f46"),e("5511")),s=Object(n.a)(i,(function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"game-header",style:{width:t.vWidth}},[e("div",{staticClass:"go-back",on:{click:t.handleGoBack,touchstart:t.handleGoBackThis}}),e("p",{staticClass:"header-title"},[e("span",[t._v(t._s(t.headerTitle))]),e("span",{staticClass:"add-info"},[t._v(t._s(t.addInfo))])]),e("div",{staticClass:"additional"},[t._t("add")],2)])}),[],!1,null,null,null);a.a=s.exports},b3b5:function(t,a,e){},bc96:function(t,a,e){var i=e("cb3d")("match");t.exports=function(a){var e=/./;try{"/./"[a](e)}catch(t){try{return e[i]=!1,!"/./"[a](e)}catch(t){}}return!0}},de01:function(t,a,e){"use strict";var i=e("2c53");e.n(i).a}}]);
(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-f99180c8"],{"465a":function(e,t,o){"use strict";var a=o("dd43"),n=o.n(a);n.a},"9ed6":function(e,t,o){"use strict";o.r(t);var a=function(){var e=this,t=e.$createElement,o=e._self._c||t;return o("div",{staticClass:"login"},[o("div",{staticClass:"content1"},[o("form",[o("div",{staticClass:"content-item"},[o("input",{directives:[{name:"model",rawName:"v-model",value:e.loginForm.account,expression:"loginForm.account"}],staticClass:"item-input",attrs:{placeholder:"请输入用户名或邮箱"},domProps:{value:e.loginForm.account},on:{input:function(t){t.target.composing||e.$set(e.loginForm,"account",t.target.value)}}})]),o("div",{staticClass:"content-item"},[o("input",{directives:[{name:"model",rawName:"v-model",value:e.loginForm.password,expression:"loginForm.password"}],staticClass:"item-input",attrs:{type:"password",placeholder:"请输入密码"},domProps:{value:e.loginForm.password},on:{input:function(t){t.target.composing||e.$set(e.loginForm,"password",t.target.value)}}})]),o("div",{staticClass:"content-item"},[o("input",{directives:[{name:"model",rawName:"v-model",value:e.loginForm.verityCode,expression:"loginForm.verityCode"}],staticClass:"item-verify",attrs:{placeholder:"验证码"},domProps:{value:e.loginForm.verityCode},on:{input:function(t){t.target.composing||e.$set(e.loginForm,"verityCode",t.target.value)}}}),o("img",{staticClass:"content-item-verity",attrs:{src:e.loginForm.veritySrc},on:{click:function(t){return e.handleReset(t)}}})]),o("div",{staticClass:"submit",on:{click:e.handleSubmit}})])]),o("div",{directives:[{name:"show",rawName:"v-show",value:e.showPoptip,expression:"showPoptip"}],staticClass:"poptip-shade"},[o("div",{staticClass:"poptip"},[o("p",[e._v("因您退出后台时间超过24小时，机器人下注行为自动暂停了，点击确定后机器人行为将重新激活！")]),o("div",{staticClass:"enter",on:{click:e.handleGoIndex}},[o("span",[e._v("确定")])])])])])},n=[],i={data:function(){return{loginForm:{account:"",password:"",verityCode:"",veritySrc:"/api/Merchant/MixVerifyCode?time="+(new Date).getTime()},loading:!1,showPoptip:!1}},beforeRouteEnter:function(e,t,o){o(function(e){window.addEventListener("keydown",e.enterClick)})},beforeRouteLeave:function(e,t,o){var a=this;window.removeEventListener("keydown",a.enterClick),o()},methods:{handleReset:function(e){e.target.src="/api/Merchant/MixVerifyCode?time="+(new Date).getTime()},enterClick:function(e){var t=this;13===e.keyCode&&t.handleSubmit()},handleGoIndex:function(){var e=this;localStorage.getItem("userId")&&e.$router.push({name:"index"})},handleSubmit:function(){var e=this;e.loginForm.account?e.loginForm.password?e.loginForm.verityCode?e.loading||(e.loading=!0,e.axios({url:"/api/Merchant/MerchantLogin",method:"get",params:{name:e.loginForm.account,password:e.loginForm.password,code:e.loginForm.verityCode}}).then(function(t){100===t.data.Status?(localStorage.setItem("merchantID",t.data.Model.MerchantID),localStorage.setItem("loginName",t.data.Model.MerchantName),localStorage.setItem("userId",t.data.Model.MerchantID),localStorage.setItem("password",e.loginForm.password),localStorage.setItem("roomNum",t.data.Model.RoomNum),localStorage.setItem("safetyCode",t.data.Model.SeurityNo),localStorage.setItem("againVerify",t.data.Model.Status),localStorage.setItem("openVerify",t.data.Model.Status),localStorage.setItem("MarsCurrency",t.data.Model.MarsCurrency),e.setLocalData("MerchantAuthorization",t.data.Model.Key),e.setLocalData("Token",t.data.Model.Token),t.data.Model.Tips?e.showPoptip=!0:e.$router.push({name:"index"}),setTimeout(function(){e.loading=!1},1500)):(e.loading=!1,e.$Message.error(t.data.Message))}).catch(function(t){e.loading=!1})):e.$Message.error("请输入验证码"):e.$Message.error("请输入密码"):e.$Message.error("请输入账号")}}},r=i,s=(o("465a"),o("6691")),l=Object(s["a"])(r,a,n,!1,null,"109f9fdc",null);t["default"]=l.exports},dd43:function(e,t,o){}}]);
//# sourceMappingURL=chunk-f99180c8.1588215759171.js.map
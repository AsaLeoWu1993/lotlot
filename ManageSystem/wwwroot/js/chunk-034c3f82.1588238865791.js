(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-034c3f82"],{"0857":function(e,t,a){"use strict";a("a0e0");var s=a("a013"),i=a("f425"),o=a("dad2"),n="toString",r=/./[n],l=function(e){a("e5ef")(RegExp.prototype,n,e,!0)};a("b6f1")(function(){return"/a/b"!=r.call({source:"a",flags:"b"})})?l(function(){var e=s(this);return"/".concat(e.source,"/","flags"in e?e.flags:!o&&e instanceof RegExp?i.call(e):void 0)}):r.name!=n&&l(function(){return r.call(this)})},"0d0f":function(e,t,a){"use strict";var s=a("854c"),i=a.n(s);i.a},"3c6b":function(e,t,a){"use strict";var s=a("a013"),i=a("b146"),o=a("b0f4"),n=a("35dd");a("629c")("match",1,function(e,t,a,r){return[function(a){var s=e(this),i=void 0==a?void 0:a[t];return void 0!==i?i.call(a,s):new RegExp(a)[t](String(s))},function(e){var t=r(a,e,this);if(t.done)return t.value;var l=s(e),c=String(this);if(!l.global)return n(l,c);var d=l.unicode;l.lastIndex=0;var u,h=[],p=0;while(null!==(u=n(l,c))){var m=String(u[0]);h[p]=m,""===m&&(l.lastIndex=o(c,i(l.lastIndex),d)),p++}return 0===p?null:h}]})},"854c":function(e,t,a){},a0e0:function(e,t,a){a("dad2")&&"g"!=/./g.flags&&a("ddf7").f(RegExp.prototype,"flags",{configurable:!0,get:a("f425")})},a341:function(e,t,a){"use strict";a.r(t);var s=function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"content"},[a("div",{staticClass:"query"},[a("div",{staticClass:"query-item"},[a("i-select",{staticStyle:{"margin-left":"10px",width:"220px"},on:{"on-change":e.handleChangeType},model:{value:e.selectUserType,callback:function(t){e.selectUserType=t},expression:"selectUserType"}},e._l(e.userTypeList,function(t,s){return a("i-option",{key:s,attrs:{value:t.value}},[e._v(e._s(t.title))])}),1),a("i-input",{staticStyle:{width:"160px"},attrs:{placeholder:"搜索用户ID或者用户昵称"},model:{value:e.searchUsername,callback:function(t){e.searchUsername=t},expression:"searchUsername"}}),a("div",{staticClass:"search",on:{click:e.handleSearch}},[e._v("搜索")]),a("div",{staticClass:"member-num"},[e._v("当前注册会员："),a("span",{staticStyle:{color:"#ff9933"},domProps:{textContent:e._s(e.memberNum)}}),e._v("（最大注册数200）")])],1),e._m(0)]),a("div",{staticClass:"list"},[e._m(1),a("div",{staticClass:"list-content"},e._l(e.userData,function(t,s){return a("div",{key:s,staticClass:"list-item"},[a("div",{staticClass:"item-info item-1"},[a("img",{attrs:{src:t.header,alt:"",title:""},on:{error:function(a){return e.handleImageError(t)}}})]),a("div",{staticClass:"item-info item-2"},[a("p",[e._v(e._s(t.username))])]),a("div",{staticClass:"item-info item-3"},[a("span",[e._v(e._s(t.showId))])]),a("div",{staticClass:"item-info item-4"},[a("span",[e._v(e._s(t.balance))])]),a("div",{staticClass:"item-info item-5"},[a("span",[e._v(e._s(t.proLoss))])]),a("div",{staticClass:"item-info item-6"},[a("span",[e._v(e._s(t.betGameName||"无"))])]),a("div",{staticClass:"item-info item-7"},[a("switchs",{attrs:{open:t.record},on:{switch:function(a){return e.handleChangeFlyingSheet(t)}}})],1),a("div",{staticClass:"item-info item-8"},[a("div",{staticClass:"operate"},[a("span",{on:{click:function(a){return e.handleScoreHistory(t)}}},[e._v("上下分记录")]),a("span",{on:{click:function(a){return e.handleAccountHistory(t)}}},[e._v("账变记录")]),a("span",{class:["正常"===t.status?"":"no-show"],on:{click:function(a){return e.handleCollecAcction(t)}}},[e._v("收款账户")]),a("span",{style:{cursor:t.isFalse?"default":"pointer"},on:{click:function(a){return e.handleChange(t)}}},[e._v(e._s(t.isFalse?"机器人托":"假人"===t.status?"切换真人":"切换假人"))]),a("span",{class:["正常"===t.status||"冻结"===t.status?"":"no-show"],on:{click:function(a){return e.handleFreezing(t)}}},[e._v(e._s("冻结"!==t.status?"冻结":"解冻"))]),a("span",{on:{click:function(a){return e.handleDelete(t)}}},[e._v("删除")]),a("span",{class:["正常"===t.status?"":"no-show"],on:{click:function(a){return e.handleAgent(t)}}},[e._v(e._s(t.isAgent?"取消代理":"设为代理"))])]),a("span",{staticClass:"other-setting",on:{click:function(a){return e.handleSettingUser(t)}}},[e._v("账户设置")])])])}),0)]),a("Page",{staticClass:"footer-page",attrs:{total:e.page.total,"page-size":e.page.pageSize,current:e.page.pageNum,"show-sizer":""},on:{"on-page-size-change":e.handleChangeSize,"on-change":e.handleChangePage}}),a("div",{directives:[{name:"show",rawName:"v-show",value:e.showUserInfo||e.showLotteryRecode||e.showBranchRecode||e.showAccount,expression:"showUserInfo || showLotteryRecode || showBranchRecode || showAccount"}],staticClass:"modal-mask",style:{"align-items":e.clientHeight<800?"flex-start":"center"}},[a("div",{directives:[{name:"show",rawName:"v-show",value:e.showUserInfo,expression:"showUserInfo"}],staticClass:"user-info user-setting"},[a("div",{staticClass:"user-info-header"},[a("span",[e._v("用户信息")]),a("span",{staticClass:"modal-close",on:{click:function(t){return e.handleCloseModal("showUserInfo")}}},[e._v("X")])]),a("div",{staticClass:"user-info-c"},[a("div",{staticClass:"user-top top-1"},[a("div",{staticClass:"user-info-h"},[a("img",{attrs:{src:e.imgDataUrl||e.userInfo.header,alt:"",title:""}}),a("p",{staticClass:"change-header",on:{click:function(t){e.showUploadHeader=!e.showUploadHeader}}},[e._v("更改头像")])]),a("div",{staticClass:"user-info-r"},[a("div",{staticClass:"usre-top-i"},[a("div",{staticClass:"user-info-c-i"},[a("span",{staticClass:"user-info-c-i-l"},[e._v("昵称")]),a("i-input",{staticClass:"user-info-c-i-r",attrs:{disabled:""},model:{value:e.userInfo.nickname,callback:function(t){e.$set(e.userInfo,"nickname",t)},expression:"userInfo.nickname"}})],1),a("div",{staticClass:"user-info-c-i"},[a("span",{staticClass:"user-info-c-i-l"},[e._v("ID")]),a("i-input",{staticClass:"user-info-c-i-r",attrs:{disabled:""},model:{value:e.userInfo.showId,callback:function(t){e.$set(e.userInfo,"showId",t)},expression:"userInfo.showId"}})],1),a("div",{staticClass:"user-info-c-i"},[a("span",{staticClass:"user-info-c-i-l"},[e._v("登录账号")]),a("i-input",{staticClass:"user-info-c-i-r",attrs:{disabled:""},model:{value:e.userInfo.usrename,callback:function(t){e.$set(e.userInfo,"usrename",t)},expression:"userInfo.usrename"}})],1)]),a("div",{staticClass:"usre-top-i"},[a("div",{staticClass:"user-info-c-i"},[a("span",{staticClass:"user-info-c-i-l"},[e._v("余额")]),a("i-input",{staticClass:"user-info-c-i-r",attrs:{disabled:""},model:{value:e.userInfo.accountBalance,callback:function(t){e.$set(e.userInfo,"accountBalance",t)},expression:"userInfo.accountBalance"}})],1),a("div",{directives:[{name:"show",rawName:"v-show",value:"假人"!==e.userInfo.status,expression:"userInfo.status !== '假人'"}],staticClass:"user-info-c-i"},[a("span",{staticClass:"user-info-c-i-l"},[e._v("最近一次登录")]),a("i-input",{staticClass:"user-info-c-i-r",attrs:{disabled:""},model:{value:e.userInfo.loginTime,callback:function(t){e.$set(e.userInfo,"loginTime",t)},expression:"userInfo.loginTime"}})],1)])])]),a("div",{staticClass:"user-top"},[e._m(2),a("div",{staticClass:"user-top-item"},[a("div",{staticClass:"user-info-c-i"},[a("p",{staticClass:"user-info-c-i-l"},[e._v("用户发言")]),a("i-select",{staticClass:"user-info-c-i-r",model:{value:e.userInfo.speakingStatus,callback:function(t){e.$set(e.userInfo,"speakingStatus",t)},expression:"userInfo.speakingStatus"}},e._l(e.userSpeakList,function(t,s){return a("i-option",{key:s,attrs:{value:t.status}},[e._v(e._s(t.title))])}),1)],1),a("div",{staticClass:"user-info-c-i"},[a("p",{staticClass:"user-info-c-i-l"},[e._v("彩票回水方案")]),a("i-select",{staticClass:"user-info-c-i-r",model:{value:e.userInfo.level,callback:function(t){e.$set(e.userInfo,"level",t)},expression:"userInfo.level"}},e._l(e.userLevelList,function(t,s){return a("i-option",{key:s,attrs:{value:t.value}},[e._v(e._s(t.title))])}),1)],1),a("div",{staticClass:"user-info-c-i"},[a("p",{staticClass:"user-info-c-i-l"},[e._v("备注名")]),a("i-input",{staticClass:"user-info-c-i-r",model:{value:e.userInfo.memoName,callback:function(t){e.$set(e.userInfo,"memoName",t)},expression:"userInfo.memoName"}})],1),a("div",{staticClass:"user-info-c-i"},[a("p",{staticClass:"user-info-c-i-l"},[e._v("视讯回水方案")]),a("i-select",{staticClass:"user-info-c-i-r",model:{value:e.userInfo.videoLevel,callback:function(t){e.$set(e.userInfo,"videoLevel",t)},expression:"userInfo.videoLevel"}},e._l(e.userVideoLevelList,function(t,s){return a("i-option",{key:s,attrs:{value:t.value}},[e._v(e._s(t.title))])}),1)],1),a("div",{directives:[{name:"show",rawName:"v-show",value:!e.userInfo.isFalse,expression:"!userInfo.isFalse"}],staticClass:"user-info-c-i"},[a("p",{staticClass:"user-info-c-i-l"},[e._v("登录密码")]),a("i-input",{staticClass:"user-info-c-i-r",attrs:{placeholder:"修改登录密码填，不修改留空"},model:{value:e.userInfo.password,callback:function(t){e.$set(e.userInfo,"password",t)},expression:"userInfo.password"}})],1),a("div",{staticClass:"user-info-c-i"},[a("p",{staticClass:"user-info-c-i-l"},[e._v("群信息显示")]),a("RadioGroup",{staticStyle:{"margin-left":"8px"},model:{value:e.userInfo.showType,callback:function(t){e.$set(e.userInfo,"showType",t)},expression:"userInfo.showType"}},[a("Radio",{attrs:{label:"nickname",disabled:!e.userInfo.memoName}},[a("span",[e._v("昵称")])]),a("Radio",{attrs:{label:"remark",disabled:!e.userInfo.memoName}},[a("span",[e._v("备注名")])])],1)],1)])]),a("div",{staticClass:"operate"},[a("span",{on:{click:e.handleSubmit}},[e._v("确定")]),a("span",{on:{click:function(t){return e.handleCloseModal("showUserInfo")}}},[e._v("取消")])])])]),a("div",{directives:[{name:"show",rawName:"v-show",value:e.showLotteryRecode,expression:"showLotteryRecode"}],staticClass:"lottery-recode"},[a("div",{staticClass:"user-info-header"},[a("span",[e._v(e._s(e.lotteryTitle))]),a("span",{staticClass:"modal-close",on:{click:function(t){return e.handleCloseModal("showLotteryRecode")}}},[e._v("X")])]),a("div",{staticClass:"lottery-recode-c"},[a("div",{directives:[{name:"show",rawName:"v-show",value:"history"===e.lotteryType,expression:"lotteryType === 'history'"}],staticClass:"recode-search"},[a("DatePicker",{staticStyle:{width:"420px"},attrs:{type:"datetimerange",options:e.forbiddenTime,format:"yyyy-MM-dd HH:mm",placeholder:"选择搜索的时间"},model:{value:e.searchDate,callback:function(t){e.searchDate=t},expression:"searchDate"}}),a("span",[e._v("期数：")]),a("i-input",{staticStyle:{width:"80px"},attrs:{placeholder:"期数搜索"},model:{value:e.searchPeriod,callback:function(t){e.searchPeriod=t},expression:"searchPeriod"}}),a("i-button",{on:{click:e.handleSearchLottery}},[e._v("搜索")])],1),a("i-table",{attrs:{loading:e.childPage.isLoading,height:"514",columns:e.lotteryCols,data:e.lotteryList}}),a("Page",{staticClass:"footer-page",attrs:{total:e.childPage.total,"page-size":e.childPage.pageSize,current:e.childPage.pageNum,"show-sizer":""},on:{"on-page-size-change":e.handleChangeChildSize,"on-change":e.handleChangeChildPage}})],1)]),a("div",{directives:[{name:"show",rawName:"v-show",value:e.showBranchRecode,expression:"showBranchRecode"}],staticClass:"branch-recode"},[a("div",{staticClass:"user-info-header"},[a("span",[e._v(e._s(e.lotteryTitle))]),a("span",{staticClass:"modal-close",on:{click:function(t){return e.handleCloseModal("showBranchRecode")}}},[e._v("X")])]),a("div",{staticClass:"list-header"},[a("div",{staticClass:"list-title"},[a("span",[e._v(e._s(e.lotteryTitle))]),a("div",{directives:[{name:"show",rawName:"v-show",value:"score"===e.type,expression:"type === 'score'"}],staticClass:"list-info"},[a("span",[e._v("上分合计：")]),a("span",[e._v(e._s(e.totalScore))]),a("span",[e._v("下分合计：")]),a("span",[e._v(e._s(e.totalReturn))])])])]),a("div",{directives:[{name:"show",rawName:"v-show",value:"history"===e.lotteryType,expression:"lotteryType === 'history'"}],staticClass:"recode-search"},[a("DatePicker",{staticStyle:{width:"420px"},attrs:{type:"datetimerange",options:e.forbiddenTime,format:"yyyy-MM-dd HH:mm",placeholder:"选择搜索的时间"},model:{value:e.searchDate,callback:function(t){e.searchDate=t},expression:"searchDate"}}),a("i-button",{on:{click:e.handleSearchLottery}},[e._v("搜索")])],1),a("div",[a("i-table",{staticClass:"table-style",attrs:{loading:e.childPage.isLoading,height:"510",columns:e.recodeCols,data:e.recodeList}}),a("Page",{directives:[{name:"show",rawName:"v-show",value:"account"===e.type,expression:"type === 'account'"}],staticClass:"footer-page",attrs:{total:e.childPage.total,"page-size":e.childPage.pageSize,current:e.childPage.pageNum,"show-sizer":""},on:{"on-page-size-change":e.handleChangeChildSize,"on-change":e.handleChangeChildPage}})],1)]),a("div",{directives:[{name:"show",rawName:"v-show",value:e.showAccount,expression:"showAccount"}],staticClass:"branch-recode"},[a("div",{staticClass:"user-info-header"},[a("span",[e._v(e._s(e.lotteryTitle))]),a("span",{staticClass:"modal-close",on:{click:function(t){return e.handleCloseModal("showAccount")}}},[e._v("X")])]),a("i-table",{attrs:{loading:e.childPage.isLoading,columns:e.accountInfoCols,data:e.accountInfoList}}),a("p",{staticStyle:{padding:"20px",color:"red"}},[e._v("点击图片显示原图")]),a("div",{directives:[{name:"show",rawName:"v-show",value:e.qrcodeIcon,expression:"qrcodeIcon"}],staticClass:"show-qrcode",on:{click:e.hiddenQrcode}},[a("img",{staticStyle:{"max-width":"100%","max-height":"100%"},attrs:{src:e.qrcodeIcon,alt:"",title:""}}),a("p",{staticStyle:{padding:"20px",color:"#fff"}},[e._v("点击隐藏图片")])])],1)]),a("Modal",{attrs:{"class-name":"vertical-center-modal",width:"300"},model:{value:e.showDelete,callback:function(t){e.showDelete=t},expression:"showDelete"}},[a("p",{staticStyle:{color:"#0d1941","font-size":"16px","text-align":"center"},attrs:{slot:"header"},slot:"header"},[a("span",[e._v("确认删除")])]),a("p",{staticStyle:{"text-align":"center"}},[e._v("是否确认删除？")]),a("div",{staticClass:"modal-footer",attrs:{slot:"footer"},slot:"footer"},[a("div",{staticClass:"enter-cancel",on:{click:function(t){e.showDelete=!1}}},[e._v("取消")]),a("div",{staticClass:"enter-ok",on:{click:e.handleEnterDelete}},[e._v("确定")])])]),a("Modal",{attrs:{"class-name":"vertical-center-modal",width:"300"},model:{value:e.showCancelAgent,callback:function(t){e.showCancelAgent=t},expression:"showCancelAgent"}},[a("p",{staticStyle:{color:"#0d1941","font-size":"16px","text-align":"center"},attrs:{slot:"header"},slot:"header"},[a("span",[e._v(e._s(e.poptipText))])]),a("p",{staticStyle:{"text-align":"center"}},[e._v(e._s("是否确定"+e.poptipText+"?"))]),a("div",{staticClass:"modal-footer",attrs:{slot:"footer"},slot:"footer"},[a("div",{staticClass:"enter-cancel",on:{click:function(t){e.showCancelAgent=!1}}},[e._v("取消")]),a("div",{staticClass:"enter-ok",on:{click:e.handleEnter}},[e._v("确定")])])]),a("my-upload",{attrs:{width:100,height:100,"img-format":"png"},on:{"crop-success":e.handleCropSuccess},model:{value:e.showUploadHeader,callback:function(t){e.showUploadHeader=t},expression:"showUploadHeader"}})],1)},i=[function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"query-poptip"},[a("p",[e._v("说明：整个后台的用户名默认显示用户昵称，如果在账户设置填写了用户备注，则用户名显示备注名，这样方便管理员识别会员玩家。")])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"list-item list-header"},[a("div",{staticClass:"item-info item-1"},[a("span",[e._v("头像")])]),a("div",{staticClass:"item-info item-2"},[a("span",[e._v("用户名")])]),a("div",{staticClass:"item-info item-3"},[a("span",[e._v("ID")])]),a("div",{staticClass:"item-info item-4"},[a("span",[e._v("账户余额")])]),a("div",{staticClass:"item-info item-5"},[a("span",[e._v("今日盈亏")])]),a("div",{staticClass:"item-info item-6"},[a("span",[e._v("今日投注游戏")])]),a("div",{staticClass:"item-info item-7"},[a("span",[e._v("是否飞单")])]),a("div",{staticClass:"item-info item-8"},[a("span",[e._v("操作")])])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"list-header"},[a("div",{staticClass:"list-title"},[a("span",[e._v("账户设置/修改")])])])}],o=(a("0857"),a("608b"),a("e680"),a("7bc1"),a("3c6b"),a("873a")),n=a("663a"),r={components:{switchs:o["a"],myUpload:n["a"]},data:function(){var e=this;return{qrcodeIcon:"",showUploadHeader:!1,poptipText:"",clientHeight:document.body.clientHeight,gameList:[{title:"所有游戏",value:"0"},{title:"北京赛车",value:"1"},{title:"幸运飞艇",value:"2"},{title:"重庆时时彩",value:"3"},{title:"极速赛车",value:"4"},{title:"澳洲10",value:"5"},{title:"澳洲5",value:"6"}],videoGameList:[{title:"所有游戏",value:"0"},{title:"百家乐",value:"1"},{title:"龙虎",value:"2"},{title:"牛牛",value:"3"}],showLoading:!1,searchUsername:"",selectUserType:0,userInfo:{id:"",speakingStatus:"false",usrename:"",loginTime:"",nickname:"",accountBalance:0,password:"",level:1,videoLevel:1,remark:"",memoName:"",showType:""},showDelete:!1,page:{total:0,pageSize:10,pageNum:1},childPage:{total:0,pageSize:10,pageNum:1,isLoading:!1},userLevelList:[],userVideoLevelList:[],userSpeakList:[{title:"禁止",status:"false"},{title:"开放",status:"true"}],lotteryTitle:"",lotteryType:"",searchDate:[(new Date).pattern("yyyy-MM-dd")+" 00:00",(new Date).pattern("yyyy-MM-dd HH:mm")],searchPeriod:"",selectItem:{},showCancelAgent:!1,userTypeList:[{title:"所有用户",value:0},{title:"正式用户",value:"1"},{title:"虚假用户",value:"2"},{title:"封禁踢人",value:"3"}],columnListShow:[{title:"赛车盈利",value:"racing"},{title:"飞艇盈利",value:"airship"},{title:"时时彩盈利",value:"timeHonored"},{title:"极速盈利",value:"extremeSpeed"},{title:"澳10盈利",value:"australiaTen"},{title:"澳5盈利",value:"australiaFive"},{title:"上下分记录",value:"records"},{title:"账变记录",value:"accountChange"},{title:"操作",value:"operate"}],branchCols:[{title:"用户",key:"usrename"},{title:"金额",key:"balance"},{title:"申请时间",key:"time"},{title:"上分/下分",key:"status",width:200}],accountCols:[{title:"时间",key:"time",width:110},{title:"变动金额",key:"money",width:120},{title:"变动后余额",key:"balance",width:120},{title:"信息",render:function(e,t){return e("div",{style:{"word-break":"break-word"}},t.row.info)}},{title:"备注",key:"remark"}],recodeCols:[],recodeList:[],lotteryCols:[{title:"期号",width:120,key:"id"},{title:"投注金额",width:100,key:"betting"},{title:"投注类型",minWidth:100,key:"type"},{title:"盈亏",width:100,key:"profitLoss"},{title:"中奖号码",minWidth:160,key:"winningNum"},{title:"开奖号码",minWidth:160,key:"lotteryNum"},{title:"开奖时间",minWidth:160,key:"lotteryTime"}],lotteryList:[],userData:[],showUserInfo:!1,showLotteryRecode:!1,showBranchRecode:!1,showAccount:!1,accountInfoCols:[{title:"账户号码",key:"id",align:"center"},{title:"二维码",align:"center",render:function(t,a){return t("div",[t("img",{style:{width:"40px",height:"40px",cursor:"pointer"},attrs:{src:a.row.icon},on:{click:function(){e.showQrcode(a.row.icon)}}})])}},{title:"账户类型",key:"type",align:"center"}],accountInfoList:[],forbiddenTime:{disabledDate:function(e){return e&&e.valueOf()>=Date.now()}},isLoading:!1,type:"",gameType:"",imgDataUrl:"",memberNum:0}},mounted:function(){var e=this;e.getData(),e.handleMethodsList(),e.handleVideoMethodsList(),e.handleGetExistCount()},computed:{totalScore:function(){var e=this;return e.recodeList.reduce(function(e,t){return e+(1==t.statusCode&&1==t.type?parseInt(t.balance.match(/\d.*/g)[0]):0)},0)},totalReturn:function(){var e=this;return e.recodeList.reduce(function(e,t){return e+(1==t.statusCode&&2==t.type?parseInt(t.balance.match(/\d.*/g)[0]):0)},0)}},methods:{handleEnter:function(){var e=this;switch(e.type){case"agent":e.handleActAsAgent();break;case"status":e.handleChangeStatus();break}},handleImageError:function(e){e["error"]?e.header="/UserImages/default.png":(e.header=e.header.split("?")[0]+"?time=".concat((new Date).getTime()),e["error"]=1)},handleCropSuccess:function(e,t){var a=this;a.imgDataUrl=e,console.log(e,t)},handleUploadIcon:function(){var e=this,t=new FormData,a=e.dataURItoFile(e.imgDataUrl);a.size>5242880?e.$Message.error("上传图片不能超过5M"):e.isLoading||(e.isLoading=!0,t.append("fileinput",a),e.$axios({url:"/api/User/UpdateUserImages",method:"post",params:{userID:e.userInfo.id},data:t}).then(function(t){e.isLoading=!1,e.showUserInfo=!1,e.imgDataUrl="",100===t.data.Status?(e.$Message.success(t.data.Message),e.getData()):e.$Message.error(t.data.Message)}))},dataURItoFile:function(e){var t;t=e.split(",")[0].indexOf("base64")>=0?atob(e.split(",")[1]):unescape(e.split(",")[1]);for(var a=e.split(",")[0].split(":")[1].split(";")[0],s=new Uint8Array(t.length),i=0;i<t.length;i++)s[i]=t.charCodeAt(i);return new File([s],"upload.png",{type:a})},showQrcode:function(e){var t=this;t.qrcodeIcon=e},hiddenQrcode:function(){var e=this;e.qrcodeIcon=""},handleMethodsList:function(){var e=this;e.$axios({url:"/api/User/GetGrogrammeList"}).then(function(t){if(100===t.data.Status){var a=t.data.Data;e.userLevelList=a.map(function(t){return{value:t.ID,title:t.Name+"-"+e.gameList.find(function(e){return(t.GameType||"0")==e.value}).title}})}})},handleGetExistCount:function(){var e=this;e.$axios({url:"/api/User/GetExistingCount"}).then(function(t){100===t.data.Status&&(e.memberNum=t.data.Keyword)})},handleVideoMethodsList:function(){var e=this;e.$axios({url:"/api/User/GetVideoGrogrammeList"}).then(function(t){if(100===t.data.Status){var a=t.data.Data;e.userVideoLevelList=a.map(function(t){return{value:t.ID,title:t.Name+"-"+e.videoGameList.find(function(e){return(t.GameType||"0")==e.value}).title}})}})},handleSearch:function(){var e=this;e.page.pageNum=1,e.getData()},handleChangeType:function(e){var t=this;t.page.pageNum=1,t.getData()},handleSettingUser:function(e){var t=this;t.$axios({url:"/api/User/GetUserInfo",params:{userID:e.id}}).then(function(a){if(100===a.data.Status){var s=a.data.Model;t.userInfo={id:s.ID,showId:s.OnlyCode,speakingStatus:s.Talking.toString(),usrename:s.LoginName,loginTime:s.LoginTime,nickname:s.NickName||e.nickname,accountBalance:s.UserMoney,password:s.Password,level:s.ProgrammeID,videoLevel:s.VideoProgrammeID,remark:s.Remark,header:e.header,status:e.status,isFalse:e.isFalse,memoName:s.MemoName,showType:s.ShowType?"nickname":"remark"},t.showUserInfo=!0}else t.$Message.error(a.data.Message)}).catch(function(){t.$Message.error("查询用户信息失败")})},handleCloseModal:function(e){var t=this;t.childPage.total=0,t.childPage.pageNum=1,t.childPage.pageSize=10,t.imgDataUrl="",t[e]=!t[e]},handleSearchLottery:function(){var e=this;if(e.showBranchRecode||e.showLotteryRecode)switch(e.type){case"score":e.getBranchRecodeList();break;case"account":e.getAccountHistory();break;case"game":e.getGameData();break}},handleRacingToday:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="today",t.gameType=1,t.type="game",t.getGameData(),t.showLotteryRecode=!0},handleRacingHistory:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="history",t.gameType=1,t.type="game",t.getGameData(),t.showLotteryRecode=!0},handleAirshipToday:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="today",t.gameType=2,t.type="game",t.getGameData(),t.showLotteryRecode=!0},handleAirshipHistory:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="history",t.gameType=2,t.type="game",t.getGameData(),t.showLotteryRecode=!0},handleTimeToday:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="today",t.gameType=3,t.type="game",t.getGameData(),t.showLotteryRecode=!0},handleTimeHistory:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="history",t.gameType=3,t.type="game",t.getGameData(),t.showLotteryRecode=!0},handleSpeedToday:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="today",t.gameType=4,t.type="game",t.getGameData(),t.showLotteryRecode=!0},handleSpeedHistory:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="history",t.gameType=4,t.type="game",t.getGameData(),t.showLotteryRecode=!0},handleTenToday:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="today",t.gameType=5,t.type="game",t.getGameData(),t.showLotteryRecode=!0},handleTenHistory:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="history",t.gameType=5,t.type="game",t.getGameData(),t.showLotteryRecode=!0},handleFiveToday:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="today",t.gameType=6,t.type="game",t.getGameData(),t.showLotteryRecode=!0},handleFiveHistory:function(e){var t=this;t.lotteryTitle=e.column.title,t.userInfo=e.row,t.lotteryType="history",t.gameType=6,t.type="game",t.getGameData(),t.showLotteryRecode=!0},getGameData:function(){var e=this,t={};"today"===e.lotteryType?(t["startTime"]=(new Date).pattern("yyyy-MM-dd")+" 00:00",t["endTime"]=(new Date).pattern("yyyy-MM-dd HH:mm")):(t["startTime"]=e.searchDate[0],t["endTime"]=e.searchDate[1]),t["userID"]=e.userInfo.id,t["gameType"]=e.gameType,e.searchPeriod&&(t["nper"]=e.searchPeriod),t["start"]=e.childPage.pageNum,t["pageSize"]=e.childPage.pageSize,e.lotteryList=[],e.childPage.isLoading||(e.childPage.isLoading=!0,e.$axios({url:"/api/User/GetUserGameInfos",params:t}).then(function(t){if(e.childPage.isLoading=!1,100===t.data.Status){var a=t.data.Data;e.childPage.total=t.data.Total,e.lotteryList=a.map(function(e){return{id:e.Nper,betting:e.AllBet,type:e.BetType,profitLoss:e.Loss,winningNum:e.WinNums,lotteryNum:e.LotNums,lotteryTime:e.LotTime}})}}))},handleScoreToday:function(e){var t=this;t.recodeCols=t.branchCols,t.lotteryTitle="上下分记录",t.userInfo=e,t.lotteryType="today",t.type="score",t.showBranchRecode=!0,t.getBranchRecodeList()},handleScoreHistory:function(e){var t=this;t.recodeCols=t.branchCols,t.lotteryTitle="上下分记录",t.userInfo=e,t.searchDate=[(new Date).pattern("yyyy-MM-dd")+" 00:00",(new Date).pattern("yyyy-MM-dd")+" 23:59"],t.lotteryType="history",t.showBranchRecode=!0,t.type="score",t.getBranchRecodeList()},getBranchRecodeList:function(){var e=this,t={};e.childPage.isLoading||(e.childPage.isLoading=!0,"today"===e.lotteryType?(t["startTime"]=(new Date).pattern("yyyy-MM-dd")+" 00:00",t["endTime"]=(new Date).pattern("yyyy-MM-dd HH:mm")):(t["startTime"]=e.searchDate[0],t["endTime"]=e.searchDate[1]),t["userID"]=e.userInfo.id,e.recodeList=[],e.$axios({url:"/api/User/GetUserBrancHis",params:t}).then(function(t){if(e.childPage.isLoading=!1,100===t.data.Status){var a=t.data.Data;e.recodeList=a.map(function(e){return{usrename:e.NickName+"("+e.OnlyCode+")",balance:e.Amount,time:e.ApplyTime,status:e.Message,type:e.ChangeType,statusCode:e.Message.match("成功")||e.Message.match("系统")?1:2}})}}))},handleAccountToday:function(e){var t=this;t.recodeCols=t.accountCols,t.lotteryTitle="账变记录",t.userInfo=e,t.lotteryType="today",t.type="account",t.showBranchRecode=!0,t.getAccountHistory()},handleAccountHistory:function(e){var t=this;t.recodeCols=t.accountCols,t.lotteryTitle="账变记录",t.userInfo=e,t.searchDate=[(new Date).pattern("yyyy-MM-dd")+" 00:00",(new Date).pattern("yyyy-MM-dd")+" 23:59"],t.lotteryType="history",t.type="account",t.showBranchRecode=!0,t.getAccountHistory()},getAccountHistory:function(){var e=this,t={};e.childPage.isLoading||(e.childPage.isLoading=!0,"today"===e.lotteryType?(t["startTime"]=(new Date).pattern("yyyy-MM-dd")+" 00:00",t["endTime"]=(new Date).pattern("yyyy-MM-dd")+" 23:59"):(t["startTime"]=e.searchDate[0],t["endTime"]=e.searchDate[1]),t["userID"]=e.userInfo.id,t["start"]=e.childPage.pageNum,t["pageSize"]=e.childPage.pageSize,e.recodeList=[],e.$axios({url:"/api/User/GetUserAccountChange",params:t}).then(function(t){if(e.childPage.isLoading=!1,100===t.data.Status){var a=t.data.Data;e.childPage.total=t.data.Total,e.recodeList=a.map(function(e){return{time:e.ApplyTime,money:e.Amount,balance:e.Balance,info:e.Message,remark:e.Remark}})}}))},handleChangeFlyingSheet:function(e){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/User/RecordOperation",params:{userID:e.id}}).then(function(e){100===e.data.Status?t.$Message.success(e.data.Message):t.$Message.error(e.data.Message),t.isLoading=!1,t.getData()}))},handleChange:function(e){var t=this;e.isFalse||(t.poptipText="假人"===e.status?"切换真人":"切换假人",t.type="status",t.selectItem=e,t.showCancelAgent=!0)},handleChangeStatus:function(){var e=this;e.isLoading||(e.isLoading=!0,e.$axios({url:"/api/User/DummyOperation",params:{userID:e.selectItem.id}}).then(function(t){e.showCancelAgent=!1,100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.isLoading=!1,e.getData()}))},handleFreezing:function(e){var t=this;"正常"!==e.status&&"冻结"!==e.status||t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/User/FrozenOperation",params:{userID:e.id}}).then(function(e){100===e.data.Status?t.$Message.success(e.data.Message):t.$Message.error(e.data.Message),t.isLoading=!1,t.getData()}))},handleDelete:function(e){var t=this;t.userInfo.id=e.id,t.showDelete=!0},handleEnterDelete:function(){var e=this;e.isLoading||(e.isLoading=!0,e.$axios({url:"/api/User/DeleleUser",method:"post",params:{userID:e.userInfo.id}}).then(function(t){100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.isLoading=!1,e.showDelete=!1,e.page.pageNum=1,e.getData()}))},handleCollecAcction:function(e){var t=this;"正常"===e.status&&(t.handleGetAcction(e.id),t.lotteryTitle="下分账户",t.showAccount=!0)},handleGetAcction:function(e){var t=this;t.childPage.isLoading||(t.childPage.isLoading=!0,t.$axios({url:"/api/User/GetUserReceivables",params:{userID:e}}).then(function(e){if(t.childPage.isLoading=!1,t.accountInfoList=[],100===e.data.Status){var a=e.data.Model.WeChatUrl,s=e.data.Model.AlipayUrl;e.data.Model.WeChat&&t.accountInfoList.push({id:e.data.Model.WeChat,type:"微信",icon:e.data.Model.WeChat?-1===a.indexOf("://")?"/"+a:a:"无"}),e.data.Model.Alipay&&t.accountInfoList.push({id:e.data.Model.Alipay,type:"支付宝",icon:e.data.Model.Alipay?-1===s.indexOf("://")?"/"+s:s:"无"})}}))},handleAgent:function(e){var t=this;e.isAgent?(t.selectItem=e,t.poptipText="取消代理",t.type="agent",t.showCancelAgent=!0):t.handleActAsAgent(e)},handleActAsAgent:function(e){var t=this,a=e;a||(a=t.selectItem),"正常"===a.status&&(t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/User/AvatarOperation",params:{userID:a.id}}).then(function(e){100===e.data.Status?(t.showCancelAgent=!1,t.$Message.success(e.data.Message)):t.$Message.error(e.data.Message),t.isLoading=!1,t.getData()})))},handleSubmit:function(){var e=this;e.userInfo.nickname?e.isLoading||(e.isLoading=!0,e.$axios({url:"/api/User/UpdateUserInfo",method:"post",data:{ID:e.userInfo.id,Talking:"true"===e.userInfo.speakingStatus,UserMoney:e.userInfo.accountBalance,Remark:e.userInfo.remark,Password:e.userInfo.password||"",NickName:e.userInfo.nickname,ProgrammeID:e.userInfo.level,VideoProgrammeID:e.userInfo.videoLevel,MemoName:e.userInfo.memoName,ShowType:"nickname"===e.userInfo.showType}}).then(function(t){e.isLoading=!1,100===t.data.Status?e.imgDataUrl?e.handleUploadIcon():(e.$Message.success(t.data.Message),e.showUserInfo=!1,e.getData()):e.$Message.error(t.data.Message)})):e.$Message.error("请填写用户昵称")},handleChangeChildSize:function(e){var t=this;t.childPage.pageSize=e,t.childPage.pageNum=1,t.handleSearchLottery()},handleChangeChildPage:function(e){var t=this;t.childPage.pageNum=e,t.handleSearchLottery()},handleChangeSize:function(e){var t=this;t.page.pageSize=e,t.page.pageNum=1,t.getData()},handleChangePage:function(e){var t=this;t.page.pageNum=e,t.getData()},getData:function(){var e=this,t={};if(e.searchUsername&&(t["loginName"]=e.searchUsername),e.selectUserType)switch(e.selectUserType){case"0":break;case"1":t["status"]=1;break;case"2":t["status"]=4;break;case"3":t["status"]=2;break}t["start"]=e.page.pageNum,t["pageSize"]=e.page.pageSize,e.isLoading||(e.isLoading=!0,e.showLoading=!0,e.$axios({url:"/api/User/SearchUserInfo",params:t}).then(function(t){if(100===t.data.Status){var a=t.data.Data;e.page.total=t.data.Total,e.userData=a.map(function(e){return{id:e.ID,showId:e.OnlyCode,header:e.Avatar?-1!=e.Avatar.indexOf("://")?e.Avatar:"/"+e.Avatar+"?time=".concat((new Date).getTime()):"",username:e.NickName,nickname:e.NickName,loginName:e.LoginName,balance:e.UserMoney,isAgent:e.IsAgent,record:e.Record,status:e.Status,proLoss:e.ProLoss,betGameName:e.BetGameName,isFalse:e.IsSupport}})}e.isLoading=!1,e.showLoading=!1}))}}},l=r,c=(a("0d0f"),a("6691")),d=Object(c["a"])(l,s,i,!1,null,"799261eb",null);t["default"]=d.exports}}]);
//# sourceMappingURL=chunk-034c3f82.1588238865791.js.map
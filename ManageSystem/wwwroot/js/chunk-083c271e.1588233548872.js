(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-083c271e"],{"0857":function(t,e,a){"use strict";a("a0e0");var i=a("a013"),s=a("f425"),n=a("dad2"),c="toString",r=/./[c],o=function(t){a("e5ef")(RegExp.prototype,c,t,!0)};a("b6f1")(function(){return"/a/b"!=r.call({source:"a",flags:"b"})})?o(function(){var t=i(this);return"/".concat(t.source,"/","flags"in t?t.flags:!n&&t instanceof RegExp?s.call(t):void 0)}):r.name!=c&&o(function(){return r.call(this)})},"119c":function(t,e,a){"use strict";var i=a("b6f1");t.exports=function(t,e){return!!t&&i(function(){e?t.call(null,function(){},1):t.call(null)})}},"2d41":function(t,e,a){"use strict";var i=a("b1c2"),s=a.n(i);s.a},"2d43":function(t,e,a){var i=a("01f5"),s=a("6462"),n=a("db4b"),c=a("b146"),r=a("5824");t.exports=function(t,e){var a=1==t,o=2==t,l=3==t,u=4==t,m=6==t,d=5==t||m,p=e||r;return function(e,r,v){for(var f,k,w=n(e),b=s(w),h=i(r,v,3),g=c(b.length),y=0,C=a?p(e,g):o?p(e,0):void 0;g>y;y++)if((d||y in b)&&(f=b[y],k=h(f,y,w),t))if(a)C[y]=k;else if(k)switch(t){case 3:return!0;case 5:return f;case 6:return y;case 2:C.push(f)}else if(u)return!1;return m?-1:l||u?u:C}}},"45fb":function(t,e,a){"use strict";a.r(e);var i=function(){var t=this,e=t.$createElement,a=t._self._c||e;return a("div",{staticClass:"back-water"},[a("div",{staticClass:"content"},[a("div",{staticClass:"list-header"},[a("div",{staticClass:"list-title"},[a("span",[t._v("回水方案")]),a("div",{staticClass:"list-operate"},[a("i-select",{staticStyle:{width:"185px"},on:{"on-change":t.handleChangeMethod},model:{value:t.selectMethodName,callback:function(e){t.selectMethodName=e},expression:"selectMethodName"}},[a("i-option",{attrs:{value:"0"}},[t._v("所有方案")]),t._l(t.methodNameList,function(e,i){return a("i-option",{key:i,attrs:{value:e.value}},[t._v(t._s(e.title))])})],2),a("div",{staticClass:"add",on:{click:t.handleAddBackwater}},[t._v("添加方案")])],1)])]),t._m(0),a("div",{staticClass:"data-list"},[t._m(1),a("div",{staticClass:"data-con"},t._l(t.backwaterData,function(e,i){return a("div",{key:i,staticClass:"data-item"},[a("p",{staticClass:"item-info item-1"},[t._v(t._s(e.name))]),a("p",{staticClass:"item-info item-2"},[t._v(t._s(t.gameList.find(function(t){return t.type==e.type}).name))]),a("p",{staticClass:"item-info item-3"},[t._v(t._s(e.minStatistics))]),a("p",{staticClass:"item-info item-4"},[t._v(t._s(e.maxStatistics))]),a("p",{staticClass:"item-info item-5"},[t._v(t._s(e.proportion))]),a("p",{staticClass:"item-info item-6"},[t._v(t._s(t.modeList.find(function(t){return t.value==e.mode}).title))]),a("div",{staticClass:"item-info item-7"},[a("span",{on:{click:function(a){return t.handleModify(e)}}},[t._v("修改")]),a("span",{on:{click:function(a){return t.handleDelete(e)}}},[t._v("删除")])])])}),0)]),a("Page",{staticClass:"footer-page",attrs:{total:t.page.total,"page-size":t.page.pageSize,current:t.page.pageNum,"show-sizer":""},on:{"on-page-size-change":t.handleChangeSize,"on-change":t.handleChangePage}})],1),a("div",{directives:[{name:"show",rawName:"v-show",value:t.backwater.showMask,expression:"backwater.showMask"}],staticClass:"back-water-mask"},[a("div",{directives:[{name:"show",rawName:"v-show",value:t.backwater.showMask,expression:"backwater.showMask"}],staticClass:"back-water-item"},[a("div",{staticClass:"back-water-item-h"},[a("span",[t._v(t._s(t.backwater.header))]),a("span",{staticClass:"modal-close",on:{click:t.handleCloseModal}},[t._v("X")])]),a("div",{staticClass:"back-water-item-c"},[a("div",{staticClass:"item-top"},[a("div",{staticClass:"columns-item"},[a("p",{staticClass:"columns-item-label"},[t._v("方案名称")]),a("i-select",{staticClass:"columns-item-select",model:{value:t.backwater.level,callback:function(e){t.$set(t.backwater,"level",e)},expression:"backwater.level"}},t._l(t.methodNameList,function(e,i){return a("i-option",{key:i,attrs:{value:e.value}},[t._v(t._s(e.title))])}),1)],1),a("div",{staticClass:"columns-item"},[a("p",{staticClass:"columns-item-label"},[t._v("方案模式")]),a("i-select",{staticClass:"columns-item-select",model:{value:t.backwater.mode,callback:function(e){t.$set(t.backwater,"mode",e)},expression:"backwater.mode"}},t._l(t.modeList,function(e,i){return a("i-option",{key:i,attrs:{value:e.value}},[t._v(t._s(e.title))])}),1)],1),a("div",{staticClass:"columns-item"},[a("p",{staticClass:"columns-item-label"},[t._v("适用游戏")]),a("i-select",{staticClass:"columns-item-select",model:{value:t.backwater.type,callback:function(e){t.$set(t.backwater,"type",e)},expression:"backwater.type"}},t._l(t.gameList,function(e,i){return a("i-option",{key:i,attrs:{value:e.type}},[t._v(t._s(e.name))])}),1)],1)]),a("div",{staticClass:"item-top"},[a("div",{staticClass:"columns-item"},[a("p",{staticClass:"columns-item-label"},[t._v("最小统计")]),a("InputNumber",{staticClass:"columns-item-input",attrs:{min:0},model:{value:t.backwater.minStatistics,callback:function(e){t.$set(t.backwater,"minStatistics",e)},expression:"backwater.minStatistics"}})],1),a("div",{staticClass:"columns-item"},[a("p",{staticClass:"columns-item-label"},[t._v("最大统计")]),a("InputNumber",{staticClass:"columns-item-input",attrs:{min:0},model:{value:t.backwater.maxStatistics,callback:function(e){t.$set(t.backwater,"maxStatistics",e)},expression:"backwater.maxStatistics"}})],1),a("div",{staticClass:"columns-item"},[a("p",{staticClass:"columns-item-label"},[t._v("回水比例")]),a("InputNumber",{staticClass:"columns-item-input",attrs:{max:100,min:0},model:{value:t.backwater.proportion,callback:function(e){t.$set(t.backwater,"proportion",e)},expression:"backwater.proportion"}}),a("span",[t._v("%")])],1)]),a("div",{staticClass:"columns-item footer"},[a("div",{staticClass:"submit",on:{click:t.handleSubmit}},[t._v("提交")])])])])]),a("Modal",{attrs:{title:"确认删除"},on:{"on-ok":t.handleEnterDelete,"on-cancel":function(e){t.backwater.showDelete=!1}},model:{value:t.backwater.showDelete,callback:function(e){t.$set(t.backwater,"showDelete",e)},expression:"backwater.showDelete"}},[a("p",[t._v("是否确认删除？")])])],1)},s=[function(){var t=this,e=t.$createElement,a=t._self._c||e;return a("div",{staticClass:"query-poptip"},[a("p",[t._v("说明：因为玩家回水方案选择中没有区分游戏，选定一种回水方案时，就等于选了所有游戏的方案，所以这里不同的游戏都需要设置一个相同方案名的配置；比如：设置方案名为“方案一”的方案需要添加相同的方案名称6条记录，分别指向不同的6款游戏。")])])},function(){var t=this,e=t.$createElement,a=t._self._c||e;return a("div",{staticClass:"data-header"},[a("p",{staticClass:"item-info item-1"},[t._v("方案名称")]),a("p",{staticClass:"item-info item-2"},[t._v("适用游戏")]),a("p",{staticClass:"item-info item-3"},[t._v("最小统计")]),a("p",{staticClass:"item-info item-4"},[t._v("最大统计")]),a("p",{staticClass:"item-info item-5"},[t._v("回水比例（%）")]),a("p",{staticClass:"item-info item-6"},[t._v("回水模式")]),a("p",{staticClass:"item-info item-7"},[t._v("操作")])])}],n=(a("0857"),a("608b"),a("b745"),{data:function(){return{showLoading:!1,selectMethodName:"0",activeGameType:"lottery",gameBigList:[{title:"彩票游戏",key:"lottery"},{title:"视讯游戏",key:"video"}],backwater:{header:"",id:"",level:"",type:"",minStatistics:0,maxStatistics:0,proportion:0,mode:"",submitText:"",showMask:!1,showDelete:!1,method:0},methodNameList:[{title:"方案一",value:"1"},{title:"方案二",value:"2"},{title:"方案三",value:"3"},{title:"方案四",value:"4"},{title:"方案五",value:"5"},{title:"方案六",value:"6"},{title:"方案七",value:"7"}],modeList:[{title:"流水模式",value:"1"},{title:"输赢模式",value:"2"}],gameList:[],page:{total:0,pageSize:10,pageNum:1},backwaterData:[],isLoading:!1}},mounted:function(){var t=this;t.getGameList().then(function(){t.getData()})},methods:{handleSelectGameBigType:function(t){var e=this;e.activeGameType=t.key,e.getGameList().then(function(){e.getData()})},getGameList:function(){var t=this;return new Promise(function(e,a){"lottery"==t.activeGameType?t.$axios({url:"/api/Merchant/GetGameList"}).then(function(i){if(100===i.data.Status){var s=i.data.Data;t.gameList=s.map(function(t){return{name:t.GameName,type:t.GameType,key:t.NickName}}).sort(function(t,e){return t.type-e.type}),t.gameList.unshift({name:"所有游戏",type:"0",key:"all"}),e()}a()}):(t.gameList=[],t.gameList.push({name:"所有游戏",type:"0",key:"all"}),t.gameList.push({name:"百家乐",type:"1",key:"bjl"}),e())})},handleChangeMethod:function(){var t=this;t.page.pageNum=1,t.getData()},handleAddBackwater:function(){var t=this;t.backwater.header="添加方案",t.backwater.method=1,t.backwater.showMask=!0},handleModify:function(t){var e=this;e.backwater.header="修改方案",e.backwater.method=2,e.backwater.showMask=!0,e.getItem(t.id)},handleDelete:function(t){var e=this;e.backwater.id=t.id,e.backwater.showDelete=!0},handleEnterDelete:function(){var t=this,e="";t.isLoading||(t.isLoading=!0,e="lottery"==t.activeGameType?"/api/BackwaterSetup/DeleteBackwaterByID":"/api/BackwaterSetup/DeleteVideoBackwaterByID",t.$axios({url:e,method:"post",params:{backID:t.backwater.id}}).then(function(e){100===e.data.Status?t.$Message.success(e.data.Message):t.$Message.error(e.data.Message),t.isLoading=!1,t.backwater.showDelete=!1,t.page.total%t.page.pageSize===1&&(t.page.pageNum=parseInt(t.page.total/t.page.pageSize)),t.getData()}))},handleCloseModal:function(){var t=this;t.backwater={header:"",id:"",level:"",type:"",minStatistics:0,maxStatistics:0,proportion:0,mode:"",submitText:"",showMask:!1,showDelete:!1},t.backwater.showMask=!1},handleChangeSize:function(t){var e=this;e.page.pageSize=t,e.page.pageNum=1,e.getData()},handleChangePage:function(t){var e=this;e.page.pageNum=t,e.getData()},getData:function(){var t=this,e={},a="";0==t.page.pageNum&&(t.page.pageNum=1),e["start"]=t.page.pageNum,e["pageSize"]=t.page.pageSize,e["name"]="0"!=t.selectMethodName?t.methodNameList.find(function(e){return e.value==t.selectMethodName}).title:"",a="lottery"==t.activeGameType?"/api/BackwaterSetup/GetBackwaterList":"/api/BackwaterSetup/GetVideoBackwaterList",t.isLoading||(t.isLoading=!0,t.showLoading=!0,t.$axios({url:a,params:e}).then(function(e){if(100===e.data.Status){var a=e.data.Data;t.page.total=e.data.Total,t.backwaterData=a.map(function(t){return{id:t.ID,level:t.UserType,type:t.GameType||"0",minStatistics:t.Minrecord,maxStatistics:t.Maxrecord,proportion:t.Odds+"%",mode:t.Pattern,name:t.Name}})}else t.$Message.error(e.data.Message);t.isLoading=!1,t.showLoading=!1}))},handleSubmit:function(){var t=this,e={},a="";if(t.backwater.maxStatistics)if(t.backwater.minStatistics)if(t.backwater.proportion)if(1===t.backwater.method){if(!t.backwater.mode)return void t.$Message.error("请选择返水模式");if(!t.backwater.level)return void t.$Message.error("请选择方案名称");t.isLoading||(t.isLoading=!0,"0"!=t.backwater.type&&(e["GameType"]=t.backwater.type),e["Maxrecord"]=t.backwater.maxStatistics,e["Minrecord"]=t.backwater.minStatistics,e["Odds"]=t.backwater.proportion,e["Pattern"]=t.backwater.mode,e["Name"]=t.methodNameList.find(function(e){return e.value==t.backwater.level}).title,a="lottery"==t.activeGameType?"/api/BackwaterSetup/AddBackwater":"/api/BackwaterSetup/AddVideoBackwater",t.$axios({url:a,method:"post",data:e}).then(function(e){100===e.data.Status?t.$Message.success(e.data.Message):t.$Message.error(e.data.Message),t.backwater.maxStatistics=0,t.backwater.minStatistics=0,t.backwater.proportion=0,t.backwater.type="",t.backwater.mode="",t.backwater.level="",t.isLoading=!1,t.backwater.showMask=!1,t.getData()}))}else t.isLoading||(t.isLoading=!0,"0"!=t.backwater.type&&(e["GameType"]=t.backwater.type),e["Maxrecord"]=t.backwater.maxStatistics,e["Minrecord"]=t.backwater.minStatistics,e["Odds"]=t.backwater.proportion,e["Pattern"]=t.backwater.mode,e["Name"]=t.methodNameList.find(function(e){return e.value==t.backwater.level}).title,e["ID"]=t.backwater.id,a="lottery"==t.activeGameType?"/api/BackwaterSetup/UpdateBackwater":"/api/BackwaterSetup/UpdateVideoBackwater",t.$axios({url:a,method:"post",data:e}).then(function(e){100===e.data.Status?t.$Message.success(e.data.Message):t.$Message.error(e.data.Message),t.isLoading=!1,t.backwater.showMask=!1,t.getData()}));else t.$Message.error("请输入返水比例");else t.$Message.error("请输入最小统计");else t.$Message.error("请输入最大统计")},getItem:function(t){var e=this,a="";a="lottery"==e.activeGameType?"/api/BackwaterSetup/GetBackwaterByID":"/api/BackwaterSetup/GetVideoBackwaterByID",e.$axios({url:a,params:{backID:t}}).then(function(t){if(100===t.data.Status){var a=t.data.Model;e.backwater.type=(a.GameType||"0").toString(),e.backwater.maxStatistics=a.Maxrecord,e.backwater.minStatistics=a.Minrecord,e.backwater.proportion=a.Odds,e.backwater.mode=a.Pattern.toString(),e.backwater.level=e.methodNameList.find(function(t){return t.title==a.Name}).value,e.backwater.id=a.ID}})}}}),c=n,r=(a("2d41"),a("6691")),o=Object(r["a"])(c,i,s,!1,null,"56afeb6e",null);e["default"]=o.exports},5824:function(t,e,a){var i=a("f691");t.exports=function(t,e){return new(i(t))(e)}},"608b":function(t,e,a){"use strict";var i=a("b2f5"),s=a("2d43")(5),n="find",c=!0;n in[]&&Array(1)[n](function(){c=!1}),i(i.P+i.F*c,"Array",{find:function(t){return s(this,t,arguments.length>1?arguments[1]:void 0)}}),a("644a")(n)},a0e0:function(t,e,a){a("dad2")&&"g"!=/./g.flags&&a("ddf7").f(RegExp.prototype,"flags",{configurable:!0,get:a("f425")})},b1c2:function(t,e,a){},b5b8:function(t,e,a){var i=a("94ac");t.exports=Array.isArray||function(t){return"Array"==i(t)}},b745:function(t,e,a){"use strict";var i=a("b2f5"),s=a("648a"),n=a("db4b"),c=a("b6f1"),r=[].sort,o=[1,2,3];i(i.P+i.F*(c(function(){o.sort(void 0)})||!c(function(){o.sort(null)})||!a("119c")(r)),"Array",{sort:function(t){return void 0===t?r.call(n(this)):r.call(n(this),s(t))}})},f691:function(t,e,a){var i=a("88dd"),s=a("b5b8"),n=a("8b37")("species");t.exports=function(t){var e;return s(t)&&(e=t.constructor,"function"!=typeof e||e!==Array&&!s(e.prototype)||(e=void 0),i(e)&&(e=e[n],null===e&&(e=void 0))),void 0===e?Array:e}}}]);
//# sourceMappingURL=chunk-083c271e.1588233548872.js.map
(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-cb67f76c"],{"2d78":function(e,a,t){},d52d:function(e,a,t){"use strict";t.r(a);var n=function(){var e=this,a=e.$createElement,t=e._self._c||a;return t("div",{staticClass:"sensi-opera-log"},[t("i-table",{attrs:{columns:e.sensiOperaLogCols,height:"600",data:e.sensiOperaLogData}}),t("Page",{staticClass:"footer-page",attrs:{total:e.page.total,"page-size":e.page.pageSize,current:e.page.pageNum,"show-sizer":""},on:{"on-page-size-change":e.handleChangeSize,"on-change":e.handleChangePage}}),t("loading",{attrs:{loading:e.isLoading}})],1)},i=[],o={data:function(){return{page:{total:0,pageSize:10,pageNum:1},sensiOperaLogCols:[{title:"类型",align:"center",key:"type"},{title:"操作位置",align:"center",key:"operaPosition"},{title:"操作前内容",align:"center",render:function(e,a){return e("textarea",{style:{height:"80px",border:"none",backgroundColor:"#fff"},attrs:{disabled:!0}},a.row.beforeOpera)}},{title:"操作后内容",align:"center",render:function(e,a){return e("textarea",{style:{height:"80px",border:"none",backgroundColor:"#fff"},attrs:{disabled:!0}},a.row.afterOpera)}},{title:"操作时间",align:"center",key:"operaTime"}],sensiOperaLogData:[],isLoading:!1}},mounted:function(){var e=this;e.getData()},methods:{handleChangeSize:function(e){var a=this;a.page.pageSize=e,a.page.pageNum=1,a.getData()},handleChangePage:function(e){var a=this;a.page.pageNum=e,a.getData()},getData:function(){var e=this,a={};a["start"]=e.page.pageNum,a["pageSize"]=e.page.pageSize,e.isLoading||(e.isLoading=!0,e.$axios({url:"/api/Setup/GetSensitiveInfos",params:a}).then(function(a){if(100===a.data.Status){var t=a.data.Data;e.page.total=a.data.Total,e.sensiOperaLogData=t.map(function(e){return{id:e.ID,type:e.Type,operaPosition:e.Location,beforeOpera:e.OpAcontent,afterOpera:e.OpBcontent,operaTime:e.CreateTime}})}e.isLoading=!1}))}}},r=o,s=(t("e279"),t("6691")),g=Object(s["a"])(r,n,i,!1,null,"6f02b860",null);a["default"]=g.exports},e279:function(e,a,t){"use strict";var n=t("2d78"),i=t.n(n);i.a}}]);
//# sourceMappingURL=chunk-cb67f76c.1588215759171.js.map
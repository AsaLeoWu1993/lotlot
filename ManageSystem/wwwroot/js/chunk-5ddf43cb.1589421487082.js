(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-5ddf43cb"],{"026c":function(t,a,e){"use strict";e.r(a);var i=function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"fly-fail-record"},[e("div",{staticClass:"content"},[t._m(0),e("div",{staticClass:"query-list"},[e("span",[t._v("时间：")]),e("DatePicker",{staticClass:"search-date",attrs:{type:"daterange",format:"yyyy-MM-dd",placeholder:"选择搜索的时间"},model:{value:t.searchDate,callback:function(a){t.searchDate=a},expression:"searchDate"}}),e("div",{staticStyle:{"flex-grow":"1"}}),e("div",{staticClass:"search-button",on:{click:t.getFlyFailRecord}},[e("span",[t._v("查询")])])],1),e("div",{staticClass:"list-data"},[t._m(1),e("div",{staticClass:"data-con"},[t._l(t.showDataList,function(a,i){return e("div",{key:i,staticClass:"data-item"},[e("p",{staticClass:"item-info item-1"},[t._v(t._s(a.uuid))]),e("p",{staticClass:"item-info item-3"},[t._v(t._s(a.gameName))]),e("p",{staticClass:"item-info item-4"},[t._v(t._s(a.perids))]),e("p",{staticClass:"item-info item-5",attrs:{title:a.currentBet}},[t._v(t._s(a.currentBet))]),e("p",{staticClass:"item-info item-6"},[t._v(t._s(a.bet))]),e("p",{staticClass:"item-info item-7"},[t._v(t._s(a.time))]),e("p",{staticClass:"item-info item-8"},[t._v(t._s(a.status))])])}),t.showDataList.length?t._e():e("div",{staticClass:"no-data"},[e("span",[t._v("暂无数据")])])],2)]),e("Page",{directives:[{name:"show",rawName:"v-show",value:t.page.total,expression:"page.total"}],staticClass:"footer-page",attrs:{total:t.page.total,"page-size":t.page.pageSize,current:t.page.pageNum,"show-sizer":""},on:{"on-page-size-change":t.handleChangeSize,"on-change":t.handleChangePage}})],1),e("loading",{attrs:{loading:t.isLoading}})],1)},s=[function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"list-header"},[e("div",{staticClass:"list-title"},[e("span",[t._v("外部飞单列表")])])])},function(){var t=this,a=t.$createElement,e=t._self._c||a;return e("div",{staticClass:"data-header"},[e("p",{staticClass:"item-info item-1"},[t._v("UUID")]),e("p",{staticClass:"item-info item-2"},[t._v("游戏")]),e("p",{staticClass:"item-info item-4"},[t._v("期号")]),e("p",{staticClass:"item-info item-5"},[t._v("注单内容")]),e("p",{staticClass:"item-info item-6"},[t._v("注单金额")]),e("p",{staticClass:"item-info item-7"},[t._v("注单时间")]),e("p",{staticClass:"item-info item-8"},[t._v("状态")])])}],n={data:function(){return{showDataList:[],page:{total:0,pageSize:10,pageNum:1},isLoading:!1,searchDate:[new Date,new Date((new Date).setDate((new Date).getDate()+1))]}},mounted:function(){var t=this;t.getFlyFailRecord()},methods:{getFlyFailRecord:function(){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/Setup/GetFlySheetFail",params:{startTime:t.searchDate[0].pattern("yyyy-MM-dd")+" 00:00",endTime:t.searchDate[1].pattern("yyyy-MM-dd")+" 23:59",start:t.page.pageNum,pageSize:t.page.pageSize}}).then(function(a){if(t.isLoading=!1,100===a.data.Status){var e=a.data.Data;t.page.total=a.data.Total,t.showDataList=e.map(function(t){var a=t.Orders.map(function(t){return t.content}).join(" "),e=t.Orders.map(function(t){return t.money}).join(" ");return{uuid:t.UUID,gameName:t.GameName,perids:t.Nper,currentBet:a,bet:e,time:t.Time,status:t.Status}})}}))},handleChangeSize:function(t){var a=this;a.page.pageSize=t,a.page.pageNum=1,a.getFlyFailRecord()},handleChangePage:function(t){var a=this;a.page.pageNum=t,a.getFlyFailRecord()}}},c=n,r=(e("52d1"),e("6691")),o=Object(r["a"])(c,i,s,!1,null,"31bb442b",null);a["default"]=o.exports},"52d1":function(t,a,e){"use strict";var i=e("e49e"),s=e.n(i);s.a},e49e:function(t,a,e){}}]);
//# sourceMappingURL=chunk-5ddf43cb.1589421487082.js.map
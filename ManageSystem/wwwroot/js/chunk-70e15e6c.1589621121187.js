(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-70e15e6c"],{"15bc":function(a,t,e){"use strict";var s=e("a77d"),i=e.n(s);i.a},"1f39":function(a,t,e){"use strict";e.r(t);var s=function(){var a=this,t=a.$createElement,e=a._self._c||t;return e("div",{staticClass:"manual-up-down"},[e("div",{staticClass:"content"},[a._m(0),e("div",{staticClass:"data-con"},[e("div",{staticClass:"data-list"},[e("div",{staticClass:"data-item"},[e("span",[a._v("用户名")]),e("Select",{staticClass:"column-input",attrs:{filterable:""},model:{value:a.username,callback:function(t){a.username=t},expression:"username"}},a._l(a.userList,(function(t,s){return e("Option",{key:s,attrs:{value:t.id}},[a._v(a._s(t.title))])})),1),e("i-select",{staticClass:"item-select",model:{value:a.upDown,callback:function(t){a.upDown=t},expression:"upDown"}},a._l(a.upDownList,(function(t,s){return e("i-option",{key:s,attrs:{value:t.key}},[a._v(a._s(t.title))])})),1),e("InputNumber",{staticClass:"column-input",attrs:{min:0,placeholder:"输入处理金额(正数)"},on:{"on-focus":function(t){return a.handleFocus(t)}},model:{value:a.upDownScore,callback:function(t){a.upDownScore=t},expression:"upDownScore"}}),e("span",[a._v("备注")]),e("Input",{staticClass:"column-input",attrs:{placeholder:"备注信息"},model:{value:a.remark,callback:function(t){a.remark=t},expression:"remark"}}),e("div",{staticClass:"operate",on:{click:a.handleSubmit}},[a._v("确定")])],1)])])]),e("div",{staticClass:"content"},[e("div",{staticClass:"list-header"},[e("div",{staticClass:"list-title"},[e("span",[a._v("上下分记录")]),e("div",{staticClass:"list-poptip"},[e("span",[a._v("上分合计：")]),e("span",[a._v(a._s(a.allUp))]),e("span",[a._v("下分合计：")]),e("span",[a._v(a._s(a.allDown))])])])]),e("div",{staticClass:"query-list"},[e("span",[a._v("时间：")]),e("DatePicker",{staticClass:"search-date",attrs:{type:"daterange",format:"yyyy-MM-dd",placeholder:"选择搜索的时间"},model:{value:a.searchDate,callback:function(t){a.searchDate=t},expression:"searchDate"}}),e("i-select",{staticClass:"item-select",model:{value:a.recordType,callback:function(t){a.recordType=t},expression:"recordType"}},a._l(a.recordTypeList,(function(t,s){return e("i-option",{key:s,attrs:{value:t.key}},[a._v(a._s(t.title))])})),1),e("Input",{staticClass:"column-input",attrs:{placeholder:"用户ID/用户名"},on:{"on-blur":a.handleBlur},model:{value:a.searchName,callback:function(t){a.searchName=t},expression:"searchName"}}),e("div",{staticClass:"search-button",on:{click:a.handleSearch}},[e("span",[a._v("查询")])])],1),e("div",{staticClass:"data-con"},[a._m(1),e("div",{staticClass:"data-list"},[a._l(a.allUpDownList,(function(t,s){return e("div",{key:s,staticClass:"data-item1"},[e("p",{staticClass:"item-info"},[a._v(a._s(t.nickname))]),e("p",{staticClass:"item-info"},[a._v(a._s(t.showId))]),e("p",{staticClass:"item-info"},[a._v(a._s(t.status))]),e("p",{staticClass:"item-info"},[a._v(a._s(t.money))]),e("p",{staticClass:"item-info"},[a._v(a._s(t.time))]),e("p",{staticClass:"item-info",attrs:{title:t.remark}},[a._v(a._s(t.remark))]),e("p",{staticClass:"item-info"},[a._v(a._s(t.operate))])])})),a.allUpDownList.length?a._e():e("div",{staticClass:"no-data"},[e("span",[a._v("暂无数据")])])],2)]),e("Page",{staticClass:"footer-page",attrs:{total:a.page.total,"page-size":a.page.pageSize,current:a.page.pageNum,"show-sizer":""},on:{"on-page-size-change":a.handleChangeSize,"on-change":a.handleChangePage}})],1)])},i=[function(){var a=this,t=a.$createElement,e=a._self._c||t;return e("div",{staticClass:"list-header"},[e("div",{staticClass:"list-title"},[e("span",[a._v("手动上下分")])])])},function(){var a=this,t=a.$createElement,e=a._self._c||t;return e("div",{staticClass:"data-header"},[e("p",{staticClass:"item-info"},[a._v("用户名")]),e("p",{staticClass:"item-info"},[a._v("ID")]),e("p",{staticClass:"item-info"},[a._v("申请类别")]),e("p",{staticClass:"item-info"},[a._v("金额")]),e("p",{staticClass:"item-info"},[a._v("申请时间")]),e("p",{staticClass:"item-info"},[a._v("备注")]),e("p",{staticClass:"item-info"},[a._v("状态")])])}],n={data:function(){return{showLoading:!1,page:{total:0,pageSize:10,pageNum:1},searchName:"",recordType:0,recordTypeList:[{key:0,title:"所有记录"},{key:1,title:"上分(申请)"},{key:2,title:"下分(申请)"},{key:3,title:"上分(手动)"},{key:4,title:"下分(手动)"}],searchDate:[new Date,new Date((new Date).setDate((new Date).getDate()+1))],remark:"",username:"",upDown:1,allUp:0,allDown:0,allUpDownList:[],upDownList:[{key:1,title:"加款"},{key:2,title:"扣款"}],userList:[],upDownScore:0,isLoading1:!1}},mounted:function(){var a=this;a.getUserList(),a.getData(),a.getDataTotal()},methods:{handleBlur:function(){var a=this;a.searchName&&(a.page.pageNum=1)},handleFocus:function(a){a.target.select()},getDataTotal:function(){var a=this,t={};a.recordType&&(t["status"]=a.recordType),t["startTime"]=a.searchDate[0].pattern("yyyy-MM-dd")+" 00:00",t["endTime"]=a.searchDate[1].pattern("yyyy-MM-dd")+" 23:59",a.$axios({url:"/api/Setup/GetTotalIntegral",params:t}).then((function(t){if(100===t.data.Status){var e=t.data.Data;a.allUp=e.UpScore,a.allDown=e.DownScore}}))},handleChangeSize:function(a){var t=this;t.page.pageSize=a,t.page.pageNum=1,t.getData(),t.getDataTotal()},handleChangePage:function(a){var t=this;t.page.pageNum=a,t.getData(),t.getDataTotal()},handleSearch:function(){var a=this;a.getData(),a.getDataTotal()},handleSubmit:function(){var a=this,t="";if(a.username)if(a.upDownScore)if(a.remark){switch(a.upDown){case 1:t="/api/Setup/ManualUpper";break;case 2:t="/api/Setup/ManualLower";break}a.isLoading||(a.isLoading=!0,a.$axios({url:t,method:"post",params:{userID:a.username,branch:a.upDownScore,remark:a.remark}}).then((function(t){100===t.data.Status?a.$Message.success(t.data.Message):a.$Message.error(t.data.Message),a.username="",a.upDownScore=0,a.remark="",a.isLoading=!1,a.getData(),a.getDataTotal()})))}else a.$Message.error("请输入备注");else a.$Message.error("请输入处理金额");else a.$Message.error("请输入用户名")},getUserList:function(){var a=this;a.isLoading||(a.isLoading=!0,a.$axios({url:"/api/Setup/GetUpDownUserList",params:{status:!0}}).then((function(t){if(100===t.data.Status){var e=t.data.Data;a.userList=e.map((function(a){return{title:a.NickName+"("+a.OnlyCode+")",id:a.UserID}}))}a.isLoading=!1})))},getData:function(){var a=this,t={};a.searchDate.length?(t["startTime"]=a.searchDate[0].pattern("yyyy-MM-dd")+" 00:00",t["endTime"]=a.searchDate[1].pattern("yyyy-MM-dd")+" 23:59",a.recordType&&(t["status"]=a.recordType),t["start"]=a.page.pageNum,t["pageSize"]=a.page.pageSize,a.searchName&&(t["keyword"]=a.searchName),a.isLoading1||(a.isLoading1=!0,a.showLoading=!0,a.$axios({url:"/api/Setup/GetIntegralRecord",params:t}).then((function(t){if(100===t.data.Status){var e=t.data.Data;a.page.total=t.data.Total,a.allUpDownList=e.map((function(a){return{id:a.ID,nickname:a.NickName,loginName:a.LoginName,money:a.Amount,time:a.ApplyTime.substr(0,16),status:a.Status+("申请"===a.OrderStatus.substr(0,2)?a.OrderStatus.substr(2,2)+"(申请)":("充值"===a.OrderStatus.substr(0,2)?"上分":"下分")+"(手动)"),operate:a.Management,remark:a.Remark,showId:a.OnlyCode}}))}a.isLoading1=!1,a.showLoading=!1})))):a.$Message.error("请选择日期")}}},r=n,o=(e("15bc"),e("4023")),l=Object(o["a"])(r,s,i,!1,null,"9b73a4bc",null);t["default"]=l.exports},a77d:function(a,t,e){}}]);
//# sourceMappingURL=chunk-70e15e6c.1589621121187.js.map
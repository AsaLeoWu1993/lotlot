(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-a51b0962"],{"06e7":function(e,t,a){"use strict";var i=a("e7c0"),s=a.n(i);s.a},"119c":function(e,t,a){"use strict";var i=a("b6f1");e.exports=function(e,t){return!!e&&i(function(){t?e.call(null,function(){},1):e.call(null)})}},"953d":function(e,t,a){"use strict";a.r(t);var i=function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"false-user"},[a("div",{staticClass:"false-user-list"},[e._m(0),a("div",{staticClass:"partition-line"}),a("div",{staticClass:"list-data"},[e._m(1),a("div",{staticClass:"data-con"},e._l(e.falseUserData,function(t,i){return a("div",{key:i,staticClass:"data-item"},[a("div",{staticClass:"item-info"},[a("img",{attrs:{src:t.header,alt:"",title:""},on:{error:function(a){return e.handleImageError(t)}}})]),a("div",{staticClass:"item-info",attrs:{title:t.nickname}},[e._v(e._s(t.nickname))]),a("div",{staticClass:"item-info"},[e._v(e._s(t.showId))]),a("div",{staticClass:"item-info"},[e._v(e._s(t.balance))]),a("div",{staticClass:"item-info"},[e._v(e._s(t.openGame))]),a("div",{staticClass:"item-info"},[a("switchs",{attrs:{open:t.open},on:{switch:function(a){return e.handleSwitch(t)}}})],1),a("div",{staticClass:"item-info operate"},[a("div",{staticClass:"item-button",on:{click:function(a){return e.handleShowModifyUser(t)}}},[e._v("设置")]),a("div",{staticClass:"item-button",on:{click:function(a){return e.handleDeleteUser(t)}}},[e._v("删除")])])])}),0),a("Page",{staticClass:"footer-page",attrs:{total:e.page.total,"page-size":e.page.pageSize,current:e.page.pageNum,"show-sizer":""},on:{"on-page-size-change":e.handleChangeSize,"on-change":e.handleChangePage}})],1)]),a("div",{staticClass:"false-user-operate"},[a("div",{staticClass:"operate-list"},[e._m(2),a("div",[a("div",{staticClass:"operate-item",on:{click:e.handleShowAddUser}},[e._v("增加虚拟用户")]),a("div",{staticClass:"operate-item",on:{click:function(t){return e.handleOperateUser(!0)}}},[e._v("启用全部用户")]),a("div",{staticClass:"operate-item",on:{click:function(t){return e.handleOperateUser(!1)}}},[e._v("停用全部用户")])])]),a("div",{staticClass:"behavior"},[a("div",{staticClass:"false-user-header"},[a("span",{staticClass:"false-user-title"},[e._v("操作设置")]),a("span",{staticClass:"false-user-ope",on:{click:e.handleShowAI}},[e._v("添加行为")])]),a("div",{staticClass:"behavior-list"},[e._m(3),a("div",{staticClass:"behavior-c"},e._l(e.behaviorList,function(t,i){return a("div",{key:i,staticClass:"behavior-c-item"},[a("span",[e._v(e._s(t.name))]),a("span",[e._v(e._s(t.num))]),a("div",{staticClass:"item-o"},[a("span",{on:{click:function(a){return e.handleShowEdit(t.id)}}},[e._v("修改")]),a("span",{on:{click:function(a){return e.handleShowDelete(t.id)}}},[e._v("删除")])])])}),0)])])]),a("div",{directives:[{name:"show",rawName:"v-show",value:e.addFalseUser.show||e.modifyFalseUser.show,expression:"addFalseUser.show || modifyFalseUser.show"}],staticClass:"false-user-mask"},[a("div",{directives:[{name:"show",rawName:"v-show",value:e.addFalseUser.show,expression:"addFalseUser.show"}],staticClass:"add-false-user"},[a("div",{staticClass:"add-header"},[a("span",[e._v("添加虚拟用户")]),a("span",{staticClass:"modal-close",on:{click:function(t){return e.handleCloseModal("addFalseUser")}}},[e._v("X")])]),a("div",{staticClass:"add-content"},[a("div",{staticClass:"add-item"},[a("p",{staticClass:"columns-item-label"},[e._v("用户昵称")]),a("i-input",{staticClass:"columns-item-input",model:{value:e.addFalseUser.nickname,callback:function(t){e.$set(e.addFalseUser,"nickname",t)},expression:"addFalseUser.nickname"}})],1),e._e(),e._e(),a("div",{staticClass:"add-item"},[a("i-button",{on:{click:e.handleAddFalseUser}},[e._v("添加")])],1)])]),a("div",{directives:[{name:"show",rawName:"v-show",value:e.modifyFalseUser.show,expression:"modifyFalseUser.show"}],staticClass:"modify-false-user"},[a("div",{staticClass:"modify-header"},[a("span",[e._v("虚拟用户设置")]),a("span",{staticClass:"modal-close",on:{click:function(t){return e.handleCloseModal("modifyFalseUser")}}},[e._v("X")])]),a("div",{staticClass:"modify-content"},[a("div",{staticClass:"modify-c-h"},[a("div",{staticClass:"modify-item"},[e.modifyFalseUser.header?a("img",{staticClass:"user-header",attrs:{src:e.showUploadImg||e.addFalseUser.header||e.modifyFalseUser.header,alt:"用户头像"}}):e._e(),a("p",{staticClass:"change-header",on:{click:function(t){e.showUploadHeader=!e.showUploadHeader}}},[e._v("更改头像")])]),a("div",{staticClass:"modify-list"},[a("div",{staticClass:"modify-column"},[a("div",{staticClass:"modify-item"},[a("span",{staticClass:"columns-item-label"},[e._v("昵称")]),a("i-input",{staticClass:"columns-item-input",model:{value:e.modifyFalseUser.nickname,callback:function(t){e.$set(e.modifyFalseUser,"nickname",t)},expression:"modifyFalseUser.nickname"}})],1),a("div",{staticClass:"modify-item"},[a("span",{staticClass:"columns-item-label"},[e._v("ID")]),a("i-input",{staticClass:"columns-item-input",attrs:{disabled:""},model:{value:e.modifyFalseUser.showId,callback:function(t){e.$set(e.modifyFalseUser,"showId",t)},expression:"modifyFalseUser.showId"}})],1)]),a("div",{staticClass:"modify-column"},[a("div",{staticClass:"modify-item column"},[a("span",[e._v("选择手动发言房间：")]),a("select",{directives:[{name:"model",rawName:"v-model",value:e.modifyFalseUser.gameType,expression:"modifyFalseUser.gameType"}],on:{change:function(t){var a=Array.prototype.filter.call(t.target.options,function(e){return e.selected}).map(function(e){var t="_value"in e?e._value:e.value;return t});e.$set(e.modifyFalseUser,"gameType",t.target.multiple?a:a[0])}}},e._l(e.gameList,function(t,i){return a("option",{key:i,domProps:{value:t.type}},[e._v(e._s(t.name))])}),0)]),a("div",{staticClass:"modify-item input"},[a("input",{directives:[{name:"model",rawName:"v-model",value:e.modifyFalseUser.inputText,expression:"modifyFalseUser.inputText"}],attrs:{placeholder:"请输入聊天内容"},domProps:{value:e.modifyFalseUser.inputText},on:{input:function(t){t.target.composing||e.$set(e.modifyFalseUser,"inputText",t.target.value)}}})]),a("div",{staticClass:"modify-item"},[a("div",{staticClass:"send-button",on:{click:e.handleSend}},[e._v("发送聊天")])])])])]),a("div",{staticClass:"modify-c-b"},[e._m(4),a("div",{staticClass:"game-list"},e._l(e.modifyFalseUser.selectGames,function(t,i){return a("div",{key:i,staticClass:"game-item"},[a("switchs",{attrs:{open:t.Check},on:{switch:function(a){return e.handleSwitchGame(t)}}}),a("p",{domProps:{textContent:e._s(e.gameList.find(function(e){return e.type===t.GameType}).name)}}),a("span",[e._v("下注行为")]),a("select",{directives:[{name:"model",rawName:"v-model",value:t.BehaviorID,expression:"item.BehaviorID"}],on:{change:function(a){var i=Array.prototype.filter.call(a.target.options,function(e){return e.selected}).map(function(e){var t="_value"in e?e._value:e.value;return t});e.$set(t,"BehaviorID",a.target.multiple?i:i[0])}}},e._l(e.behaviorList,function(t,i){return a("option",{key:i,domProps:{value:t.id}},[e._v(e._s(t.name))])}),0)],1)}),0)]),a("div",{staticClass:"modify-operate"},[a("div",{staticClass:"enter-button",on:{click:e.handleModifyFalseUser}},[e._v("确定")]),a("div",{staticClass:"cancel-button",on:{click:function(t){return e.handleCloseModal("modifyFalseUser")}}},[e._v("取消")])]),a("div",{staticClass:"modify-footer"})])])]),a("Modal",{attrs:{styles:{top:"200px"},width:"300"},model:{value:e.showDelete,callback:function(t){e.showDelete=t},expression:"showDelete"}},[a("p",{staticStyle:{color:"#0d1941","font-size":"16px","text-align":"center"},attrs:{slot:"header"},slot:"header"},[a("span",[e._v("确认删除")])]),a("p",{staticStyle:{"text-align":"center"}},[e._v("是否确认删除？")]),a("div",{staticClass:"modal-footer",attrs:{slot:"footer"},slot:"footer"},[a("div",{staticClass:"enter-cancel",on:{click:function(t){e.showDelete=!1}}},[e._v("取消")]),a("div",{staticClass:"enter-ok",on:{click:e.handleEnter}},[e._v("确定")])])]),a("input",{directives:[{name:"show",rawName:"v-show",value:!1,expression:"false"}],ref:"uploadImg",attrs:{type:"file"}}),a("aiSetting",{attrs:{aiShow:e.AIShow,id:e.AIID,userId:e.AIUSERID},on:{closeModal:e.handleCloseAIModal,editBehavior:e.handleEditBehavior}}),a("my-upload",{attrs:{width:100,height:100,"img-format":"png"},on:{"crop-success":e.handleCropSuccess},model:{value:e.showUploadHeader,callback:function(t){e.showUploadHeader=t},expression:"showUploadHeader"}})],1)},s=[function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"false-user-header"},[a("span",{staticClass:"false-user-title"},[e._v("虚拟用户列表")]),a("p",{staticClass:"false-user-poptip"},[a("span",[e._v("注：该列表只显示通过右侧增加的虚拟用户，正常改虚拟的不在此显示")]),a("br"),a("span",[e._v("新增虚拟用户后，需要设置游戏行为后才能启用")])])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"data-header"},[a("p",{staticClass:"item-info"},[e._v("头像")]),a("p",{staticClass:"item-info"},[e._v("用户名")]),a("p",{staticClass:"item-info"},[e._v("ID")]),a("p",{staticClass:"item-info"},[e._v("账户余额")]),a("p",{staticClass:"item-info"},[e._v("选择游戏")]),a("p",{staticClass:"item-info"},[e._v("是否启用")]),a("p",{staticClass:"item-info"},[e._v("操作")])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"false-user-header"},[a("span",{staticClass:"false-user-title"},[e._v("操作设置")])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"behavior-header"},[a("span",[e._v("名称")]),a("span",[e._v("方案数量")]),a("span",[e._v("操作")])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"false-user-header"},[a("span",{staticClass:"false-user-title"},[e._v("账户参与游戏及下注行为选择")]),a("div",{staticClass:"false-user-poptip"},[a("p",[e._v("注：最大参与游戏最多选择两项，正常玩家无法在多个游戏中期期不漏的投注订单")]),a("p",[e._v("如果在多个游戏中增加虚拟玩家，请分别建立虚拟用户，并指定不同的游戏")])])])}],n=(a("f763"),a("608b"),a("b745"),a("e680"),a("7bc1"),function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{directives:[{name:"show",rawName:"v-show",value:e.aiShow,expression:"aiShow"}]},[a("div",{staticClass:"ai-setting-shade",style:{"align-items":e.clientHeight<800?"flex-start":"center"}},[a("div",{directives:[{name:"show",rawName:"v-show",value:e.aiShow,expression:"aiShow"}],staticClass:"ai-active",style:{width:e.showMethod?"1071px":"720px"}},[a("div",{staticClass:"ai-header"},[a("span",[e._v("行为管理")]),a("span",{staticClass:"modal-close",on:{click:function(t){return e.handleCloseModal(2)}}},[e._v("X")])]),a("div",{staticClass:"ai-setting-list"},[a("div",{staticClass:"active-list"},[a("div",{staticClass:"active-1"},[e._m(0),a("div",{staticClass:"active-item"},[a("span",[e._v("名称：")]),a("i-input",{staticClass:"column-input",staticStyle:{width:"228px"},model:{value:e.active.BehaviorName,callback:function(t){e.$set(e.active,"BehaviorName",t)},expression:"active.BehaviorName"}})],1),a("div",{staticClass:"active-item"},[a("Checkbox",{model:{value:e.active.Attack.Check,callback:function(t){e.$set(e.active.Attack,"Check",t)},expression:"active.Attack.Check"}},[e._v("开战")]),a("InputNumber",{staticClass:"columns-item-input",attrs:{min:0},model:{value:e.active.Attack.StartTime,callback:function(t){e.$set(e.active.Attack,"StartTime",t)},expression:"active.Attack.StartTime"}}),a("span",[e._v("至")]),a("InputNumber",{staticClass:"columns-item-input",attrs:{min:0},model:{value:e.active.Attack.EndTime,callback:function(t){e.$set(e.active.Attack,"EndTime",t)},expression:"active.Attack.EndTime"}}),a("span",[e._v("秒内使用方案进行攻击。（需勾选方案“启用”按钮）")])],1),a("div",{staticClass:"active-item"},[a("Checkbox",{model:{value:e.active.AttackQuery.Check,callback:function(t){e.$set(e.active.AttackQuery,"Check",t)},expression:"active.AttackQuery.Check"}},[e._v("开战")]),a("InputNumber",{staticClass:"columns-item-input",staticStyle:{width:"50px"},attrs:{min:0},model:{value:e.active.AttackQuery.StartTime,callback:function(t){e.$set(e.active.AttackQuery,"StartTime",t)},expression:"active.AttackQuery.StartTime"}}),a("span",[e._v("至")]),a("InputNumber",{staticClass:"columns-item-input",staticStyle:{width:"50px"},attrs:{min:0},model:{value:e.active.AttackQuery.EndTime,callback:function(t){e.$set(e.active.AttackQuery,"EndTime",t)},expression:"active.AttackQuery.EndTime"}}),a("span",[e._v("秒内使用查询指令，几率为")]),a("InputNumber",{staticClass:"columns-item-input",staticStyle:{width:"50px"},attrs:{min:0},model:{value:e.active.AttackQuery.Probability,callback:function(t){e.$set(e.active.AttackQuery,"Probability",t)},expression:"active.AttackQuery.Probability"}}),a("span",[e._v("(1为100%)")]),a("span",[e._v("命令")]),a("i-input",{staticClass:"column-input",staticStyle:{width:"130px"},model:{value:e.active.AttackQuery.Keyword,callback:function(t){e.$set(e.active.AttackQuery,"Keyword",t)},expression:"active.AttackQuery.Keyword"}})],1),a("div",{staticClass:"active-item"},[a("Checkbox",{model:{value:e.active.ArmisticeQuery.Check,callback:function(t){e.$set(e.active.ArmisticeQuery,"Check",t)},expression:"active.ArmisticeQuery.Check"}},[e._v("休战")]),a("InputNumber",{staticClass:"columns-item-input",staticStyle:{width:"50px"},attrs:{min:0},model:{value:e.active.ArmisticeQuery.StartTime,callback:function(t){e.$set(e.active.ArmisticeQuery,"StartTime",t)},expression:"active.ArmisticeQuery.StartTime"}}),a("span",[e._v("至")]),a("InputNumber",{staticClass:"columns-item-input",staticStyle:{width:"50px"},attrs:{min:0},model:{value:e.active.ArmisticeQuery.EndTime,callback:function(t){e.$set(e.active.ArmisticeQuery,"EndTime",t)},expression:"active.ArmisticeQuery.EndTime"}}),a("span",[e._v("秒内使用查询指令，几率为")]),a("InputNumber",{staticClass:"columns-item-input",staticStyle:{width:"50px"},attrs:{min:0},model:{value:e.active.ArmisticeQuery.Probability,callback:function(t){e.$set(e.active.ArmisticeQuery,"Probability",t)},expression:"active.ArmisticeQuery.Probability"}}),a("span",[e._v("(1为100%)")]),a("span",[e._v("命令")]),a("i-input",{staticClass:"column-input",staticStyle:{width:"130px"},model:{value:e.active.ArmisticeQuery.Keyword,callback:function(t){e.$set(e.active.ArmisticeQuery,"Keyword",t)},expression:"active.ArmisticeQuery.Keyword"}})],1),a("div",{staticClass:"active-item"},[a("Checkbox",{model:{value:e.active.UpCmd.Check,callback:function(t){e.$set(e.active.UpCmd,"Check",t)},expression:"active.UpCmd.Check"}},[e._v("余额低于")]),a("InputNumber",{staticClass:"columns-item-input",staticStyle:{width:"107px"},attrs:{min:0},model:{value:e.active.UpCmd.Limit,callback:function(t){e.$set(e.active.UpCmd,"Limit",t)},expression:"active.UpCmd.Limit"}}),a("span",[e._v("时，发送上分指令，操作金额")]),a("InputNumber",{staticClass:"columns-item-input",staticStyle:{width:"107px"},attrs:{min:0},model:{value:e.active.UpCmd.Variety,callback:function(t){e.$set(e.active.UpCmd,"Variety",t)},expression:"active.UpCmd.Variety"}}),a("span",[e._v("命令")]),a("i-input",{staticClass:"column-input",staticStyle:{width:"130px"},model:{value:e.active.UpCmd.Keyword,callback:function(t){e.$set(e.active.UpCmd,"Keyword",t)},expression:"active.UpCmd.Keyword"}})],1),a("div",{staticClass:"active-item"},[a("Checkbox",{model:{value:e.active.DownCmd.Check,callback:function(t){e.$set(e.active.DownCmd,"Check",t)},expression:"active.DownCmd.Check"}},[e._v("余额高于")]),a("InputNumber",{staticClass:"columns-item-input",staticStyle:{width:"107px"},attrs:{min:0},model:{value:e.active.DownCmd.Limit,callback:function(t){e.$set(e.active.DownCmd,"Limit",t)},expression:"active.DownCmd.Limit"}}),a("span",[e._v("时，发送下分指令，操作金额")]),a("InputNumber",{staticClass:"columns-item-input",staticStyle:{width:"107px"},attrs:{min:0},model:{value:e.active.DownCmd.Variety,callback:function(t){e.$set(e.active.DownCmd,"Variety",t)},expression:"active.DownCmd.Variety"}}),a("span",[e._v("命令")]),a("i-input",{staticClass:"column-input",staticStyle:{width:"130px"},model:{value:e.active.DownCmd.Keyword,callback:function(t){e.$set(e.active.DownCmd,"Keyword",t)},expression:"active.DownCmd.Keyword"}})],1),a("div",{staticClass:"active-item"},[a("Checkbox",{model:{value:e.active.StopCmd.Check,callback:function(t){e.$set(e.active.StopCmd,"Check",t)},expression:"active.StopCmd.Check"}},[e._v("余额低于")]),a("InputNumber",{staticClass:"columns-item-input",staticStyle:{width:"107px"},attrs:{min:0},model:{value:e.active.StopCmd.Limit,callback:function(t){e.$set(e.active.StopCmd,"Limit",t)},expression:"active.StopCmd.Limit"}}),a("span",[e._v("时，停止攻击")]),a("Checkbox",{staticStyle:{marginLeft:"50px"},model:{value:e.active.EndPoint,callback:function(t){e.$set(e.active,"EndPoint",t)},expression:"active.EndPoint"}},[e._v("游戏结束时下分")])],1)]),e._m(1),e._m(2),a("div",{staticClass:"method-operate"},[a("span",{on:{click:e.handleSaveBehavior}},[e._v("保存")]),a("span",{on:{click:function(t){return e.handleCloseModal(2)}}},[e._v("取消")])])]),a("div",{directives:[{name:"show",rawName:"v-show",value:e.showMethod,expression:"showMethod"}],staticClass:"method-list"},[e._m(3),a("div",{staticClass:"method-l"},[a("div",{staticClass:"method-item"},[a("span",[e._v("方案列表：")]),a("Select",{staticStyle:{width:"110px"},on:{"on-change":e.handleChangeMethod},model:{value:e.method.checkId,callback:function(t){e.$set(e.method,"checkId",t)},expression:"method.checkId"}},e._l(e.methodHeaderList,function(t,i){return a("Option",{key:i,attrs:{value:t.id}},[e._v(e._s(t.name))])}),1),a("Select",{staticStyle:{width:"110px"},attrs:{disabled:""},model:{value:e.method.checkType,callback:function(t){e.$set(e.method,"checkType",t)},expression:"method.checkType"}},[a("Option",{attrs:{value:"1"}},[e._v("不翻倍")]),a("Option",{attrs:{value:"2"}},[e._v("中翻倍")]),a("Option",{attrs:{value:"3"}},[e._v("不中翻倍")])],1)],1),a("div",{staticClass:"method-item"},[a("span",[e._v("金额设置：")]),a("i-input",{staticClass:"column-input",staticStyle:{width:"176px"},attrs:{disabled:""},model:{value:e.method.money,callback:function(t){e.$set(e.method,"money",t)},expression:"method.money"}}),a("Checkbox",{staticStyle:{marginLeft:"2px"},attrs:{disabled:""},model:{value:e.method.enable,callback:function(t){e.$set(e.method,"enable",t)},expression:"method.enable"}},[e._v("启用")])],1)]),a("div",{staticClass:"bet-list"},[a("p",[e._v("此方案可随机抽取注单列表（点击表格添加或修改）")]),e._l(e.method.betList,function(t,i){return a("p",{key:i},[e._v(e._s(t.value))])})],2),a("div",{staticClass:"method-item operate-list"},[a("span",{on:{click:function(t){e.addShow=!0}}},[e._v("添加方案")]),a("span",{on:{click:function(t){e.selectMethod=e.method,e.modifyShow=!0}}},[e._v("修改方案")]),a("span",{on:{click:e.handleDeleteMethod}},[e._v("删除方案")])])])])]),a("div",{directives:[{name:"show",rawName:"v-show",value:e.addShow||e.modifyShow,expression:"addShow || modifyShow"}],staticClass:"edit-panel"},[a("div",{staticClass:"ai-header"},[a("span",[e._v(e._s(e.addShow?"新增方案":"修改方案"))]),a("span",{staticClass:"modal-close",on:{click:function(t){return e.handleCloseModal(3)}}},[e._v("X")])]),a("div",{staticClass:"edit-item"},[a("span",[e._v("方案名称：")]),a("i-input",{staticStyle:{width:"140px"},model:{value:e.selectMethod.name,callback:function(t){e.$set(e.selectMethod,"name",t)},expression:"selectMethod.name"}}),a("Select",{staticStyle:{width:"140px"},model:{value:e.selectMethod.checkType,callback:function(t){e.$set(e.selectMethod,"checkType",t)},expression:"selectMethod.checkType"}},[a("Option",{attrs:{value:"1"}},[e._v("不翻倍")]),a("Option",{attrs:{value:"2"}},[e._v("中翻倍")]),a("Option",{attrs:{value:"3"}},[e._v("不中翻倍")])],1)],1),a("div",{staticClass:"edit-item"},[a("span",[e._v("金额设置：")]),a("i-input",{staticStyle:{width:"228px"},model:{value:e.selectMethod.money,callback:function(t){e.$set(e.selectMethod,"money",t)},expression:"selectMethod.money"}}),a("Checkbox",{staticStyle:{marginLeft:"2px"},model:{value:e.selectMethod.enable,callback:function(t){e.$set(e.selectMethod,"enable",t)},expression:"selectMethod.enable"}},[e._v("启用")])],1),a("div",{staticClass:"edit-item"},[a("span",[e._v("投注信息：")]),a("div",{staticClass:"edit-operate"},[a("span",{staticClass:"plus",on:{click:e.handleAddBet}}),a("span",{staticClass:"minus",on:{click:e.handleMinusBet}})])]),a("div",{staticClass:"edit-item"},[a("ul",e._l(e.selectMethod.betList,function(t,i){return a("li",{key:i},[a("i-input",{staticClass:"column-input",attrs:{placeholder:"请填写投注信息"},model:{value:t.value,callback:function(a){e.$set(t,"value",a)},expression:"item.value"}})],1)}),0)]),a("div",{staticClass:"edit-item save-item"},[a("span",{domProps:{textContent:e._s(e.addShow?"确定保存":"修改")},on:{click:e.handleOperate}}),a("span",{on:{click:e.handleAddList}},[e._v("导入模板")])])]),e.batchShow?a("div",{staticClass:"add-batch"},[a("div",{staticClass:"batch-header"},[a("p",{staticClass:"batch-title"},[e._v("批量添加"),a("span",{staticClass:"modal-close",on:{click:function(t){return e.handleCloseModal(4)}}},[e._v("X")])]),a("p",{staticClass:"batch-poptip"},[e._v('各个投注信息之间用逗号","或者分号";"隔开，支持中文符号')])]),a("div",{staticClass:"batch-content"},[a("textarea",{directives:[{name:"model",rawName:"v-model",value:e.batchContent,expression:"batchContent"}],domProps:{value:e.batchContent},on:{input:function(t){t.target.composing||(e.batchContent=t.target.value)}}})]),a("div",{staticClass:"enter-add",on:{click:e.handleEnterBatch}},[e._v("确认添加")]),a("div",{staticClass:"list-operate"},[a("div",{staticClass:"import-item",on:{click:e.importTenMethod}},[e._v("导入10球模板")]),a("div",{staticClass:"import-item",on:{click:e.importFiveMethod}},[e._v("导入5球模板")])])]):e._e(),a("loading",{attrs:{loading:e.isLoading}}),a("Modal",{attrs:{title:"确认删除"},on:{"on-ok":e.handleEnterDelete,"on-cancel":function(t){e.showDelete=!1}},model:{value:e.showDelete,callback:function(t){e.showDelete=t},expression:"showDelete"}},[a("p",[e._v("是否确认删除？")])])],1)])}),o=[function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"active-header"},[a("span",{staticClass:"active-title"},[e._v("行为设置")])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"active-2"},[a("p",{staticClass:"poptip-header"},[e._v("行为设置说明")]),a("ul",[a("li",[e._v("1.攻击时间可根据需求酌情微调，攻击时间范围尽量覆盖对应游戏类型的整个未封盘时间区间。")]),a("li",[e._v("2.开战休战订单查询和流水查询，默认触发概率0.1比较接近实际场景，各个行为中的开战和休战起止尽量设置一些差异。")]),a("li",[e._v("3.余额低于XX停止攻击，默认不勾选。选中后会根据场景触发停止运行。")]),a("li",[e._v("4.余额低于XX上分，此数值的比例请对应您的方案金额。不然会出现冲大买小的现象。")])])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"active-2"},[a("p",{staticClass:"poptip-header"},[e._v("方案管理说明")]),a("ul",[a("li",[e._v("1.方案列表中，每多一个方案，则会在每期多一个投注订单，订单是从方案中随机抽取的。重盖概率接近于0。用户可根据自己的情况在方案中增加、修改和删除。")]),a("li",[e._v("2.默认方案已经设置了近万种组合，如果不愿意自定义，则直接使用预设的方案就能满足几乎全部场景。")])])])},function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"active-header"},[a("span",{staticClass:"active-title"},[e._v("方案设置")])])}],c=(a("7364"),a("d4d5"),a("34a3"),{props:{aiShow:{type:Boolean,default:!1},id:{type:String}},data:function(){var e=this;return{showMethod:!1,batchContent:"",clientHeight:document.body.clientHeight,showDelete:!1,addShow:!1,modifyShow:!1,activeShow:!1,showAcitveCol:[{title:"名称",key:"name",align:"center"},{title:"方案数量",key:"num",align:"center"},{desc:"operate",title:"操作",className:"record-item",align:"center",render:function(t,a){return t("div",[t("Button",{props:{type:"primary",size:"small"},style:{marginRight:"5px"},on:{click:function(){e.handleShowActive(a.row)}}},"修改行为")])}}],aiSettingList:[{id:"",name:"test",num:2}],active:{BehaviorId:"",BehaviorName:"演示模板-10球",Attack:{StartTime:15,EndTime:250,Check:!0},AttackQuery:{keyword:"查",Probability:.1,StartTime:20,EndTime:120,Check:!0},ArmisticeQuery:{Keyword:"查",Probability:.1,StartTime:15,EndTime:60,Check:!0},UpCmd:{Limit:500,Variety:1e3,Keyword:"上分",Check:!0},DownCmd:{Limit:2e3,Variety:1e3,Keyword:"下分",Check:!0},StopCmd:{Limit:0,Check:!0},EndPoint:!1},method:{checkId:"",checkType:"",name:"",money:"",betList:[],enable:!1},selectMethod:{checkId:"",checkType:"1",name:"",money:"",betList:[{value:""}],enable:!1},isLoading:!1,isMLoading:!1,methodHeaderList:[],batchShow:!1}},watch:{aiShow:function(e){var t=this;e&&(t.active.BehaviorId=t.id,t.methodHeaderList=[],t.id?(t.showMethod=!0,t.handleGetBehaviorInfo(),t.handleGetMethodList()):(t.method={checkId:"",checkType:"",name:"",money:"",betList:[],enable:!1},t.active={BehaviorId:"",BehaviorName:"演示模板-10球",Attack:{StartTime:15,EndTime:250,Check:!0},AttackQuery:{Keyword:"查",Probability:.1,StartTime:20,EndTime:120,Check:!0},ArmisticeQuery:{Keyword:"查",Probability:.1,StartTime:15,EndTime:60,Check:!0},UpCmd:{Limit:500,Variety:1e3,Keyword:"上分",Check:!0},DownCmd:{Limit:2e3,Variety:1e3,Keyword:"下分",Check:!0},StopCmd:{Limit:0,Check:!0},EndPoint:!1}))}},methods:{importTenMethod:function(){var e=this;e.handleSendFile("ten")},importFiveMethod:function(){var e=this;e.handleSendFile("five")},handleSendFile:function(e){var t=this,a="";switch(e){case"five":a="/pro/5ball.txt";break;case"ten":a="/pro/10ball.txt";break}t.isMLoading||(t.isMLoading=!0,t.$axios({url:a}).then(function(e){if(t.isMLoading=!1,200===e.status){var a=e.data.split(/\n/);t.selectMethod.betList=a.map(function(e){return{value:e}}),t.batchShow=!1}}))},handleDeleteMethod:function(){var e=this;e.method.checkId?e.showDelete=!0:e.$Message.error("没有选择方案，不能删除！")},handleEnterBatch:function(){var e=this,t=e.batchContent.replace(/,|，|；|;|\n|\r|\r\n|↵/g,";"),a=t.split(";");e.selectMethod.betList=a.map(function(e){return{value:e}}),e.batchShow=!1},handleAddList:function(){var e=this;e.batchShow=!0},handleChangeMethod:function(){var e=this;e.isMLoading||(e.isMLoading=!0,e.$axios({url:"/api/Setup/GetProgramInfo",params:{programID:e.method.checkId}}).then(function(t){if(e.isMLoading=!1,100===t.data.Status){var a=t.data.Model;e.method={checkId:a.ProgramID,name:a.ProgramName,checkType:a.DoubleType+"",money:a.Amountset,enable:a.IsEnable,betList:a.BetTypeList.map(function(e){return{value:e}})}}}))},handleOperate:function(){var e=this;e.addShow?e.handleModifyMethod("/api/Setup/AddProgramInfo"):e.handleModifyMethod("/api/Setup/PutProgramInfo")},handleEnterDelete:function(){var e=this;e.isMLoading||(e.isMLoading=!0,e.$axios({url:"/api/Setup/DeleteProgramInfo",method:"delete",params:{programID:e.method.checkId}}).then(function(t){e.isMLoading=!1,100===t.data.Status?(e.$Message.success(t.data.Message),e.method={checkId:"",checkType:"",name:"",money:"",betList:[],enable:!1},e.handleGetMethodList()):e.$Message.error(t.data.Message)}))},handleModifyMethod:function(e){var t=this,a=t.selectMethod.money.replace(/，/g,",").split(","),i=!1;a.forEach(function(e){Number(e)||(i=!0)}),i?t.$Message.error("金额设置有误，请重新设置"):t.selectMethod.money&&t.selectMethod.name?0!==t.selectMethod.betList.filter(function(e){return e.value}).length?t.isMLoading||(t.isMLoading=!1,t.$axios({url:e,method:"put",data:{BehaviorID:t.id,ProgramID:t.selectMethod.checkId,ProgramName:t.selectMethod.name,DoubleType:t.selectMethod.checkType,Amountset:t.selectMethod.money.replace("，",","),IsEnable:t.selectMethod.enable,BetTypeList:t.selectMethod.betList.map(function(e){return e.value})}}).then(function(e){t.isMLoading=!1,100===e.data.Status?(t.$Message.success(e.data.Message),t.addShow&&t.handleGetMethodList(),t.addShow=!1,t.modifyShow&&t.handleGetMethodList(),t.modifyShow=!1,t.selectMethod.name="",t.selectMethod.checkType="",t.selectMethod.money="",t.selectMethod.enable=!1,t.selectMethod.betList=[]):t.$Message.error(e.data.Message)})):t.$Message.error("投注信息有误，请重新设置"):t.$Message.error("方案信息有误，请重新设置")},handleGetMethodList:function(){var e=this;e.isMLoading||(e.isMLoading=!0,e.$axios({url:"/api/Setup/GetProgramList",params:{behaviorID:e.id}}).then(function(t){e.isMLoading=!1,100===t.data.Status&&(e.methodHeaderList=t.data.Data.map(function(e){return{name:e.ProgramName,id:e.ProgramID}}),e.methodHeaderList.length&&(e.method.checkId||(e.method.checkId=e.methodHeaderList[0].id),e.handleChangeMethod()))}))},handleGetBehaviorInfo:function(){var e=this;e.isLoading||(e.isLoading=!0,e.$axios({url:"/api/Setup/GetBehaviorInfo",params:{behaviorID:e.active.BehaviorId}}).then(function(t){e.isLoading=!1,100===t.data.Status&&(e.active=t.data.Model)}))},handleAddBet:function(){var e=this,t=e.selectMethod.betList.length;e.selectMethod.betList.splice(t,0,{value:""})},handleMinusBet:function(){var e=this,t=e.selectMethod.betList.length;1===t?e.$Message.error("至少保留一个投注信息"):e.selectMethod.betList.splice(t-1,1)},handleCloseModal:function(e){var t=this;switch(1!==e&&2!==e||(t.showMethod=!1,t.method={checkId:"",checkType:"",name:"",money:"",betList:[],enable:!1},t.active={BehaviorId:"",BehaviorName:"",Attack:{StartTime:0,EndTime:0,Check:!1},AttackQuery:{keyword:"",Probability:0,StartTime:0,EndTime:0,Check:!1},ArmisticeQuery:{Keyword:"",Probability:0,StartTime:0,EndTime:0,Check:!1},UpCmd:{Limit:0,Variety:0,Keyword:"",Check:!1},DownCmd:{Limit:0,Variety:0,Keyword:"",Check:!1},StopCmd:{Limit:0,Check:!1},EndPoint:!1}),e){case 1:t.$emit("closeModal");break;case 2:t.$emit("closeModal");break;case 3:t.addShow=!1,t.modifyShow=!1,t.batchShow=!1;break;case 4:t.batchShow=!1;break}},handleShowActive:function(e){var t=this;t.activeShow=!0},handleSaveBehavior:function(){var e=this,t="";e.active.BehaviorName?(t=e.id?"/api/Setup/PutBehaviorInfo":"/api/Setup/AddBehaviorInfo",e.isLoading||(e.isLoading=!0,e.$axios({url:t,method:"put",data:{BehaviorID:e.id,BehaviorName:e.active.BehaviorName,Attack:{StartTime:e.active.Attack.StartTime,EndTime:e.active.Attack.EndTime,Check:e.active.Attack.Check},AttackQuery:{Probability:e.active.AttackQuery.Probability,Keyword:e.active.AttackQuery.Keyword,StartTime:e.active.AttackQuery.StartTime,EndTime:e.active.AttackQuery.EndTime,Check:e.active.AttackQuery.Check},ArmisticeQuery:{Probability:e.active.ArmisticeQuery.Probability,Keyword:e.active.ArmisticeQuery.Keyword,StartTime:e.active.ArmisticeQuery.StartTime,EndTime:e.active.ArmisticeQuery.EndTime,Check:e.active.ArmisticeQuery.Check},UpCmd:{Check:e.active.UpCmd.Check,Limit:e.active.UpCmd.Limit,Variety:e.active.UpCmd.Variety,Keyword:e.active.UpCmd.Keyword},DownCmd:{Check:e.active.DownCmd.Check,Limit:e.active.DownCmd.Limit,Variety:e.active.DownCmd.Variety,Keyword:e.active.DownCmd.Keyword},StopCmd:{Check:e.active.StopCmd.Check,Limit:e.active.StopCmd.Limit},EndPoint:e.active.EndPoint}}).then(function(t){e.isLoading=!1,100===t.data.Status&&(e.$Message.success(t.data.Message),e.id||(e.showMethod=!0,e.$emit("editBehavior",t.data.Keyword)))}))):e.$Message.error("请输入行为名称")}}}),l=c,r=(a("06e7"),a("6691")),d=Object(r["a"])(l,n,o,!1,null,null,null),m=d.exports,h=a("873a"),u=a("663a"),v={components:{aiSetting:m,switchs:h["a"],myUpload:u["a"]},data:function(){return{page:{total:0,pageSize:10,pageNum:1},showLoading:!1,AIID:"",AIUSERID:"",AIShow:!1,addFalseUser:{show:!1,nickname:"",header:"",status:"prohibit"},modifyFalseUser:{show:!1,id:"",showId:"",nickname:"",header:"",selectGames:[],userId:"",isLoading:!1,gameType:"1",inputText:""},gameList:[],falseUserStatusList:[{title:"启用",value:"enable"},{title:"禁用",value:"prohibit"}],showUploadHeader:!1,falseUserData:[],isLoading:!1,showDelete:!1,uploadImg:"",showUploadImg:"",behaviorList:[],behaviorId:"",gameStatus:[]}},mounted:function(){var e=this;e.getGameList().then(function(){e.getData(),e.handleGetBehaviorList(),e.getRoomDataStatus()})},methods:{handleSend:function(){var e=this;if(!e.modifyFalseUser.inputText)return!1;e.$axios({url:"/api/Setup/ShamUserSendMessage",params:{userID:e.modifyFalseUser.userId,gameType:e.modifyFalseUser.gameType,message:e.modifyFalseUser.inputText}}).then(function(t){100==t.data.Status?(e.modifyFalseUser.inputText="",e.$Message.success("发送成功")):e.$Message.success("发送失败")}).catch(function(){e.$Message.success("发送失败")})},getRoomDataStatus:function(){var e=this;e.$axios({url:"/api/Room/GetGameIDs"}).then(function(t){if(100===t.data.Status){var a=t.data.Data;e.gameStatus=a.map(function(e){return{id:e.ID,status:e.Status,type:e.Type}})}})},handleCropSuccess:function(e,t){var a=this,i=new FormData,s=a.dataURItoFile(e);s.size>5242880?a.$Message.error("上传图片不能超过5M"):(a.showUploadImg=e,a.isLoading||(a.isLoading=!0,i.append("fileinput",s),a.$axios({url:"/api/User/UpdateUserImages",method:"put",params:{userID:a.modifyFalseUser.userId},data:i}).then(function(e){a.isLoading=!1,a.$Message.success(e.data.Message),100===e.data.Status&&a.getData()})))},dataURItoFile:function(e){var t;t=e.split(",")[0].indexOf("base64")>=0?atob(e.split(",")[1]):unescape(e.split(",")[1]);for(var a=e.split(",")[0].split(":")[1].split(";")[0],i=new Uint8Array(t.length),s=0;s<t.length;s++)i[s]=t.charCodeAt(s);return new File([i],"upload.png",{type:a})},getGameList:function(){var e=this;return new Promise(function(t,a){e.$axios({url:"/api/Merchant/GetGameList"}).then(function(i){if(100===i.data.Status){var s=i.data.Data;e.gameList=s.map(function(e){return{name:e.GameName,type:e.GameType,key:e.NickName,value:0}}).sort(function(e,t){return e.type-t.type}),t()}a()})})},handleImageError:function(e){e.header="/UserImages/default.png"},handleChangeSize:function(e){var t=this;t.page.pageSize=e,t.page.pageNum=1,t.getData()},handleChangePage:function(e){var t=this;t.page.pageNum=e,t.getData()},handleEditBehavior:function(e){var t=this;t.AIID=e},handleShowEdit:function(e){var t=this;t.AIID=e,t.handleShowAI()},handleShowDelete:function(e){var t=this;t.behaviorId=e,t.showDelete=!0},handleEnter:function(){var e=this;e.behaviorId?e.handleDeleteBehavior():e.handleEnterDelete()},handleDeleteBehavior:function(){var e=this;e.isLoading||(e.isLoading=!0,e.$axios({url:"/api/Setup/DeleteBehavior",method:"delete",params:{behaviorID:e.behaviorId}}).then(function(t){100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.isLoading=!1,e.behaviorId="",e.showDelete=!1,e.handleGetBehaviorList()}))},handleSwitch:function(e){var t=this;t.behaviorList.length<1?t.$Message.error("没有添加行为，不能启用该虚拟用户"):t.isLoading||(t.isLoading=!0,t.showLoading=!0,t.$axios({url:"/api/Setup/ShamUserOpenOrClose",method:"put",params:{id:e.id}}).then(function(e){100===e.data.Status?t.$Message.success(e.data.Message):t.$Message.error(e.data.Message),t.showLoading=!1,t.isLoading=!1,t.getData()}))},handleSwitchGame:function(e){var t=this,a=t.gameStatus.find(function(t){return t.type==e.GameType}).status;1==a?e.Check=!e.Check:t.$Message.error("该游戏未开启，不能启用行为!")},handleGetBehaviorList:function(){var e=this;e.modifyFalseUser.isLoading||(e.modifyFalseUser.isLoading=!0,e.$axios({url:"/api/Setup/GetBehaviorManage"}).then(function(t){e.modifyFalseUser.isLoading=!1,100===t.data.Status&&(e.behaviorList=t.data.Data.map(function(e){return{name:e.BehaviorName,num:e.Count,id:e.BehaviorID}}))}))},handleShowAI:function(){var e=this;e.AIShow=!0},handleCloseAIModal:function(){var e=this;e.AIID="",e.handleGetBehaviorList(),e.AIShow=!1},handleShowAddUser:function(){var e=this;e.addFalseUser.show=!0},handleOperateUser:function(e){var t=this;t.isLoading||(t.isLoading=!0,t.$axios({url:"/api/Setup/ShamUserAllOpenOrClost",method:"put",params:{derail:e}}).then(function(e){t.isLoading=!1,100===e.data.Status?t.$Message.success(e.data.Message):t.$Message.error(e.data.Message),t.getData()}))},handleShowModifyUser:function(e){var t=this;t.modifyFalseUser.userId=e.userId,t.modifyFalseUser.showId=e.showId,t.modifyFalseUser.nickname=e.nickname,t.modifyFalseUser.loginName=e.loginName,t.modifyFalseUser.header=e.header,t.modifyFalseUser.selectGames=e.selectGames,t.modifyFalseUser.show=!0},handleDeleteUser:function(e){var t=this;t.modifyFalseUser.id=e.userId,t.showDelete=!0},handleUploadImg:function(){var e=this;e.$refs["uploadImg"].onchange=function(t){e.uploadImg=t.target.files[0];var a=window.URL||window.webkitURL;e.showUploadImg=a.createObjectURL(e.uploadImg)},e.$refs["uploadImg"].click()},handleAddFalseUser:function(){var e=this;e.addFalseUser.nickname?e.isLoading||(e.isLoading=!0,e.$axios({url:"/api/Setup/AddShamUser",method:"post",params:{userName:e.addFalseUser.nickname,derail:!1}}).then(function(t){100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.isLoading=!1,e.addFalseUser.show=!1,e.getData()})):e.$Message.error("请输入用户昵称")},handleCloseModal:function(e){var t=this;t[e].show=!1},handleModifyFalseUser:function(){var e=this,t={},a={};if(e.modifyFalseUser.nickname){t["nickName"]=e.modifyFalseUser.nickname;var i=e.modifyFalseUser.selectGames.find(function(e){return e.Check&&!e.BehaviorID});i?e.$Message.error("启用游戏后，必须选择下注行为"):(i=e.modifyFalseUser.selectGames.filter(function(e){return e.Check}),i&&i.length>2?e.$Message.error("最大只能选择两个不同的彩种"):(t["userID"]=e.modifyFalseUser.userId,a["gameCheckInfo"]=[],e.modifyFalseUser.selectGames.forEach(function(e){a["gameCheckInfo"].push({GameType:e.GameType,Check:e.Check,BehaviorID:e.BehaviorID})}),e.isLoading||(e.isLoading=!0,e.$axios({url:"/api/Setup/PutShamUserInfo",method:"put",params:t,data:a}).then(function(t){100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.isLoading=!1,e.modifyFalseUser.show=!1,e.getData()}))))}else e.$Message.error("请填写用户昵称")},getData:function(){var e=this,t={};t["start"]=e.page.pageNum,t["pageSize"]=e.page.pageSize,e.isLoading||(e.isLoading=!0,e.showLoading=!0,e.$axios({url:"/api/Setup/GetShamUserListInfo",params:t}).then(function(t){if(100===t.data.Status){var a=t.data.Data;e.page.total=t.data.Total,e.falseUserData=a.map(function(e){return{id:e.ID,showId:e.OnlyCode,balance:e.UserMoney,nickname:e.NickName,loginName:e.LoginName,header:e.Avatar?-1===e.Avatar.indexOf("://")?"/"+e.Avatar:e.Avatar:"",selectGames:e.GameBetInfo,userId:e.UserID,openGame:e.OpenGame,open:e.Check}})}e.isLoading=!1,e.showLoading=!1}))},handleEnterDelete:function(){var e=this;e.isLoading||(e.isLoading=!0,e.$axios({url:"/api/User/DeleleUser",method:"delete",params:{userID:e.modifyFalseUser.id}}).then(function(t){100===t.data.Status?e.$Message.success(t.data.Message):e.$Message.error(t.data.Message),e.isLoading=!1,e.showDelete=!1,e.page.total%e.page.pageSize===1&&(e.page.pageNum=parseInt(e.page.total/e.page.pageSize)),e.getData()}))}}},p=v,f=(a("fe9b"),Object(r["a"])(p,i,s,!1,null,"731ec342",null));t["default"]=f.exports},b745:function(e,t,a){"use strict";var i=a("b2f5"),s=a("648a"),n=a("db4b"),o=a("b6f1"),c=[].sort,l=[1,2,3];i(i.P+i.F*(o(function(){l.sort(void 0)})||!o(function(){l.sort(null)})||!a("119c")(c)),"Array",{sort:function(e){return void 0===e?c.call(n(this)):c.call(n(this),s(e))}})},cf9e:function(e,t,a){},e7c0:function(e,t,a){},fe9b:function(e,t,a){"use strict";var i=a("cf9e"),s=a.n(i);s.a}}]);
//# sourceMappingURL=chunk-a51b0962.1588152423169.js.map
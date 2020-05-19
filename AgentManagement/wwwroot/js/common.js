// JavaScript Document
var str= $(".nav-wrap").html();
function setMenu(){
	if (!str) {
		return;
	}
	let strList = str.split('</li>'),
		objStr = '';
	var map_width = $(window).width();
	if(map_width < 768) {
		$('nav#menu-nav').mmenu({
			offCanvas: {
				position: "right"
			},
			extensions: ['effect-slide-menu', 'pageshadow', 'theme-dark', "pagedim-black"],
			counters: false,

			slidingSubmenus: true,

			navbar: {
				title: '平台导航'
			},
			navbars: [{

				position: 'top',
				content: [
					'prev',
					'title',
					'close'
				]
			}]
		});
		$('nav#menu-nav').css('opacity', '1');//小屏菜单闪现处理
		//默认强制关闭
		var api = $('nav#menu-nav').data('mmenu');
		api.setSelected(
			$('.metismenu li')
		)
		//api.close()
	}else{
		$(".mm-menu").remove();
		for (let i = 0; i < strList.length; i++) {
			if (strList[i] && strList[i].indexOf('high-level') !== -1) {
				if (localStorage.getItem('isHighest') == '1') {
					objStr += (strList[i] + '</li>');
				}
			} else {
				objStr += (strList[i] + '</li>');
			}
		}
		window.objStr = objStr;
		$(".nav-wrap").empty().append(objStr);
		setTimeout(function() {
			if (localStorage.getItem('isHighest') == '1') {
				$('#high-level').show();
			}
		}, 300)
	}
}

setMenu();
$(window).on("resize", setMenu);

//首页充值方式选择show出内容
$('.payway').click(function(){
	var ind=$('.payway').index(this);
	$(this).attr('class','btn btn-sm btn-warning payway').siblings('.btn').attr('class','btn btn-sm btn-default payway')
	$('.payway-con').eq(ind).show().siblings('.payway-con').hide()
})


//选项卡
$(function() {
	function tabs(tabTit, on, tabCon) {
		$(tabTit).children().click(function() {
			$(this).addClass(on).siblings().removeClass(on);
			var index = $(tabTit).children().index(this);
			$(tabCon).children().eq(index).addClass(on).siblings().removeClass(on);
		});
	};

	tabs(".tab-hd", "cur", ".tab-bd");
})

//内部小选项卡
$(function() {
	function tabs(tabTit, on, tabCon) {
		$(tabTit).children().click(function() {
			$(this).addClass(on).siblings().removeClass(on);
			var index = $(tabTit).children().index(this);
			$(tabCon).children().eq(index).addClass(on).siblings().removeClass(on);
		});
	};

	tabs(".smtabs-hd", "active", ".smtabs-bd");
})


//响应式选项卡	

$(function() {
	function tabs(tabTit, on, tabCon) {
		$(tabTit).children().click(function() {
			let that = this,
				_tabTit = tabTit,
				_tabCon = tabCon;
			if ($(this).attr("class").indexOf('verify') > 0) {
				layer.prompt({title: '请输入超级管理密码', formType: 1,skin:'myprompt'},
					function(value, index) {
						let ind = index;
						axios({
							url: '/api/AdvancedSetup/SuperValidation',
							params: {
								pwd: value
							}
						}).then(function(data) {
							layer.msg(data.data.Message, {
								time: 1500
							});
							if (data.data.Status === 100) {
								layer.close(ind);
								$('#advance-bottom').show();
								$(that).addClass(on).siblings().removeClass(on);
								var index = $(_tabTit).children().index(that);
								$(_tabCon).children().eq(index).addClass(on).siblings().removeClass(on);
							} else if (data.data.Status === 301) {
								window.location.href = window.location.origin + '/login.html';
							}
						})
					}
				);
			} else if ($(this).attr("class").indexOf('online') > 0) {
				$('#advance-bottom').hide();
				$(that).addClass(on).siblings().removeClass(on);
				var index = $(_tabTit).children().index(that);
				$(_tabCon).children().eq(index).addClass(on).siblings().removeClass(on);
			} else {
				$('#advance-bottom').show();
				$(that).addClass(on).siblings().removeClass(on);
				var index = $(_tabTit).children().index(that);
				$(_tabCon).children().eq(index).addClass(on).siblings().removeClass(on);
			}
		});
	};

	tabs(".tabs-response .tabs-head", "cur", ".tabs-response .tabs-body");
})

//元素非空
var responsetabs=$('.tabs-menulist');
if (responsetabs.length){
	var swiper = new Swiper('.tabs-menulist', {
		freeMode: true,
		slidesPerView: 'auto',
		freeModeSticky: true,
		loopedSlides: 13,
		prevButton: '.swiper-button-prev',
		nextButton: '.swiper-button-next',
		slidesPerGroup : 1,
		resistanceRatio: 0, //禁止边缘回弹
	});
}
$(function(){
	showbtn();
})

$(window).resize(function() {
	showbtn();
});

//显示和隐藏选项卡两侧按钮
function showbtn(){
	var allwidth=$('.tabs-menu').width();

	var sumWidth =0;
	$(".swiper-wrapper").children('.swiper-slide').each(function(){
	sumWidth += $(this).outerWidth();
	});

	if(sumWidth>allwidth){
		$('.tabs-menulist').addClass('showbtn');
	}else{
		$('.tabs-menulist').removeClass('showbtn');
	};
	if (responsetabs.length){
		swiper.update();
	}
}

//点击银行卡或者支付宝弹出信息
$('.m-click').click(function(){
	var map_width = $(window).width();
	if(map_width < 768) {
		$(this).next('.pover-con').show();
		$(this).next('.pover-con').css({'z-index':'101','position':'fixed','top':'40%'})
		$(this).siblings('.shadowbg').show();
	}
})
//小屏下关闭遮罩
$('.shadowbg').click(function(){
	$(this).hide()
})


//点击空白关闭pover信息
$(document).click(function(event) {
	var _con = $('.m-click'); // 设置目标区域
	if(!_con.is(event.target) && _con.has(event.target).length === 0) {
		_con.next().hide();
		$('.shadowbg').hide()
	}
});


//按钮组点击选中状态(蓝色)
$('.btns-group-blue .btn').click(function(){
	$(this).attr('class','btn btn-sm btn-primary').siblings().attr('class','btn btn-sm btn-default')
})

//按钮组点击选中状态(绿色)
$('.btns-group-green .btn').click(function(){
	$(this).attr('class','btn btn-sm btn-success').siblings().attr('class','btn btn-sm btn-default')
})

//按钮组点击选中状态(橙色)
$('.btns-group-orange .btn').click(function(){
	$(this).attr('class','btn btn-sm btn-warning').siblings().attr('class','btn btn-sm btn-default')
})



//layer询问层显示背景遮罩
$(document).on("click",".showshadow",function(){
	$(".myconfim").prev().css("display","block")
	$(".myprompt").prev().css("display","block")
})


//手机下选择日期插件屏蔽软键盘
function setDate(){
	var map_width = $(window).width();
	if(map_width < 768) {
		$(".date-normal").focus(function() {
			document.activeElement.blur();
		});
	}
}
setDate();
$(window).on("resize", setDate);
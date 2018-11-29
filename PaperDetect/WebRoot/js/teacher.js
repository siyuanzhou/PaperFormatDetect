/*目录*/
/*
1.window.load
2.jq
3.function

*/

var htdata;
var htdata1;
//载入页面后执行，一个页面只能有一个window.onload,但是可以有多个$(document).ready();
window.onload = update;


$(document).ready(function() {
 


    //$('.date-picker-control').css({'float':'left','clear':'right',});
//全选事件
$('#check').click(function(){
      $('#eventLog input[name="checkbox"]').prop('checked',this.checked);
})
$('#check1').click(function(){
      $('#eventLog1 input[name="checkbox"]').prop('checked',this.checked);
})




//输入框离开焦点后执行
$('#search_01').blur(function(){
	$('#search_01').popover('hide');
});
$('#search_02').blur(function(){
	$('#search_02').popover('hide');
});


//删除选项
$('#delete').click(function(){
	var datas  = '';
	if($('#eventLog input[name="checkbox"]:checked').size()==0)
	{
		alertWarning('请至少选择一个选项！');
	}
	else
	{
		
		$('#eventLog input[name="checkbox"]:checked').each(function(){
      datas += $(this).attr('id').substring(5)+' ';
		});
		$.ajax({
			type:'get',
			url:'allHistoryDelete.action',
			data:{pids:datas},
			success:function(){
				preprocess();
				page();
				//postprocess();
        alertInfo('论文删除成功！');
        $('#check').attr('checked',false);
			},
			error:function(){
				alertWarning('AjaxError of delete');
			},
		});
	}
});

$('#delete1').click(function(){
	var datas  = '';
	if($('#eventLog1 input[name="checkbox"]:checked').size()==0)
	{
		alertWarning('请至少选择一个选项！');
	}
	else
	{
		$('#eventLog1 input[name="checkbox"]:checked').each(function(){
      datas += $(this).attr('id').substring(5)+' ';
    });
		$.ajax({
			type:'get',
			url:'adviceshowDelete.action',
			data:{pids:datas},
			success:function(){
				preprocess1();
				page1();
				//postprocess1();
        alertInfo('反馈意见删除成功！');
        $('#check1').attr('checked',false);
			},
			error:function(){
				alertWarning('AjaxError of delete1');
			},
		});
	}
});



//修改密码的输入检测
  var flag = 0;
  $('#pwd1').bind('change', function() {
    if ($('#pwd1').val().length < 6 || $('#pwd1').val().length > 20 || flag == 1) {
      $('#pwd1').popover('show');
    } else {
      $('#pwd1').popover('hide');
      $('#pwd1').css('border-color', 'rgb(200,200,200)');
    }
    $.ajax({
      type: 'post',
      url: 'pass.action',
      data: {
        password: $('#pwd1').val()
      },
      success: function(msg) {
        if (msg == 1) {
          $('#pwd1').popover('show');
          flag = 1;
        } else {
          $('#pwd1').popover('hide');
          $('#pwd1').css('border-color', 'rgb(200,200,200)');
          flag = 0;
        }
      },
      error: function() {
        alert('AjaxError1');
      },
    });

    if ($('#pwd2').val() != $('#pwd1').val() && $('#pwd2').hasClass('clicked')) {
      $('#pwd2').popover('show');
    } else {
      $('#pwd2').popover('hide');
      $('#pwd2').css('border-color', 'rgb(200,200,200)');
    }

  });




  $('#pwd2').bind('change', function() {
    if ($('#pwd2').val() != $('#pwd1').val()) {
      $('#pwd2').popover('show');
    } else {
      $('#pwd2').popover('hide');
      $('#pwd2').css('border-color', 'rgb(200,200,200)');
    }
  });




  $('#queren').click(function() {
    if ($('#pwd1').val().length < 6 || $('#pwd1').val().length > 20 || flag == 1) {
      $('#pwd1').css('border-color', 'red');
      $('#pwd2').css('border-color', 'red');
    }
    if ($('#pwd2').val() != $('#pwd1').val()) {
      $('#pwd2').css('border-color', 'red');
    }
    if (flag == 1) {
      $('#pwd1').css('border-color', 'red');
      $('#pwd2').css('border-color', 'red');
    }

    $.ajax({
      type: 'post',
      url: 'update.action',
      data: {
        password: $('#old_id').val(),
        newpassword: $('#pwd1').val()
      },
      success: function(msg) {
        if (msg == 0) {
          $('#old_id').popover('show');
        } else {
          alert('修改成功！');
          $('#old_id').popover('hide');
          window.location.reload();
        }
      },
      error: function() {
        alert('AjaxError2');
      },
    });
  });








//与分页有关的jq

$("#page").on("pageClicked", function(event, data) {
  $('#check').attr('checked',false);
  $("#eventLog").empty();
  var start = data.pageIndex * data.pageSize;
  var end = (data.pageIndex + 1) * data.pageSize < htdata.historyList.length ? (data.pageIndex + 1) * data.pageSize : htdata.historyList.length;
  updatePaper(start, end);
}).on('jumpClicked', function(event, data) {
  $('#check').attr('checked',false);
  $("#eventLog").empty();
  var start = data.pageIndex * data.pageSize;
  var end = (data.pageIndex + 1) * (data.pageSize + 1) < htdata.historyList.length ? (data.pageIndex + 1) * data.pageSize : htdata.historyList.length;
  updatePaper(start, end);
}).on('pageSizeChanged', function(event, data) {
  $('#check').attr('checked',false);
  $("#eventLog").empty();
  var start = data.pageIndex * data.pageSize;
  var end = (data.pageIndex + 1) * data.pageSize < htdata.historyList.length ? (data.pageIndex + 1) * data.pageSize : htdata.historyList.length;
  updatePaper(start, end);
});




$("#page1").on("pageClicked", function(event, data) { //分页按钮事件
  $('#check1').attr('checked',false);
  $("#eventLog1").empty();
  var start = data.pageIndex * data.pageSize;
  var end = (data.pageIndex + 1) * data.pageSize < htdata1.adviceList.length ? (data.pageIndex + 1) * data.pageSize : htdata1.adviceList.length;
  updateAdvice(start, end)
}).on('jumpClicked', function(event, data) { //跳转按钮点击事件
  $('#check1').attr('checked',false);
  $("#eventLog1").empty();
  var start = data.pageIndex * data.pageSize;
  var end = (data.pageIndex + 1) * (data.pageSize + 1) < htdata1.adviceList.length ? (data.pageIndex + 1) * data.pageSize : htdata1.adviceList.length;
  updateAdvice(start, end)
}).on('pageSizeChanged', function(event, data) { //页面大小切换事件
  $('#check1').attr('checked',false);
  $("#eventLog1").empty();
  var start = data.pageIndex * data.pageSize;
  var end = (data.pageIndex + 1) * data.pageSize < htdata1.adviceList.length ? (data.pageIndex + 1) * data.pageSize : htdata1.adviceList.length;
  updateAdvice(start, end)
});


});



//键盘事件
 $(document).keypress(function(e) {  
    // 回车键事件  
       if(e.which == 13&&$('#search_01').is(':focus')) {  
        search_01($('#select_01').val(),encodeURI($('#search_01').val()));
         //jQuery(".confirmButton").click();
        }
        if(e.which == 13&&$('#search_02').is(':focus')) {  
        search_02($('#select_02').val(),encodeURI($('#search_02').val()));
         //jQuery(".confirmButton").click();
        }
   }); 




/*function*/

//消息提示
function alertInfo(msg) {
  BootstrapDialog.show({
    type: BootstrapDialog.TYPE_INFO,
    title: "消息提示",
    message: msg,
  });
}
function alertWarning(msg) {
  BootstrapDialog.show({
    type: BootstrapDialog.TYPE_DANGER,
    title: "消息提示",
    message: msg,
  });
}




//detect the number of paper at times
function getNumber(){
  $.ajaxSetup({ cache: false }); 
  $.ajax({
    type:'get',
    url:'sum.action',
    success:function(msg){
      $('#number').html(msg);
    },
  });
}

//detect the number of paper at first
function getNumber1(){
  $.ajaxSetup({ cache: false }); 
  $.ajax({
    type:'get',
    url:'sum.action',
    success:function(msg){
      var new_content = $('<span style="float:right;">现在已检测<span id="number" style="color:red;">'+msg+'</span>篇论文</span>').appendTo($('#subheader'));
    },
  });
}

//注销登录
function loginout(){
    $.ajax({
      type:'get',
      url:'logout.action',
      success:function(){
        window.location.href='login.jsp';
      },
    });
  }



//pagination of the paper page
function page(){
  ///djahgjkhg 

  $("#page").pagination({
    pageIndex: 0,
    pageSize: 10,
    total: 100,
    debug: true,
    showInfo: true,
    showJump: true,
    showPageSizes: true,
    loadFirstPage: true,
    firstBtnText: '首页',
    lastBtnText: '尾页',
    prevBtnText: '上一页',
    nextBtnText: '下一页',
    totalName: 'total',
    jumpBtnText: '跳转',
    infoFormat: '{start} ~ {end}条，共{total}条',
    remote: {
      url:'allHistory.action' ,
      params: null,
      success: function(data) {
        htdata = data;
        var pageindex = $("#page").pagination('getPageIndex');
        var pagesize = $("#page").pagination('getPageSize');
        $("#eventLog").empty();
        var start = pageindex * pagesize;
        var end = (pageindex + 1) * pagesize < htdata.historyList.length ? (pageindex + 1) * pagesize : htdata.historyList.length;
        updatePaper(start, end);
      }
    }
  });
}



//pagination of the advice page
function page1(){
  
  $("#page1").pagination({
    pageIndex: 0, //指定当前页数
    pageSize: 10, //每页显示数据数
    total: 100, //总数据数，生成分页则必须配置该属性以生成分页，
    debug: true,
    showInfo: true, //是否显示分页信息
    showJump: true, //是否显示跳转页
    showPageSizes: true, //是显示选择每页的数量
    loadFirstPage: true, //是否加载第一页，页面第一次打开可设置不加载首页数据，首页数据是页面直接返回，之后点击分页按钮自动加载数据
    firstBtnText: '首页', //首页显示文字
    lastBtnText: '尾页', //尾页显示文字
    prevBtnText: '上一页', //上页显示文字
    nextBtnText: '下一页', //下页显示文字
    totalName: 'total',
    jumpBtnText: '跳转', //跳转显示文字
    infoFormat: '{start} ~ {end}条，共{total}条', //显示分页信息
    remote: {
      url: 'adviceshow.action',
      params: null,
      success: function(data) {
        htdata1 = data;
        var pageindex = $("#page1").pagination('getPageIndex'); //获取当前的pageIndex
        var pagesize = $("#page1").pagination('getPageSize'); //获取当前的pageSize
        $("#eventLog1").empty();
        var start = pageindex * pagesize;
        var end = (pageindex + 1) * pagesize < htdata1.adviceList.length ? (pageindex + 1) * pagesize : htdata1.adviceList.length;
        updateAdvice(start, end)
      }
    }
  });
}

//deal with event of pagination after deleting
function preprocess(){
  $("#page").pagination('destroy');
  //$("#page").empty();
  //$('#eventLog').empty();
}
function preprocess1(){
  $("#page1").pagination('destroy');
  //$("#page1").empty();
  //$('#eventLog1').empty();
}
function postprocess(){
  if($("#eventLog:has(div)").length==0)
    var new_tbody=$('<div>抱歉，没有您要查找的信息！</div>').appendTo($('#eventLog'));
}
function postprocess1(){
  if($("#eventLog1:has(div)").length==0)
    var new_tbody=$('<div>抱歉，没有您要查找的信息！</div>').appendTo($('#eventLog1'));
}





//function of searching
function search_01(a,b){
  $("#page").pagination('destroy');
  $("#page").empty();
  $('#eventLog').empty();
  $("#page").pagination({
    pageIndex: 0,
    pageSize: 10,
    total: 10,
    debug: true,
    showInfo: true,
    showJump: true,
    showPageSizes: true,
    loadFirstPage: true,
    firstBtnText: '首页',
    lastBtnText: '尾页',
    prevBtnText: '上一页',
    nextBtnText: '下一页',
    totalName: 'total',
    jumpBtnText: '跳转',
    infoFormat: '{start} ~ {end}条，共{total}条',
    remote: {
      url:'allHistoryFind.action',
      params: {findType: a,findValue: b,},
      success: function(data) {
        htdata = data;
        var pageindex = $("#page").pagination('getPageIndex');
        var pagesize = $("#page").pagination('getPageSize');
        $("#eventLog").empty();
        var start = pageindex * pagesize;
        var end = (pageindex + 1) * pagesize < htdata.historyList.length ? (pageindex + 1) * pagesize : htdata.historyList.length;
        updatePaper(start, end);
      }
    }
  });
  if($("#eventLog:has(div)").length==0)
    var new_tbody=$('<div>抱歉，没有您要查找的信息！</div>').appendTo($('#eventLog'));
}



function search_02(a,b){
  $("#page1").pagination('destroy');
  $("#page1").empty();
  $('#eventLog1').empty();
  $("#page1").pagination({
    pageIndex: 0, //指定当前页数
    pageSize: 10, //每页显示数据数
    total: 10, //总数据数，生成分页则必须配置该属性以生成分页，
    debug: true,
    showInfo: true, //是否显示分页信息
    showJump: true, //是否显示跳转页
    showPageSizes: true, //是显示选择每页的数量
    loadFirstPage: true, //是否加载第一页，页面第一次打开可设置不加载首页数据，首页数据是页面直接返回，之后点击分页按钮自动加载数据
    firstBtnText: '首页', //首页显示文字
    lastBtnText: '尾页', //尾页显示文字
    prevBtnText: '上一页', //上页显示文字
    nextBtnText: '下一页', //下页显示文字
    totalName: 'total',
    jumpBtnText: '跳转', //跳转显示文字
    infoFormat: '{start} ~ {end}条，共{total}条', //显示分页信息
    remote: {
      url: 'adviceshowFind.action',
      params: {findType: a,findValue: b,},
      success: function(data) {
        htdata1 = data;
        var pageindex = $("#page1").pagination('getPageIndex'); //获取当前的pageIndex
        var pagesize = $("#page1").pagination('getPageSize'); //获取当前的pageSize
        var start = pageindex * pagesize;
        var end = (pageindex + 1) * pagesize < htdata1.adviceList.length ? (pageindex + 1) * pagesize : htdata1.adviceList.length;
        updateAdvice(start, end)
      }
    }
  });
  if($("#eventLog:has(div)").length==0)
    var new_tbody=$('<div>抱歉，没有您要查找的信息！</div>').appendTo($('#eventLog1'));
}




//the function of window.onload
function update(){
  page();
  page1();
  getNumber1();
  setInterval('getNumber()',15000);
  //时间插件
  datePickerController.createDatePicker({
    // Associate the text input to a DD/MM/YYYY date format
    formElements:{"start":"%Y-%m-%d"}
    });
    datePickerController.createDatePicker({
    // Associate the text input to a DD/MM/YYYY date format
    formElements:{"start1":"%Y-%m-%d"}
    });

}

//pagination of advice page
function updateAdvice(start, end) {
  for (var i = start; i < end; i++) {
    var new_tbody = $('<div id="tbody1' + htdata1.adviceList[i].aid + '" class="row" style="margin-bottom: 20px;border-bottom:rgb(248,248,248) 2px solid;font-weight: bold;width:100%;"><input id="check' + htdata1.adviceList[i].aid + '" type="checkbox" name="checkbox" class="col-md-2"><div class="row col-md-10"><div class="col-md-2">' + htdata1.adviceList[i].aid + '</div><div class="col-md-2">' + htdata1.adviceList[i].name + '</div><div class="col-md-2">' + htdata1.adviceList[i].sid + '</div><div class="col-md-3">' + htdata1.adviceList[i].email + '</div><div class="col-md-3"><a class="btn btn-primary" type="button" data-toggle="collapse" data-target="#collapse' + htdata1.adviceList[i].aid + '" aria-expanded="false" aria-controls="collapse' + htdata1.adviceList[i].aid + '">查看反馈意见</a></div></div></div><div class="collapse" style="margin-right:20px;" id="collapse' + htdata1.adviceList[i].aid + '"><div class="well">' + htdata1.adviceList[i].advice + '</div></div>').appendTo($('#eventLog1'));
    $("div[id^=tbody1]:odd").css('background-color', 'rgb(230,230,230)');
  }
}
// pagination of paper page
function updatePaper(start, end) {
  for (var i = start; i < end; i++) {
    var new_tbody = $('<div id="tbody0' + htdata.historyList[i].pid + '" class="row" style="margin-bottom: 20px;border-bottom:rgb(248,248,248) 2px solid;font-weight: bold;width:100%;"><input id="check' + htdata.historyList[i].pid + '" type="checkbox" name="checkbox" class="col-md-1"><div class="row col-md-11"><div class="col-md-2">' + htdata.historyList[i].name + '</div><div class="col-md-2">' + htdata.historyList[i].sid + '</div><div class="col-md-6" style="overflow:hidden;"><a href="download.action?paper_name=' + encodeURI(encodeURI(htdata.historyList[i].paper_id)) + '">' + htdata.historyList[i].paper_id + '</a></div><div class="col-md-2"><a href="download.action?paper_name=report' + encodeURI(encodeURI(htdata.historyList[i].paper_id)) + '">点击下载</a></div></div></div>').appendTo($('#eventLog'));
    $("div[id^=tbody0]:odd").css('background-color', 'rgb(230,230,230)');
  }
}

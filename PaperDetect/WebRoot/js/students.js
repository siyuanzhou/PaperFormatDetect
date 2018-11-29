var global = (function() { return this || (1,eval)('(this)'); }());
global.fileSize = 0;
window.onload = loadfunc;
function loadfunc() {
  getNumber1();
  $('#input01').filestyle({
    buttonText: '浏览'
  });
  loadinfo();
  updatehistory();
  setInterval('getNumber()',15000);
}

function loadinfo() {
  $.ajaxSetup({ cache: false }); 
  $.ajax({
    url: 'personal.action',
    success: function(data) {
      var json = eval("(" + data + ")");
      var datas = json.personal;
      if (data.stat == 0) {
        return;
      } else {
        $("#st_name").html(datas[0]);
        $("#st_id").html(datas[1]);
        $("#st_class").html(datas[2]);
        if (datas[3] == 'man') {
          $("#st_sex").html('男');
        } else {
          $("#st_sex").html('女');
        }


      }
    }
  });
}


function getNumber1(){
  $.ajaxSetup({ cache: false }); 
  $.ajax({
    type:'get',
    url:'sum.action',
    success:function(msg){
      var new_content = $('<span style="float:right;">现在已检测<span id="number" style="color:rgb(245,245,220);">'+msg+'</span>篇论文</span>').appendTo($('#subheader'));
    },
  });
}

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

//文件大小检测
function fileChange(target,fileSize) {
if (/msie/i.test(navigator.userAgent) && !window.opera && !target.files) {
var filePath = target.value;
var fileSystem = new ActiveXObject("Scripting.FileSystemObject"); 
var file = fileSystem.GetFile (filePath);
fileSize = file.Size;
} else {
fileSize = target.files[0].size;
}
return fileSize;
}





function updatehistory() {
  $.ajaxSetup({ cache: false }); 
  $.ajax({
    url: 'history.action',
    dataType: 'json',
    success: function(data) {
      var json = eval("(" + data + ")");
      var datas = json.historyList;
      if (data.stat == 0) {
        return;
      } else {
        var n = datas.length;
        var $htable = $("#historytable");
        $htable.html('');
        for (var i = 0; i < n; i++) {
          var trNew = $("<tr></tr>");
          trNew.append("<td>" + datas[i].ptime + "</td>");
          trNew.append("<td id='pname_" + datas[i].pid + "'>" + datas[i].paper_id + "</td>");
          trNew.append("<td><button type='button' class='btn btn-info' onclick='updatereport(" + datas[i].pid + ")'>查看检测报告</button></td>");
          $htable.append(trNew);
        }

      }
    }
  });
}

function updatereport(pId) {
  var upData = {};
  upData['pId'] = pId;
  $.ajaxSetup({ cache: false }); 
  $.ajax({
    url: 'report.action',
    data: upData,
    success: function(data) {
      var json = eval("(" + data + ")");
      var datas = json.report;
      if (data.stat == 0) {
        return;
      } else {
        $("#report-text").empty();
        $('#download,#back,#tucao').remove();
        var n = datas.length;
        $("#report-text").append("<h3 style='text-align:center;'>" + datas[1]+ "</h3>");
        $("#report-text").append("<h5 style='text-align:right;'>时间："+ datas[2]+ "</h5>");
        if (n >= 2) {
          for (var i = 3; i < n; i++) {
            if(datas[i]=='封面:'||datas[i]=='摘要:'||datas[i]=='正文:'||datas[i]=='页眉页脚:'||datas[i]=='图:'||datas[i]=='表:'||datas[i]=='目录:')
            {
              $("#report-text").append("<span style='color:red;'>"+datas[i]+"<span><br/>");
            }
            else
            $("#report-text").append("<span>"+datas[i]+"<span>"+"<br/>");
          }
        }
        
        $("#report").append("<a id='back' href='#header' class='btn btn-lg btn-primary btn-block' style='display:inline;margin-top:20px;width:135px;margin-left:106px;'>返回顶部</a><a id='tucao' href='advice.jsp' style='display:inline;margin-top:20px;width:135px;margin-left:250px;text-decoration:none;cursor:pointer;'>我要吐槽</a><a id='download' href='download.action?paper_name=report" + encodeURIComponent(encodeURIComponent(datas[0])) + "' class='btn btn-lg btn-primary btn-block' style='display:inline;margin-top:20px;width:135px;margin-left:258px;'>下载检测报告</a>");
        //$("#report").append("<a id='download' href='download.action?paper_name=report" + encodeURI(encodeURI(datas[0])) + "' class='btn btn-lg btn-primary btn-block' style='margin-top:20px;width:135px;margin-left:800px;'>下载检测报告</a>");
      }
      $('#ptab a:eq(2)').tab('show');
    }
  });
}




function uploadfile() {
	if(fileSize <= 5000)
	{
		alertWarning('您上传的文件为空，请重新上传！');
		return ;
	}
  $('#checkreport-btn').hide();
  $.ajaxSetup({ cache: false }); 
  updatehistory();
  setTimeout("updatehistory()", "2000");
  var docname = $("#input01").val();
  var docnames = docname.split('.');
  if (docname == "") {
    alertWarning("没有选择论文，请重新上传！");
    $('#checkreport-btn').hide();
    return;

  } /*else if (docnames[docnames.length-1] != 'docx') {
    alertWarning('论文格式不是.docx，请重新上传！');
    $('#checkreport-btn').hide();
    return;
  }*/

  //传送是否屏蔽代码选项和工程选项的信息
  var p=0;
  if($('#pingbi input').is(':checked'))
    p=1;
  $.ajax({
    type:'get',
    url:'uploadTemp.action',
    data:{temp:p,flag:$('[name="ty"]:checked').val(),},
    success:function(){

    },
  });


  //去掉屏蔽代码的提示
  $('#pingbi').addClass('collapse');
  //显示进度条   
  $('#progress,#progresser').removeClass('collapse');
  //连续发送ajax请求，直到后台消息确认之后，显示查看报告按钮，进度条消失 
    var delta = 10;
    $('#progress p').html('10%');
    $('#progress').css('width', '10%');
  var show = setInterval(function() {
    $.ajax({
      type: 'get',
      url: 'deadline.action',
      success: function(msg) {
        if (msg == 1) {

          $('#progress p').html('100% Complete!');
          $('#progress').css('width', '100%');
          //延时1s
          setTimeout(function() {
            $('#progress,#progresser').addClass('collapse');
      $('#progress p').html('10%');
      $('#progress').css('width', '10%');
          }, 1000);
          setTimeout(function() {
            $('#checkreport-btn').show();
            getNumber();
          }, 1500);

          clearInterval(show);
        }else if(msg ==2){
        	alertWarning('&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp很抱歉检测程序无法识别您上传的论文，请尝试将您的论文另存为docx格式后重新上传，如果仍然无法得到检测报告，请使用页面下方的联系方式邮件与我们联系，或者在反馈意见页面给我们留言，我们将\
  <span style="float:left;">&nbsp通过人工方式为您提供检测服务。</span>');
        	setTimeout(function() {
                $('#progress,#progresser').addClass('collapse');
          $('#progress p').html('10%');
          $('#progress').css('width', '10%');
              }, 1000);
        	  $('#pingbi').removeClass('collapse');
        	clearInterval(show);
        }else {
          $('#checkreport-btn').hide();
          if(fileSize<26214400)
          {
            if (delta < 90) {
              $('#progress p').html(delta + '%');
              $('#progress').css('width', delta + '%');
              delta = delta + 3;
            }else if (delta < 99) {
              delta = delta + 1;
              $('#progress p').html(delta + '%');
              $('#progress').css('width', delta + '%');
            }
          }
          else
          {
            if (delta < 90) {
              $('#progress p').html(delta + '%');
              $('#progress').css('width', delta + '%');
              delta = delta + 3;
            }else if (delta < 99) {
              delta = delta + 1;
              $('#progress p').html(delta + '%');
              $('#progress').css('width', delta + '%');
            }
          }


        }
      },
      error: function() {
        alertWarning('系统出错，请重新上传！');
      },
    });
  }, 1000);

  setTimeout("deletefilename()", "1500");
}


function deletefilename() {
  $(":file").filestyle('clear');
}


function checkreport() {
  $.ajaxSetup({ cache: false }); 
  $.ajax({
    url: 'report_new.action',
    success: function(data) {
      if(data == 0)
      {
        alertWarning('系统繁忙，请重新上传！');
        return ;
      }
      var json = eval("(" + data + ")");
      var datas = json.report;
      if (data.stat == 0) {
        return;
      } else {
        $("#report-text").empty();
        $('#download,#back,#tucao').remove();
        var n = datas.length;
        $("#report-text").append("<h3 style='text-align:center;'>" + datas[1] + "</h3>");
        $("#report-text").append("<h5 style='text-align:right;'>时间："+ datas[2] + "</h5>");
        if (n >= 2) {
          for (var i = 3; i < n; i++) {
            if(datas[i]=='封面:'||datas[i]=='摘要:'||datas[i]=='正文:'||datas[i]=='页眉页脚:'||datas[i]=='图:'||datas[i]=='表:'||datas[i]=='目录:')
            {
              $("#report-text").append("<span style='color:red;'>"+datas[i]+"<span><br/>");
            }
            else
            $("#report-text").append("<span>"+datas[i]+"<span>"+"<br/>");
          }
        }
        $("#report").append("<a id='back' href='#header' class='btn btn-lg btn-primary btn-block' style='display:inline;margin-top:20px;width:135px;margin-left:106px;'>返回顶部</a><a id='tucao' href='advice.jsp' style='display:inline;margin-top:20px;width:135px;margin-left:250px;text-decoration:none;cursor:pointer;'>我要吐槽</a><a id='download' href='download.action?paper_name=report" + encodeURI(encodeURI(datas[0])) + "' class='btn btn-lg btn-primary btn-block' style='display:inline;margin-top:20px;width:135px;margin-left:258px;'>下载检测报告</a>");
      }
      $('#ptab a:eq(2)').tab('show');
    }
  });
}
//<a id='tucao' href='advice.jsp' style='display:inline;margin-top:8px;margin-left:343px;text-decoration:none;cursor:pointer;'>我要吐槽</a>



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




$(document).ready(function() {
  $.ajaxSetup({ cache: false }); 
  var flag = 0;
  $('#pwd1').change(function() {
    if ($('#pwd1').val().length < 6 || $('#pwd1').val().length > 20 || flag == 1) {
      $('#check1').html("请输入5位以上21位以下密码，且只能是密码或数字~").css('color', 'red').css('font-size', '13px');
    } else {
      $('#check1').html('');
      $('#pwd2').css('border-color', 'rgb(200,200,200)');
    }

  });



  $('#pwd2').change(function() {
    if ($('#pwd2').val().length < 6 || $('#pwd2').val().length > 20) {
      $('#check2').html("请输入5位以上21位以下密码，且只能是密码或数字~").css('color', 'red').css('font-size', '13px');
    } else {
      $('#check2').html('');
      $('#pwd2').css('border-color', 'rgb(200,200,200)');


      $.ajax({
        type: 'post',
        url: 'pass.action',
        data: {
          new_pwd: $('#pwd2').val()
        },
        success: function(msg) {
          if (msg == 1) {
            $('#check2').html("请输入5位以上21位以下密码，且只能是密码或数字~").css('color', 'red').css('font-size', '13px');
            flag = 1;
          } else {
            $('#check2').html('');
            $('#pwd2').css('border-color', 'rgb(200,200,200)');
            flag = 0;
          }
        },
      });
    }

    if ($('#pwd3').val() != $('#pwd2').val()) {
      $('#check3').html('您两次输入的密码不一致，请重新输入！').css('color', 'red').css('font-size', '13px');
    } else {
      $('#check3').html('');
      $('#pwd3').css('border-color', 'rgb(200,200,200)');
    }
  });


  $('#pwd3').change(function() {
    if ($('#pwd3').val() != $('#pwd2').val()) {
      $('#check3').html('您两次输入的密码不一致，请重新输入！').css('color', 'red').css('font-size', '13px');
    } else {
      $('#check3').html('');
      $('#pwd3').css('border-color', 'rgb(200,200,200)');
    }
  });
  $('#queren').click(function() {
    if ($('#pwd2').val().length < 6 || $('#pwd2').val().length > 20 || flag == 1) {
      $('#check2').html("请输入5位以上21位以下密码，且只能是密码或数字~").css('color', 'red').css('font-size', '13px');
      $('#pwd2,#pwd3').css('border-color', 'red');
      $('#pwd2,#pwd3').val('');
    }
    if ($('#pwd3').val() != $('#pwd2').val()) {
      $('#check3').html('您两次输入的密码不一致，请重新输入！').css('color', 'red').css('font-size', '13px');
      $('#pwd3').css('border-color', 'red');
      $('#pwd3').val('');
    }


    if (flag == 1) {
      $('#pwd2').css('border-color', 'red');
      $('#pwd3').css('border-color', 'red');
      $('#pwd2,#pwd3').val('');
    }


    if ($('#pwd2').val().length < 6 || $('#pwd2').val().length > 20 || $('#pwd3').val() != $('#pwd2').val() || flag == 1) {
      alertWarning('您输入的新密码有问题，请重新输入！');
    } else {
      old = $('#pwd1').val();
      newa = $('#pwd2').val();
      $.ajax({
        data: {
          password: old,
          newpassword: newa
        },
        url: 'update.action',
        success: function(msg) {
          if (msg == 1) {
            alertInfo("修改成功");
            $('#pwd1,#pwd2,#pwd3').val('');
          } else {
            alertWarning("您的初始密码有问题！");
            $('#pwd1').css('border-color', 'red');
            $('#pwd1').val('');
          }
        },
      });
    }

  });
});

function alertWarning(msg) {
  BootstrapDialog.show({
    type: BootstrapDialog.TYPE_DANGER,
    title: "消息提示",
    message: msg,
  });
}

function alertInfo(msg) {
  BootstrapDialog.show({
    type: BootstrapDialog.TYPE_INFO,
    title: "消息提示",
    message: msg,
  });
}

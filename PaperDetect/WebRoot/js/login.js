//自动登录问题
var cookieName_username = "LOGIN_USER_NAME_TEST";
var cookieName_password = "LOGIN_PASSWORD_TEST";
var cookieName_autologin = "LOGIN_AUTO_TEST";

//得到Cookie信息
function getUserInfoByCookie() {
  var uname = getCookie(cookieName_username);//获取学号
  if (uname != null && !uname.toString().isnullorempty()) {
    GetObj('user').value = uname;
  }

  var upass = getCookie(cookieName_password);//获取密码
  if (upass != null && !upass.toString().isnullorempty()) {
    GetObj('pwd2').type = 'password';
    GetObj('pwd2').value = upass;
  }
  // var autologin = getCookie(cookieName_autologin);
  if (!uname.toString().isnullorempty()&&!upass.toString().isnullorempty())
    GetObj('ck_rmbUser').checked = true;
}

$(document).ready(function() {
  $('#forget').click(function() {
    if ($('#user').val() == '21512345') {
      alertWarning('您不能进行密码验证，请联系管理员重置密码！');
    } else {
      window.location.href = 'security_01.jsp';
    }
  });
  $('#user').bind('change', function() {
    //alertWarning((/[2-4](08|09|10|11|12|13|14|15)\d{5}/).test($('#user').val()));
    if($("#user").val().length == 0){

      $("#user").attr("data-content","这里不可以空着哦~");
      $('#user').popover('show');
    } else if(!(/[2-4](08|09|10|11|12|13|14|15)\d{5}/).test($('#user').val())) {
      $('#user').attr('data-content',"您的学号格式不对哦~");
      $('#user').popover('show');
    } else {

      $('#user').popover('hide');
      $('#user').css('border-color', 'rgb(200,200,200)');
    }
  });

  //点击后，如果不是checked，就需要取消cookie
  $("#ck_rmbUser").bind("click", function () {
    if(!$('#ck_rmbUser').checked){
      delCookie(cookieName_username);
      delCookie(cookieName_password);
      delCookie(cookieName_autologin);
    }

  });
  $('#pwd2').bind('change', function() {
    if ($('#pwd2').val().length == 0) {
      $("#pwd2").attr("data-content","这里不可以空着哦~");
      $('#pwd2').popover('show');
    }else if(!(/[0-9a-zA-Z]{5,20}/).test($('#pwd2').val())){
      $('#pwd2').attr('data-content',"您的密码位数或符号不对哦~");
      $('#pwd2').popover('show');
    } else {
      $('#pwd2').popover('hide');
      $('#pwd2').css('border-color', 'rgb(200,200,200)');
    }
  });
  
  $('#denglu').click(function() {

    if(($('#user').val().length == 0)||($('#pwd2').val().length == 0)){
      if ($('#user').val().length == 0) {
        $('#user').css('border-color', 'red');
      }
      if ($('#pwd2').val().length == 0) {
        $('#pwd2').css('border-color', 'red');
      }
      alertWarning("内容不可以为空！");
      return;
    }else if(!(/[2-4](08|09|10|11|12|13|14|15)\d{5}/).test($('#user').val())||!(/[0-9a-zA-Z]{5,20}/).test($('#pwd2').val())){
      if(!(/[2-4](08|09|10|11|12|13|14|15)\d{5}/).test($('#user').val())){
        $('#user').css('border-color', 'red');
      }
      if (!(/[0-9a-zA-Z]{5,20}/).test($('#pwd2').val())) {
        $('#pwd2').css('border-color', 'red');
      }
      alertWarning("您的输入有问题！");
      return;
    }

      //取消以前的信息
      delCookie(cookieName_username);
      delCookie(cookieName_password);
      var autoSave = GetObj('ck_rmbUser');
    //保存在新的cookie中
    if (autoSave.checked) {
      SetCookie(cookieName_username, $("#user").val(), 7);//保存到cookie中7天
      //alert($('#user').val());
      SetCookie(cookieName_password, $("#pwd2").val(), 7);
      //alert($('#pwd2').val());
    }

    $.ajax({
      type: 'post',
      url: 'login.action',
          //asyc:false,
          data: {
            username: $('#user').val(),
            password: $('#pwd2').val()
          },
          success: function(msg) {
            if (msg == 0) {
              alertWarning("用户名错误");
              $('#user,#pwd2').val('');
            }
            if (msg == 2) {
              alertWarning("密码错误");
              $('#pwd2').val('');
            }
            if (msg == 1) {
              location.href = 'students.jsp';
            }
            if (msg == -1) {
              location.href = 'teacher.jsp';
            }
          },
        });
  });

  });
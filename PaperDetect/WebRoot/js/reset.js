var userflag = false;
var pw1flag = false;
var pw2flag = false;
var codeflag = false;
window.onload = delcache();

function delcache() {
  $('#user').val() != '';
  $('#code').val() != '';
}
$(document).ready(function() {

  $("#pwd2").focus(function() {
    // 点击过
    if ($(this).hasClass("clicked")) {
      // 处理...
    }
    // 没有点击过
    else {
      // 添加 class
      $(this).addClass("clicked");
      // 处理...
    }
  });

  $('#user').bind('change', function() {
    if ($('#user').val() != '') {
      $('#user').css('border-color', 'rgb(200,200,200)');
      $.ajax({
        type: 'post',
        url: 'isExist.action',
        data: {
          username: $('#user').val()
        },
        success: function(msg) {
          if (msg == 0) {
            $('#user').popover('show');
            userflag = false;
          } else {
            $('#user').popover('hide');
            userflag = true;
          }
        }
      });
    }
  });

  var reg = /^[A-Za-z0-9]+$/;
  $('#code').bind('change', function() {
    if ($('#code').val().length != 4 || !reg.test($('#code').val())) {
      $('#code').popover('show');
      codeflag = false;
    } else {
      $('#code').popover('hide');
      $('#code').css('border-color', 'rgb(200,200,200)');
      codeflag = true;
    }
  });


  $('#pwd1').bind('change', function() {
    if ($('#pwd1').val().length < 6 || $('#pwd1').val().length > 20) {
      $('#pwd1').popover('show');
      pw1flag = false;
    } else {
      $('#pwd1').popover('hide');
      $('#pwd1').css('border-color', 'rgb(200,200,200)');

      $.ajax({
        type: 'post',
        url: 'pass.action',
        data: {
          password: $('#pwd1').val()
        },
        success: function(msg) {
          if (msg == 1) {
            $('#pwd1').popover('show');
            pw1flag = false;
          } else if ($('#pwd1').val().length >= 6 && $('#pwd1').val().length <= 20) {
            $('#pwd1').popover('hide');
            $('#pwd1').css('border-color', 'rgb(200,200,200)');
            pw1flag = true;
          }
        },
      });
    }
    if ($('#pwd2').val() != $('#pwd1').val() && $('#pwd2').hasClass('clicked')) {
      $('#pwd2').popover('show');
      pw2flag = false;
    } else {
      $('#pwd2').popover('hide');
      $('#pwd2').css('border-color', 'rgb(200,200,200)');
      pw2flag = true;
    }
  });

  $('#pwd2').bind('change', function() {
    if ($('#pwd2').val() != $('#pwd1').val()) {
      $('#pwd2').popover('show');
      pw2flag = false;
    } else {
      $('#pwd2').popover('hide');
      $('#pwd2').css('border-color', 'rgb(200,200,200)');
      pw2flag = true;
    }
  });


  $('#reset').click(function() {
    if (!codeflag) {
      $('#code').css('border-color', 'red');
      $('#code').popover('show');
    }

    if ($('#user').val() == '21512345') {
      alertWarning("用户已存在！请重新注册");
      $('#user,#code,#pwd1,#pwd2').val('');
      return;
    }

    if (!userflag) {
      $('#user').css('border-color', 'red');
      $('#user').popover('show');
    }
    if (!pw1flag) {
      $('#pwd1').css('border-color', 'red');
      $('#pwd2').css('border-color', 'red');
      $('#pwd1').popover('show');
      $('#pwd2').popover('show');
    }
    if (!pw2flag) {
      $('#pwd2').css('border-color', 'red');
    }
    if (!userflag || !pw1flag || !pw2flag || !codeflag) {
      alertWarning('您输入的信息有误！');
      return;
    }
    username_value = $('#user').val();
    //转码有问题
    password_value = $('#pwd2').val();
    $.ajax({
      type: 'post',
      url: 'resetPassword.action',
      data: {
        username: username_value,
        identity: $('#code').val(),
        password: password_value
      },
      success: function(msg) {
        if (msg == 0) {
          alertWarning("验证码错误！");
          $('#code,#user,#pwd1,#pwd2').val('');
          setTimeout("window.location.reload()","3000");
        } else {
          alertInfo("重置密码成功");
          setTimeout("loadlogin()","3000");
        }
      },
    });
  });
});

function loadlogin(){
  window.location.href = '/PaperDetect/login.jsp';
}

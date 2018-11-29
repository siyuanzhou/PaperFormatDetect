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

  var flag = 0;
  var flag1 = 0;
  var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;


  $('#myname').bind('change', function() {
    if ($('#myname').val().length > 10) {
      $('#myname').attr('data-content','姓名请少于十个字~');
      $('#myname').popover('show');
    } else if( $('#myname').val().length == 0){
      $('#myname').attr('data-content','这里不可以空着哦~');
      $('#myname').popover('show');
    }else {
      $('#myname').popover('hide');
      $('#myname').css('border-color', 'rgb(200,200,200)');
    }
  });


  $('#user').bind('change', function() {
  
    if($('#user').val().length == 0){
      $('#user').attr('data-content','这里不可以空着哦~');
      $('#user').popover('show');
    }else if (!(/[2-4](08|09|10|11|12|13|14|15)\d{5}/).test($('#user').val())) {
      $('#user').attr('data-content','您的学号格式不对哦~');
      $('#user').popover('show');
    }else {
      $('#user').popover('hide');
      $('#user').css('border-color', 'rgb(200,200,200)');
    }
  });


  $('#mail').bind('change', function() {
    if($('#mail').val().length == 0){
      flag1 = 0;
      $('#mail').attr('data-content','这里不可以空着哦~');
      $('#mail').popover('show');
    }else if (!re.test($('#mail').val())) {
      flag1 = 0;
      $('#mail').attr('data-content','您的邮箱格式不对哦~');
      $('#mail').popover('show');
    } else {
      flag1=1;
      $('#mail').popover('hide');
      $('#mail').css('border-color', 'rgb(200,200,200)');
    }
  });



  $('#pwd1').bind('change', function() {
    if($('#pwd1').val().length == 0){
      $('#pwd1').attr('data-content','这里不可以空着哦~');
      $('#pwd1').popover('show');
    }else if ($('#pwd1').val().length < 6 || $('#pwd1').val().length > 20 || flag == 1) {
      $('#pwd1').attr('data-content','请输入5位以上21位以下的密码，并且只能是数字和字母哦~');
      $('#pwd1').popover('show');
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
             $('#pwd1').attr('data-content','请输入5位以上21位以下的密码，并且只能是数字和字母哦~');
            $('#pwd1').popover('show');
            flag = 1;
          } else if ($('#pwd1').val().length >= 6 && $('#pwd1').val().length <= 20) {
            $('#pwd1').popover('hide');
            $('#pwd1').css('border-color', 'rgb(200,200,200)');
            flag = 0;//flag=0表示没问题
          }
        },
      });
    }

    // if($("#pwd2").val().length == 0){
    //   $('#pwd2').attr('data-content','这里不可以空着哦~');
    //   $('#pwd2').popover('show');
    // }else if ($('#pwd2').val() != $('#pwd1').val() && $('#pwd2').hasClass('clicked')) {
    //   $('#pwd2').attr('data-content','您输入的确认密码和密码不一致哦~');
    //   $('#pwd2').popover('show');
    // } else {
    //   $('#pwd2').popover('hide');
    //   $('#pwd2').css('border-color', 'rgb(200,200,200)');
    // }
  });
  $('#pwd2').bind('change', function() {
    if($("#pwd2").val().length == 0){
      $('#pwd2').attr('data-content','这里不可以空着哦~');
      $('#pwd2').popover('show');
    }else if ($('#pwd2').val() != $('#pwd1').val()) {
      $('#pwd2').attr('data-content','您输入的确认密码和密码不一致哦~');
      $('#pwd2').popover('show');
    } else {
      $('#pwd2').popover('hide');
      $('#pwd2').css('border-color', 'rgb(200,200,200)');
    }
  });
  // $('#myclass').bind('change', function() {
  //   if ($('#myclass').val().length != 4 || !$.isNumeric($('#myclass').val()) || parseInt($('#myclass').val()) < 1000 || parseInt($('#myclass').val()) > 6000) {
  //     $('#myclass').popover('show');
  //   } else {
  //     $('#myclass').popover('hide');
  //     $('#myclass').css('border-color', 'rgb(200,200,200)');
  //   }
  // });

  //问题1：点击注册按钮直接弹出error提示，没有进入action
  //问题2：两次输入的密码没有进行比较是否相同
  //问题3：注册姓名要小于10位，学号必须是9位，密码大于4位小于20位的判断也没写
  $('#zhuce').click(function() {
    if ($('#myname').val().length > 10 || $('#myname').val().length == 0) {
      $('#myname').css('border-color', 'red');
    }
    if (!(/[2-4](08|09|10|11|12|13|14|15)\d{5}/).test($('#user').val())) {
      $('#user').css('border-color', 'red');
    }
    if (flag1 == 0) {
      $('#mail').css('border-color', 'red');
    }
    if ($('#user').val() == '21512345') {
      alertWarning("用户已存在！请重新注册");
      $('#myname,#user,#pwd1,#pwd2').val('');
      return;
    }

    if ($('#pwd1').val().length < 6 || $('#pwd1').val().length > 20 || flag == 1) {
      $('#pwd1').css('border-color', 'red');
      $('#pwd2').css('border-color', 'red');
    }
    if ($('#pwd2').val() != $('#pwd1').val()) {
      $('#pwd2').css('border-color', 'red');
    }
    // if ($('#myclass').val().length != 4 || !$.isNumeric($('#myclass').val()) || parseInt($('#myclass').val()) < 1000 || parseInt($('#myclass').val()) > 6000) {
    //   $('#myclass').css('border-color', 'red');
    // }
    // if (flag == 1) {
    //   $('#pwd1').css('border-color', 'red');
    //   $('#pwd2').css('border-color', 'red');
    //   $('#pwd1').popover('show');
    //   $('#pwd2').popover('show');
    // }
    if($('#myname').val().length == 0 ||$('#user').val().length == 0||$('#mail').val().length == 0||$('#pwd1').val().length ==0 ||$('#pwd2').val().length == 0){
      alertWarning("内容不可以为空！");
      return;
    }
    if ($('#myname').val().length > 10 ||!(/[2-4](08|09|10|11|12|13|14|15)\d{5}/).test($('#user').val()) || ($('#pwd1').val().length < 6 || $('#pwd1').val().length > 20 || flag == 1) || $('#pwd2').val() != $('#pwd1').val() || flag1 == 0) {
      alertWarning('您输入的信息有误！');
      return;
    }
    
    name_value = encodeURI($('#myname').val());
    username_value = $('#user').val();
    //转码有问题
    grade_value1 = encodeURI($('#myclass').val());
    sex_value = $('input[name="sex"]:checked').val();
    password_value = $('#pwd2').val();
    class_value = encodeURI($('#classEdition').val());
    var grade_value = class_value + grade_value1;
    $.ajax({
      type: 'post',
      url: 'register.action',
      data: {
        name: name_value,
        username: username_value,
        email: $('#mail').val(),
        grade: grade_value,
        sex: sex_value,
        password: password_value
      },
      success: function(msg) {
        if (msg == -1) {
          alertWarning("用户已存在!请重新注册");
          $('#myname,#user,#pwd1,#pwd2,#mail').val('');
          setTimeout("window.location.reload()","3000");
        } else {
          alertInfo("注册成功");
          setTimeout("loadlater()","3000");
        }

      },
      error: function() {
        alertWarning("注册失败！请重新注册");
      },
    });

  });
});

function loadlater() {
  if ($('#mibao').is(":checked")) {
    window.location.href = '/PaperDetect/security.jsp';
  } else
    window.location.href = '/PaperDetect/login.jsp';
}
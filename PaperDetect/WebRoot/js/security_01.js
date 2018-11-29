var userflag = false;
$(document).ready(function() {

  $('#mail_set').click(function() {
    if ($('#username').val() == '') {
      alertWarning('请先在上方填写您的学号~');
    } else if (userflag) {
      $.ajax({
        type: 'get',
        url: 'reset.action',
        data: {
          username: $('#username').val(),
        },
        success: function() {
          alertInfo('请前往您的注册邮箱进行验证！');
        },
      });
    }

  });

  /*    $('#username').bind('change', function() {
        if ($('#username').val() != '201612345') {
            alertWarning('您无权进行密保验证');
        }
    });
*/

  $('#username').bind('change', function() {
    if ($('#username').val() != '') {
      $('#username').css('border-color', 'rgb(200,200,200)');
      $.ajax({
        type: 'post',
        url: 'isExist.action',
        data: {
          username: $('#username').val()
        },
        success: function(msg) {
          if (msg == 0) {
            $('#username').popover('show');
            userflag = false;
          } else {
            $('#username').popover('hide');
            userflag = true;
          }
        },
      });
    }
  });



  $('#answer1').bind('input', function() {
    if ($('#answer1').val() != '') {
      $('#answer1').css('border-color', 'rgb(200,200,200)');
    }
  });
  $('#answer2').bind('input', function() {
    if ($('#answer2').val() != '') {
      $('#answer2').css('border-color', 'rgb(200,200,200)');
    }
  });
  $('#answer3').bind('input', function() {
    if ($('#answer3').val() != '') {
      $('#answer3').css('border-color', 'rgb(200,200,200)');
    }
  });

  $('#queding').click(function() {
    if ($('#username').val() == '') {
      $('#username').css('border-color', 'red');
    }
    if ($('#answer1').val() == '') {
      $('#answer1').css('border-color', 'red');
    }
    if ($('#answer2').val() == '') {
      $('#answer2').css('border-color', 'red');
    }
    if ($('#answer3').val() == '') {
      $('#answer3').css('border-color', 'red');
    }
    if ($('#answer1').val() == '' || $('#answer2').val() == '' || $('#answer3').val() == '') {
      alertWarning('不可以空着哦~');
      return;
    } else {
      var answer1_id = encodeURI($('#answer1').val());
      var answer2_id = encodeURI($('#answer2').val());
      var answer3_id = encodeURI($('#answer3').val());
      $.ajax({
        type: 'post',
        url: 'forget.action',
        data: {
          new_answer1: answer1_id,
          new_answer2: answer2_id,
          new_answer3: answer3_id
        },
        success: function(msg) {
          //window.location.reload();
          if (msg == 1) {
            alertInfo('密保验证成功！密码已重置为“123456”');
            setTimeout("loadlater()","3000");
          } else {
            alertWarning("密保验证失败");
          }
        },
      });
    }
  });
});

function loadlater(){
  window.location.href = 'login.jsp';
}
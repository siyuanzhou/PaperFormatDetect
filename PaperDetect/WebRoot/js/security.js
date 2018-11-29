$(document).ready(function() {
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
      var answer3_id = encodeURI($('#answer2').val());
      $.ajax({
        type: 'post',
        url: 'setSecurity.action',
        data: {
          answer1: answer1_id,
          answer2: answer2_id,
          answer3: answer3_id
        },
        success: function(msg) {
          //window.location.reload();

          alertInfo('密保设置成功！');
          setTimeout("loadlater()","3000");
        },
      });
    }
  });
});


function loadlater(){
  window.location.href = 'login.jsp';
}
//注销登录
function loginout(){
  $.ajax({
    type:'get',
    url:'logout.action',
    success:function(){;
    	window.location.href='login.jsp';
    },
  });
}
$(function() {
  $('#submit').click(function() {
    if ($('#advice').val() == '') {
      alertWarning('星号内容不可以为空！');
      return;
    } else if ($('#advice').val().length > 500) {
      alertWarning('反馈意见超出字数长度限制！');
      return;
    } else {
      $.ajax({
        type: 'post',
        url: 'advice.action',
        data: {
          advice: encodeURI($('#advice').val()),
          email: $('#mail').val()
        },
        success: function(msg) {
          if (msg == 1) {
            alertInfo('信息提交成功，我们会尽快为您处理！');
            $('#advice').val('');
          } else {
            alertWarning('反馈意见提交失败，请重新提交！');
          }
        },
      });
    }
  });
});
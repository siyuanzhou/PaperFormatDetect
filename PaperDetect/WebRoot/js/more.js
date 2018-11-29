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
    $('.carousel').carousel({
      interval: 2500
    });
  });
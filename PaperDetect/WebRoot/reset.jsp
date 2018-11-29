<%@ page language="java" import="java.util.*" pageEncoding="UTF-8" %>
<%@taglib uri="/struts-tags"  prefix="struts"%>
<%
String path = request.getContextPath();
String basePath = request.getScheme()+"://"+request.getServerName()+":"+request.getServerPort()+path+"/";
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
	<title>毕业论文检测系统</title>
	<meta name="renderer" content="webkit|ie-comp|ie-stand">
	<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE11">
	<link rel="icon" type="image/x-icon" href="images/redant_icon.png">
	<meta charset="utf-8">
	<link rel="stylesheet" href="css/bootstrap.min.css">
  <script src="js/jquery-1.12.4.min.js"></script>
  <script src="js/bootstrap.min.js"></script>
	<link rel="stylesheet" type="text/css" href="css/bootstrap-dialog.min.css">
	<script type="text/javascript" src="js/bootstrap-dialog.min.js"></script>
  <script type="text/javascript" src="js/alertTool.js"></script>
  <link rel="stylesheet" type="text/css" href="css/pagecontent.css">
  <link rel="stylesheet" type="text/css" href="css/reset.css">
  <script type="text/javascript" src="js/reset.js"></script>
  <style type="text/css">
    @font-face {
  font-family: 'Glyphicons Halflings';
  src: url('fonts/glyphicons-halflings-regular.eot');
  src: url('fonts/glyphicons-halflings-regular.eot?#iefix') format('embedded-opentype'), url('fonts/glyphicons-halflings-regular.woff') format('woff'), url('fonts/glyphicons-halflings-regular.ttf') format('truetype'), url('fonts/glyphicons-halflings-regular.svg#glyphicons_halflingsregular') format('svg');
}

.glyphicon {
  position: relative;
  top: 1px;
  display: inline-block;
  font-family: 'Glyphicons Halflings';
  -webkit-font-smoothing: antialiased;
  font-style: normal;
  font-weight: normal;
  line-height: 1;
  -moz-osx-font-smoothing: grayscale;
}
  </style>
</head>
<body>
  <div class="pagecontent">
    <div id="header"><img id="image" src="images/redant.png">本科毕业论文格式检测系统</div>
    <div class="login-wrapper">
      <form action="#">
        <div class="form-group" style="margin-top:30px;">
          <div class="input-group">
            <span class="input-group-addon"><img src="images/studentid.png" style="height: 20px;width: 14px;"></span><input id="user" data-container="body" data-toggle="popover" data-placement="right" data-content="用户不存在！" data-trigger="manual" class="form-control" placeholder="学号" type="text" autocomplete="off">
          </div>
        </div>
        <div class="form-group">
          <div class="input-group">
            <span class="input-group-addon"><i class="glyphicon glyphicon-certificate"></i></span><input id="code" data-container="body" data-toggle="popover" data-placement="right" data-content="您的验证码格式不对哦~" data-trigger="manual" class="form-control" placeholder="验证码" type="text" autocomplete="off">
          </div>
        </div>
        <div class="form-group">
          <div class="input-group">
            <span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span><input id="pwd1" data-container="body" data-toggle="popover" data-placement="right" data-content="请输入5位以上21位以下的密码，并且只能是数字和字母哦~" data-trigger="manual" class="form-control" placeholder="密码" type="text" onfocus="this.type='password'" autocomplete="off">
          </div>
        </div>
        <div class="form-group">
          <div class="input-group">
            <span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span><input id="pwd2" data-container="body" data-toggle="popover" data-placement="right" data-content="您输入的确认密码和密码不一致哦~" data-trigger="manual" class="form-control" placeholder="确认密码" type="text" onfocus="this.type='password'" autocomplete="off">
          </div>
        </div>
        <input id="reset" class="btn btn-lg btn-primary btn-block" type="button" value="重置密码">
      </form>
      <span style="float:left;">
        想起密码了？
      </span>
      <a id="denglu" class="btn btn-default-outline btn-block" href="login.jsp">立即登录</a>
    </div>
  </div>
</body>
</html>
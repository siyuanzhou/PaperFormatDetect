<%@ page language="java" import="java.util.*" pageEncoding="UTF-8" %>
<%
String path = request.getContextPath();
String basePath = request.getScheme()+"://"+request.getServerName()+":"+request.getServerPort()+path+"/";
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
  <title>毕业论文检测系统</title>
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <meta name="renderer" content="webkit|ie-comp|ie-stand">
  <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE11">
  <link rel="icon" type="image/x-icon" href="images/redant_icon.png">
  <meta charset="utf-8">
  
  <script type="text/javascript" src="js/jsTool.js"></script>
  <link rel="stylesheet" href="css/bootstrap.min.css">
  <script src="js/jquery-1.12.4.min.js"></script>
  <script src="js/bootstrap.min.js"></script>
  <link rel="stylesheet" type="text/css" href="css/bootstrap-dialog.min.css">
  <script type="text/javascript" src="js/bootstrap-dialog.min.js"></script>
  <script type="text/javascript" src="js/alertTool.js"></script>
  <script type="text/javascript" src="js/jquery.cookie.js"></script>
  <link rel="stylesheet" type="text/css" href="css/pagecontent.css">
  <link rel="stylesheet" type="text/css" href="css/login.css">
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
<body onload="getUserInfoByCookie()">
<div class="pagecontent">
  <div id="header"><img id="image" src="images/redant.png">研究生学位论文格式检测系统</div>
  <div class="login-wrapper">
    <form action="#">
      <div class="form-group">
        <div class="input-group" style="margin-top: 30px;">
          <span class="input-group-addon"><img src="images/studentid.png" style="height: 20px;width: 14px;"></span><input id="user" data-container="body" data-toggle="popover" data-placement="right" data-content="" data-trigger="manual" class="form-control" placeholder="学号" type="text" autocomplete="off">
        </div>
      </div>
      <div class="form-group">
        <div class="input-group">
          <span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span><input id="pwd2" data-container="body" data-toggle="popover" data-placement="right" data-content="" data-trigger="manual" class="form-control" placeholder="密码" type="text" onfocus="this.type='password'" autocomplete="off">
        </div>
      </div>
      <div id="lala">
        <span class="text-left" style="margin-left: 20px;display: inline-block;">
          <label class="checkbox"><input id="ck_rmbUser" type="checkbox"><span>记住密码</span></label>
        </span>
        <a class="pull-right" href="security_01.jsp">忘记密码了？</a>
      </div>
      <input id="denglu" class="btn btn-lg btn-primary btn-block" type="button" value="登录">
    </form>
    <span style="float:left;">
      还没有账户？
    </span>
    <a id="zhuce" class="btn btn-default-outline btn-block" href="register.jsp">立即注册</a>
  </div>
</div>
<script type="text/javascript" src="js/login.js"></script>
</body>
</html>


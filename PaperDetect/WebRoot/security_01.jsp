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
  <link rel="stylesheet" type="text/css" href="css/security_01.css">
  <script type="text/javascript" src="js/security_01.js"></script>
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
    <div id="header"><img id="image" src="images/redant.png">研究生学位论文格式检测系统</div>
    <div class="login-wrapper">
      <form action="#">
        <h4 style="margin-top: 40px;text-align: center;margin-bottom: 30px;">请输入下列问题的答案</h4>
        <b>0、您的学号是？</b>
        <input id="username" class="form-control" type="text" data-container="body" data-toggle="popover" data-placement="right" data-content="用户不存在！" data-trigger="manual" class="form-control" autocomplete="off">
        <b>1、您父亲的姓名是？</b>
        <input id="answer1" class="form-control" type="text" autocomplete="off">
        <b>2、您母亲的姓名是？</b>
        <input id="answer2" class="form-control" type="text" autocomplete="off">
        <b>3、您的出生地是？</b>
        <input id="answer3" class="form-control" type="text" autocomplete="off">
        <div>
          <a id="mail_set" class="pull-left" style="text-decoration: none;cursor: pointer;">什么，没有设置密保？</a>
          <div class="pull-right"><a href="login.jsp" style="text-decoration: none;cursor: pointer;">立即返回登录页</a></div>
        </div><br/>
        <input id="queding" class="btn btn-lg btn-primary btn-block" type="button" value="确认">
      </form>
      <p style="margin-top: -20px;"><b>人工找回密码方法：可直接拨打18340861689，或发送短信给该号码</b></p>
    </div>
  </div>
</body>
</html>
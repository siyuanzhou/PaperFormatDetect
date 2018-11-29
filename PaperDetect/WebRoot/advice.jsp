<%@ page language="java" import="java.util.*" pageEncoding="UTF-8" %>
<%@taglib uri="/struts-tags"  prefix="struts"%>
<%
String path = request.getContextPath();
String basePath = request.getScheme()+"://"+request.getServerName()+":"+request.getServerPort()+path+"/";
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
  <head>
    <meta charset="utf-8">
    <meta name="renderer" content="webkit|ie-comp|ie-stand">
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE11">
    <title>毕业论文检测系统</title>
    <link rel="icon" type="image/x-icon" href="images/redant_icon.png">
    <link rel="stylesheet" href="css/bootstrap.min.css">
    <script src="js/jquery-1.12.4.min.js"></script>
    <script src="js/bootstrap.min.js"></script>
    <link rel="stylesheet" type="text/css" href="css/bootstrap-dialog.min.css">
    <script type="text/javascript" src="js/bootstrap-dialog.min.js"></script>
    <script type="text/javascript" src="js/alertTool.js"></script>
    <link rel="stylesheet" type="text/css" href="css/pagecontent.css">
    <link rel="stylesheet" type="text/css" href="css/advice.css">
    <script type="text/javascript" src="js/advice.js"></script>
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
    <div id="header">
      <img src="images/redant.png">
      <span>意见中心</span>
      <div class="pull-right">
        <a href="more.jsp" style="border-right:1px solid rgb(66,139,202);height:100%;padding-right:10px;margin-right:5px;">了解更多</a>
        <a href="students.jsp" style="border-right:1px solid rgb(66,139,202);height:100%;padding-right:10px;margin-right:5px;">返回首页</a>
        <a onclick="loginout();">退出登录</a>
      </div>
    </div>
    <div id="container">
      <div id="subheader">&nbsp&nbsp感谢使用本网站！</div>
      <h4><i>*</i>&nbsp&nbsp反馈意见</h4>
      <textarea id="advice" style="width:100%;height:200px;" placeholder="请输入您的反馈意见(500字以内)~" maxlength="500"></textarea>
      <!-- <h4><i>*</i>&nbsp&nbsp联系邮箱</h4>
      <input id="mail" type="text"></input>
      <p> 请留下联系邮箱，我们会将回复以邮件的方式发送到您的联系邮箱。</p> -->
      <h4 style="margin-top:30px;">联系电话</h4>
      <div>
        <p>紧急情况下，可直接拨打<b>18340861689</b>，或发送短信给该号码。</p>
        <div ><input id="submit" type="button" value="提交" class="btn btn-primary btn-block"></input></div>
      </div>
      <br>
    </div>
    <div class="foot">版权所有：大连理工大学软件学院红蚂蚁实验室<br>联系电话：18340861689&nbsp;&nbsp;邮箱：648088725@qq.com</div>
  </div>
</body>
</html>
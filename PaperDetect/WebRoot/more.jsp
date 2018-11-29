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
  <link rel="stylesheet" type="text/css" href="css/pagecontent.css">
  <link rel="stylesheet" type="text/css" href="css/more.css">
  <script type="text/javascript" src="js/more.js"></script>
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
    <span>研究生学位论文格式检测系统</span>
    <div class="pull-right">
      <a href="advice.jsp" style="border-right:1px solid rgb(66,139,202);height:100%;padding-right:10px;margin-right:5px;">反馈意见</a>
      <a href="students.jsp" style="border-right:1px solid rgb(66,139,202);height:100%;padding-right:10px;margin-right:5px;">返回首页</a>
      <a onclick="loginout();">退出登录</a>
    </div>
  </div>
  <div id="container">
    <div id="subheader">&nbsp&nbsp&nbsp&nbsp感谢使用本网站！</div>
    <div id="carousel-example-generic" class="carousel slide" data-ride="carousel">
      <!-- Indicators -->
      <ol class="carousel-indicators">
        <li data-target="#carousel-example-generic" data-slide-to="0" class="active"></li>
        <li data-target="#carousel-example-generic" data-slide-to="1"></li>
        <li data-target="#carousel-example-generic" data-slide-to="2"></li>
        <li data-target="#carousel-example-generic" data-slide-to="3"></li>
      </ol>
      <!-- Wrapper for slides -->
      <div class="carousel-inner" role="listbox">
        <div class="item active">
          <img class="lala" src="images/053101.png" alt="...">
          <div class="carousel-caption">
            <!--需要填写文字的地方-->
          </div>
        </div>
        <div class="item">
          <img class="lala" src="images/053102.png" alt="...">
          <div class="carousel-caption"  style="font-size:30px;">
            <!--需要填写文字的地方-->
          </div>
        </div>

        <div class="item">
          <img class="lala" src="images/053103.png" alt="...">
          <div class="carousel-caption" style="font-size:30px;">
            <!--需要填写文字的地方-->
          </div>
        </div>

        <div class="item">
          <img class="lala" src="images/053104.png" alt="...">
          <div class="carousel-caption">
            <!--需要填写文字的地方-->
          </div>
        </div>

      </div>
      <!-- Controls -->
      <a class="left carousel-control" href="#carousel-example-generic" role="button" data-slide="prev">
        <span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>
        <span class="sr-only">Previous</span>
      </a>
      <a class="right carousel-control" href="#carousel-example-generic" role="button" data-slide="next">
        <span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>
        <span class="sr-only">Next</span>
      </a>
    </div>
  </div>
  <div class="foot">版权所有：大连理工大学软件学院红蚂蚁实验室<br>联系电话：18340861689&nbsp;&nbsp;邮箱：648088725@qq.com</div>
</div>
</body>
</html>
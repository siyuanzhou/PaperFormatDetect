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
  <link rel="stylesheet" type="text/css" href="css/datepicker.min.css">
  <script src="js/datepicker.min.js"></script>
  <script src="js/mricode.pagination.js"></script>

  <script src="js/jquery.metadata.js"></script>
  <script src="js/mbMenu.js"></script>
  <script src="js/jquery.hoverIntent.js"></script>
  <link rel="stylesheet" type="text/css" href="css/bootstrap-dialog.min.css">
  <script src="js/bootstrap-dialog.min.js"></script>
  <script src="js/alertTool.js"></script>
  <link rel="stylesheet" type="text/css" href="css/menu_red.css" title="styles1"  media="screen" />
  <link rel="stylesheet" type="text/css" href="css/mricode.pagination.css">
  <link rel="stylesheet" type="text/css" href="css/pagecontent.css">
  <link rel="stylesheet" type="text/css" href="css/teacher.css">
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
        <a data-toggle="modal" data-target="#exampleModal" style="border-right:1px solid rgb(66,139,202);height:100%;padding-right:10px;margin-right:5px;">修改密码</a>
        <a href="more.jsp" style="border-right:1px solid rgb(66,139,202);height:100%;padding-right:10px;margin-right:5px;">了解更多</a>
         <a onclick="loginout();">退出登录</a>
      </div>
    </div>
    <div id="container">
      <div id="subheader">&nbsp&nbsp&nbsp&nbsp感谢使用本网站！</div>
      <ul class="nav nav-tabs" role="tablist">
        <li role="presentation" class="active" id="paper"><a href="#home" aria-controls="home" role="tab" data-toggle="tab">查看论文</a></li><!-- 跳转到id=home -->
        <li role="presentation" id="feedback"><a href="#profile" aria-controls="profile" role="tab" data-toggle="tab">查看反馈</a></li><!-- 跳转到id=profile
      -->         </ul>
      <div class="tab-content">
        <div role="tabpanel" class="tab-pane active" id="home">
          <div id="delete" class="btn btn-danger" style="float: left;width:80px;margin-top:-80px;margin-left: 862px;">删除</div>

        <!-- 先屏蔽掉树形查找
         <div id="menuDemo" class="rootVoices myMenu" cellspacing='0' cellpadding='0' border='0'>
            <div class="rootVoice {menu: 'menu_2'}" >树形查找</div>
          </div>-->
		

		<select id="select_01" style="width:150px;float: left;margin-top:-72px;font-size: 15px;margin-left: 52px;">
			<option value="0">姓名</option>
			<option value="1">学号</option>
			<option value="2">论文</option>
			<option value="3">年级</option>
		</select>

          <div class="form-group" style="width:300px;display: inline; float: left;margin-top:-80px;margin-left: 252px;">
          <div class="input-group">
            <input id="search_01" data-container="body" data-toggle="popover" data-placement="top" data-content="您输入的信息有误" data-trigger="manual" class="form-control" placeholder="请输入您想要查找的信息" type="text" onfocus="this.placeholder=$('#select_01 option:selected').text()+'信息'" autocomplete="off"><span onclick="search_01($('#select_01').val(),encodeURI($('#search_01').val()));" class="input-group-addon" style="cursor: pointer;"><i class="glyphicon glyphicon-search"></i></span>
          </div>
        </div>

<input onclick="window.location.reload();" type="button" class="btn btn-primary" value="查看全部" style="width:80px;display: inline; float: left;margin-top:-80px;margin-left: 732px;">





<!--实现按照时间段查找的方法-->
<input type="button" class="btn btn-primary" data-toggle="modal" data-target="#myModal" value="时间查找" style="width:80px;display: inline; float: left;margin-top:-80px;margin-left: 602px;">

<div id="myModal" class="modal fade" role="dialog" aria-labelledby="gridSystemModalLabel">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
        <h4 class="modal-title" id="gridSystemModalLabel">请输入您想要查询的时间</h4>
      </div>
      <div class="modal-body">
        <div class="container-fluid">



           <div class="form-group">
    <label for="exampleInputPassword1" style="display: block;">查询时间</label>
    <input type="text" class="form-control" id="start" placeholder="2016-6-5" style="width: 90%;display: inline-block;clear:left;">
  </div>




        </div>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
        <input id="time" type="button" class="btn btn-primary" value="确定" onclick="search_01(4,encodeURI($('#start').val()))">
      </div>
    </div><!-- /.modal-content -->
  </div><!-- /.modal-dialog -->
</div><!-- /.modal -->








          <div id="table">
           <!-- <marquee style="float:left;width:60%;margin-top: -70px;"  onmouseout="this.start()" onmouseover="this.stop()" scrollamount="2" scrolldelay="0.5">点击相应的论文和检测报告可以下载哦~</marquee>-->
            <div class="row" style="margin-bottom: 20px;border-bottom:rgb(248,248,248) 2px solid;font-weight: bold;width:100%;">
              <input id="check" type="checkbox" name="checkbox" class="col-md-1">
              <div class="row col-md-11">
                <div class="col-md-2">姓名</div>
                <div class="col-md-2">学号</div>
                <div class="col-md-6" style="text-align: center;">论文</div>
                <div class="col-md-2">检测报告</div>
              </div>
            </div>
            <div id="eventLog"></div>
            <div id="page" class="m-pagination"></div>
          </div>
        </div>
        <div role="tabpanel" class="tab-pane" id="profile">
          <div id="table1">
          <select id="select_02" style="width:150px;float: left;margin-top:-72px;font-size: 15px;">
			<option value="0">姓名</option>
			<option value="1">学号</option>
		<!--	<option value="2">班级</option>-->
		</select>

          <div class="form-group" style="width:300px;display: inline; float: left;margin-top:-80px;margin-left: 200px;">
          <div class="input-group">
            <input id="search_02" data-container="body" data-toggle="popover" data-placement="right;" data-content="您输入的信息有误" data-trigger="manual" class="form-control" placeholder="请输入您想要查找的信息" type="text" onfocus="this.placeholder=$('#select_02 option:selected').text()+'信息';" autocomplete="off"><span onclick="search_02($('#select_01').val(),encodeURI($('#search_01').val()));" class="input-group-addon" style="cursor: pointer;"><i class="glyphicon glyphicon-search"></i></span>
          </div>
        </div>


        <input onclick="window.location.reload();" type="button" class="btn btn-primary" value="查看全部" style="width:80px;display: inline; float: left;margin-top:-80px;margin-left: 550px;">
        <div id="delete1" class="btn btn-danger" style="float: left;width:80px;margin-top:-80px;margin-left: 680px;">删除</div>

<!--实现按照时间段查找的方法-->
<!--<input type="button" class="btn btn-primary" data-toggle="modal" data-target="#myModal1" value="时间查找" style="width:80px;display: inline; float: left;margin-top:-80px;margin-left: 550px;">

<div id="myModal1" class="modal fade" role="dialog" aria-labelledby="gridSystemModalLabel">
  <div class="modal-dialog" role="document">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
        <h4 class="modal-title" id="gridSystemModalLabel">请输入您想要查询的时间</h4>
      </div>
      <div class="modal-body">
        <div class="container-fluid">


<div class="form-group">
    <label for="exampleInputPassword1" style="display: block;">查询时间</label>
    <input type="text" class="form-control" id="start1" placeholder="2016-6-5" style="width: 90%;display: inline-block;clear:left;">
  </div>

          
        </div>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
        <input id="time1" type="button" class="btn btn-primary" value="确定" onclick="search_02(3,encodeURI($('#start1').val()))"> 
      </div>-->
   <!-- </div>--><!-- /.modal-content -->
  <!--</div>--><!-- /.modal-dialog -->
<!--</div>--><!-- /.modal -->





            <!--<marquee style="float:left;width:60%;margin-top: -70px;"  onmouseout="this.start()" onmouseover="this.stop()" scrollamount="2" scrolldelay="0.5">点击复选框后面的文字可以查看反馈意见</marquee>-->

            <div class="row" style="margin-bottom: 20px;border-bottom:rgb(248,248,248) 2px solid;font-weight: bold;width:100%;">
              <input id="check1" type="checkbox" name="checkbox1" class="col-md-2">
              <div class="row col-md-10">
                <div class="col-md-2">编号</div>
                <div class="col-md-2">姓名</div>
                <div class="col-md-2">学号</div>
                <div class="col-md-3">邮箱</div>
                <div class="col-md-3">反馈意见</div>
              </div>
            </div>
            <div id="eventLog1"></div>
            <div id="page1" class="m-pagination"></div>
          </div>
        </div>
      </div><!--tab>>总的div-->
    </div> <!--id=container-->
    <div class="modal fade" id="exampleModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel">
      <div class="modal-dialog" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            <h4 class="modal-title" id="exampleModalLabel">修改密码</h4>
          </div>
          <div class="modal-body">
            <form>
              <div class="form-group">
                <label for="recipient-name" class="control-label">初始密码：</label>
                <input type="text" class="form-control" id="old_id" data-container="body" data-toggle="popover" data-placement="right" data-content="您输入的密码不正确！" data-trigger="maual" class="form-control" placeholder="初始密码" onfocus="this.type='password'" autocomplete="off">
              </div>
              <div class="form-group">
                <label for="recipient-name" class="control-label">新密码：</label>
                <input class="form-control" id="pwd1"  data-container="body" data-toggle="popover" data-placement="right" data-content="请输入5位以上21位以下的密码，并且只能是数字和字母哦~" data-trigger="manual" class="form-control" placeholder="密码" type="text" onfocus="this.type='password'" autocomplete="off">
              </div>
              <div class="form-group">
                <label for="message-text" class="control-label">确认密码</label>
                <input type="text" class="form-control" id="pwd2" data-container="body" data-toggle="popover" data-placement="right" data-content="您输入的确认密码和密码不一致哦~" data-trigger="manual" class="form-control" placeholder="确认密码" onfocus="this.type='password'" autocomplete="off">
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
            <button id="queren" type="button" class="btn btn-primary">确认</button>
          </div>
        </div>
      </div>
    </div>
    <div id="menu_2" class="menu">
      <a data-type="title" class="{action: 'document.title=(\'menu_2.1\')'}">毕业季</a>
      <a class="{action: 'document.title=(\'menu_2.2\')'}">2016</a>
      <a class="{menu: 'sub_menu_1'}">2017<span style="float: right;">></span></a>
      <a class="{menu: 'sub_menu_2'}">2018</a>
      <a data-type="separator"> </a>
      <a class="{action: 'document.title=(\'menu_2.4\')'}">2019</a>
    </div>
    <div id="sub_menu_1" class="menu">
      <a data-type="title" class="{action: 'document.title=(\'sub_menu_1.1\')'}">班级</a>
      <a data-type="separator"> </a>
      <a class="{menu:'menu_1'}">软英1201班</a>
      <a class="{action: 'document.title=(\'sub_menu_1.3\')'}">软英1202班</a>
      <a class="{action: 'document.title=(\'sub_menu_1.4\')'}">软英1203班</a>
    </div>
    <div class="foot">版权所有：大连理工大学软件学院红蚂蚁实验室<br>联系电话：18340861689&nbsp;&nbsp;邮箱：648088725@qq.com</div>
  </div>





<script language="javascript" type="text/javascript" src="js/teacher.js"></script>
</body>
</html>
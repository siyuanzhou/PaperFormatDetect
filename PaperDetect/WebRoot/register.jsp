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
  <link rel="stylesheet" type="text/css" href="css/register.css">
  <script type="text/javascript" src="js/register.js"></script>
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
  </style>>
</head>
<body>
  <div class="pagecontent">
    <div id="header"><img id="image" src="images/redant.png">研究生学位论文格式检测系统</div>
    <div class="login-wrapper">
      <form action="#">
        <div class="form-group">
          <div class="input-group" style="margin-top:30px;">
            <span class="input-group-addon"><i class="glyphicon glyphicon-user"></i></span><input id="myname" data-container="body" data-toggle="popover" data-placement="right" data-content="" data-trigger="manual" class="form-control" placeholder="姓名" type="text" autocomplete="off">
          </div>
        </div>
        <div class="form-group">
          <div class="input-group">
            <span class="input-group-addon"><img src="images/studentid.png" style="height: 20px;width: 14px;"></span><input id="user" data-container="body" data-toggle="popover" data-placement="right" data-content="" data-trigger="manual" class="form-control" placeholder="学号" type="text" autocomplete="off">
          </div>
        </div>
        <div class="form-group">
          <div class="input-group">
            <span class="input-group-addon"><i class="glyphicon glyphicon-envelope"></i></span><input id="mail" data-container="body" data-toggle="popover" data-placement="right" data-content="" data-trigger="manual" class="form-control" placeholder="邮箱" type="text" autocomplete="off">
          </div>
        </div>
        <select name="" id="classEdition" style="width: 100%;text-align: center;font-size: 15px;margin-bottom: 20px;height:30px;">
          <option value="软件学院" style="text-align: center;">国家示范性软件学院</option>
          <option value="化工与环境生命学部">化工与环境生命学部</option>
          <option value="电子信息与电气工程学部">电子信息与电气工程学部</option>
          <option value="建设工程学部">建设工程学部</option>
          <option value="运载工程与力学学部">运载工程与力学学部</option>
          <option value="机械工程与材料能源学部">机械工程与材料能源学部</option>
          <option value="管理与经济学部">管理与经济学部</option>
          <option value="人文与社会科学学部">人文与社会科学学部</option>
          <option value="建筑与艺术学部">建筑与艺术学部</option>
          <option value="马克思学院外国语学院">马克思学院</option>
          <option value="外国语学院">外国语学院</option>
          <option value="物理与光电工程学院">物理与光电工程学院</option>
          <option value="数学科学学院">数学科学学院</option>
          <option value="体育教学部国际教育学院">体育教学部</option>
          <option value="国际教育学院">国际教育学院</option>
          <option value="国防教育学院">国防教育学院</option>
          <option value="创新创业学院">创新创业学院</option>
          <option value="城市学院">城市学院</option>
          <option value="远程与继续教育学院">远程与继续教育学院</option>
          <option value="大连理工大学-立命馆大学国际信息与软件学院">大连理工大学-立命馆大学国际信息与软件学院</option>
          <option value="盘锦校区-石油与化学工程学院">盘锦校区-石油与化学工程学院</option>
          <option value="盘锦校区-文法学院">盘锦校区-文法学院</option>
          <option value=">盘锦校区-海洋科学与技术学院">盘锦校区-海洋科学与技术学院</option>
          <option value="盘锦校区-基础教学部">盘锦校区-基础教学部</option>
          <option value="盘锦校区-莱斯特国际学院">盘锦校区-莱斯特国际学院</option>
          <option value="盘锦校区-食品与环境学院">盘锦校区-食品与环境学院</option>
          <option value="盘锦校区-商学院">盘锦校区-商学院</option>
          <option value="盘锦校区-生命与医药学院">盘锦校区-生命与医药学院</option>
          <option value="盘锦校区-知识产权学院">盘锦校区-知识产权学院</option>

        </select>
        <select id="myclass" style="width: 100%;text-align: center;font-size: 15px;margin-bottom: 20px;height:30px;">
          <option value="2016级" style="text-align:center;">2016级</option>
          <option value="2015级">2015级</option>
          <option value="2014级">2014级</option>
          <option value="2013级">2013级</option>
          <option value="2012级">2012级</option>
          <option value="2011级">2011级</option>
          <option value="2010级">2010级</option>
          <option value="2009级">2009级</option>
          <option value="2008级">2008级</option>

        </select>
       <!--  <div class="form-group">
          <div class="input-group">
            <span class="input-group-addon"><img src="images/class.png" style="width: 14px;height: 20px;"></span><input id="myclass" data-container="body" data-toggle="popover" data-placement="right" data-content="请输入4位数字的班级号哦~" data-trigger="manual" class="form-control" placeholder="班级" type="text" autocomplete="off">
          </div>
        </div> -->
        <div class="form-group">
          <div class="input-group">
            <span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span><input id="pwd1" data-container="body" data-toggle="popover" data-placement="right" data-content="" data-trigger="manual" class="form-control" placeholder="密码" type="text" onfocus="this.type='password'" autocomplete="off">
          </div>
        </div>
        <div class="form-group">
          <div class="input-group">
            <span class="input-group-addon"><i class="glyphicon glyphicon-lock"></i></span><input id="pwd2" data-container="body" data-toggle="popover" data-placement="right" data-content="" data-trigger="manual" class="form-control" placeholder="确认密码" type="text" onfocus="this.type='password'"  autocomplete="off">
          </div>
        </div>
        <div id="sex">
          <div style="margin-left:12px;"><img src="images/gender.png" style="width:20px;height: 20px;"></div>
          <div>女&nbsp&nbsp<input id="woman" type="radio" checked="checked" name="sex" value="woman"></input></div>
          <div>男&nbsp&nbsp<input id="man" type="radio"  name="sex" value="man"></input></div>
        </div>
        <br/>
        <div style="float:left;margin-top: -17px;"><input id="mibao" type="checkbox">&nbsp&nbsp我想设置密保</div>
        <input id="zhuce" class="btn btn-lg btn-primary btn-block" type="button" value="注册">
      </form>
      <span style="float:left;">
        已经有账户了？
      </span>
      <a id="denglu" class="btn btn-default-outline btn-block" href="login.jsp">立即登录</a>
    </div>
  </div>
</body>
</html>
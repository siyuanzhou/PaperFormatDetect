<%@ page language="java" import="java.util.*" pageEncoding="UTF-8" %>
<%@taglib uri="/struts-tags"  prefix="struts"%>
<%
String path = request.getContextPath();
String basePath = request.getScheme()+"://"+request.getServerName()+":"+request.getServerPort()+path+"/";
%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
  <title>学位论文检测系统</title>
  	<meta name="renderer" content="webkit|ie-comp|ie-stand">
	<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE11">
  <meta charset="utf-8">
  <link rel="icon" type="image/x-icon" href="images/redant_icon.png">
  <link rel="stylesheet" href="css/bootstrap.min.css">
  <script src="js/jquery-1.12.4.min.js"></script>
  <script src="js/bootstrap.min.js"></script>
  <script type="text/javascript" src="./js/bootstrap-filestyle.min.js"></script>
  <script type="text/javascript" src="./js/students.js"></script>
  <link rel="stylesheet" type="text/css" href="css/bootstrap-dialog.min.css">
  <script type="text/javascript" src="js/bootstrap-dialog.min.js"></script>
  <script type="text/javascript" src="js/alertTool.js"></script>
  <link rel="stylesheet" type="text/css" href="css/pagecontent.css">
  <style type="text/css">
  body{
    font-family: "Arial","Microsoft YaHei","黑体","宋体",sans-serif;
  }
  #header{
    width:80%;
    margin:auto;
    height:40px;
    margin-top:30px;
    margin-bottom: 30px;
   }
  #header img{
  border-right:2px solid rgb(200,200,200);
  height: 40px;
  float: left;
  cursor: pointer;
  padding-right:20px;
  margin-right:20px;
   }
  #header span{
    font-size: 20px;
    line-height: 40px;
   }
  #header a{
    text-decoration: none;
    cursor:pointer;
    line-height: 40px;
   }
  #subheader{
    background-color: rgb(95,143,193);
    height:40px;
    width:100%;
    margin-bottom: 20px;
    border-radius: 5px;
    line-height: 40px;
    border:1px solid rgb(240,240,240);
    color: white;
   }
  #container{
    width:80%;
    margin:auto;
    min-height: 460px;
    border:1px solid rgb(240,240,240);
    border-radius: 5px;
  }
  #upload{
    margin-top:60px;
    margin-left:15%;
    width:70%;
  }
  #ziliao{
    width:50%;
    margin: auto;
    margin-top:60px;

  }
  #ziliao div{
    margin-top:20px;
    font-size:18px;
  }
  #ziliao div span{
    width:200px;
    margin-right:30px;
    float: left;
  }
  #collapseExample{
    width:70%;
    margin-left:25%;
  }
  .well div{
    margin-bottom: 20px;
    height:30px;
    line-height: 30px;
  }
  .well div span{
    width: 100px;
    font-size:18px;
  }
  #history{
    width:90%;
    margin: auto;
    font-size:18px;
    margin-top:20px;
  }
  #history table th{
    text-align: center;
    font-weight: bold;
  }
  #history table td{
    text-align: center;
    vertical-align: middle;
  }
  #report{
    margin: auto;
    font-size:18px;
    margin-top: 20px;
  }
  #report p{
    margin-top: 5px;
    margin-bottom: 5px;
  }
  #explanation{
  	width:45%;
  	height:320px;
  	float: right;
  	border: 1px solid rgb(200,200,200);
    margin-top:20px;
  	/*overflow: scroll;*/
    margin-right:30px;
    padding: 0px;
  }
  .foot{
  width: 60%;
  margin-left: 20%;
	text-align: center;
	height:50px;
	margin-top:50px;
	bottom: 0;
	border-top:1px solid black;
}
#pwd1,#pwd2,#pwd3{
	border:1px solid rgb(200,200,200);
}

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
        <a href="more.jsp" style="border-right:1px solid rgb(66,139,202);height:100%;padding-right:10px;margin-right:5px;">了解更多</a>
        <a href="advice.jsp" style="border-right:1px solid rgb(66,139,202);height:100%;padding-right:10px;margin-right:5px;">反馈意见</a>
        <a href="login.jsp" onclick="loginout();">退出登录</a>
      </div>
    </div>
    <div id="container">
      <div id="subheader">&nbsp&nbsp欢迎使用本系统！</div>
      <!-- Nav tabs -->
      <ul class="nav nav-tabs" role="tablist" id="ptab">
        <li role="presentation" class="active"><a href="#home" aria-controls="home" role="tab" data-toggle="tab">上传</a></li>
        <li role="presentation"><a href="#messages" aria-controls="messages" role="tab" data-toggle="tab" onclick="updatehistory()">检测历史</a></li>
        <li role="presentation"><a href="#report" aria-controls="report" role="tab" data-toggle="tab">检测报告</a></li>
        <li role="presentation"><a href="#profile" aria-controls="profile" role="tab" data-toggle="tab">个人设置</a></li>
      </ul>
      <!-- Tab panes -->
      <div class="tab-content">
        <!--上传文档-->
        <div role="tabpanel" class="tab-pane active" id="home">
          <div id="upload"  style="width:40%;margin-left: 30px;float:left;margin-top:20px;">
          <p style="margin-left:0px;color:red;">请上传格式为.doc<i style="color: red;">x</i>的文档(可将格式为.doc的文档另存为.docx格式再上传,仅把扩展名从doc改为docx是不行的哦~)</p>
            <iframe name='hidden_frame' id="hidden_frame" style="display:none"></iframe>
            <form role="form"  name="upload" id = "uploadform" action="upload.action" enctype="multipart/form-data" method="post" target="hidden_frame">
              <div class="col-lg-14">
                <div class="form-group">
                  <input type="file" name="upload" id="input01"  onchange="fileSize = fileChange(this);">
                </div>
              </div>
              <div class="col-lg-14">
                <div class="form-group">
                  <input type="radio" name="ty" value="0" checked>&nbsp&nbsp&nbsp&nbsp工学型论文
                  <input type="radio" name="ty" value="1" style="margin-left: 50px;">&nbsp&nbsp&nbsp&nbsp工程型论文
                </div>
              </div>

              <div class="col-lg-14">
                <div id="progresser" class="progress collapse" style="float:right;z-index: 100;height:33px;width:70%;">
                  <div id="progress" class="progress-bar progress-bar-striped active collapse" role="progressbar" aria-valuenow="45" aria-valuemin="0" aria-valuemax="100" style="width: 10%;"><p style="font-size: 14px;vertical-align: middle;margin-top:8px;">10%</p></div>
                </div>
                <!-- <span id="pingbi" title="加快检测速度" style="float:right;line-height: 35px;font-size: 15px;"><input type="checkbox" name="">&nbsp&nbsp 屏蔽论文中的代码部分</span> -->
                <button type="submit" class="btn btn-success" onclick="uploadfile();" style="float:left;">论文上传</button>
                <button class="btn btn-info" id="checkreport-btn" onclick="checkreport()" style="float:right;display: none;">查看检测报告</button>
              </div>
            </form>
          </div>
          <div id="explanation" class="well">
            <iframe src="details.html" frameborder="0" style="width:100%; height:100%;overflow: scroll;">
            </iframe>
          </div>
          <div style="width:40%;margin-left: 30px;float:left;margin-top:20px;height:188px;border:0px solid rgb(200,200,200);border-radius:4px;font-family: "Gill Sans Extrabold", Helvetica, sans-serif;">
            &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp本网站目前支持的浏览器(包括但不限于)：<br/>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp1.&nbsp&nbsp360安全浏览器8&nbsp&nbsp
            <br/>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp2.&nbsp&nbspIE11&nbsp&nbsp<br/>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp3.&nbsp&nbspChrome31&nbsp&nbsp<br/>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp4.&nbsp&nbspfirefox27.0.1&nbsp&nbsp<br/>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp5.&nbsp&nbspEdge&nbsp&nbsp
            <!-- <p>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp如使用以下(包括但不限于)浏览器，<b style="color:red;">本网站不保证完美的显示效果：<img src="images/360.png" width=15 height=15>360安全浏览器8(兼容模式)</b></p> -->
          </div>
        </div>
        <!--检测历史-->
        <div role="tabpanel" class="tab-pane" id="messages">
          <div id="history">
            <table class="table table-hover table-striped">
              <thead>
                <tr>
                  <th>上传时间</th>
                  <th>检测论文</th>
                  <th>检测报告</th>
                </tr>
              </thead>
              <tbody id="historytable">
              </tbody>
            </table>
          </div>
        </div>
        <!--检测报告-->
        <div role="tabpanel" class="tab-pane" id="report">
          <div  class="modal-dialog" style="width:100%;">
            <div class="modal-content" style="width: 80%; margin: auto;">
              <div class="modal-body" id="report-text">
                  <h3 style='text-align:center;'>请先上传学位论文或从检测历史中选择一项查看检测报告!</h3>
              </div>
            </div>
          </div>
        </div>
        <!--个人设置-->
        <div role="tabpanel" class="tab-pane" id="profile" style="height: auto;">
          <div id="ziliao">
            <div style="margin-top:15px;"><span class="glyphicon glyphicon-user" style="width:15px;clear:left;"></span><span style="line-height: 20px;">姓名</span><span id="st_name"></span></div>
            <br/>
            <div style="margin-top:15px;"><span style="width:15px;clear: left;"><img src="images/studentid.png" style="width:20px;height:20px;"></span><span style="line-height: 20px;">学号</span><span id="st_id"></span></div>
            <br/>
            <div style="margin-top:15px;"><span style="width:15px;clear:left;"><img src="images/class.png" style="width:20px;height:20px;"></span><span style="line-height: 20px;">年级</span><span id="st_class"></span></div>
            <br/>
            <div style="margin-top:15px;"><span style="width:15px;clear: left;"><img src="images/gender.png" style="width:20px;height:20px;"></span><span style="line-height: 20px;">性别</span><span id="st_sex"></span></div>
            <br/>
            <div style="margin-top:15px;"><div data-toggle="collapse" data-target="#collapseExample" aria-expanded="false" aria-controls="collapseExample"><span class="glyphicon glyphicon-cog" style="width:15px;clear: left;cursor: pointer;"></span><span style="float: left;line-height: 20px;cursor: pointer;">点击修改密码</span></div></div>
            <br/>
          </div>
          <div class="collapse" id="collapseExample" style="float: left;margin-top:20px;">
            <div class="well">
              <div style="clear:left;">
                <i style="color:red;float: left;">*&nbsp&nbsp</i>
                <span style="float: left;">初始密码</span>
                <input id="pwd1" type="text" onfocus="this.type='password'" autocomplete="off">
                <span id="check1" style="margin-left:10px;"></span>
              </div>
              <div>
                <i style="color:red;float: left;">*&nbsp&nbsp</i>
                <span style="float: left;">新密码</span>
                <input id="pwd2" type="text" onfocus="this.type='password'" autocomplete="off">
                <span id="check2" style="margin-left:10px;"></span>
              </div>
              <div>
                <i style="color:red;float: left;">*&nbsp&nbsp</i>
                <span style="float: left;">确认密码</span>
                <input id="pwd3" type="text" onfocus="this.type='password'" autocomplete="off">
                <span id="check3" style="margin-left:10px;"></span>
              </div>
              <input id="queren" type="button" class="btn btn-primary btn-block" style="width:80px;" value="确认">
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="foot">版权所有：大连理工大学软件学院红蚂蚁实验室<br>联系电话：18340861689&nbsp;&nbsp;邮箱：648088725@qq.com</div>
  </div>
</body>
<script type="text/javascript">

getNumber();
$(document).ready(function() {

  $('#collapseExample').collapse();
  $('li').click(function() {
    $('#collapseExample').collapse('hide');
    $('#check1,#check2,#check3').html('');
    $('#pwd1,#pwd2,#pwd3').css('border-color', 'rgb(200,200,200)');
    $('#pwd1,#pwd2,#pwd3').val('');
  });




  $('#checkreport-btn').click(function(){
  	$('#checkreport-btn').hide();
  	setTimeout(function(){$('#pingbi').removeClass('collapse');},1000);
  });




  $('a[data-toggle="tab"]').on('show.bs.tab', function(e) {
    var activeTab = $(e.target).text();// 激活的选项卡名称
    if(activeTab == "个人设置"){
      $("#container").css('min-height','590px');
    }else{
      $("#container").css('min-height','460px');
    }
  });



  
});
</script>
</html>
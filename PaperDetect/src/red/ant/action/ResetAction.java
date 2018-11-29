package red.ant.action;

import java.util.Arrays;
import java.util.Collections;
import java.util.List;
import java.util.Properties;
import javax.mail.Message;
import javax.mail.Session;
import javax.mail.Transport;
import javax.mail.internet.InternetAddress;
import javax.mail.internet.MimeMessage;
import javax.servlet.http.HttpServletRequest;
import org.apache.struts2.ServletActionContext;
import red.ant.service.StudentService;
import red.ant.service.TeacherService;
import red.ant.util.WebLogger;

import com.opensymphony.xwork2.ActionSupport;

/**
 * 自动发送邮件
 * @author 杨军军
 *
 */
public class ResetAction extends ActionSupport{

	private static final long serialVersionUID = 1L;

	private String username;//学号
	
	private TeacherService teacherService;
	private StudentService studentService;
	public String getUsername() {
		return username;
	}
	public void setUsername(String username) {
		this.username = username;
	}
	public TeacherService getTeacherService() {
		return teacherService;
	}
	public void setTeacherService(TeacherService teacherService) {
		this.teacherService = teacherService;
	}
	public StudentService getStudentService() {
		return studentService;
	}
	public void setStudentService(StudentService studentService) {
		this.studentService = studentService;
	}
	
	 public  void myexecute() throws Exception {
		 WebLogger webLogger=WebLogger.getLogger();
		 HttpServletRequest request=ServletActionContext.getRequest();
			String user_ip=getIpAddr(request);
		   //生成邮件对象      
		   Properties prop = new Properties();
		   prop.setProperty("mail.host", "smtp.sohu.com");
		   prop.setProperty("mail.transport.protocol", "smtp");
		   prop.setProperty("mail.smtp.auth", "true");
		   //使用JavaMail发送邮件的5个步骤
		   //1、创建session
		   Session session = Session.getInstance(prop);
		   //开启Session的debug模式，这样就可以查看到程序发送Email的运行状态
		    session.setDebug(true);
		   //2、通过session得到transport对象
		    Transport ts = session.getTransport();
		   //3、使用邮箱的用户名和密码连上邮件服务器，发送邮件时，发件人需要提交邮箱的用户名和密码给smtp服务器，用户名和密码都通过验证之后才能够正常发送邮件给收件人。
		    ts.connect("smtp.sohu.com","redAntC110","asdfasdf1234");
		    //4、创建邮件
		    Message message = createSimpleMail(session);
		     //5、发送邮件
		     ts.sendMessage(message, message.getAllRecipients());
		     ts.close();
		     webLogger.log_All(user_ip,username, "邮箱验证重置密码");
	}
	 
	public  MimeMessage createSimpleMail(Session session) throws Exception {
		
		
		//用户邮箱
		String userEmail="";
		String identity_number=get_identity();
		if(username.equals("21512345")==true)
		{
			//获取用户邮箱
			userEmail=teacherService.getEmail(username);
			//将随机生成的验证码存储进数据库
			teacherService.setIdentity(username, identity_number);
		}
		else
		{
			//获取用户邮箱
			userEmail=studentService.getEmail(username);
			//将随机生成的验证码存储进数据库
			studentService.setIdentity(username, identity_number);
		}
		//创建邮件对象
		MimeMessage message = new MimeMessage(session);
		//指明邮件的发件人
		message.setFrom(new InternetAddress("redAntC110@sohu.com"));
		//指明邮件的收件人，数据库获取用户邮箱
		message.setRecipient(Message.RecipientType.TO, new InternetAddress(userEmail));
		//邮件的标题
		message.setSubject("论文检测系统的密码重置");
		//邮件的文本内容
		//message.setContent("要使用新的密码，请将以下字符输入验证框内，完成重置密码的操作<br/>"+identity_number,"text/html;charset=UTF-8");
		message.setContent("要使用新的密码，请将以下字符输入验证框内，完成重置密码的操作<br/><br/>"+identity_number+"<br/><br/>"+"<a href=\"http://210.30.97.53:8090/PaperDetect/reset.jsp\">点击链接进入重置密码界面</a>","text/html;charset=UTF-8");
		//返回创建好的邮件对象
		return message;
	}
	/**
	 * 返回四位数字和字母的随机组合
	 * @return
	 */
	public String get_identity()
	{
	     String[] beforeShuffle = new String[] { "2", "3", "4", "5", "6", "7",  
	                "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",  
	                "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V",  
	                "W", "X", "Y", "Z" };  
	        List list = Arrays.asList(beforeShuffle);  
	        Collections.shuffle(list); //打乱原来的顺序 
	        StringBuilder sb = new StringBuilder();  
	        for (int i = 0; i < list.size(); i++) {  
	            sb.append(list.get(i));  
	        }  
	        String afterShuffle = sb.toString();  
	        String result = afterShuffle.substring(5, 9);  
	        return result;  

	}
	 public String getIpAddr(HttpServletRequest request) {     
	      String ip = request.getHeader("x-forwarded-for");     
	      if(ip == null || ip.length() == 0 || "unknown".equalsIgnoreCase(ip)) {     
	         ip = request.getHeader("Proxy-Client-IP");     
	     }     
	      if(ip == null || ip.length() == 0 || "unknown".equalsIgnoreCase(ip)) {     
	         ip = request.getHeader("WL-Proxy-Client-IP");     
	      }     
	     if(ip == null || ip.length() == 0 || "unknown".equalsIgnoreCase(ip)) {     
	          ip = request.getRemoteAddr();     
	     }     
	     return ip;     
	}    
}

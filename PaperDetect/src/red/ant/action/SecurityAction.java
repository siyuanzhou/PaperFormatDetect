package red.ant.action;

import java.net.URLDecoder;
import java.security.MessageDigest;
import javax.servlet.http.HttpSession;
import org.apache.struts2.ServletActionContext;
import red.ant.po.Student;
import red.ant.service.StudentService;
import sun.misc.BASE64Encoder;
import com.opensymphony.xwork2.ActionSupport;
/**
 * 密保
 * @author Administrator
 *
 */
public class SecurityAction extends ActionSupport {
	//设置密保的三个答案,不需要username属性
	private String answer1;
	private String answer2;
	private String answer3;
	
	//验证学号是否存在
	private String username;
	
	//验证密保的三个答案,需要输入username属性
	private String new_answer1;
	private String new_answer2;
	private String new_answer3;
	
	//重置密码的密码
	//private String password;
	
	private StudentService studentService;

	public String getAnswer1() {
		return answer1;
	}

	public void setAnswer1(String answer1) {
		this.answer1 = answer1;
	}

	public String getAnswer2() {
		return answer2;
	}

	public void setAnswer2(String answer2) {
		this.answer2 = answer2;
	}

	public String getAnswer3() {
		return answer3;
	}

	public void setAnswer3(String answer3) {
		this.answer3 = answer3;
	}

	
	public String getUsername() {
		return username;
	}

	public void setUsername(String username) {
		this.username = username;
	}

	public String getNew_answer1() {
		return new_answer1;
	}

	public void setNew_answer1(String new_answer1) {
		this.new_answer1 = new_answer1;
	}

	public String getNew_answer2() {
		return new_answer2;
	}

	public void setNew_answer2(String new_answer2) {
		this.new_answer2 = new_answer2;
	}

	public String getNew_answer3() {
		return new_answer3;
	}

	public void setNew_answer3(String new_answer3) {
		this.new_answer3 = new_answer3;
	}

	public StudentService getStudentService() {
		return studentService;
	}

	public void setStudentService(StudentService studentService) {
		this.studentService = studentService;
	}
	/**
	 * 注册之后跳转到设置密保界面
	 * @throws Exception
	 */
	public void addexecute() throws Exception
	{
		answer1=URLDecoder.decode(answer1, "utf-8");
		answer2=URLDecoder.decode(answer2, "utf-8");
		answer3=URLDecoder.decode(answer3, "utf-8");
		HttpSession session=ServletActionContext.getRequest().getSession();
		String user=(String) session.getAttribute("username");
		studentService.setSecurity(user, answer1, answer2, answer3);
		//密保问题设置成功
		ServletActionContext.getResponse().getWriter().println("1");
	}
	/**
	 * 点击忘记密码之后进行密保问题验证
	 * @throws Exception
	 */
	public void forgetexecute() throws Exception
	{
		new_answer1=URLDecoder.decode(new_answer1, "utf-8");
		new_answer2=URLDecoder.decode(new_answer2, "utf-8");
		new_answer3=URLDecoder.decode(new_answer3, "utf-8");
		HttpSession session=ServletActionContext.getRequest().getSession();
		String username=(String) session.getAttribute("security");
			Student student=studentService.getAll(username);;
			int flag=0;
			if(new_answer1.equals(student.getAnswer1())==true)
			{
				flag++;
			}
			if(new_answer2.equals(student.getAnswer2())==true)
			{
				flag++;
			}

			if(new_answer3.equals(student.getAnswer3())==true)
			{
				flag++;
			}
			System.out.println("flag:"+flag);
			//split(" ");---得到数组
			//data[0]--answer1是(1)否(0)正确，。。。
			if(flag>=2)
			{
				String passwordMD5="";
				try
				{
					MessageDigest md5=MessageDigest.getInstance("MD5");
					BASE64Encoder base64en = new BASE64Encoder();
					//加密后的字符串
					passwordMD5=base64en.encode(md5.digest("123456".getBytes("utf-8")));
				}
				catch(Exception e)
		 		{
		 			 e.printStackTrace();
		 		}
				studentService.reset(username,passwordMD5);
				ServletActionContext.getResponse().getWriter().println("1");
			}
			else
			{
				ServletActionContext.getResponse().getWriter().println("0");
			}
	}
	/**
	 * 验证用户账号是否存在
	 * @throws Exception
	 */
	public void IsExistexecute() throws Exception
	{
		if(studentService.IsHas(username).equals("1")==true || username.equals("21512345")==true)
		{
			//验证用户是否存在
			HttpSession session=ServletActionContext.getRequest().getSession();
			session.setAttribute("security", username);
			ServletActionContext.getResponse().getWriter().println("1");
		}
		else
			ServletActionContext.getResponse().getWriter().println("0");
	}
}

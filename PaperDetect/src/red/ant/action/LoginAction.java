package red.ant.action;
import java.security.MessageDigest;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;
import org.apache.struts2.ServletActionContext;
import com.opensymphony.xwork2.ActionSupport;
import red.ant.service.StudentService;
import red.ant.service.TeacherService;
import red.ant.util.WebLogger;
import sun.misc.BASE64Encoder;
/**
 * �û���¼
 * @author �����
 *
 */
public class LoginAction extends ActionSupport{
	//�������ڷ�װ�û��������������
	private String username=ServletActionContext.getRequest().getParameter("username");
	private String password=ServletActionContext.getRequest().getParameter("password");
	private StudentService studentService;
	private TeacherService teacherService;
	
	public StudentService getStudentService() {
		return studentService;
	}
	public void setStudentService(StudentService studentService) {
		this.studentService = studentService;
	}
	
	public TeacherService getTeacherService() {
		return teacherService;
	}
	public void setTeacherService(TeacherService teacherService) {
		this.teacherService = teacherService;
	}
	public String getUsername() {
		return username;
	}
	public void setUsername(String username) {
		this.username = username;
	}	
	public String getPassword() {
		return password;
	}
	public void setPassword(String password) {
		this.password = password;
	}
	public void myexecute() throws Exception {
		//System.out.println("--------------");
		
		HttpSession session=ServletActionContext.getRequest().getSession();
		WebLogger webLogger = WebLogger.getLogger();
		HttpServletRequest request=ServletActionContext.getRequest();
		String user_ip=getIpAddr(request);
		//�˺Ų�������ѧ�������Ҳ�������ʦ�˺�
		System.out.println(username+"   "+password);
		if(studentService.IsHas(username).equals("0")==true && username.equals("21512345")==false)
		{
			//�˺ţ��û�������
			ServletActionContext.getResponse().getWriter().println("0");
		}	
		else
		{
			System.out.println("ѧ�ţ�"+username+"���룺"+password);
			String passwordMD5="";
			try
			{
				MessageDigest md5=MessageDigest.getInstance("MD5");//MD5����
				BASE64Encoder base64en = new BASE64Encoder();//������ת����ת��Ϊ���Դ���ģ�������ʾ��
				//���ܺ���ַ���
				passwordMD5=base64en.encode(md5.digest(password.getBytes("utf-8")));
			}
			catch(Exception e)
	 		{
	 			 e.printStackTrace();
	 		}
			String userId_teacher=teacherService.login(username, passwordMD5);
			String userId_student=studentService.login(username, passwordMD5);
			if(userId_student==null && userId_teacher==null)
			{
				//�������
				webLogger.log_LoginFailure(user_ip, username);
				ServletActionContext.getResponse().getWriter().println("2");
			}
			if(userId_student!=null)
			{
				session.setAttribute("ip", user_ip);
				webLogger.log_Login(user_ip, username, "student");
				session.setAttribute("username", getUsername());
				session.setAttribute("name", studentService.getName(getUsername()));
				ServletActionContext.getResponse().getWriter().println("1");
			}
			else if(userId_teacher!=null)
			{
				session.setAttribute("ip", user_ip);
				webLogger.log_Login(user_ip, username, "teacher");
				session.setAttribute("username", getUsername());
				ServletActionContext.getResponse().getWriter().println("-1");
			}
		}
	}
	/**
	 * ��ȡ��ʵip��ַ
	 * @param request
	 * @return
	 */
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
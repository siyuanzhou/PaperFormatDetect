package red.ant.action;

import java.net.URLDecoder;
import javax.servlet.http.HttpSession;
import org.apache.struts2.ServletActionContext;
import red.ant.service.AdviceService;
import red.ant.service.StudentService;
import com.opensymphony.xwork2.ActionSupport;
/**
 * 反馈意见（学生界面）
 * @author Administrator
 *
 */
public class AdviceAction extends ActionSupport{

	private String advice;//反馈意见
	private AdviceService adviceService;
	private StudentService studentService;
	public String getAdvice() {
		return advice;
	}
	public void setAdvice(String advice) {
		this.advice = advice;
	}
	public AdviceService getAdviceService() {
		return adviceService;
	}
	public void setAdviceService(AdviceService adviceService) {
		this.adviceService = adviceService;
	}
	
	public StudentService getStudentService() {
		return studentService;
	}
	public void setStudentService(StudentService studentService) {
		this.studentService = studentService;
	}
	public void myexecute() throws Exception
	{
		advice=URLDecoder.decode(advice,"utf-8");
		HttpSession session=ServletActionContext.getRequest().getSession();
		String sid=(String)session.getAttribute("username");
		String name=(String)session.getAttribute("name");
		String email=studentService.getEmail(sid);
		adviceService.save( sid, email, advice,name);
		ServletActionContext.getResponse().getWriter().println("1");	
	}
}

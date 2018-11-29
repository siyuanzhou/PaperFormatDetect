package red.ant.action;


import javax.servlet.http.HttpSession;

import org.apache.struts2.ServletActionContext;

import red.ant.util.WebLogger;

import com.opensymphony.xwork2.Action;

/**
 * ×¢Ïú
 * @author Administrator
 *
 */
public class LogoutAction implements Action {
public String execute() throws Exception {
	WebLogger webLogger=WebLogger.getLogger();
	HttpSession session=ServletActionContext.getRequest().getSession();
	String user_ip=(String)session.getAttribute("ip");
	String user_name=(String)session.getAttribute("username");
	webLogger.log_All(user_ip, user_name, "×¢Ïú");
	session.invalidate();
	return "success";
}

}


//过滤器，实现除注册界面外全部过滤
package red.ant.util;

import java.io.IOException;

import javax.servlet.Filter;
import javax.servlet.FilterChain;
import javax.servlet.FilterConfig;
import javax.servlet.ServletException;
import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

public class RefowardAuthorityFilter implements Filter {
private FilterConfig config;
	public void destroy() {
		this.config=null;

	}
	public void doFilter(ServletRequest request, ServletResponse response,
			FilterChain chain) throws IOException, ServletException {
	  HttpServletRequest requ=(HttpServletRequest) request;
	  //HttpServletResponse respon=(HttpServletResponse) response;
	  HttpSession session=requ.getSession();
	  //String requestpath=requ.getServletPath();
	  String loginpage=config.getInitParameter("loginpage");
	  String url=requ.getServletPath();
	  System.out.println("url="+url);
	  System.out.println("username="+session.getAttribute("username"));
	  //已有学生账号且请求登录页面
	  if(session.getAttribute("username")!=null  && url.equals("/login.jsp")==true && session.getAttribute("username").equals("201612345")==false){
		  System.out.println("账号为学生，请求登录界面");
		 //requ.setAttribute("tip", "你还没有登录！");
		  request.getRequestDispatcher("/students.jsp").forward(request, response);
	  }
	  else if(session.getAttribute("username")!=null  && url.equals("/login.jsp")==true && session.getAttribute("username").equals("201612345")==true){
		  System.out.println("账号为老师，请求登录界面");
		 //requ.setAttribute("tip", "你还没有登录！");
		  request.getRequestDispatcher("/teacher.jsp").forward(request, response);
	  }
	  else 
	  { 
		  chain.doFilter(request, response);
	  }
		  /*********************************
		  System.out.println("-------------");
		
		  if(session.getAttribute("username")==null  && url.equals("/register.jsp")==true && url.equals("/security_01.jsp")==true)
		  {
			  System.out.println("--------注册或忘记密码--------");
			  chain.doFilter(request, response);
		  }
		//学生登录访问老师账号
		   if(session.getAttribute("username")!=null && session.getAttribute("username").equals("201612345")==false &&url.equals("/teacher.jsp")==true )
		  {
			   System.out.println("---学生越权---------");
			  request.getRequestDispatcher("/login.jsp").forward(request, response);
			  //request.getRequestDispatcher("/students.jsp").forward(request, response);
		  }
		   //老师登录访问学生账号
		   else if(session.getAttribute("username").equals("201612345")==true && url.equals("/students.jsp")==true)
		  {
			   System.out.println("---老师越权---------");
			  request.getRequestDispatcher("/login.jsp").forward(request, response);
			  //request.getRequestDispatcher("/teacher.jsp").forward(request, response);
		  }
		   else
		   {
			   System.out.println("---下一条链---------");
			   chain.doFilter(request, response);
		   }
		  
	  }
	  ******************************/
	  
	}

	public void init(FilterConfig arg0) throws ServletException {
		// System.out.println("过滤初始化！！！！！！！！！！！");
		this.config=arg0;

	}
}

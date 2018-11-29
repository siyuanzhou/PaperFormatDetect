package red.ant.action;

import java.io.File;
import java.io.FileInputStream;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import javax.servlet.http.HttpSession;
import org.apache.struts2.ServletActionContext;
import red.ant.util.WebLogger;
import com.opensymphony.xwork2.ActionSupport;
/**
 * 下载检测论文及报告（老师和学生页面）
 * @author Administrator
 *
 */
public class DownloadAction extends ActionSupport{
	private String paper_name=ServletActionContext.getRequest().getParameter("paper_name");
	private String fileName;
	
	public String getPaper_name() {
		return paper_name;
	}

	public void setPaper_name(String paper_name) {
		this.paper_name = paper_name;
	}
	
	 @SuppressWarnings("deprecation")
	public String getFileName() throws UnsupportedEncodingException {
	    System.out.println("get--"+paper_name);
		 String str3=paper_name.substring(0, 6);
		 if(str3.equals("report")==true)
		 {
			 String str2=paper_name.substring(6, paper_name.length()-5);
			 String dir="D:\\PaperFormatDetection\\Reports\\";
			 String f_name=str2+".pdf";
			 String rPath=dir+f_name;
			 File file = new File(rPath); 
			 if(!file.exists()){
				 f_name.replaceAll(".pdf", ".txt");
			 }
				//return str2+"_report.txt";
			 	String new_str2=str2+"_report.txt";
				 return  new String(f_name.getBytes(), "ISO8859-1");
		 }
		 else
		 {
			 return new String(paper_name.getBytes(), "ISO8859-1");
		 }
		
	}

	public void setFileName(String fileName) {
		this.fileName = fileName;
	}

	//返回一个输入流，作为一个客户端来说是一个输入流，但对于服务器端是一个 输出流  
	public InputStream getDownloadFile() throws Exception  
    {   
		WebLogger webLogger=WebLogger.getLogger();
		HttpSession session=ServletActionContext.getRequest().getSession();
		String user_ip=(String)session.getAttribute("ip");
		String user_name=(String)session.getAttribute("username");
		
    	paper_name=URLDecoder.decode(paper_name,"utf-8");
    	 String str4=paper_name.substring(0, 6);
		 if(str4.equals("report")==true)//report
		 {
			 String str=paper_name.substring(6, paper_name.length()-5);
			 //文件名称
			 String user_str=str+"_report.txt";
			 webLogger.log_All(user_ip, user_name, "下载",user_str);
			 String realPath = null;;  
			 realPath=("D:\\PaperFormatDetection\\Reports\\"+paper_name.substring(6)).replaceAll(".docx",".pdf").replaceAll(".doc",".pdf");

			 File file = new File(realPath); 
			 if(!file.exists()){
				 realPath=realPath.replaceAll(".pdf", ".txt");
			 }
			 System.out.println("4444444444    "+realPath);
        	 return new FileInputStream(realPath);//绝对路径 
		 }
		 else
		 {
			 File file=new File(".");
			 webLogger.log_All(user_ip, user_name, "下载",paper_name);
			 return new FileInputStream(file.getCanonicalFile().getParent()+"\\paperFolder"+"\\"+paper_name);
		 }
    }  
      
    @Override  
    public String execute() throws Exception {    
        return SUCCESS;  
    }  
}

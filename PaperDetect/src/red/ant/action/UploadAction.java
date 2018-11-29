package red.ant.action;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.SequenceInputStream;
import java.text.SimpleDateFormat;
import java.util.Date;
import javax.servlet.http.HttpSession;
import org.apache.commons.io.IOUtils;
import org.apache.struts2.ServletActionContext;
import red.ant.service.PaperService;
import red.ant.util.WebLogger;
import com.opensymphony.xwork2.ActionSupport;

/**
 * 上传论文
 * 
 * @author Administrator
 * 
 */
public class UploadAction extends ActionSupport {

	private File upload;// 上传文件
	private String uploadContentType;// 封装文件类型
	private String uploadFileName;// 文件名
	private String savePath;// 保存路径
	private String temp = ServletActionContext.getRequest()
			.getParameter("temp");// 是否屏蔽代码（1-屏蔽）
	private String flag = ServletActionContext.getRequest()
			.getParameter("flag");// 硕士类型
	private PaperService paperService;
	// 获取系统唯一当前时间
	private final SimpleDateFormat df = new SimpleDateFormat(
			"yyyy-MM-dd-HH-mm-ss");// 设置日期格式
	private final String paper_time = df.format(new Date());// new
															// Date()为获取当前系统时间

	// 无全局变量？前台的每次ajax请求都创建一个新的实例；
	public File getUpload() {
		return upload;
	}

	public void setUpload(File upload) {
		this.upload = upload;
	}

	public String getUploadContentType() {
		return uploadContentType;
	}

	public void setUploadContentType(String uploadContentType) {
		this.uploadContentType = uploadContentType;
	}

	public String getUploadFileName() {
		uploadFileName = uploadFileName.replace(" ", "-");
		int i, j;
		// 文件名称只能是.docx
		// 存储后缀名
		String[] data = uploadFileName.split(".");
		String str = "";
		// 存储文件名（除去后缀）
		String str2 = "";
		str = ".docx";
		str2 = uploadFileName.substring(0, uploadFileName.length() - 5);
		if(uploadFileName.endsWith(".doc")){
			str=".doc";
			str2 = uploadFileName.substring(0, uploadFileName.length() - 4);
		}
		HttpSession session = ServletActionContext.getRequest().getSession();
		String username = (String) session.getAttribute("username");
		String name = (String) session.getAttribute("name");
		// 文件名称不能有冒号（:），否则无法存储文件
		return name + "_" + username + "_" + str2 + "_" + paper_time + str;
	}

	public void setUploadFileName(String uploadFileName) {
		this.uploadFileName = uploadFileName;
	}

	public String getSavePath() throws IOException {
		// 将相对路径转换成绝对路径
		File file = new File(".");
		//return file.getCanonicalFile().getParent() + "\\paperFolder";
		return "D:\\PaperFormatDetection\\Papers\\";
	}

	public void setSavePath(String savePath) {
		this.savePath = savePath;
	}

	public String getTemp() {
		return temp;
	}

	public void setTemp(String temp) {
		this.temp = temp;
	}

	public String getFlag() {
		return flag;
	}

	public void setFlag(String flag) {
		this.flag = flag;
	}

	public PaperService getPaperService() {
		return paperService;
	}

	public void setPaperService(PaperService paperService) {
		this.paperService = paperService;
	}

	// 上传action
	public String execute() throws IOException {
		// 绝对路径
		WebLogger webLogger = WebLogger.getLogger();
		webLogger.log_All("开始", "上传",
				"系统当前时间：" + (Long) System.currentTimeMillis());
		HttpSession session = ServletActionContext.getRequest().getSession();
		String user_ip = (String) session.getAttribute("ip");
		String relativePath = getSavePath() + "\\" + getUploadFileName();
		String sid = (String) session.getAttribute("username");// 当前用户学号
		String sname = (String) session.getAttribute("name");
		// 获取文件名称的后缀
		String str = uploadFileName.substring(uploadFileName.length() - 4,
				uploadFileName.length());
		if(uploadFileName.endsWith("docx"))
			str="docx";
		else if(uploadFileName.endsWith("doc"))
			str="doc";

		if (upload != null && sid != null && (str.equals("docx") || str.equals("doc"))) {
			InputStream is = null;
			try {
				is = new FileInputStream(getUpload());
			} catch (FileNotFoundException e) {
				// TODO Auto-generated catch block
				webLogger.log_All("文件", "上传", "异常");
				e.printStackTrace();
				return null;// 文件为空直接结束
			}
			File file = new File(getSavePath());
			if (!file.exists() && !file.isDirectory()) {
				file.mkdir();
			}
			OutputStream os = new FileOutputStream(getSavePath() + "\\"
					+ getUploadFileName());
			// 将InputStream里的byte拷贝到OutputStream
			IOUtils.copy(is, os);
			os.flush();
			webLogger.log_All("结束", "上传",
					"系统当前时间：" + (Long) System.currentTimeMillis());
			IOUtils.closeQuietly(is);
			IOUtils.closeQuietly(os);
			//upload.delete();
			// ORA-12899: 列 "PAPERUSER2016"."PAPER"."PAPER_ID" 的值太大 (实际值: 106,
			// 最大值: 100)
			// 数据库中varchar(100)，字符占一个字节，汉字占2个字节（GBK），汉字占3个字节（UTF-8）,我们的编码为utf-8
			try {
				myexec();
			} catch (Exception e) {
				// TODO Auto-generated catch block
				webLogger.log_All(user_ip, sid, "单机版调用出现异常:"
						+ getUploadFileName());
				e.printStackTrace();
			}

			// 除去.docx的文件名
			String reportStr = getUploadFileName().substring(0,
					getUploadFileName().length() - 5);
			if(uploadFileName.endsWith(".docx"))
				reportStr= getUploadFileName().substring(0,
						getUploadFileName().length() - 5);
			else if(uploadFileName.endsWith(".doc"))
				reportStr= getUploadFileName().substring(0,
						getUploadFileName().length() - 4);
			//File tempFile = new File("Papers");
			String realPath = (getSavePath() + getUploadFileName()).replaceAll("Papers", "Reports");
			File reportFile = new File(realPath);
			if (reportFile.exists())// 如果生成报告
			{
				paperService
						.save(relativePath, sid, sname, getUploadFileName());
				webLogger.log_All(user_ip, sid, "检测报告生成成功:"
						+ getUploadFileName());
				session.setAttribute("timeFlag", "true");
			} else {
				webLogger.log_All(user_ip, sid, "未生成报告:" + getUploadFileName());
				session.setAttribute("timeFlag", "false");
			}
			return SUCCESS;
		} else
			return null;
	}

	public String myexec() throws Exception {
		HttpSession session = ServletActionContext.getRequest().getSession();
		WebLogger weblogger = WebLogger.getLogger();
		String user_ip = (String) session.getAttribute("ip");
		String user_name = (String) session.getAttribute("username");
		// 待检测论文路径
		String testPath = getSavePath() + getUploadFileName();
		String FileName = getUploadFileName();
		try{
			String s;
			String evm="D:\\PaperFormatDetection\\PaperFormatDetection\\PaperFormatDetection\\PaperFormatDetection\\bin\\Debug";
			String commond=evm+"\\PaperFormatDetection.exe  "+testPath+"  "+session.getAttribute("flag");
			Process process = Runtime.getRuntime().exec(commond);
			SequenceInputStream sis = new SequenceInputStream(
					process.getInputStream(), process.getErrorStream());
			InputStreamReader isr = new InputStreamReader(sis, "gbk");
			BufferedReader br = new BufferedReader(isr);
			while((s=br.readLine()) != null)
				System.out.println(s);
		    process.waitFor();
		}catch(Exception e){
			System.out.print(e.getMessage());
		}

		// 应用程序调用完成后销毁专属模板
		//tempfile.delete();
		return "success";
	}

	public void myexecute() throws Exception {
		WebLogger weblogger = WebLogger.getLogger();
		HttpSession session = ServletActionContext.getRequest().getSession();
		if (session.getAttribute("timeFlag") != null
				&& session.getAttribute("timeFlag").equals("true")) {
			ServletActionContext.getResponse().getWriter().println("1");
			session.removeAttribute("timeFlag");
		} else if (session.getAttribute("timeFlag") != null
				&& session.getAttribute("timeFlag").equals("false")) {
			ServletActionContext.getResponse().getWriter().println("2");
			session.removeAttribute("timeFlag");
		} else {
			ServletActionContext.getResponse().getWriter().println("0");
		}

	}

	public void myexecute2() throws Exception {
		HttpSession session = ServletActionContext.getRequest().getSession();
		session.setAttribute("temp", temp);
		session.setAttribute("flag", flag);
	}

	// 页面计数器
	public void sumPaper() throws IOException {
		int sum = paperService.Sum();
		ServletActionContext.getResponse().getWriter().println(sum);
	}
}

class TimeOut extends Thread {
	Process process;

	TimeOut(Process process) {
		this.process = process;
	}

	public void run() {
		try {
			// 安全考虑，延时18秒后再销毁线程
			Thread.currentThread().sleep(18000);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		// 如果此时线程已经结束，销毁函数未产生异常
		process.destroy();
	}
}
class TimeOut1 extends Thread {
	Process process;

	TimeOut1(Process process) {
		this.process = process;
	}

	public void run() {
		try {
			// 安全考虑，延时18秒后再销毁线程
			Thread.currentThread().sleep(4000);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		// 如果此时线程已经结束，销毁函数未产生异常
		process.destroy();
	}
}
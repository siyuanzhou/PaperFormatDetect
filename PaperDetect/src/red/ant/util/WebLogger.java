package red.ant.util;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;

public class WebLogger {
	private volatile static WebLogger webLogger = null;
	private static Logger logger = null;
	
	private WebLogger(){
		logger=Logger.getLogger(WebLogger.class.getName()); 
		
        // 获得当前目录路径
        String filePath = this.getClass().getResource("/").getPath();
        // 找到log4j.properties配置文件所在的目录(已经创建好)
        filePath = filePath.substring(1).replace("bin", "src");
        PropertyConfigurator.configure(filePath + "log4j.properties");
        
		//PropertyConfigurator.configure("loginlog4j.properties");
	}
	
	public static WebLogger getLogger(){
		if(webLogger == null){
			synchronized(WebLogger.class){
				if(webLogger == null){
					webLogger = new WebLogger();
				}
			}
		}
		return webLogger;
	}
	
	public void log_Login(String ip,String account,String 操作类别){
		logger.error(ip  + "  "+account + "  登录  " + "  " + 操作类别 );
	}
	
	public void log_LoginFailure(String ip,String account){
		logger.error(ip +"  "+ account + "  登录  "  + "  " + "登录失败 " );
	}
	
	public void log_All(String ip,String account, String ...strings){
		String s = ip + "  " + account + "  ";
		for(String a:strings){
			s = s + a + "  ";
		}
		logger.error(s);
	}
	
}

package red.ant.action;

import red.ant.service.PaperService;
import red.ant.util.WebLogger;
import com.opensymphony.xwork2.ActionSupport;
/**
 * 删除报告及论文（老师界面）
 * @author 杨军军
 *
 */
public class PaperDeleteAction extends ActionSupport{
	private String pid;
	private PaperService paperService;
	
	public String getPid() {
		return pid;
	}

	public void setPid(String pid) {
		this.pid = pid;
	}

	public PaperService getPaperService() {
		return paperService;
	}

	public void setPaperService(PaperService paperService) {
		this.paperService = paperService;
	}

	public String execute() throws Exception {
		// TODO Auto-generated method stub
		WebLogger webLogger=WebLogger.getLogger();
		webLogger.log_All("删除论文", "编号为", pid);
		paperService.delete(pid);
		return SUCCESS;
	}

}

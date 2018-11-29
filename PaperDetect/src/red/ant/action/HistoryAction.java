package red.ant.action;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpSession;

import org.apache.struts2.ServletActionContext;

import red.ant.po.Paper;
import red.ant.service.PaperService;

import com.opensymphony.xwork2.Action;
import com.opensymphony.xwork2.ActionContext;
/**
 * 查看检测历史（学生界面）
 * @author 杨军军
 *
 */
public class HistoryAction implements Action{

	private String historyList;
	private PaperService paperService;
	
	public String getHistoryList() {
		return historyList;
	}

	public void setHistoryList(String historyList) {
		this.historyList = historyList;
	}

	public PaperService getPaperService() {
		return paperService;
	}

	public void setPaperService(PaperService paperService) {
		this.paperService = paperService;
	}

	public String execute() throws Exception {
		// TODO Auto-generated method stub
		HttpSession session=ServletActionContext.getRequest().getSession();
		String username=(String)session.getAttribute("username");//获取用户学号
		List<Paper> list=paperService.findByUsername(username);
		historyList="{\"historyList\":"+red.ant.util.JsonUtil.listToJson(list)+"}";
		System.out.println("!!!!!!!!"+historyList+"!!!!!!!!");
		return SUCCESS;
	}

}

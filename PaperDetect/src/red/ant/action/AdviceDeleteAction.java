package red.ant.action;
import red.ant.service.AdviceService;
import com.opensymphony.xwork2.ActionSupport;
/**
 * 删除反馈意见（老师界面）
 * @author Administrator
 *
 */
public class AdviceDeleteAction extends ActionSupport{

	private String aid;//编号数组
	private AdviceService adviceService;

	
	public String getAid() {
		return aid;
	}
	public void setAid(String aid) {
		this.aid = aid;
	}
	public AdviceService getAdviceService() {
		return adviceService;
	}
	public void setAdviceService(AdviceService adviceService) {
		this.adviceService = adviceService;
	}
	public String execute() throws Exception
	{
		adviceService.delete(aid);
		return SUCCESS;
	}
}

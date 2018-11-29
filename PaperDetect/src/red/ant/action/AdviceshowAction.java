package red.ant.action;

import java.util.List;
import net.sf.json.JSONArray;
import net.sf.json.JSONObject;
import red.ant.po.Advice;
import red.ant.service.AdviceService;
import com.opensymphony.xwork2.ActionSupport;
/**
 * 展示反馈意见（老师界面）
 * @author Administrator
 *
 */
public class AdviceshowAction extends ActionSupport{

	private JSONObject adviceList;
	private AdviceService adviceService;

	
	public JSONObject getAdviceList() {
		return adviceList;
	}
	public void setAdviceList(JSONObject adviceList) {
		this.adviceList = adviceList;
	}
	public AdviceService getAdviceService() {
		return adviceService;
	}
	public void setAdviceService(AdviceService adviceService) {
		this.adviceService = adviceService;
	}
	public String execute() throws Exception
	{
		List<Advice> list=adviceService.show();
		JSONArray jsonArray=JSONArray.fromObject(list);
		int num=list.size();
		adviceList=new JSONObject();
		adviceList.element("adviceList", jsonArray);
		adviceList.put("total", num);
		return SUCCESS;
		
	}
}

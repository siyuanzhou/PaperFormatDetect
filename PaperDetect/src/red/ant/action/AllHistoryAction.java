package red.ant.action;
import java.util.List;

import red.ant.po.Paper;
import red.ant.service.PaperService;
import com.opensymphony.xwork2.Action;
import net.sf.json.*;
/**
 * 查看检测历史（老师界面）
 * @author 杨军军
 *
 */
public class AllHistoryAction implements Action{
	private JSONObject jsonList;
	private PaperService paperService;
	
	
	public JSONObject getJsonList() {
		return jsonList;
	}

	public void setJsonList(JSONObject jsonList) {
		this.jsonList = jsonList;
	}

	public PaperService getPaperService() {
		return paperService;
	}

	public void setPaperService(PaperService paperService) {
		this.paperService = paperService;
	}
	public String execute() throws Exception {
		// TODO Auto-generated method stub
		List<Paper> list=paperService.show();
		JSONArray jsonArray = JSONArray.fromObject(list);
		jsonList = new JSONObject();//以下为默认返回
		int num=list.size();
		jsonList.element("historyList", jsonArray);
		jsonList.put("total",num);
		return SUCCESS;
	}

}

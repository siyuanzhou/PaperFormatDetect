package red.ant.action;

import java.net.URLDecoder;
import java.util.List;
import net.sf.json.JSONArray;
import net.sf.json.JSONObject;
import red.ant.po.Advice;
import red.ant.service.AdviceService;
import com.opensymphony.xwork2.ActionSupport;
/**
 * 查找反馈意见（老师界面）
 * @author Administrator
 *
 */
public class AdviceshowFindAction extends ActionSupport{

	private String findType;//查找类别（姓名，学号）
	private String findValue;//查找值
	private JSONObject adviceList;
	private AdviceService adviceService;

	
	public String getFindType() {
		return findType;
	}
	public void setFindType(String findType) {
		this.findType = findType;
	}
	public String getFindValue() {
		return findValue;
	}
	public void setFindValue(String findValue) {
		this.findValue = findValue;
	}
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
		findValue=URLDecoder.decode(findValue,"utf-8");
		List<Advice> list;
		if(findType.equals("0"))
		{
			findValue=URLDecoder.decode(findValue,"utf-8");
			list=adviceService.findByName(findValue);
		}
		else
		{
			list=adviceService.findByUsername(findValue);
		}
		JSONArray jsonArray=JSONArray.fromObject(list);
		int num=list.size();
		adviceList=new JSONObject();
		adviceList.element("adviceList", jsonArray);
		adviceList.put("total", num);
		return SUCCESS;
		
	}
}

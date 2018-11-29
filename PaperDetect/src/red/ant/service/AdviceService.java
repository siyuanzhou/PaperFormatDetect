package red.ant.service;

import java.util.List;

import red.ant.po.Advice;

public interface AdviceService {

	/**
	 * 保存反馈意见
	 * @param aid
	 * @param sid
	 * @param email
	 * @param advice
	 * @return
	 */
	public String save(String sid,String email,String advice,String name);
	public List<Advice> show();
	public List<Advice> findByName(String name);
	public List<Advice> findByUsername(String username);
	public void delete(String aid);
}

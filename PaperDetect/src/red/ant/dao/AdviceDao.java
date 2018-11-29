package red.ant.dao;


import java.util.List;

import red.ant.po.Advice;

public interface AdviceDao 
{
	public Integer save(Advice advice);
	public void update(Advice advice);
	public Advice get(Integer aid);
	public List<Advice> findAll();
	/**
	 * 教师页面查找功能
	 * @param name
	 * @return
	 */
	public List<Advice> findByName(String name);
	public List<Advice> findByUsername(String username);
	/**
	 * 删除advice实例
	 * @param advice
	 */
	public void delete(Advice advice);
}
package red.ant.dao.impl;


import java.util.List;

import org.springframework.orm.hibernate3.support.HibernateDaoSupport;

import red.ant.dao.AdviceDao;
import red.ant.po.Advice;

public class AdviceDaoHibernate extends HibernateDaoSupport implements AdviceDao
{
	public Integer save(Advice advice)
	{
		return (Integer)getHibernateTemplate().save(advice);
	}
	public void update(Advice advice)
	{
		getHibernateTemplate().update(advice);
	}
	public Advice get(Integer aid)
	{
		return (Advice)getHibernateTemplate().get(Advice.class, aid);
	}
	public List<Advice> findAll() {
		// TODO Auto-generated method stub
		List<Advice> list=getHibernateTemplate().find("from Advice");
		if(list!=null && list.size()!=0)
    		return list;
    	else
    		return null;
	}
	public List<Advice> findByName(String name) {
		// TODO Auto-generated method stub
		List<Advice> list=getHibernateTemplate().find("from Advice model where model.name=?",name);
		if(list!=null && list.size()!=0)
    		return list;
    	else
    		return null;
	}
	public List<Advice> findByUsername(String username) {
		// TODO Auto-generated method stub
		List<Advice> list=getHibernateTemplate().find("from Advice model where model.sid=?",username);
		if(list!=null && list.size()!=0)
    		return list;
    	else
    		return null;
	}
	public void delete(Advice advice) {
		// TODO Auto-generated method stub
		getHibernateTemplate().delete(advice);
	}
}

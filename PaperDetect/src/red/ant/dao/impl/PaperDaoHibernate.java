package red.ant.dao.impl;

import java.util.List;

import org.springframework.orm.hibernate3.support.HibernateDaoSupport;

import red.ant.dao.PaperDao;
import red.ant.po.Paper;

public class PaperDaoHibernate extends HibernateDaoSupport implements PaperDao {


	public Paper get(String pid) {
		// TODO Auto-generated method stub
		return (Paper)getHibernateTemplate().get(Paper.class, pid);
	}
	
	public String save(Paper paper) {
		// TODO Auto-generated method stub
		return (String)getHibernateTemplate().save(paper);
	}
	public void update(Paper paper) {
		// TODO Auto-generated method stub
		getHibernateTemplate().update(paper);
	}

	public List<Paper> findAll() {
		// TODO Auto-generated method stub
		List<Paper> list=getHibernateTemplate().find("from Paper model order by model.ptime desc");
		if(list!=null && list.size()!=0)
			return list;
		else
			return null;
	}
	public List<Paper> findByUsername(String sid)
	{	//pid排序无效，ptime排序有效，奇怪、、、
		List<Paper> list=getHibernateTemplate().find("from Paper model where model.sid=? order by model.ptime desc",sid);
		if(list!=null && list.size()!=0)
			return list;
		else
			return null;
	}

	public List<Paper> findByName(String name) {
		// TODO Auto-generated method stub
		List<Paper> list=getHibernateTemplate().find("from Paper model where model.name=? order by model.ptime desc",name);
		if(list!=null && list.size()!=0)
			return list;
		else
			return null;
	}

	public List<Paper> findByPaperName(String paperName) {
		// TODO Auto-generated method stub
		List<Paper> list=getHibernateTemplate().find("from Paper model where model.paper_id like ? order by model.ptime desc","%"+paperName+"%");
		if(list!=null && list.size()!=0)
			return list;
		else
			return null;
	}

	public List<Paper> findByPtime(String ptime) {
		// TODO Auto-generated method stub
		List<Paper> list=getHibernateTemplate().find("from Paper model where model.ptime like ? order by model.ptime desc","%"+ptime+"%");
		if(list!=null && list.size()!=0)
			return list;
		else
			return null;
	}

	public void delete(Paper paper) {
		// TODO Auto-generated method stub
		getHibernateTemplate().delete(paper);
	}
}

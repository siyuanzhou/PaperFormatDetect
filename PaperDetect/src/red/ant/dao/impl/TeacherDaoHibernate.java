package red.ant.dao.impl;

import java.util.List;

import org.springframework.orm.hibernate3.support.HibernateDaoSupport;

import red.ant.dao.TeacherDao;
import red.ant.po.Teacher;

public class TeacherDaoHibernate extends HibernateDaoSupport implements TeacherDao{

	public String save(Teacher teacher) {
		// TODO Auto-generated method stub
		return (String)getHibernateTemplate().save(teacher);
	}

	public void update(Teacher teacher) {
		// TODO Auto-generated method stub
		getHibernateTemplate().update(teacher);
	}

	public Teacher get(String username) {
		// TODO Auto-generated method stub
		return (Teacher)getHibernateTemplate().get(Teacher.class, username);
	}
	public Teacher findByNameAndPass(String username, String password) {
		List<Teacher> pp=getHibernateTemplate().find("from Teacher  p where p.username=? and p.password=?",new String[]{username,password});
		if(pp!=null && pp.size()>0) 
		{
			return pp.get(0);
		}
		return null;
	}

	public Teacher userIdentity(String username, String identity) {
		// TODO Auto-generated method stub
		List<Teacher> pp=getHibernateTemplate().find("from Teacher  p where p.username=? and p.identity=?",new String[]{username,identity});
		if(pp!=null && pp.size()>0) 
		{
			return pp.get(0);
		}
		return null;
	}

}

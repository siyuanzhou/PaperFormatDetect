package red.ant.dao.impl;


import java.util.List;
import java.io.*;

import org.springframework.orm.hibernate3.support.HibernateDaoSupport;

import red.ant.dao.StudentDao;
import red.ant.po.Student;

public class StudentDaoHibernate extends HibernateDaoSupport implements StudentDao {

	public List<Student> findByName(String username) {
		List pp=  getHibernateTemplate().find("from Student p where p.username like ?",username);
		return pp;
		
	}

	public Student get(String username) {
		return (Student)getHibernateTemplate().get(Student.class, username);
	}
//返回刚刚保存的Person实例的标识属性值
	public String save(Student student) {
		return (String) getHibernateTemplate().save(student);
	}

	public void update(Student student) {
		getHibernateTemplate().update(student);
	}

	public Student findStudentByNameAndPass(String username, String password) {
		List<?> pp=getHibernateTemplate().find("from Student  p where p.username=? and p.password=?",new String[]{username,password});
		if(pp!=null && pp.size()>0) 
		{
			return (Student) pp.get(0);
		}
		return null;
	}
    public String findName(String username)
    {
    	List<Student> list=getHibernateTemplate().find("from Student model where model.username=?",username);
    	return list.get(0).getName();
    }
    public Student findByUsername(String username)
    {
    	List<Student> list=getHibernateTemplate().find("from Student model where model.username=?",username);
    	if(list!=null && list.size()!=0)
    		return list.get(0);
    	else
    		return null;
    }

	public List<Student> findAll() {
		System.out.println("写入到。。");
		List<Student> list=getHibernateTemplate().find("from Student");
		if(list!=null && list.size()!=0)
    		return list;
    	else
    		return null;
	}

	public Student userIdentity(String username, String identity) {
		// TODO Auto-generated method stub
		List<Student> pp=getHibernateTemplate().find("from Student  p where p.username=? and p.identity=?",new String[]{username,identity});
		if(pp!=null && pp.size()>0) 
		{
			return (Student) pp.get(0);
		}
		return null;
	}

	public List<Student> findByGrade(String grade) {
		// TODO Auto-generated method stub
		List<Student> list=getHibernateTemplate().find("from Student model where model.grade like ?","%"+grade+"%");
    	if(list!=null && list.size()!=0)
    		return list;
    	else
    		return null;
	}
}

package red.ant.service.impl;

import red.ant.po.Teacher;
import red.ant.service.TeacherService;
import red.ant.dao.TeacherDao;
public class TeacherServiceImpl implements TeacherService{

	private TeacherDao teacherDao;
	
	public TeacherDao getTeacherDao() {
		return teacherDao;
	}

	public void setTeacherDao(TeacherDao teacherDao) {
		this.teacherDao = teacherDao;
	}

	public String save(String username, String password, String name,String email) {
		// TODO Auto-generated method stub
		Teacher teacher=new Teacher(username,password,name,email);
		teacherDao.save(teacher);
		return username;
	}

	public String login(String username, String password) {
		// TODO Auto-generated method stub
		Teacher teacher=teacherDao.findByNameAndPass(username,password);
		if(teacher!=null)
		{
			return teacher.getUsername();
		}
		else
			return null;
	}

	public void update(String username, String password) {
		// TODO Auto-generated method stub
		Teacher teacher=teacherDao.get(username);
		teacher.setPassword(password);
		teacherDao.update(teacher);
	}

	public String getEmail(String username) {
		// TODO Auto-generated method stub
		Teacher teacher=teacherDao.get(username);
		return teacher.getEmail();
	}

	public void setIdentity(String username, String identity) {
		// TODO Auto-generated method stub
		Teacher teacher=teacherDao.get(username);
		teacher.setIdentity(identity);
		teacherDao.update(teacher);
	}

	public String resetPassword(String username, String identity,String password) {
		// TODO Auto-generated method stub
		Teacher teacher=teacherDao.userIdentity(username, identity);
		if(teacher!=null)
		{
			teacher.setPassword(password);
			teacher.setIdentity(null);
			teacherDao.update(teacher);//验证通过修改完密码之后，验证码置为空
			return "success";
		}
		return null;
	}

}

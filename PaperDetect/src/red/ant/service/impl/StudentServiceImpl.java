package red.ant.service.impl;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import com.opensymphony.xwork2.ActionContext;

import red.ant.dao.StudentDao;
import red.ant.po.Student;
import red.ant.service.StudentService;

public class StudentServiceImpl implements StudentService {
    private StudentDao studentDao;
    
	public void setStudentDao(StudentDao studentDao) {
		this.studentDao = studentDao;
	}
    
	public StudentDao getStudentDao() {
		return studentDao;
	}

	public String login(String username,String password){
		
		Student p=studentDao.findStudentByNameAndPass(username, password);
		if(p!=null){
			
			return p.getUsername();
		}
		return null;
	}
	
	public void update(String username,String password){
		
		Student student=studentDao.findByUsername(username);
		student.setPassword(password);
		studentDao.update(student);	
	}

	public String save(String username,String password1,String grade,String name,String sex,String email){
		Student student=new Student(username,password1,grade,name,sex,email);		
		String id=studentDao.save(student);
		return id;
	}

	public String save(String username,String grade,String name,String sex){
		Student student=new Student();
		student.setName(name);
		student.setGrade(grade);
		student.setSex(sex);
		student.setUsername(username);
		student.setPassword("123456");
		String id=studentDao.save(student);
		return id;
	}
	public List<Student> show(String username) {
		// TODO Auto-generated method stub
		List<Student> list=studentDao.findByName(username);
		return list;
	}
	public String getName(String username)
	{
		return studentDao.findName(username);
	}
	public Student getAll(String username)
	{
		return studentDao.findByUsername(username);
	}

	public void setSecurity(String username, String answer1, String answer2,
			String answer3) {
		Student student=studentDao.findByUsername(username);
		student.setAnswer1(answer1);
		student.setAnswer2(answer2);
		student.setAnswer3(answer3);
		studentDao.update(student);
	}
	public void reset(String username,String password)
	{
		Student student=studentDao.findByUsername(username);
		student.setPassword(password);
		studentDao.update(student);
	}

	public String IsHas(String username) {
		// TODO Auto-generated method stub
		List<Student> list=studentDao.findAll();
		if(list!=null && list.size()!=0)
		{
			int i;
			for(i=0;i<list.size();i++)
			{
				if(list.get(i).getUsername().equals(username)==true)
					return "1";
			}
			return "0";
		}
		else
		{
			return "0";
		}
	}

	public String getEmail(String username) {
		// TODO Auto-generated method stub
		Student student=studentDao.get(username);
		return student.getEmail();
	}

	public void setIdentity(String username,String identity) {
		// TODO Auto-generated method stub
		Student student=studentDao.get(username);
		student.setIdentity(identity);
		studentDao.update(student);
	}

	public String resetPassword(String username, String identity,String password) {
		// TODO Auto-generated method stub
		
		Student student=studentDao.userIdentity(username, identity);
		if(student!=null)
		{
			student.setPassword(password);
			student.setIdentity(null);//验证通过修改完密码之后，验证码置为空
			studentDao.update(student);
			return "success";
		}
		return null;
	}

	public List<String> findUsernameByGrade(String grade) {
		// TODO Auto-generated method stub
		List<Student> list=studentDao.findByGrade(grade);
		if(list==null)
		{
			return null;
		}
		else
		{
			List<String> list1=new ArrayList();
			for(int i=0;i<list.size();i++)
			{
				list1.add(list.get(i).getUsername());
			}
			return list1;
		}
	}
}

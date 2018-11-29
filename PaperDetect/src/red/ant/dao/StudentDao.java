package red.ant.dao;


import java.util.List;

import red.ant.po.Student;

public interface StudentDao {
public String save(Student student);
public void update(Student student);
public Student get(String username);
public List<Student> findByName(String name);
public Student findStudentByNameAndPass(String name,String password);
public String findName(String username);
//修改密码之用
public Student findByUsername(String username);
/**
 * 验证用户是否存在	
 * @return
 */
public List<Student> findAll();
/**
 * 寻找同时匹配学号和验证码的实例是否存在
 * @param username
 * @param identity
 * @return
 */
public Student userIdentity(String username,String identity);
/**
 * 根据班级查找
 * @param grade
 * @return
 */
public List<Student> findByGrade(String grade);

}


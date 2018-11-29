package red.ant.service;

import java.util.List;

import red.ant.po.Student;

public interface StudentService {
public String login(String username,String password);
//修改密码之用
public void update(String username, String password);
/**
 * 注册时保存用户信息
 * @param username
 * @param password1
 * @param grade
 * @param name
 * @param sex
 * @param email
 * @return
 */
public String save(String username,String password1,String grade,String name,String sex,String email);
public String save(String username,String grade,String number,String sex);
public List<Student> show(String username);
public String getName(String username);
public Student getAll(String username);
/**
 * 根据用户名查找对应元组，将密保答案保存至对应元组中
 * @param username
 * @param answer1
 * @param answer2
 * @param answer3
 */
public void setSecurity(String username,String answer1,String answer2,String answer3);
/**
 * 重置密码(暂时无用)
 * @param username
 * @param password
 */
public void reset(String username,String password);
/**
 * 验证用户是否存在
 * @param userrname
 * @return
 */
public String IsHas(String username);
/**
 * 根据学号获取email
 * @param username
 * @return
 */
public String getEmail(String username);
/**
 * 根据用户名更新验证码
 * @param username
 */
public void setIdentity(String username,String identity);
/**
 * 找到符合（username,identity）的实例，重置密码为password
 * @param username
 * @param identity
 * @param password
 * @return
 */
public String resetPassword(String username,String identity,String password);
/**
 * 教师页面查找同班级的学生论文
 * @param grade
 * @return
 */
public List<String> findUsernameByGrade(String grade);
}

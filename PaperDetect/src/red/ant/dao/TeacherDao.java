package red.ant.dao;

import red.ant.po.Teacher;

public interface TeacherDao {
public String save(Teacher teacher);
public void update(Teacher teacher);
public Teacher get(String username);
public Teacher findByNameAndPass(String username, String password);
/**
 * 寻找同时匹配学号和验证码的实例是否存在,存在返回实例，不存在返回null
 * @param username
 * @param identity
 * @return
 */
public Teacher userIdentity(String username,String identity);
}


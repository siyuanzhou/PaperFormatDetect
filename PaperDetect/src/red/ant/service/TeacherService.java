package red.ant.service;

public interface TeacherService {
	public String save(String username,String password,String name,String email);
	public String login(String username,String password);
	public void update(String username, String password);
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
	}
package red.ant.po;

public class Teacher {
		private String username;//老师教职工号
		private String password;//密码
		private String name;//老师姓名
		private String email;
		private String identity;//验证码
		public Teacher(){
			
		}
		
		public Teacher(String username, String password,String name,String email) {
			this.username=username;
			this.password=password;
			this.name=name;
			this.email=email;
		}
		public String getUsername() {
			return username;
		}
		public void setUsername(String username) {
			this.username = username;
		}

		public String getPassword() {
			return password;
		}

		public void setPassword(String password) {
			this.password = password;
		}
		public String getName()
		{
			return name;
		}
		public void setName(String name)
		{
			this.name=name;
		}

		public String getEmail() {
			return email;
		}

		public void setEmail(String email) {
			this.email = email;
		}

		public String getIdentity() {
			return identity;
		}

		public void setIdentity(String identity) {
			this.identity = identity;
		}
		
}

package red.ant.po;

public class Student {
	    private String sex;//性别
		private String username;//学生号
		private String password;//密码
		private String grade;//班级
		private String name;//学生姓名
		private String answer1;
		private String answer2;
		private String answer3;
		private String email;
		private String identity;//验证码
		public Student(){
			
		}
		
		public String getGrade() {
			return grade;
		}

		public void setGrade(String grade) {
			this.grade= grade;
		}

		public Student(String username, String password) {
			this.username=username;
			this.password=password;
		}
		public Student(String username, String password1,String grade,String name,String sex,String email) {
			this.username=username;
			this.password=password1;
			this.grade=grade;
			this.name=name;
			this.sex=sex;
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
		public String getSex()
		{
			return sex;
		}
		public void setSex(String sex)
		{
			this.sex=sex;
		}
		
		
		public String getAnswer1() {
			return answer1;
		}
		public void setAnswer1(String answer1) {
			this.answer1 = answer1;
		}
	
		public String getAnswer2() {
			return answer2;
		}
		public void setAnswer2(String answer2) {
			this.answer2 = answer2;
		}
	
		public String getAnswer3() {
			return answer3;
		}
		public void setAnswer3(String answer3) {
			this.answer3 = answer3;
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

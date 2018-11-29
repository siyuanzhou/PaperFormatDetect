package red.ant.po;

public class Advice {
	    private Integer aid;//主键
		private String sid;//学生学号
		private String advice;//反馈意见
		private String email;//邮箱
		private String name;//姓名
		public Advice(){
			
		}
		public Advice(Integer aid,String sid,String advice,String email,String name)
		{
			this.aid=aid;
			this.sid=sid;
			this.advice=advice;
			this.email=email;
			this.name=name;
		}
		
		public Integer getAid() {
			return aid;
		}
		public void setAid(Integer aid) {
			this.aid = aid;
		}
		public String getSid() {
			return sid;
		}
		public void setSid(String sid) {
			this.sid = sid;
		}
		public String getAdvice() {
			return advice;
		}
		public void setAdvice(String advice) {
			this.advice = advice;
		}
		public String getEmail() {
			return email;
		}
		public void setEmail(String email) {
			this.email = email;
		}
		public String getName() {
			return name;
		}
		public void setName(String name) {
			this.name = name;
		}
		
}

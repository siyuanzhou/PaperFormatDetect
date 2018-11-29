package red.ant.po;

import java.util.Date;

public class Paper {
	    private String pid;//论文编号
	    private String paper_id;//论文名称
		private String paper_path;//论文路径
		private String ptime;//上传时间
		private String sid;//学生学号
		private String name;//学生姓名
		public Paper(){
			
		}
		public Paper(String pid,String paper_path,String ptime,String sid,String name,String paper_id)
		{
			this.pid=pid;
			this.paper_path=paper_path;
			this.ptime=ptime;
			this.sid=sid;
			this.name=name;
			this.paper_id=paper_id;
		}
		public String getPid() {
			return pid;
		}
		public void setPid(String pid) {
			this.pid = pid;
		}
		public String getPaper_path() {
			return paper_path;
		}
		public void setPaper_path(String paper_path) {
			this.paper_path = paper_path;
		}
		public String getPtime() {
			return ptime;
		}
		public void setPtime(String ptime) {
			this.ptime = ptime;
		}
		public String getSid() {
			return sid;
		}
		public void setSid(String sid) {
			this.sid = sid;
		}
		public String getName() {
			return name;
		}
		public void setName(String name) {
			this.name = name;
		}
		public String getPaper_id() {
			return paper_id;
		}
		public void setPaper_id(String paper_id) {
			this.paper_id = paper_id;
		}
		
		
}

package red.ant.service;

import java.util.Date;
import java.util.List;

import red.ant.po.Paper;

public interface PaperService {
public void update(String username, String password);
public String save(String paper_path,String sid,String name,String paper_id);//pid和time内部生成
public List<Paper> findByUsername(String username);
public String getPath(String pid);
public String getTestPath(String pid);
public String getPaper_id(String pid);
/**
 * 老师可查看所有学生的检查历史
 * @return
 */
public List<Paper> show();
public Integer Sum();
/**
 * 教师页面的查找功能
 * @param name
 * @return
 */
public List<Paper> fidByName(String name);
public List<Paper> findByPaperName(String paperName);
public List<Paper> findByPtime(String ptime);
/**
 * 删除pid对应的数据库实例及文件及报告
 * @param pid
 */
public void delete(String pid);

}
package red.ant.dao;
import java.util.List;

import red.ant.po.Paper;

public interface PaperDao {
public String save(Paper paper);
public void update(Paper paper);
public Paper get(String pid);
public List<Paper> findAll();
public List<Paper> findByUsername(String username);
/**
 * 教师页面查找功能
 * @param name
 * @return
 */
public List<Paper> findByName(String name);
/**
 * 教师界面查找功能
 * @param paperName
 * @return
 */
public List<Paper> findByPaperName(String paperName);
/**
 * 教师界面查找功能
 * @param ptime
 * @return
 */
public List<Paper> findByPtime(String ptime);
public void delete(Paper paper);
}

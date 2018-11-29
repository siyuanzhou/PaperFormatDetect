package red.ant.service.impl;

import java.util.List;

import red.ant.dao.AdviceDao;
import red.ant.po.Advice;
import red.ant.service.AdviceService;

public class AdviceServiceImpl implements AdviceService{

	private AdviceDao adviceDao;
	
	public AdviceDao getAdviceDao() {
		return adviceDao;
	}

	public void setAdviceDao(AdviceDao adviceDao) {
		this.adviceDao = adviceDao;
	}

	public String save(String sid, String email, String advice1,String name ) {
		// TODO Auto-generated method stub
		List<Advice> list=adviceDao.findAll();
		Integer aid;
		int i,max=0;
		if(list!=null && list.size()!=0)
		{
			for(i=0;i<list.size();i++)
			{
				if(list.get(i).getAid()>max)
					max=list.get(i).getAid();
			}
			aid=max+1;
		}
		else
		{
			aid=0;
		}
		System.out.println(aid+sid+advice1+email+"--------");
		Advice advice=new Advice(aid,sid,advice1,email,name);
		System.out.println("adviceÍê³É------");
		adviceDao.save(advice);

		return sid;
	}

	public List<Advice> show() {
		// TODO Auto-generated method stub
		List<Advice> list=adviceDao.findAll();
		if(list!=null && list.size()!=0)
		{
			return list;
		}
		return null;
	}

	public List<Advice> findByName(String name) {
		// TODO Auto-generated method stub
		List<Advice>  list=adviceDao.findByName(name);
		return list;
	}

	public List<Advice> findByUsername(String username) {
		// TODO Auto-generated method stub
		List<Advice> list=adviceDao.findByUsername(username);
		return list;
	}

	public void delete(String aid) {
		// TODO Auto-generated method stub
		String[] data=aid.split(" ");
		Advice advice=new Advice();
		for(String str:data)
		{
			advice=adviceDao.get(Integer.parseInt(str));
			adviceDao.delete(advice);
		}
	}
	

}

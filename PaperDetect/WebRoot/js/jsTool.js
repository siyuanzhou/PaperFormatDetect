/**
 * 操作Cookie   添加   
 * @param name
 * @param value
 * @return
 */
function SetCookie(name,value,days)//两个参数，一个是cookie的名子，一个是值
{
    var Days = 30;
    if(typeof(days)=="undefined"||isNaN(days))
        Days=parseInt(days.toString());
     //此 cookie 将被保存 30 天 -1为浏览器关闭　　
     if(Days!=-1)
     {
        var exp = new Date();    //new Date("December 31, 9998");
        exp.setTime(exp.getTime() + Days*24*60*60*1000);
        document.cookie = name + "="+ escape (value) + ";expires=" + exp.toGMTString();
    }
    else
    {
        document.cookie = name + "="+ escape (value) + ";expires=-1";
    }
}

/**
 * 操作Cookie 提取   后台必须是escape编码
 * @param name
 * @return
 */
function getCookie(name)//取cookies函数
{
    var arr = document.cookie.match(new RegExp("(^| )"+name+"=([^;]*)(;|$)"));
    if(arr != null) return unescape(arr[2]); return null;
}
/**
 * 操作Cookie 删除
 * @param name
 * @return
 */
function delCookie(name)//删除cookie
{   
    var exp = new Date();
    exp.setTime(exp.getTime() - (86400 * 1000 * 1));
    var cval=getCookie(name);
    if(cval!=null)
        document.cookie = name + "="+ escape (cval) + ";expires="+exp.toGMTString();
}

/**
 * 根据ID获取对象
 * @param objName
 * @return
 */
 function GetObj(objName)
 {
    if(typeof(objName)=="undefined")
        return null;
    if(document.getElementById)
        return eval('document.getElementById("'+objName+'")');
    else
        return eval('document.all.'+objName);
    
}
/**
 * 给String 添加trim方法
 */
 String.prototype.trim=function(){
    return this.replace(/(^\s*)|(\s*$)/g, "");
}
/**
 * 给String添加isNullOrempty的方法
 */
 String.prototype.isnullorempty=function()
 {
    if(this==null||typeof(this)=="undefined"||this.trim()=="")
        return true;
    return false;
    
}
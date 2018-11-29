# PaperFormatDetection
论文格式自动检测系统——PaperFormatDetection

**项目背景**：此项目隶属于大连理工大学国家级创业训练项目，由三个人负责主要的模块的开发，并交由实验室网站组上线，主要解决高校毕业生论文格式不规范，而人工检查费时费力这一现实问题。

**项目内容**：此项目的在于检测论文中所有格式错误，并加以修改。可以得到整篇论文的错误格式报告和修改格式后的论文。

通过运用OpenXMLSDK，比较和修改用户上传的论文的XML标签与标准模板XML标签，来达到检测和修改论文格式的目的。截止到目前，这个系统已经在校内网上线，大规模测试了三次，并且已经检测了**超过6000**篇论文。

**我的工作**: 负责图，表，页眉页脚等模块的开发，负责字体字号格式检测与修改函数编写。

**用到的技术**: C#, OpenXMLSDK , XML提取，字符串匹配

**成果**： 自动检测系统、修改程序各一套，专利、软件著作权各一项

**Github:** <https://github.com/Moflowerszww/PaperFormatDetection>

**开发的过程分为了以下几个部分：**

（1） 研究word论文格式与对应的xml映射关系，利用OpenXML等工具研究word下.doc及.docx文件实现过程中用到的标签、类、库，为提取属性及属性对比打下基础。

（2） 如何从xml中提取格式特征,即利用C#语言提取出每个格式特征的XML标签，并运用到检测过程当中。

（3） 上传的的论文与指定的模板进行对比，须分别提取上传论文与给定模板的同种属性，依次完成所有属性的对比。

（4） 全面且准确的将word论文格式封装进库，拟建立一个新的架构层次，避免对比过程中出现耦合，利于系统后期维护与升级。

**主要功能运行**

![](http://ww1.sinaimg.cn/large/007aqPQQly1fxnrrja4tsg308w054npf.gif)

**网站首页**

![](http://ww1.sinaimg.cn/large/007aqPQQly1fxnl1puoybj313t0mk7wi.jpg)

**检测页面**

![](http://ww1.sinaimg.cn/large/007aqPQQly1fxnurw6g2kj30mn0buaeu.jpg)

**生成的报告页**

![](http://ww1.sinaimg.cn/large/007aqPQQly1fxnl378z59j30gi0cidg6.jpg)

**结果统计分析页**

![](http://ww1.sinaimg.cn/large/007aqPQQly1fxnl4b89zbj30j10bc0tf.jpg)

**项目证书**

![](http://ww1.sinaimg.cn/mw690/007aqPQQly1fxnl80cma7j32c0340x6s.jpg)
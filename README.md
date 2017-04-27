## 对原有项目改造的一个新框架

## 目录结构如下
**Assets**--根目录
-	Standard Assets -- 第三方插件目录 -- 主要为了加快编译速度--
-	XLua -- XLua就用xlua
-	StreamingAssets  --  打包后的资源的存放目录
-	Resources -- 一定会被打包到游戏的资源-- 这里面一定要少放东西
-	Plugins -- 所有需要引入的第3方插件--
-	Scripts -- 游戏代码目录--
-	FrameWork -- 也是代码 --但是会把和具体游戏无关的部分提取到这边
-	GameAssets -- 游戏资源--
	-	Models   -- 再下面根据功能细分之类的就不放这了 ---
	-	Materials -- 再下面根据功能细分之类的就不放这了 ---
	-	Shaders -- 再下面根据功能细分之类的就不放这了 ---
	-	Sounds --
	-	Textures --
	-	UI --
-	Scenes  -- 场景文件
-	ZTest -- 测试代码用例等等 --加Z就会放到目录的最后了
--  GameStart  -- 一个startScene 和一个 start 脚本，游戏入口

##特殊文件
    
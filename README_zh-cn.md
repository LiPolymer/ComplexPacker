# ComplexPacker

[English](README.md)

一个简单的自动化工具,可用于MC 资源包/数据包/低代码Mod,以及更多可能的用途!

这个工具可以用来从源文件快速 构建/测试/部署 你的项目,以及用于你的项目CI

要快速开始使用本工具,你需要创建以下目录结构
```
some-project
 | ComplexPacker可执行文件
 | src
 |  | pack.mcmeta
 |  \ ...
 \ task
    | build.cps
    \ ...
```
在`build.cps`中定义构建脚本

之后打开ComplexPacker可执行文件,进入交互模式(Interactive Mode)

输入`build`并执行即可运行`build.cps`

>什么是`.cps`?
> 
> CPS,即ComplexPackerScript,是专用于这个程序的脚本文件。
> 
> 从首行开始,一行一行执行,语句请参阅 [CPS.md](CPS.md)
> 
> 如果你认为通过示列学习更加合适,请参阅 [CPSExample.md](CPSExample.md)

关于CI配置,可以查看[ArCore](https://gitlab.com/SiWG/ArCore),这是整合包[theArcadia](https://modrinth.com/modpack/thearcadia)魔改核心模组的仓库,使用ComplexPacker提供CI支持.
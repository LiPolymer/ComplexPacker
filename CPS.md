# ComplexPacckerScript(CPS)关键字列表
**开发中,随时变动**
## echo
基本用法 `echo <msg>`

会在控制台打印`<msg>`

该操作由`DawnUtils.Terminal()`终端处理器处理,因此可以解析彩色文字
> DawnUtils彩色文字标准
> 
> `&c%<ColorTag>%&`
>
>其中`<ColorTag>`与游戏Minecraft内基于`&<ColorTag>`的着色系统大致相同
## var
基本用法 `var <name> <value>`

设定变量`<name>`为值`<value>`

由于每个语句在执行前都会经过变量解析器

所以可以在任意位置使用变量

变量解析格式`_%<name>%_`
## env
基本用法 `env <name> <value>`

设定环境变量`<name>`为值`<value>`

环境变量作用类似注册表,在解析器启动时就已经自动定义了一部分

有些语句也会自动调用作为参数使用

*暂时无法直接调用*
## makecopy
基本用法 `makecopy <operation> [...]`

有关联的环境变量:
+ `project.cache`------用于定义 *缓存目录*
+ `project.rootdest` --用于定义 *工程目录*

可用的`operation`有:
> makecopy **make**
>
> 该语句会启动`makecopy`,将环境变量`project.rootdest`定义的目录中**除了 排除列表定义的例外 以外的所有文件**复制到`project.cache`中。*一般用于等待下一步操作*

> makecopy **ignore** (op) [arg]
>
> 该语句用于对makecopy的排除列表进行操作.
>
>`(op)`有三个,分别是`add`,`rmv`,`list`
>
> `makecopy ignore add <arg>` 会将`<arg>`添加到排除列表中
>
> 而`makecopy ignore rmv <arg>` 会将`<arg>`从排除列表中移除
>
> 此处需要注意的是目前对排除项的解析是在复制开始时进行的,意味着要移除一个排除项,`rmv`的对象只能和`add`的对象**完全相同**
>
> 显而易见的,`makecopy ignore list`则会列出所有排除项。

## showscript/hidescript
CPS解释器默认会在终端打印当前执行的语句,该功能默认打开

使用`hidescript`来关闭该功能

必要时使用`showscript`重新打开

## sleep
基本用法 `sleep <time:ms>`

无需多言

.cs:`Thread.Sleep(<time>);`

.py:`time.sleep(<time>)`

## del
基本用法 `del <path>`

无需多言

**del**ete
## clsl/load
*乱取的名*

基本用法 `clsl <path>` `load <path>`

执行`<path>`指向的脚本

两者区别:

`clsl`会单独启动一个新的解释器实例,这通常意味着环境变量,makecopy排除列表等修改不会被继承;

而`load`的效果则相当于将指定脚本插入该位置进行运行,使用执行load的解释器实例.

## shdo
基本用法 `shdo <target> <args>`

执行系统shell操作

`<target>·`后该行所有内容都将被解析为`<args>`

## pkg
暂时只能 `pkg make zip`

有关联的环境变量:
+ `project.cache`------用于定义 *缓存目录*
+ `project.outdest`------用于定义 *输出目录*
+ `project.artifactName`------用于定义 *产物文件名*

将环境变量`project.cache`中现存的所有文件以`project.artifactName`文件名(含扩展名)的zip压缩包形式输出到`project.outdest`中

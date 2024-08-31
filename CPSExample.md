# ComplexPackerScript示列
### 一个非常普通的,忽略`.psd`文件的`build.cps`
```
env project.artifactName Pack.zip
makecopy ignore add *.psd
makecopy cleanup
makecopy make
pkg make zip
makecopy cleanup
echo &c%2%& BUILD SUCCESSED!
```
#### 行为:
- 将`src`文件夹内除`*.psd`以外的所有文件复制到`cache`文件夹
- 将`cache`文件夹内所有内容以`zip压缩包`的形式输出到`build/Pack.zip`
- 在控制台显示绿色的`BUILD SUCCESSED!`
>注意:
> 
> env语句是在定义/修改环境变量
> 
> 而环境变量在解释器启动时就被定义了一些:
> 
> `CPScriptInterpreter.cs` *为阅读便利,进行了修改*
> ```csharp
>            public Dictionary<string, string> EnvVars = new()
>            {
>                ["project.cache"] = "./cache",
>                ["project.outdest"] = "./build",
>                ["project.rootdest"] = "./src",
>                ["project.artifactName"] = "artifact"
>            };
>```
# ComplexPacker

[简体中文](README_zh-cn.md)

A simple automation tool for`minecraft resourcepack/datapack/lowcodemod`and more!

This tool can be used to quickly build/test/deploy your project from source files, as well as for your project CI.

To quickly start using this tool, you need to create the following directory structure
```
some-project
|ComplexPacker executable file
| src
|  | pack.mcmeta
|  \ ...
\ task
   | build.cps
   \ ...
```
Define a build script in ` build. cps `
Afterward, open the ComplexPacker executable file and enter Interactive Mode
Enter 'build' and execute to run 'build. cps'`
>What is'. cps'?
>
>CPS, also known as ComplexPackerScript, is a script language specifically designed for this program.
>
>Starting from the first line, execute line by line. Please refer to [CPS.md](../CPS.md) for statements' documents
>
>If you think column based learning is more appropriate, please refer to [CPSExample.md](../CPSExample.md)
>
Regarding CI configuration, you can refer to [ArCore](https://gitlab.com/SiWG/ArCore), which is the repository for the mod-pack [theArcadia](https://modrinth.com/modpack/thearcadia) modified core module, and uses ComplexPacker to provide CI support.
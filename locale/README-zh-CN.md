<h1 align="center">EventPlayer</h1>

<p align="center">
    <a href="https://assetstore.unity.com/packages/tools/level-design/bezier-solution-113074"><img src="https://github.com/Threeyes/EventPlayer/wiki/images/Logo.png" alt="Logo" width="300px" height="200px" />
    <br />
	<a><img src="https://img.shields.io/badge/%20Unity-2028.4+%20-blue" /></a>
	<a href="https://github.com/Threeyes/AliveCursorSDK/blob/main/LICENSE"><img src="https://img.shields.io/badge/License-MIT-brightgreen.svg" /></a>
    <br />
</p>

## 语言
<p float="left">
  <a href="https://github.com/Threeyes/EventPlayer/blob/main/locale/README-zh-CN.md">中文</a> | 
  <a href="https://github.com/Threeyes/EventPlayer">English</a>
</p>


## 简介
我已经厌倦了总是写不能复用的脚本，所以写了这个可视化事件管理库，无需额外的编辑器窗口！

EventPlayer扩展了Unity的内置事件系统，提供了几个在特定时间调用事件的组件，如延迟、重复和倒计时。您可以对事件进行可视化重新排序、重组或停用，并在所需时间执行它们。


## 特性
- 简单粗暴，无需编程！
- 通过组或序列的形式可视化管理事件，将他们按ID进行标记。
- 支持大多数平台。
- 支持以下插件：
    - [Timeline](https://docs.unity3d.com/Packages/com.unity.timeline@1.5/manual/index.html): Unity的时间轴库。
    - [VideoPlayer](https://docs.unity3d.com/Manual/class-VideoPlayer.html): Unity内置的视频播放库。
    - [BezierSolution](https://assetstore.unity.com/packages/tools/level-design/bezier-solution-113074): 运行时贝塞尔曲线库。
    - [DoTweenPro](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416): 强大的动画库。
    - (您还可以编写代码来支持更多插件...)


## 注意
该插件依赖于 **Threeyes Core库** (AssetStore版本已经包含该库). 如果你的项目没有此库，请从[GitHub](https://github.com/Threeyes/Core)上下载。


## 初始化/更新
### 添加/删除外部插件支持
单击“Tools/Threeeyes/EventPlayer Setting”菜单，选择需要使用的插件，然后单击“Apply”。激活前请确保项目内拥有这些插件，并已经正确链接到它们的asmdef文件（如果存在）。


## 案例
配置好外部支持后，将“Samples”文件夹中的所有场景文件添加到BuildSettings窗口，然后运行第一个场景，按左/右键可切换场景。你可以从Hierarchy窗口的**Remarker**组件中看到更多说明。


## 故障排查
### 更新到新版本后部分文件冲突怎么办？
可能是我移动或重命名了文件夹，请先删除旧插件并重新导入最新插件（务必事先备份项目！）。

### Unity 版本为 2018.4 或更高，找不到 Timeline 插件怎么办？
由于 Unity 新版本已默认移除 Timeline，您需要从包管理器中下载 Timeline，之后即可正常使用。如果您正在使用 asmdef，还需要引用相关文件：https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html

### 为什么与 DOTweenPro 相关的代码报错？
这可能是因为 DOTweenPro 对应的库没有正确链接。首先，请确认 DOTweenPro 文件夹内包含 'DOTweenPro.Scripts.asmdef' 文件，如不存在，可通过菜单 'Tools/Demographics/DOTween Utility Panel' 打开设置窗口，然后点击 'Create ASMDEF...' 生成它。如果仍有错误，可能是上述生成的 asmdef 文件的元数据信息发生了变化，您需要将该 asmdef 文件添加到 Threeyes.EventPlayer.asmdef 的 'Assembly Definition References' 列表中并点击应用。

### 示例项目的材质在内置渲染管线下变为紫色？
示例默认包含 URP 材质。您需要从示例文件夹中导入 'Materials_BuildInRP.unitypackage' 以覆盖所有现有材质。

### 示例项目的材质在 HDRP 渲染管线下变为紫色怎么办？
首先，按照上述方法导入内置渲染管线的所有材质，然后选中所有材质，接着在菜单中选择 'Edit/Rendering/Materials/Convert selected Built-in Materials to HDRP' 转换为 HDRP 材质。


## 更多信息
[Unity商店版本](https://assetstore.unity.com/packages/tools/visual-scripting/event-player-116731)

[Unity论坛](https://forum.unity.com/threads/release-event-player-visual-play-and-organize-unityevent.536984/)

[Unity社区-中文教程](https://developer.unity.cn/projects/603086a7edbc2a00202c3878)
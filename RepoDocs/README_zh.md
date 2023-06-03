<div align="center">

# Arcript

---

<!-- [![Version](https://img.shields.io/github/v/release/Misaka12456/Arcript?display_name=tag)](https://github.com/Misaka12456/Arcript/releases) -->

![Version](https://img.shields.io/badge/Version-0.1.0%28early%20developing%20status%29-blue) ![Unity Version](https://img.shields.io/badge/Unity%20Version-2020.2.1f1-orange)
[![Misaka Castle Member Project](https://img.shields.io/badge/Misaka%20Castle-Memeber%20Project-fuchsia)](https://misakacastle.moe) ![123 Open-Source Organization 10 Years Appointment Member Project](https://img.shields.io/badge/Team123it%2010%20Years%20Appointment-Member%20Project-brightgreen)
[![License](https://img.shields.io/badge/license-Team123it--MIT%202.0-blue)](https://team123it.org/LICENSE.html)

使用Unity和纯C#编写的功能强大的视觉小说(VN)脚本制作工具(不仅是Arcaea[^2])。

中文版 | [English](../README.md)

**版权提示: 请参阅[License](#开源协议)部分以了解DMCA版权投诉请求的相关内容。**

</div>

## 脚本格式

``Arcript``使用了名为 ``aspt``[^3]的格式存储所有的VN脚本指令。每个 ``aspt``脚本必须先经过构建才能够被用到Arcaea或者其它支持的VN游戏引擎中。``Arcript``提供了两个可选的 ``aspt``脚本构建的目标:

- ``.vns``: 用于Arcaea的VN脚本格式。尽管它本质上也是明文脚本格式，但它的适合人类直接编辑的可读性仍然不够好。
- ``.acpkg``: 用于Arcript的二进制脚本包格式。针对支持的VN游戏引擎的快速加载而经过了优化。

`<a id="arcriptPlus"></a>`

## Arcript+

尽管整个Arcript企划完全是从Arcaea的VN故事机制中得来的灵感，但Arcript并没有计划只支持Arcaea的VN故事机制。  
这就是为什么会出现一组名为 ``Arcript+``的特性组。  

``Arcript+``是 ``Arcript``独有的特性。  
其中多数的均设计用于纯VN游戏(比如 ``Galgame``)的开发。  

**注意: 虽然看起来似乎是这样，但Arcript+并不是Arcript的某个特殊版本！它只是Arcript独有的特性的统称**

下面是几个具有代表性的Arcript+特性:  
  
- **选择支**: 提供一种让玩家选择他们自己剧情走向的方法。通常用于Galgame中的不同结局分支。这里是一个选择支机制的[示例截图](https://i.imgur.com/0hzDE5l.png!/%5BArcript%5DArcript%2B%E7%89%B9%E6%80%A7%E9%A2%84%E8%A7%88%28%E9%80%89%E6%8B%A9%E6%94%AF%E6%9C%BA%E5%88%B6%29))[^4]。
- **If判断语句**: 提供一种允许检查变量的值并决定跳转到哪个分支的方法。通常用于Galgame中的角色好感度检查。
- **屏幕中央文本**: 提供一种在屏幕中央(而不是屏幕下方的对话框)显示文本的方法。通常用于文字冒险类VN游戏中的故事设定介绍(或既不是对话也不是角色所想的内容的文本)。
- **全屏视频播放**: 提供一种在VN游戏中全屏播放视频的方法。通常用于Galgame中的OP/ED视频播放。
- *...以及更多*

## Arcript 二次开发接口

``Arcript``提供了一些接口用于二次开发。以下是二级开发接口的示例：

- [Arcript.Data.ICustomImageFormat](Assets/Scripts/Data/ICustomImageFormat.cs): 实现该接口以创建自定义的图片格式解析器。
  ``Arcript``将会尝试在所有已加载的程序集中找继承自 ``ICustomImageFormat``接口且有[ImgFormatExportAttribute](Assets/Scripts/Data/ImgFormatExportAttribute.cs)特性的类，并使用找到的类加载自定义的图片格式。
  - 示例: [Arcript.FormatExt.Siglus](Assets/ArcriptPlugins/Arcript.FormatExt.Siglus)[^5]: 对Arcript提供Siglus/RealLive[^6]游戏引擎图片格式(``.g00``)的解析器扩展插件。
- *...更多接口开发在计划中*

## 代码贡献须知

``Arcript``仍旧处于初期开发阶段。如果您想贡献这个项目，请为这个项目创建一个有价值的[Pull Request](https://github.com/Misaka12456/Arcript/pulls) 或者 [Issue](https://github.com/Misaka12456/Arcript/issues)。  
在 ``Arcript``的第一个版本(``0.1.0``)发布后，``Arcript``的主要开发工作将移动至 ``develop``分支。  
``master``分支将在那之后只用于发布 ``Arcript``的稳定版本。

## 开源协议

``Arcript``主企划基于[123 Open-Source Organization MIT开源许可协议 v2](https://github.com/Team123it/team123it.github.io/blob/master/LICENSE.html)授权。  
``Arcript``的插件均基于它们自己的协议授权。查看[ArcriptPlugins](Assets/ArcriptPlugins)以了解详情。  
<a id="dmcaRelated"></a>
**``Arcript``开发过程中可能会出现使用到版权贴图的情况。如果侵犯到了您或您的组织的版权，请联系[开发者](mailto:main@misakacastle.moe)，我们将会尽快删除。**

## 感谢

感谢以下项目为 ``Arcript``提供了灵感和/或帮助:

- [morkt/GARbro](https://github.com/morkt/GARbro): 提供了一些自定义游戏引擎资源格式解析器的原型。
- [xmoezzz/SiglusExtract](https://github.com/xmoezzz/SiglusExtract): 一个用于从使用Siglus/RealLive[^6]游戏引擎的游戏中提取资源的工具。
- [jp-netsis/RubyTextMeshPro](https://github.com/jp-netsis/RubyTextMeshPro): 一个为TextMeshPro提供日文振假名(不局限于日文的注音效果)支持的插件。
- [Cysharp/UniTask](https://github.com/Cysharp/UniTask): 一个为Unity提供强大的async/await支持的库。
  同时也感谢[所有一直以来支持 ``Arcript``项目的人](THANKS.md)。

<!-- [^4]: Example screenshot is a highly-restored screenshot of game 'floral·flowlove' by [Saga Planets](https://sagaplanets.product.co.jp/). See [floral·flowlove Official Website](https://sagaplanets.product.co.jp/works/flowlove/) and [Copyright Tip](#dmcaRelated) for more information. -->

[^1]: ``Arcaea``是由Lowiro Limited开发的音乐游戏。更多信息请参阅[Arcaea官方网站](https://arcaea.lowiro.com/)。
    
[^2]: Arcript支持Arcaea不支持的特性。这些特性被称为 ``Arcript+``。您也可以将 ``Arcript+``特性用于开发视觉小说游戏，而不仅仅是将它们用于Arcaea。更多信息请参阅[Arcript+](#arcriptPlus)章节。
    
[^3]: ``aspt``: 使用yaml格式编写的Arcript纯文本VN脚本(``Arcript Plain VN Script``)。
    
[^4]: 示例截图是对由[Saga Planets](https://sagaplanets.product.co.jp/)开发的游戏《floral·flowlove》的 ``Arcript``内高度还原的截图。更多信息请参阅[《floral·flowlove》官方网站](https://sagaplanets.product.co.jp/works/flowlove/)和[版权提示](#dmcaRelated)。
    
[^5]: 内置插件 ``Arcript.FormatExt.Siglus``使用了 ``morkt``的 ``GARbro``项目中的代码。更多信息请参阅[GARbro项目仓库](https://github.com/morkt/GARbro)。
    
[^6]: ``Siglus(TM)``、``Siglus(TM) Engine``、``RealLive(TM)``和 ``RealLive(TM) Engine``是 ``Key/Visual Art's``的商标。更多信息请参阅[Key/Visual Art's官方网站](https://key.visualarts.gr.jp/)。

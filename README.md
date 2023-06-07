<div align="center">

[![Arcript](Assets/Skin/InternalTextures/Icon/Arcripts/Arcript_Small_128x128.png)](https://github.com/Misaka12456/Arcript)  

# Arcript

<!-- [![Version](https://img.shields.io/github/v/release/Misaka12456/Arcript?display_name=tag)](https://github.com/Misaka12456/Arcript/releases) -->

![Version](https://img.shields.io/badge/Version-0.1.0%28early%20developing%20status%29-blue) ![Unity Version](https://img.shields.io/badge/Unity%20Version-2020.2.1f1-orange)
[![Misaka Castle Member Project](https://img.shields.io/badge/Misaka%20Castle-Memeber%20Project-fuchsia)](https://misakacastle.moe) ![123 Open-Source Organization 10 Years Appointment Member Project](https://img.shields.io/badge/Team123it%2010%20Years%20Appointment-Member%20Project-brightgreen)
[![License](https://img.shields.io/badge/license-Team123it--MIT%202.0-blue)](https://team123it.org/LICENSE.html)

Powerful Not-Only-Arcaea[^2] Visual Novel (VN) Script Creator written in Unity and pure C#.

[中文版](RepoDocs/README_zh.md) | English

**Copyright Tip: For DMCA takedown request, please see [License](#license) section**

</div>

## Discussion Area
Welcome to join ``Arcript`` Official QQ Group and Discord Server to chat everything about ``Arcript``!  
- Official Discord Server: *&lt;preparing&gt;*
- Official QQ Discussing Group: [827728743](http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=VswkWdzXWdcE4vp1I8HJ8umljGeXD-R8&authKey=sLS2vGE%2FM%2BDWdI0KebfcrTGQvlxxwW9E%2FqzyYgqF309jV6uDFCyR3Csl4zZMxD0f&noverify=0&group_code=827728743)

## Script Formats

``Arcript`` uses ``aspt``[^3] to store all VN script commands.Every ``aspt`` script should be built before they can be used in Arcaea or various supported VN game engine(s).  
``Arcript`` provides 2 candidates to build ``aspt`` scripts:

- ``.vns``: Plain Script/The original VN Script for Arcaea. Though it is plain script text, it is still not readable enough to be directly edited by human.
- ``.acpkg``: Binary Script/Arcript Script Package. It is binary-formatted & optimized for quick loading by supported VN game engine(s).

## Arcript+

Though the whole Arcript Project was started by inspiration from Arcaea VN Story, Arcript doesn't restrain herself to supporting Arcaea VN Story only.  
And that's why there is a group of features named ``Arcript+``.  

``Arcript+`` are the features that distincted from ``Arcript``.  
Most of them are designed for pure VN Game (such as ``Galgame``) development.  

**Caution: Arcript+ is not a special edition of Arcript! It is just a name for Arcript-only VN features**

Here are some reprenstative Arcript+ features:

- **Branch Select(選択支)**: Provides a way to let player choose their own story route. Usually used in Galgames for different endings. Here is an [example screenshot](https://i.imgur.com/0hzDE5l.png!/%5BArcript%5DArcript+%20Feature%20Preview%20(Branch%20Select))[^4] of Branch Select feature.
- **If Check Statements**: Provides a way to let script check the value of a variable and decide which branch to go. Usually used in Galgames for character favorability check.
- **Center Screen Text**: Provides a way to let script display text in the center of the screen. Usually used in Text Adventure VN Game for story plot setting (or neither dialogue nor thought text).
- **Video FullScreen Play**: Provides a way to play fullscreen video in VN Game. Usually used in Galgames for OP/ED video play.
- *...and more*

## Arcript Secondary Development(2nd-D) Interfaces

``Arcript`` provides several interfaces for 2nd-Ds. Here are examples of 2nd-D interfaces:

- [Arcript.Data.ICustomImageFormat](Assets/Scripts/Data/ICustomImageFormat.cs): Inherit this to create your own image format parser.  
  ``Arcript`` will attempt to find all classes with ``ICustomImageFormat`` interface and [ImgFormatExportAttribute](Assets/Scripts/Data/ImgFormatExportAttribute.cs) attribute in all loaded assembiles, and then load images using found classes.  
  - Example: [Arcript.FormatExt.Siglus](Assets/ArcriptPlugins/Arcript.FormatExt.Siglus)[^5]: A Siglus/RealLive[^6] image format (``.g00``) parser extension plugin for Arcript.
- *...more planning*

## Contributing Notices

``Arcript`` is still in early developing status. If you want to contribute to this project, just make a valuable [Pull Request](https://github.com/Misaka12456/Arcript/pulls) or [Issue](https://github.com/Misaka12456/Arcript/issues) to this project.  
After the first ``Arcript`` version (``0.1.0``) is released, main development for ``Arcript`` will be moved to ``develop`` branch.  
``master`` branch will only be used for releasing stable versions after it.  

## License

``Arcript`` main project is licensed under [123 Open-Sourced Organization MIT Public License v2](https://github.com/Team123it/team123it.github.io/blob/master/LICENSE.html).  
``Arcript`` plugins are licensed under their own licenses. See [ArcriptPlugins](Assets/ArcriptPlugins) for more information.  
<a id="dmcaRelated"></a>  
**It is possible that ``Arcript`` may use some textures with copyright during early developing. If this violates your and/or your organization's copyrights, please contact [developer](mailto:main@misakacastle.moe), we will remove them ASAP.**

## Thanks

Thanks all of the following projects for their great works:

- [morkt/GARbro](https://github.com/morkt/GARbro): Provides several custom-game-engine resource format parser prototypes.
- [xmoezzz/SiglusExtract](https://github.com/xmoezzz/SiglusExtract): A tool to extract resources from game using Siglus/RealLive[^6] VN Game Engine.
- [jp-netsis/RubyTextMeshPro](https://github.com/jp-netsis/RubyTextMeshPro): A Ruby Text (Furigana) support plugin for TextMeshPro.
- [Cysharp/UniTask](https://github.com/Cysharp/UniTask): A powerful async/await library for Unity.

And thanks to [all people](RepoDocs/THANKS.md) who supports to ``Arcript`` project.

<!-- References & Comments -->

[^1]: ``Arcaea`` is a rhythm game developed by Lowiro Limited. See [Arcaea Official Website](https://arcaea.lowiro.com/) for more information.
    
[^2]: Arcript+ supports the features that don't supported in Arcaea. These features are named as ``Arcript+``. You can also use ``Arcript+`` features to develop Visual Novel Games instead of put them into Arcaea only. See [Arcript+](#Arcript+) section for more information.
    
[^3]: ``aspt``: Arcript Plain VN Script, written in yaml.

[^4]: Example screenshot is a highly-restored screenshot of game 'floral·flowlove' by [Saga Planets](https://sagaplanets.product.co.jp/) inside ``Arcript``. See [floral·flowlove Official Website](https://sagaplanets.product.co.jp/works/flowlove/) and [Copyright Tip](#dmcaRelated) for more information.
    
[^5]: Internal Plugin ``Arcript.FormatExt.Siglus`` uses codes from ``GARbro project`` by morkt. See [GARbro Project Repository](https://github.com/morkt/GARbro) for more information.
    
[^6]: ``Siglus``, ``Siglus Engine``, ``RealLive`` and ``RealLive Engine`` are copyrighted trademarks of ``Key/Visual Art's``. See [Key/Visual Art&#39;s Official Website](https://key.visualarts.gr.jp/) for more information.
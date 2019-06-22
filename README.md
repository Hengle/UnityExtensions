# UnityExtensions
Unity 通用扩展，包括编辑器、运行时的即用扩展、API 扩展。
注意，源代码使用了最新的 C# 特性，作者只能保证在 Unity 2018.3 及以上版本可以正常编译（注意切换到 .NET 4.x），如果你想在旧版本中使用，请自行修复编译问题。

- 即用扩展
   - Unity 菜单：Assets->Create->Unity Extensions，你可能会用到 Layers 脚本自动生成功能。
   - Unity 菜单：Component->Unity Extensions，你可能会用到游戏对象池（Game Object Pool）、 FPS 显示、Tween 系统、Path 系统、本地化系统等功能。
   - Unity 菜单：Window->Unity Extensions，目前有一个测量工具，设计者可能会喜欢——比如解谜游戏设计者 :-)
   
- API 扩展
   - 编辑器：位于 UnityExtensions.Editor 命名空间。有一些实用工具，推荐你快速浏览一下，或许可以为你的开发节约时间。
   - 运行时（位于 UnityExtensions 命名空间）
      - Attributes：位于 RuntimeExtensions/Attributes 目录，有一些实用的 Attribute 帮助你快速定制编辑器。
      - 基类型：位于 RuntimeExtensions/BaseClasses 目录，你会喜欢 ScriptableAssetSingleton，可以帮你方便地做项目配置文件并在脚本中访问。
      - 扩展方法：位于 RuntimeExtensions/Extensions 目录，在你的代码中只要简单的添加 using UnityExtensions 即可使用这些扩展方法。
      - 状态机：位于 RuntimeExtensions/StateMachines 目录，包含一般状态机和专为 UI 设计的栈状态机。
      - 工具箱：位于 RuntimeExtensions/Utilities 目录，对应脚本中的各种 Kit 类。
      - 存档：位于 RuntimeExtensions/Save 目录，包含一个跨平台的存档系统（目前仅实现 Standardalone/FileSystem）。
      - 本地化：位于 RuntimeExtensions/Localization 目录，使用 Excel 开发本地化内容的本地化系统，支持玩家添加和修改语言。
      - 其他补充类型：位于 RuntimeExtensions/Supplements 目录，你可能会用到快速链表、树或者可创建实例的 Random 类型。
      
关于一些 API 的使用范例位于 UnityExtensions/Test 目录，如果你不清楚该怎么用的时候可以参考下。


<p align="center">
  <img src="https://github.com/yuyang9119/UnityExtensions/blob/master/Documents/StackStateMachine.gif"><br>
   栈状态机示例：使用栈状态机管理你的 UI 逻辑
</p>


<p align="center">
  <img src="https://github.com/yuyang9119/UnityExtensions/blob/master/Documents/Tween.gif"><br>
   Tween 动画系统编辑器：使用一个控制器控制多个动画效果，在编辑器下即时预览
</p>


<p align="center">
  <img src="https://github.com/yuyang9119/UnityExtensions/blob/master/Documents/Tween2.gif"><br>
   Tween 动画系统 UI 示例：通过 UI 事件系统可以方便的与 Tween 系统交互
</p>


<p align="center">
  <img src="https://github.com/yuyang9119/UnityExtensions/blob/master/Documents/TweenMaterialProperty.png"><br>
   Tween 动画系统材质属性：利用 MaterialPropertyBlock 实现的材质动画可避免复制材质，并且可自动识别属性数据类型
</p>


<p align="center">
  <img src="https://github.com/yuyang9119/UnityExtensions/blob/master/Documents/Path.gif"><br>
   Path 系统：包含 Bezier Path 和 Cardinal Path 两种路径，可以方便得使物体沿着路径移动
</p>

<p align="center">
  <img src="https://github.com/yuyang9119/UnityExtensions/blob/master/Documents/LocalizationExcel.png"><br>
   本地化系统：使用 Excel 开发本地化内容，发布后保留原始文件即可支持玩家 MOD
</p>

<p align="center">
  <img src="https://github.com/yuyang9119/UnityExtensions/blob/master/Documents/Localization.png"><br>
   本地化系统：在运行时切换语言
</p>


欢迎提交 Bug 报告，共同修复、改进这个通用扩展库。我是独立游戏开发者，如果你想和我联系，你可以在这些 QQ 群里找到我：

    - Unity 游戏开发：333763528
    - Unity 图形技术：371834666
    - 独立游戏开发：123020960

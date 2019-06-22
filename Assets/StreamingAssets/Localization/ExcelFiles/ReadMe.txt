
本地化系统
使用 Excel 表格开发本地化内容，然后导出配置和语言包。可配合 Unity UI 或 TextMeshPro 使用。支持玩家修改或添加语言。


===================================================================================================================================


Localization
|
|----ExcelFiles - 原始 Excel 表格文件夹，发布后删除可减小体积，但也不便于玩家制作自己的语言
|    |
|    |----Configuration.xlsx - 本地化配置表格文件
|    |    |
|    |    |----Fonts(sheet) - 字体表
|    |    |    |
|    |    |    |----Name(column) - 字体名称，字体名称不可重复，“@System”代表系统默认字体，目前仅 Unity UI 支持系统默认字体
|    |    |    |
|    |    |    |----ResourcePath(column) - 项目中字体资源路径，相对于 Resources 目录，运行时会根据需求自动加载或卸载字体资源
|    |    |
|    |    |----Languages(sheet) - 语言表
|    |         |
|    |         |----Type(row) - 语言类型，语言类型不可重复，当使用导出工具导出语言包时，语言类型作为语言包的文件名使用
|    |         |
|    |         |----Name(row) - 语言名称，用于在游戏中显示
|    |         |
|    |         |----#Menu, #Subtitle, ...(row) - 文本样式，文本样式不可重复，必须使用“#”开头，可自定义多个
|    |         |
|    |         |----FontName(row) - 文本样式的字体名称，必须从 Fonts 表格中选择
|    |         |
|    |         |----FontSize(row) - 文本样式的字体大小
|    |         |
|    |         |----Bold(row) - 文本样式的粗体选项
|    |         |
|    |         |----Italic(row) - 文本样式的斜体选项
|    |         |
|    |         |----Underline(row) - 文本样式的下划线选项，目前仅 TextMeshPro 支持
|    |         |
|    |         |----Strikethrough(row) - 文本样式的删除线选项，目前仅 TextMeshPro 支持
|    |         |
|    |         |----CaseMode(row) - 文本样式的大小写模式，目前仅 TextMeshPro 支持
|    |         |
|    |         |----CharacterSpacing(row) - 文本样式的字符间距调节，目前仅 TextMeshPro 支持
|    |         |
|    |         |----WordSpacing(row) - 文本样式的单词间距调节，目前仅 TextMeshPro 支持
|    |         |
|    |         |----LineSpacing(row) - 文本样式的行间距调节
|    |         |
|    |         |----ParagraphSpacing(row) - 文本样式的段落间距调节，目前仅 TextMeshPro 支持
|    |
|    |----Menus.xlsx, Subtitles.xlsx, ... - 任意名称和数量的本地化表格文件（可存放于子文件夹）
|         |
|         |----Scene1, Scene2, ...(sheet) - 本地化表，每个文件中可以含有多个本地化表
|              |
|              |----Name(column) - 本地化条目名称，这个名称在所有文件中都不可重复，留空则整行被忽略，不可含有“{”“}”“\”
|              |
|              |----Style(column) - 本地化条目文本样式，留空则不应用样式，否则必须使用语言表中定义的文本样式，“^”代表上一条目样式
|              |
|              |----zh-CN, en-US, ...(column) - 语言列，数量不限，必须使用语言表中定义的语言类型。使用“{...}”引用其他文本，转义符有“\n”“\t”“\\”“\{”
|
|----ExportTool - 导出工具文件夹，发布后可删除
|    |
|    |----ExportTool.exe - 导出工具，从资源浏览器中打开此程序可将 ExcelFiles 中的所有表格文件导出为游戏可用的配置和语言包文件
|
|----Configuration, zh-CN, en-US, ... - 由导出工具导出的配置和语言包文件


===================================================================================================================================


Q：我用的不是 Windows 系统，无法运行 ExportTool.exe 怎么办？

A：ExportTool 源代码可以在这里找到：https://github.com/yuyang9119/UnityExtensions。


Q：可以混合使用 Unity UI Text 和 TextMeshPro Text 吗？

A：不可以，你必须二选一。在项目的 Scripting Define Symbols 中添加 TEXT_MESH_PRO 以选择 TextMeshPro。

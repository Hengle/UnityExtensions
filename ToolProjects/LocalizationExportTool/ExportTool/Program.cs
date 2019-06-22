using System;
using System.IO;
using System.Text;
using System.Collections.Generic;


namespace ExportTool
{
    /// <summary>
    /// 注意：修改本程序可能同时需要修改 LocalizationManager
    /// </summary>
    static class Program
    {
        struct KeyValue<K, V>
        {
            public K key;
            public V value;
        }

        // 字体数据
        struct FontData
        {
            public string resourcePath;     // empty for system font
            public int fontIndex;           // for indexing
        }

        // 样式数据
        struct StyleData
        {
            public string fontName;
            public float fontSize;
            public int fontStyle;
            public float characterSpacing;
            public float wordSpacing;
            public float lineSpacing;
            public float paragraphSpacing;

            public const int boldBit = 0;
            public const int italicBit = 1;
            public const int underlineBit = 2;
            public const int lowerBit = 3;
            public const int upperBit = 4;
            public const int smallcapsBit = 5;
            public const int strikethroughBit = 6;
        }

        // 文本数据
        struct TextData
        {
            public int textIndex;
            public string styleName;        // nullable
        }

        // 语言数据
        struct LanguageData
        {
            public string name;
            public List<StyleData> styles;
            public List<string> texts;
        }


        static Dictionary<string, FontData> fonts = new Dictionary<string, FontData>();
        static Dictionary<string, LanguageData> languages = new Dictionary<string, LanguageData>();
        static Dictionary<string, int> styleNames = new Dictionary<string, int>();
        static Dictionary<string, TextData> textNames = new Dictionary<string, TextData>(1024);


        static void SetBit0(this ref int value, int bit) { value = value & (~(1 << bit)); }
        static void SetBit1(this ref int value, int bit) { value = value | (1 << bit); }
        static void SetBit(this ref int value, int bit, bool is1) { if (is1) value.SetBit1(bit); else value.SetBit0(bit); }


        static void ReadConfigurationExcel(string path)
        {
            // read fonts sheet
            void ReadFonts()
            {
                // skip first line
                ExcelReader.ReadLine();

                while (ExcelReader.ReadLine())
                {
                    string name = ExcelReader.GetTrimmedString(0);

                    FontData data;
                    if (name == "@System") data.resourcePath = string.Empty;
                    else data.resourcePath = ExcelReader.GetTrimmedString(1);
                    data.fontIndex = -1;

                    fonts.Add(name, data);
                }

                // indexing
                var order = new List<string>(fonts.Count);
                foreach (var item in fonts)
                {
                    order.Add(item.Key);
                }
                for (int i = 0; i < order.Count; i++)
                {
                    var value = fonts[order[i]];
                    value.fontIndex = i;
                    fonts[order[i]] = value;
                }
            }

            // read languages sheet
            void ReadLanguages()
            {
                // type
                ExcelReader.ReadLine();
                int count = ExcelReader.fieldCount;
                var languages = new KeyValue<string, LanguageData>[count];
                for (int i = 1; i < count; i++)
                {
                    var type = ExcelReader.GetTrimmedString(i);
                    languages[i].key = type;
                    languages[i].value.styles = new List<StyleData>(1024);
                    languages[i].value.texts = new List<string>(1024);
                }

                // name
                ExcelReader.ReadLine();
                for (int i = 0; i < count; i++)
                {
                    languages[i].value.name = ExcelReader.GetTrimmedString(i);
                }

                // style
                while (ExcelReader.ReadLine())
                {
                    var fieldName = ExcelReader.GetString(0)?.Trim();
                    if (string.IsNullOrEmpty(fieldName)) continue;

                    if (fieldName.StartsWith("#") && fieldName.Length > 1)
                    {
                        styleNames.Add(fieldName.Remove(0, 1), styleNames.Count);
                        for (int i = 1; i < count; i++)
                        {
                            languages[i].value.styles.Add(new StyleData());
                        }
                    }
                    else
                    {
                        int styleIndex = styleNames.Count - 1;

                        for (int i = 1; i < count; i++)
                        {
                            var style = languages[i].value.styles[styleIndex];

                            switch (fieldName)
                            {
                                case "FontName":
                                    style.fontName = ExcelReader.GetTrimmedString(i);
                                    break;
                                case "FontSize":
                                    style.fontSize = ExcelReader.GetFloat(i);
                                    break;
                                case "Bold":
                                    style.fontStyle.SetBit(StyleData.boldBit, ExcelReader.GetYesNo(i));
                                    break;
                                case "Italic":
                                    style.fontStyle.SetBit(StyleData.italicBit, ExcelReader.GetYesNo(i));
                                    break;
                                case "Underline":
                                    style.fontStyle.SetBit(StyleData.underlineBit, ExcelReader.GetYesNo(i));
                                    break;
                                case "Strikethrough":
                                    style.fontStyle.SetBit(StyleData.strikethroughBit, ExcelReader.GetYesNo(i));
                                    break;
                                case "CaseMode":
                                    switch (ExcelReader.GetTrimmedString(i).ToLower())
                                    {
                                        case "normal": break;
                                        case "lower": style.fontStyle.SetBit(StyleData.lowerBit, true); break;
                                        case "upper": style.fontStyle.SetBit(StyleData.upperBit, true); break;
                                        case "smallcaps": style.fontStyle.SetBit(StyleData.smallcapsBit, true); break;
                                        default: throw ExcelReader.Exception("Can't get CaseMode from column " + (i + 1));
                                    }
                                    break;
                                case "CharacterSpacing":
                                    style.characterSpacing = ExcelReader.GetFloat(i);
                                    break;
                                case "WordSpacing":
                                    style.wordSpacing = ExcelReader.GetFloat(i);
                                    break;
                                case "LineSpacing":
                                    style.lineSpacing = ExcelReader.GetFloat(i);
                                    break;
                                case "ParagraphSpacing":
                                    style.paragraphSpacing = ExcelReader.GetFloat(i);
                                    break;
                                default:
                                    throw ExcelReader.Exception("Invalid field name \"" + fieldName + "\"");
                            }

                            languages[i].value.styles[styleIndex] = style;
                        }
                    }
                }

                for (int i = 1; i < count; i++)
                {
                    Program.languages.Add(languages[i].key, languages[i].value);
                }
            }

            // read file
            ExcelReader.ReadFile(path, sheet =>
            {
                switch (sheet)
                {
                    case "Fonts":
                        ReadFonts();
                        break;

                    case "Languages":
                        ReadLanguages();
                        break;

                    default:
                        throw ExcelReader.Exception("Invalid sheet name: " + sheet);
                }
            });

            // validate font names
            foreach (var style in styleNames)
            {
                foreach (var lang in languages)
                {
                    var fontName = lang.Value.styles[style.Value].fontName;
                    if (!fonts.ContainsKey(fontName))
                        throw new Exception("Invalid font name: \"" + fontName + "\" of style \"" + style.Key + "\" of language \"" + lang.Key + "\"");
                }
            }
        }


        static void ReadLocalizationExcel(string path)
        {
            ExcelReader.ReadFile(path, sheet =>
            {
                // language texts list
                ExcelReader.ReadLine();
                int count = ExcelReader.fieldCount;
                var languageTexts = new List<string>[count];
                for (int i = 2; i < count; i++)
                {
                    languageTexts[i] = languages[ExcelReader.GetTrimmedString(i)].texts;
                }

                var textData = new TextData();

                while (ExcelReader.ReadLine())
                {
                    // text name
                    var name = ExcelReader.GetString(0)?.Trim();
                    if (string.IsNullOrEmpty(name)) continue;

                    if (name.Contains("{") || name.Contains("}") || name.Contains("\\"))
                        throw ExcelReader.Exception("Invalid text name: " + name);

                    // text style
                    var styleName = ExcelReader.GetString(1)?.Trim();
                    if (styleName != "^")
                    {
                        if (!string.IsNullOrEmpty(styleName) && !styleNames.ContainsKey(styleName))
                            throw ExcelReader.Exception("Invalid text style: " + styleName);

                        textData.styleName = styleName;
                    }

                    // add text item
                    textData.textIndex = textNames.Count;
                    textNames.Add(name, textData);
                    foreach (var lang in languages)
                    {
                        lang.Value.texts.Add(null);
                    }

                    // text content
                    for (int i = 2; i < count; i++)
                    {
                        languageTexts[i][textData.textIndex] = ExcelReader.GetString(i)??string.Empty;
                    }
                }
            });
        }


        static void ProcessTexts()
        {
            StringBuilder builder = new StringBuilder(1024);

            foreach (var lang in languages)
            {
                var texts = lang.Value.texts;

                // 先进行引用替换，直到没有引用为止（以处理嵌套引用）
                bool changed;
                do
                {
                    changed = false;
                    for (int i = 0; i < texts.Count; i++)
                    {
                        var text = texts[i];
                        if (string.IsNullOrEmpty(text)) continue;

                        int indexL = 0;
                        int indexR = 0;

                        while (indexL < text.Length && (indexL = text.IndexOf('{', indexL)) >= 0)
                        {
                            if (indexL == 0 || text[indexL - 1] != '\\')
                            {
                                indexR = text.IndexOf('}', indexL + 1);
                                if (indexR < 0)
                                    throw new Exception("Can't find paired '}' for '{'\n" + text);

                                var name = text.Substring(indexL + 1, indexR - indexL - 1);

                                // 替换文本
                                builder.Append(text, 0, indexL);
                                builder.Append(texts[textNames[name].textIndex]);
                                if (indexR < text.Length - 1) builder.Append(text, indexR + 1, text.Length - indexR - 1);
                                text = builder.ToString();
                                texts[i] = text;
                                builder.Clear();

                                changed = true;
                            }
                            else indexL++;
                        }
                    }

                } while (changed);

                // 然后处理转义符
                for (int i = 0; i < texts.Count; i++)
                {
                    var text = texts[i];
                    if (string.IsNullOrEmpty(text)) continue;

                    int index = text.IndexOf('\\');
                    if (index >= 0)
                    {
                        builder.Append(text);

                        for (; index < builder.Length; index++)
                        {
                            if (builder[index] == '\\')
                            {
                                if (index == builder.Length - 1)
                                    throw new Exception("Backslash shouldn't be last character of text\n" + text);

                                char c;
                                switch (builder[index + 1])
                                {
                                    case 'n': c = '\n'; break;
                                    case 't': c = '\t'; break;
                                    case '\\': c = '\\'; break;
                                    case '{': c = '{'; break;
                                    default:
                                        throw new Exception("Undfined backslash-conversion '\\" + builder[index + 1] + "'\n" + text);
                                }

                                builder[index] = c;
                                builder.Remove(index + 1, 1);
                            }
                        }

                        text = builder.ToString();
                        texts[i] = text;

                        builder.Clear();
                    }
                }
            }
        }


        static void WriteConfigurationFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    // fonts
                    writer.Write(fonts.Count);
                    foreach (var font in fonts)
                    {
                        writer.Write(font.Value.resourcePath);
                    }

                    // languages
                    writer.Write(languages.Count);
                    foreach (var lang in languages)
                    {
                        writer.Write(lang.Key);
                        writer.Write(lang.Value.name);

                        // styles
                        writer.Write(lang.Value.styles.Count);
                        foreach (var style in lang.Value.styles)
                        {
                            writer.Write(fonts[style.fontName].fontIndex);
                            writer.Write(style.fontSize);
                            writer.Write(style.fontStyle);
                            writer.Write(style.characterSpacing);
                            writer.Write(style.wordSpacing);
                            writer.Write(style.lineSpacing);
                            writer.Write(style.paragraphSpacing);
                        }
                    }

                    // text names
                    writer.Write(textNames.Count);
                    foreach (var text in textNames)
                    {
                        writer.Write(text.Key);
                        writer.Write(text.Value.textIndex);
                        writer.Write(string.IsNullOrEmpty(text.Value.styleName) ? -1 : styleNames[text.Value.styleName]);
                    }
                }
            }
        }


        static void WriteLanguageFile(string path, List<string> texts)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    foreach (var text in texts)
                    {
                        writer.Write(text);
                    }
                }
            }
        }


        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("----------------- START -----------------");
                Console.WriteLine();

                // 读配置表格
                Console.Write("Reading Configuration.xlsx...");
                ReadConfigurationExcel("../ExcelFiles/Configuration.xlsx");
                Console.WriteLine("Finished");

                // 读本地化表格
                var dir = new DirectoryInfo("../ExcelFiles");
                foreach (var file in dir.EnumerateFiles("*.xlsx", SearchOption.AllDirectories))
                {
                    if (file.Name != "Configuration.xlsx")
                    {
                        Console.Write("Reading " + file.Name + "...");
                        ReadLocalizationExcel(file.FullName);
                        Console.WriteLine("Finished");
                    }
                }

                // 转义、引用处理
                Console.Write("Processing Texts...");
                ProcessTexts();
                Console.WriteLine("Finished");

                // 写配置文件
                Console.Write("Writing Configuration...");
                WriteConfigurationFile("../Configuration");
                Console.WriteLine("Finished");

                // 写语言包
                foreach (var lang in languages)
                {
                    Console.Write("Writing Language [{0}]...", lang.Key);
                    WriteLanguageFile("../" + lang.Key, lang.Value.texts);
                    Console.WriteLine("Finished");
                }

                Console.WriteLine();
                Console.WriteLine("---------------- SUCCESS ----------------");
            }
            catch (Exception e)
            {
                Console.WriteLine("Unfinished");

                Console.WriteLine();
                Console.WriteLine("Exception details:");
                Console.WriteLine(e);

                Console.WriteLine();
                Console.WriteLine("---------------- FAILURE ----------------");
            }

            Console.ReadKey();
        }

    } // class Program

} // namespace ExportTool
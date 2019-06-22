using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

#if TEXT_MESH_PRO
using Font = TMPro.TMP_FontAsset;
using UIText = TMPro.TMP_Text;
#else
using Font = UnityEngine.Font;
using UIText = UnityEngine.UI.Text;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// 本地化管理器
    /// 在 Scripting Define Symbols 中添加 TEXT_MESH_PRO 以支持 Text Mesh Pro
    /// </summary>
    public struct LocalizationManager
    {
        // 字体数据
        struct FontData
        {
            public string resourcePath;         // empty for system font
            public Font font;
        }

        // 样式数据
        struct StyleData
        {
            public int fontIndex;
            public float fontSize;
            public int fontStyle;
            public float characterSpacing;
            public float wordSpacing;
            public float lineSpacing;
            public float paragraphSpacing;

            //const int boldBit = 0;
            //const int italicBit = 1;
            //const int underlineBit = 2;
            //const int lowerBit = 3;
            //const int upperBit = 4;
            //const int smallcapsBit = 5;
            //const int strikethroughBit = 6;

#if TEXT_MESH_PRO
            public void Apply(UIText target)
            {
                target.font = _fonts[fontIndex].font;
                target.fontSize = (int)fontSize;
                target.fontStyle = (TMPro.FontStyles)(fontStyle);
                target.characterSpacing = characterSpacing;
                target.wordSpacing = wordSpacing;
                target.lineSpacing = lineSpacing;
                target.paragraphSpacing = paragraphSpacing;
            }
#else
            public void Apply(UIText target)
            {
                target.font = _fonts[fontIndex].font;
                target.fontSize = (int)fontSize;
                target.fontStyle = (FontStyle)(fontStyle & 0b_11);
                target.lineSpacing = lineSpacing + 1;
            }
#endif
        }

        // 语言数据
        struct LanguageData
        {
            public string type;
            public string name;
            public StyleData[] styles;
        }

        // 文本数据
        struct TextData
        {
            public int textIndex;
            public int styleIndex;               // can be -1
        }


        static FontData[] _fonts;
        static LanguageData[] _languages;
        static Dictionary<string, TextData> _textNames;
        static string[] _texts;

        static bool _initialized = false;
        static int _languageIndex = -1;


        /// <summary>
        /// 语言发生改变时触发, arg1: old index(first time is -1), arg2: new index
        /// </summary>
        public static event Action<int, int> onLanguageChanged;


        /// <summary>
        /// 语言发生改变时触发（在 onLanguageChanged 之后触发）, arg1: old index(first time is -1), arg2: new index
        /// </summary>
        public static event Action<int, int> onLanguageChangedLate;


        /// <summary>
        /// 语言总数
        /// </summary>
        public static int languageCount
        {
            get
            {
                LoadConfiguration();
                return _languages.Length;
            }
        }


        /// <summary>
        /// 当前语言名称 (default is empty)
        /// </summary>
        public static string languageName
            => _languageIndex < 0 ? string.Empty : _languages[_languageIndex].name;


        /// <summary>
        /// 当前语言 Index (default is -1)
        /// 用户使用语言列表切换语言时使用
        /// 切换语言同时加载新语言使用到的所有字体、并卸载不再使用的所有字体
        /// 换语言会触发 onLanguageChanged
        /// </summary>
        public static int languageIndex
        {
            get => _languageIndex;
            set
            {
                LoadConfiguration();
                LoadLanguage(value);
            }
        }


        /// <summary>
        /// 当前语言类型 (default is empty)
        /// 第一次打开游戏时，可根据系统语言调用此方法；可用于存储用户配置
        /// 切换语言同时加载新语言使用到的所有字体、并卸载不再使用的所有字体
        /// 换语言会触发 onLanguageChanged
        /// </summary>
        public static string languageType
        {
            get => _languageIndex < 0 ? string.Empty : _languages[_languageIndex].type;
            set
            {
                LoadConfiguration();
                int index = Array.FindIndex(_languages, lang => lang.type == value);
                LoadLanguage(index);
            }
        }


        // 加载配置
        static void LoadConfiguration()
        {
            if (!_initialized)
            {
                var path = Application.streamingAssetsPath + "/Localization/Configuration";
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        _fonts = new FontData[reader.ReadInt32()];
                        for (int i = 0; i < _fonts.Length; i++)
                        {
                            _fonts[i].resourcePath = reader.ReadString();
                        }

                        _languages = new LanguageData[reader.ReadInt32()];
                        for (int i = 0; i < _languages.Length; i++)
                        {
                            _languages[i].type = reader.ReadString();
                            _languages[i].name = reader.ReadString();

                            _languages[i].styles = new StyleData[reader.ReadInt32()];
                            for (int j = 0; j < _languages[i].styles.Length; j++)
                            {
                                _languages[i].styles[j].fontIndex = reader.ReadInt32();
                                _languages[i].styles[j].fontSize = reader.ReadSingle();
                                _languages[i].styles[j].fontStyle = reader.ReadInt32();
                                _languages[i].styles[j].characterSpacing = reader.ReadSingle();
                                _languages[i].styles[j].wordSpacing = reader.ReadSingle();
                                _languages[i].styles[j].lineSpacing = reader.ReadSingle();
                                _languages[i].styles[j].paragraphSpacing = reader.ReadSingle();
                            }
                        }

                        int textCount = reader.ReadInt32();
                        _textNames = new Dictionary<string, TextData>(textCount);
                        _texts = new string[textCount];
                        TextData textData;
                        for (int i = 0; i < textCount; i++)
                        {
                            string name = reader.ReadString();
                            textData.textIndex = reader.ReadInt32();
                            textData.styleIndex = reader.ReadInt32();
                            _textNames.Add(name, textData);
                        }
                    }
                }

                _languages.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
                _initialized = true;
            }
        }


        // 加载指定语言
        static void LoadLanguage(int index)
        {
            if (_languageIndex != index)
            {
                // load language pack
                var path = Application.streamingAssetsPath + "/Localization/" + _languages[index].type;
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        for (int i = 0; i < _texts.Length; i++)
                        {
                            _texts[i] = reader.ReadString();
                        }
                    }
                }

                // find used fonts
                bool[] _fontInUse = new bool[_fonts.Length];
                StyleData[] styles = _languages[index].styles;
                for (int i = 0; i < styles.Length; i++)
                {
                    _fontInUse[styles[i].fontIndex] = true;
                }

                // load/unload font assets
                for (int i = 0; i < _fontInUse.Length; i++)
                {
                    if (_fontInUse[i])
                    {
                        if (_fonts[i].font == null)
                        {
                            if (string.IsNullOrEmpty(_fonts[i].resourcePath))
                            {
#if TEXT_MESH_PRO
                                // not supported
#else
                                _fonts[i].font = Font.CreateDynamicFontFromOSFont((string)null, 24);
#endif
                            }
                            else
                            {
                                _fonts[i].font = Resources.Load<Font>(_fonts[i].resourcePath);
                            }
                        }
                    }
                    else
                    {
                        if (_fonts[i].font != null)
                        {
                            if (string.IsNullOrEmpty(_fonts[i].resourcePath))
                            {
#if TEXT_MESH_PRO
                                // not supported
#else
                                Font.Destroy(_fonts[i].font);
#endif
                            }
                            else
                            {
                                Resources.UnloadAsset(_fonts[i].font);
                            }
                            _fonts[i].font = null;
                        }
                    }
                }

                int lastIndex = _languageIndex;
                _languageIndex = index;
                onLanguageChanged?.Invoke(lastIndex, index);
                onLanguageChangedLate?.Invoke(lastIndex, index);

                // 趁游戏卡住了顺便整理下内存
                GC.Collect();
            }
        }


        /// <summary>
        /// 获取语言类型
        /// </summary>
        public static string GetLanguageType(int index)
        {
            LoadConfiguration();
            return _languages[index].type;
        }


        /// <summary>
        /// 获取语言名称
        /// </summary>
        public static string GetLanguageName(int index)
        {
            LoadConfiguration();
            return _languages[index].name;
        }


        /// <summary>
        /// 根据文本名称获取文本内容（必须先设置一种语言）
        /// </summary>
        public static string GetText(string name)
        {
            return _texts[_textNames[name].textIndex];
        }


        /// <summary>
        /// 根据文本名称更新 UI 的文本和参数（必须先设置一种语言）
        /// </summary>
        public static void UpdateUI(UIText target, string name)
        {
            var textData = _textNames[name];
            target.text = _texts[textData.textIndex];

            if (textData.styleIndex >= 0)
                _languages[_languageIndex].styles[textData.styleIndex].Apply(target);
        }

    } // struct LocalizationManager

} // namespace UnityExtensions
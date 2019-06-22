using UnityEngine;
using UnityEngine.UI;

namespace UnityExtensions.Test
{
    public class Language : MonoBehaviour
    {
        public Dropdown languageList;
        public PlayableText tipText;


        static string GetDefaultLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    return "zh-CN";

                default:
                    return "en-US";
            }
        }


        private void Awake()
        {
            LocalizationManager.onLanguageChanged += (_, __) => tipText.RestartPlaying();

            LocalizationManager.languageType = GetDefaultLanguage();

            for (int i = 0; i < LocalizationManager.languageCount; i++)
            {
                languageList.options.Add(new Dropdown.OptionData(LocalizationManager.GetLanguageName(i)));
            }

            languageList.value = LocalizationManager.languageIndex;

            languageList.onValueChanged.AddListener(index => LocalizationManager.languageIndex = index);
        }
    }
}

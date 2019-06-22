#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor
{
    class LocalizationWindow : EditorWindow
    {
        [MenuItem("Window/Unity Extensions/Localization")]
        static void ShowWindow()
        {
            var window = GetWindow<LocalizationWindow>("Localization");

            window.Show();
        }


        void OnLanguageChanged(int _, int __)
        {
            Repaint();
        }


        void OnEnable()
        {
            LocalizationManager.onLanguageChanged += OnLanguageChanged;
        }


        void OnDisable()
        {
            LocalizationManager.onLanguageChanged -= OnLanguageChanged;
        }


        void OnGUI()
        {
            EditorGUILayout.Space();
            var rect = EditorGUILayout.GetControlRect();

            if (Application.isPlaying)
            {
                rect = EditorGUI.PrefixLabel(rect, EditorGUIKit.TempContent("Language"));

                string desc = string.Empty;
                if (LocalizationManager.languageIndex >= 0)
                {
                    desc = string.Format("{0} ({1})", LocalizationManager.languageName, LocalizationManager.languageType);
                }

                if (GUI.Button(rect, desc, EditorStyles.layerMaskField))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < LocalizationManager.languageCount; i++)
                    {
                        string text = string.Format("{0} ({1})", LocalizationManager.GetLanguageName(i), LocalizationManager.GetLanguageType(i));
                        int index = i;
                        menu.AddItem(new GUIContent(text), i == LocalizationManager.languageIndex, () => LocalizationManager.languageIndex = index);
                    }
                    menu.DropDown(rect);
                }
            }
            else
            {
                EditorGUI.LabelField(rect, "Can't set language in edit-mode");
            }
        }
    }
}

#endif
using UnityEngine;

#if TEXT_MESH_PRO
using UIText = TMPro.TMP_Text;
#else
using UIText = UnityEngine.UI.Text;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// UI 本地化组件. 使用时，在 UIText 组件上填写文本名称
    /// </summary>
    [AddComponentMenu("Unity Extensions/UI/Text Localization")]
    [RequireComponent(typeof(UIText))]
    public class TextLocalization : ScriptableComponent
    {
        public bool autoUpdateWidth;
        [Indent]
        public float extraWidth;

        public bool autoUpdateHeight;
        [Indent]
        public float extraHeight;


        string _textName;
        UIText _target;
        int _languageIndex = -1;


        void Awake()
        {
            _target = GetComponent<UIText>();

#if UNITY_EDITOR
            _textName = _target.text.Trim();
            if (_textName.Length != _target.text.Length)
            {
                Debug.LogError(string.Format("Text name can't start with or end with white-space characters. ({0})", _textName));
            }
#else
            _textName = _target.text;
#endif
        }


        void OnEnable()
        {
            if (_languageIndex != LocalizationManager.languageIndex)
            {
                UpdateLanguage(_languageIndex, LocalizationManager.languageIndex);
            }

            LocalizationManager.onLanguageChanged += UpdateLanguage;
        }


        void OnDisable()
        {
            LocalizationManager.onLanguageChanged -= UpdateLanguage;
        }


        protected virtual void UpdateLanguage(int oldLanguage, int newLanguage)
        {
            _languageIndex = newLanguage;
            LocalizationManager.UpdateUI(_target, _textName);

            if (autoUpdateWidth || autoUpdateHeight)
            {
                var size = this.rectTransform().sizeDelta;
                if (autoUpdateWidth) size.x = _target.preferredWidth + extraWidth;
                if (autoUpdateHeight) size.y = _target.preferredHeight + extraHeight;
                this.rectTransform().sizeDelta = size;
            }
        }


#if UNITY_EDITOR

        [CustomEditor(typeof(TextLocalization), true)]
        public class TextLocalizationEditor : BaseEditor<TextLocalization>
        {
            public override void OnInspectorGUI()
            {
                if (target._target == null) target._target = target.GetComponent<UIText>();

                if (target._target.text.Trim().Length != target._target.text.Length)
                {
                    EditorGUILayout.Space();
                    using (new GUIColorScope(new Color(1, 0.5f, 0.5f)))
                    {
                        if (EditorGUIKit.IndentedButton("Trim Text Name"))
                        {
                            Undo.RecordObject(target._target, "Trim Text Name");
                            target._target.text = target._target.text.Trim();

                            // force update UIText
                            if (target._target.enabled)
                            {
                                target._target.enabled = false;
                                target._target.enabled = true;
                            }
                        }
                    }

                    EditorGUILayout.Space();
                }

                base.OnInspectorGUI();
            }
        }

#endif

    } // class UILocalization

} // UnityExtensions
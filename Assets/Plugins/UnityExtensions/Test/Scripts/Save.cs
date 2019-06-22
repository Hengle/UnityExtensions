using UnityEngine;

namespace UnityExtensions.Test
{
    public class Save : ScriptableComponent
    {
        // GameSave
        BinarySavableCollection gameSave;
        ISaveTarget gameSaveTarget;

        // Settings
        TextSavableCollection settings;
        ISaveTarget settingsTarget;

        public static Save instance { get; private set; }


        void Start()
        {
            instance = this;

            // GameSave
            var gameSavables = new IBinarySavable[]
            {
                Player.instance,
            };
            gameSave = new BinarySavableCollection(gameSavables);

            // Settings
            var settingsSavables = new ITextSavable[]
            {
                new TextSavableField
                (
                    "Quality Level",
                    t => QualitySettings.SetQualityLevel(int.Parse(t)),
                    w => w.Write(QualitySettings.GetQualityLevel()),
                    () => QualitySettings.SetQualityLevel(0)
                ),
                new TextSavableField
                (
                    "Audio Volume",
                    t => AudioListener.volume = float.Parse(t),
                    w => w.Write(AudioListener.volume),
                    () => AudioListener.volume = 1
                ),
            };
            settings = new TextSavableCollection(settingsSavables);

            // platform-dependent
#if UNITY_PS4

            gameSaveTarget = new PS4SaveTarget("TestGameSave.bin");
            settingsTarget = new PS4SaveTarget("TestSettings.txt");

#else // Standalone

    #if STEAM

            gameSaveTarget = new SteamSaveTarget("TestGameSave.bin");
            settingsTarget = new FileSaveTarget("TestSettings.txt");

    #else // File System

            gameSaveTarget = new FileSaveTarget("TestGameSave.bin");
            settingsTarget = new FileSaveTarget("TestSettings.txt");

    #endif

#endif
        }


        public void SaveGame()
        {
            SaveManager.NewSaveTask(gameSave, gameSaveTarget);
        }


        public void LoadGame()
        {
            SaveManager.NewLoadTask(gameSave, gameSaveTarget);
        }


        public void SaveSettings()
        {
            SaveManager.NewSaveTask(settings, settingsTarget);
        }


        public void LoadSettings()
        {
            SaveManager.NewLoadTask(settings, settingsTarget);
        }


        void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(120));

            GUILayout.Label("Level: " + Player.instance.level);
            GUILayout.Label("HP: " + Player.instance.hp);

            if (GUILayout.Button("Load Game")) LoadGame();
            if (GUILayout.Button("Save Game")) SaveGame();

            GUILayout.Space(20);

            GUILayout.Label("Quality Level: " + QualitySettings.GetQualityLevel());
            GUILayout.Label("Audio Volume: " + AudioListener.volume);

            if (GUILayout.Button("Load Settings")) LoadSettings();
            if (GUILayout.Button("Save Settings")) SaveSettings();

#if UNITY_EDITOR
            GUILayout.Space(20);
            if (GUILayout.Button("Open Folder"))
                Editor.EditorApplicationKit.OpenFolder(Application.persistentDataPath);
#endif

            GUILayout.EndVertical();
        }
    }
}
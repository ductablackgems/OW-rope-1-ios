using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _0.DucLib.Scripts.Editor
{
    public class CustomBuildWindow : EditorWindow
    {
        private string cheatSymbol = "ENABLE_CHEAT";
        private string ignoreAdsSymbol = "IGNORE_ADS";
        private string IDEAVERSION = "IDEA_VERSION";
        private string CODEVER2 = "OLD_CODE";
        private string versionName = "";
        private string pass;
        private int versionCode = 0;
        private bool withCheat = false;
        private bool withIgnoreAds = false;
        private bool withIDEAVersion = false;
        private bool withCodeVer2 = false;

        private bool withDeveloperBuild;

        [MenuItem("CUSTOM BUILD/CUSTOM BUILD SETTINGS %#&C")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CustomBuildWindow));
        }

        private void OnGUI()
        {
            // =============== STYLE ==================
            var styleBtnRed = new GUIStyle(GUI.skin.button);
            styleBtnRed.normal.textColor = Color.red;
            styleBtnRed.active.textColor = Color.green;

            var styleTxtRed = new GUIStyle(GUI.skin.label);
            styleTxtRed.normal.textColor = new Color(0.780f, 0.0780f, 0.00f);
            styleTxtRed.fontStyle = FontStyle.Bold;

            var styleTxtYellow = new GUIStyle(GUI.skin.label);
            styleTxtYellow.normal.textColor = new Color(0.780f, 0.343f, 0.0312f);
            styleTxtYellow.fontStyle = FontStyle.Bold;

            var styleTxtBlue = new GUIStyle(GUI.skin.label);
            styleTxtBlue.normal.textColor = new Color(0.00f, 0.0112f, 0.670f);
            styleTxtBlue.fontStyle = FontStyle.Bold;

            var styleTxtGreen = new GUIStyle(GUI.skin.label);
            styleTxtGreen.normal.textColor = new Color(0.0265f, 0.530f, 0.144f);
            styleTxtGreen.fontStyle = FontStyle.Bold;
            // =================================


            //VERSION NAME =======================
            EditorGUILayout.BeginHorizontal();
            versionName = EditorPrefs.GetString($"{Application.productName}_gameVersion", Application.version);
            versionName = EditorGUILayout.TextField("Version", versionName);
            EditorPrefs.SetString($"{Application.productName}_gameVersion", versionName);
            if (GUILayout.Button("+", GUILayout.Width(50)))
            {
                IncVersionName();
            }

            EditorGUILayout.EndHorizontal();
            // =======================

            //VERSION CODE =======================
            EditorGUILayout.BeginHorizontal();
            versionCode = EditorPrefs.GetInt($"{Application.productName}_VersionCode", PlayerSettings.Android.bundleVersionCode);
            versionCode = int.Parse(EditorGUILayout.TextField("Version Code", versionCode.ToString()));
            EditorPrefs.SetInt($"{Application.productName}_VersionCode", versionCode);
            if (GUILayout.Button("+", GUILayout.Width(50)))
            {
                IncVersionCode();
            }

            EditorGUILayout.EndHorizontal();
            // =======================
            //
            //PASS =======================
            EditorGUILayout.BeginHorizontal();
            pass = EditorPrefs.GetString($"{Application.productName}_PassBuild", "");
            pass = EditorGUILayout.TextField("PassBuild", pass);
            EditorPrefs.SetString($"{Application.productName}_PassBuild", pass);
            EditorGUILayout.EndHorizontal();
            // =======================
            EditorGUILayout.BeginHorizontal();
            withCheat = HasSymbol(cheatSymbol);
            var cheatStyle = withCheat ? styleTxtRed : styleTxtGreen;
            EditorGUILayout.LabelField($"Cheat is {(withCheat ? "ON" : "OFF")}", cheatStyle);
            if (GUILayout.Button($"Turn Cheat {(withCheat ? "[OFF]" : "[ON]")}"))
            {
                ToggleCheat();
            }


            EditorGUILayout.EndHorizontal();
            // =======================


            EditorGUILayout.BeginHorizontal();
            withIgnoreAds = HasSymbol(ignoreAdsSymbol);
            var debugStyle = withIgnoreAds ? styleTxtRed : styleTxtGreen;
            EditorGUILayout.LabelField($"Ignore Ads is {(withIgnoreAds ? "ON" : "OFF")}", debugStyle);
            if (GUILayout.Button($"Turn Ignore Ads {(withIgnoreAds ? "[OFF]" : "[ON]")}"))
            {
                ToggleIgnoreAds();
            }

            EditorGUILayout.EndHorizontal();
            // =======================

            // =======================


            EditorGUILayout.BeginHorizontal();
            withIDEAVersion = HasSymbol(IDEAVERSION);
            var ugStyle = withIDEAVersion ? styleTxtRed : styleTxtGreen;
            EditorGUILayout.LabelField($"IDEA VER {(withIDEAVersion ? "ON" : "OFF")}", ugStyle);
            if (GUILayout.Button($"IDEA VER {(withIDEAVersion ? "[OFF]" : "[ON]")}"))
            {
                ToggleUpgradeGame();
            }

            EditorGUILayout.EndHorizontal();
            // =======================
            
            
            // =======================code ver 2


            EditorGUILayout.BeginHorizontal();
            withCodeVer2 = HasSymbol(CODEVER2);
            var codev2 = withIDEAVersion ? styleTxtRed : styleTxtGreen;
            EditorGUILayout.LabelField($"OLD CODE {(withCodeVer2 ? "ON" : "OFF")}", codev2);
            if (GUILayout.Button($"OLD CODE {(withCodeVer2 ? "[OFF]" : "[ON]")}"))
            {
                ToggleVer2();
            }

            EditorGUILayout.EndHorizontal();
            // =======================
            //

            // developer build
            EditorGUILayout.BeginHorizontal();
            withDeveloperBuild = EditorPrefs.GetInt("withDeveloperBuild", 1) == 1;

            EditorPrefs.SetInt("withDeveloperBuild", withDeveloperBuild ? 1 : 0);
            var devStyle = withDeveloperBuild ? styleTxtRed : styleTxtGreen;
            EditorGUILayout.LabelField($"Developer Build {(withDeveloperBuild ? "ON" : "OFF")}", devStyle);
            if (GUILayout.Button($"Turn Developer Build {(withDeveloperBuild ? "[OFF]" : "[ON]")}"))
            {
                withDeveloperBuild = !withDeveloperBuild;
                EditorPrefs.SetInt("withDeveloperBuild", withDeveloperBuild ? 1 : 0);
            }

            EditorGUILayout.EndHorizontal();

            if (EditorApplication.isCompiling) return;
            if (GUILayout.Button("Build Apk"))
            {
                BuildAPK();
            }

            if (GUILayout.Button("Build Android App Bundle"))
            {
                BuildAAB();
            }

            if (GUILayout.Button("Build And Run Apk"))
            {
                var build = EditorUtility.DisplayDialog("Build And Run", "Please Make Sure device is connected", "Ok",
                    "Cancel");
                if (build)
                {
                    BuildAPK(BuildOptions.AutoRunPlayer);
                }
            }

            if (GUILayout.Button("Build And Run Aab"))
            {
                var build = EditorUtility.DisplayDialog("Build And Run", "Please Make Sure device is connected", "Ok",
                    "Cancel");
                if (build)
                {
                    BuildAAB(BuildOptions.AutoRunPlayer);
                }
            }
        }

        private void BuildAAB(BuildOptions option = BuildOptions.None)
        {
            PlayerSettings.bundleVersion = versionName;
            PlayerSettings.Android.bundleVersionCode = versionCode;

            PlayerSettings.keystorePass = pass;
            PlayerSettings.keyaliasPass = pass;
            EditorUserBuildSettings.buildAppBundle = true;
            var isCheat = withCheat ? "CheatEnable" : "CheatDisable";
            var ignoreAds = !withIgnoreAds ? "EnableAds" : "DisableAds";
            var idea = withIDEAVersion ? "IdeaSupport" : "";
            
            var fixName = (!withCheat && !withIgnoreAds && !withDeveloperBuild && !withIDEAVersion)
                ? "ALPHA"
                : $"{isCheat}_{ignoreAds}";
            if (withIDEAVersion) fixName = idea;
            var fileName =
                $"{Application.productName}_ver{Application.version}_bundle{PlayerSettings.Android.bundleVersionCode}_{fixName}";
            fileName = fileName.Replace(" ", "").Replace(":", "");
            var ext = "aab";

            string productName = Application.productName;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                productName = productName.Replace(c, '_');
            }

            var customDirectory = $@"C:\Users\Admin\Documents\{productName}\{PlayerSettings.Android.bundleVersionCode}";
            if (!Directory.Exists(customDirectory))
            {
                Directory.CreateDirectory(customDirectory);
            }

            if (withDeveloperBuild)
            {
                option |= BuildOptions.Development;
            }

            var path = EditorUtility.SaveFilePanel("Save Location", customDirectory, fileName, ext);
            if (string.IsNullOrEmpty(path)) return;
            BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, option);
        }

        private void BuildAPK(BuildOptions option = BuildOptions.None)
        {
            PlayerSettings.keystorePass = pass;
            PlayerSettings.keyaliasPass = pass;
            PlayerSettings.bundleVersion = versionName;
            PlayerSettings.Android.bundleVersionCode = versionCode;
            EditorUserBuildSettings.buildAppBundle = false;
            // EditorUserBuildSettings.development = withDeveloperBuild;
            var isCheat = withCheat ? "CheatEnable" : "CheatDisable";
            // var isRelease = withRelease ? "PRODUCTION" : "ALPHA";
            var ignoreAds = !withIgnoreAds ? "EnableAds" : "DisableAds";
            var idea = withIDEAVersion ? "IdeaSupport" : "";
            var fixName = (!withCheat && !withIgnoreAds && !withDeveloperBuild && !withIDEAVersion) 
                ? "ALPHA"
                : $"{isCheat}_{ignoreAds}";
            if (withIDEAVersion) fixName = idea;
            var fileName =
                $"{Application.productName}_ver{Application.version}_bundle{PlayerSettings.Android.bundleVersionCode}_{fixName}";
            fileName = fileName.Replace(" ", "").Replace(":", "");
            var ext = "apk";

            string productName = Application.productName;
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                productName = productName.Replace(c, '_');
            }

            var customDirectory = $@"C:\Users\Admin\Documents\{productName}\{PlayerSettings.Android.bundleVersionCode}";
            if (!Directory.Exists(customDirectory))
            {
                Directory.CreateDirectory(customDirectory);
            }

            if (withDeveloperBuild)
            {
                option |= BuildOptions.Development;
            }

            var path = EditorUtility.SaveFilePanel("Save Location", customDirectory, fileName, ext);
            if (string.IsNullOrEmpty(path)) return;
            BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, option);
        }

        static string[] GetScenePaths()
        {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }

            return scenes;
        }


        private static bool HasSymbol(string symbol)
        {
            BuildTargetGroup btg;

#if UNITY_IOS
            btg = BuildTargetGroup.iOS;
#else
            btg = BuildTargetGroup.Android;
#endif

            string[] define;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(btg, out define);

            var listDefine = define.ToList();

            return listDefine.Contains(symbol);
        }

        private void ToggleCheat()
        {
            if (cheatSymbol == "") return;
            if (withCheat)
            {
                if (HasSymbol(cheatSymbol))
                {
                    RemoveSymbol(cheatSymbol);
                }
            }
            else
            {
                if (!HasSymbol(cheatSymbol))
                {
                    AddSymbol(cheatSymbol);
                }
            }
        }


        private void ToggleIgnoreAds()
        {
            if (ignoreAdsSymbol == "") return;
            if (withIgnoreAds)
            {
                if (HasSymbol(ignoreAdsSymbol))
                {
                    RemoveSymbol(ignoreAdsSymbol);
                }
            }
            else
            {
                if (!HasSymbol(ignoreAdsSymbol))
                {
                    AddSymbol(ignoreAdsSymbol);
                }
            }
        }

        private void ToggleUpgradeGame()
        {
            if (IDEAVERSION == "") return;
            if (withIDEAVersion)
            {
                if (HasSymbol(IDEAVERSION))
                {
                    RemoveSymbol(IDEAVERSION);
                }
            }
            else
            {
                if (!HasSymbol(IDEAVERSION))
                {
                    AddSymbol(IDEAVERSION);
                }
            }
        }

        
        private void ToggleVer2()
        {
            if (CODEVER2 == "") return;
            if (withCodeVer2)
            {
                if (HasSymbol(CODEVER2))
                {
                    RemoveSymbol(CODEVER2);
                }
            }
            else
            {
                if (!HasSymbol(CODEVER2))
                {
                    AddSymbol(CODEVER2);
                }
            }
        }
        private static void RemoveSymbol(string symbol)
        {
            BuildTargetGroup btg;

#if UNITY_IOS
            btg = BuildTargetGroup.iOS;
#else
            btg = BuildTargetGroup.Android;
#endif

            string[] define;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(btg, out define);

            var listDefine = define.ToList();

            if (listDefine.Contains(symbol))
            {
                Debug.Log($"Remove Script Symbol {symbol}");
                listDefine.Remove(symbol);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, listDefine.ToArray());
                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
            }
        }

        private static void AddSymbol(string symbol)
        {
            BuildTargetGroup btg;

#if UNITY_IOS
            btg = BuildTargetGroup.iOS;
#else
            btg = BuildTargetGroup.Android;
#endif

            string[] define;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(btg, out define);

            var listDefine = define.ToList();

            if (!listDefine.Contains(symbol))
            {
                Debug.Log($"Add Script Symbol :{symbol}");
                listDefine.Add(symbol);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, listDefine.ToArray());
                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
            }
        }

        private void IncVersionName()
        {
            string[] versionSplit = versionName.Split(".");
            if (versionSplit.Length == 3)
            {
                int nextPatchVer = int.Parse(versionSplit[2]) + 1;
                versionName = versionSplit[0] + "." + versionSplit[1] + "." + nextPatchVer;
                EditorPrefs.SetString($"{Application.productName}_gameVersion", versionName);
            }
            else
            {
                int nextPatchVer = int.Parse(versionName) + 1;
                versionName = nextPatchVer.ToString();
                EditorPrefs.SetString($"{Application.productName}_gameVersion", versionName);
            }

            OnGUI();
        }

        private void IncVersionCode()
        {
            versionCode = versionCode + 1;
            EditorPrefs.SetInt($"{Application.productName}_VersionCode", versionCode);
            OnGUI();
        }
    }
}
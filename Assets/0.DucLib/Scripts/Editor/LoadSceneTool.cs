using UnityEditor;
using UnityEditor.SceneManagement;

namespace _0.DucLib.Scripts.Editor
{
    public class LoadSceneTool
    {
        [MenuItem("Tools/Load Splash")]
        public static void LoadSplash()
        {
            OpenScene("Splash");
        }
        [MenuItem("Tools/Load Menu")]
        public static void LoadMenu()
        {
            OpenScene("Menu");
        }
        [MenuItem("Tools/Load Game")]
        public static void LoadGame()
        {
            OpenScene("SmallCity (Simple)");
        }
        
        [MenuItem("Tools/Demo Scene")]
        public static void LoadDemoScene()
        {
            OpenScene("DemoScene");
        }
        private static void OpenScene(string sceneName)
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    string currentSceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);

                    if (currentSceneName == sceneName)
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(scene.path);
                            return;
                        }
                    }
                }
            }
        }
    }
}
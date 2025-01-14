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
        [MenuItem("Tools/Load Mini Game")]
        public static void LoadMenu()
        {
            OpenScene("MiniGame");
        }
        [MenuItem("Tools/Load Gameplay")]
        public static void LoadGame()
        {
            OpenScene("SmallCity (Simple)");
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
using UnityEditor;
using UnityEngine;

namespace _0.DucLib.Scripts.Editor
{
    public class ClearScriptsTool : EditorWindow
    {
        [MenuItem("Tools/Clear All Scripts in Scene")]
        public static void ShowWindow()
        {
            GetWindow<ClearScriptsTool>("Clear Scripts Tool");
        }

        private void OnGUI()
        {
            GUILayout.Label("Xóa tất cả các script trong Scene", EditorStyles.boldLabel);
            GUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "Công cụ này sẽ xóa tất cả các Component là script (MonoBehaviour) được thêm vào các GameObject trong Scene hiện tại. Hãy cẩn thận khi sử dụng!",
                MessageType.Warning);

            if (GUILayout.Button("Xóa tất cả script trong Scene", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("Xác nhận",
                        "Bạn có chắc chắn muốn xóa tất cả các script trong Scene hiện tại?", "Có", "Không"))
                {
                    ClearAllScripts();
                }
            }
        }

        private static void ClearAllScripts()
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int removedCount = 0;

            foreach (GameObject obj in allObjects)
            {
                var components = obj.GetComponents<MonoBehaviour>();
                foreach (var component in components)
                {
                    if (component != null)
                    {
                        DestroyImmediate(component);
                        removedCount++;
                    }
                }
            }

            Debug.Log($"Đã xóa {removedCount} script khỏi Scene.");
            EditorUtility.DisplayDialog("Hoàn thành", $"Đã xóa {removedCount} script khỏi Scene.", "OK");
        }
    }
}


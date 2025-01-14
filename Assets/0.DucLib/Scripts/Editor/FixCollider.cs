using UnityEditor;
using UnityEngine;

namespace _0.DucLib.Scripts.Editor
{
    public class FixCollider : EditorWindow
    {
        [MenuItem("MyTools/DestroyColliderGameObject")]
        private static void DestroyGameObject()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            for (var i = 0; i < gameObjects.Length; i++)
            {
                GameObject gameObject = gameObjects[i];
                Transform[] transforms = gameObject.transform.GetComponentsInChildren<Transform>();
                GameObject mesh = null;
                for (var i1 = 0; i1 < transforms.Length; i1++)
                {
                    Transform transform = transforms[i1];
                    if (transform.name.Equals(gameObject.name))
                        mesh = transform.gameObject;
                    if (transform.name.Contains("Collider"))
                    {
                        Debug.Log($"Destroy: {transform.name}");
                        Undo.DestroyObjectImmediate(transform.gameObject);
                    }
                }

                MeshCollider mCollider = null;
                if (!mesh.TryGetComponent(out mCollider))
                    mCollider = mesh.AddComponent<MeshCollider>();
                mCollider.sharedMesh = mesh.GetComponent<MeshFilter>().sharedMesh;
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}
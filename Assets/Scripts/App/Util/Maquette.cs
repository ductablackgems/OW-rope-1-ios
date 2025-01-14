using UnityEngine;

namespace App.Util
{
	public class Maquette : MonoBehaviour
	{
		public GameObject replacementPrefab;

		protected void Awake()
		{
			if (replacementPrefab == null)
			{
				UnityEngine.Debug.LogWarning("Replacement prefab is unassigned");
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(replacementPrefab, base.transform.parent);
            gameObject.transform.localPosition = base.transform.localPosition;
            gameObject.transform.localRotation = base.transform.localRotation;
            gameObject.transform.localScale = base.transform.localScale;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}

using UnityEngine;

namespace Hero.Respawning.Snap
{
	public class GroupSnapperScript : MonoBehaviour
	{
		public SnapScriptableObject snapData;

		public SnappedObject[] snappedObjects = new SnappedObject[1];

		public void ApplySnap(string snapName, bool registerUndo = false)
		{
			int num = 0;
			string[] snapNames = snapData.snapNames;
			for (int i = 0; i < snapNames.Length; i++)
			{
				if (snapNames[i] == snapName)
				{
					ApplySnap(num, registerUndo);
					return;
				}
				num++;
			}
			UnityEngine.Debug.LogError($"Snap '{snapName}' was not found.");
		}

		public void ApplySnap(int snapIndex, bool registerUndo = false)
		{
			SnappedObject[] array = snappedObjects;
			foreach (SnappedObject snappedObject in array)
			{
				SnapGroup group;
				if (!(snappedObject.gameObject == null) && snapData.TryGetBySnappedObject(snappedObject, out group) && group.TryGetByIndex(snapIndex, out SnapItem snap))
				{
					snappedObject.gameObject.transform.localPosition = snap.position;
					snappedObject.gameObject.transform.localRotation = snap.rotation;
				}
			}
		}

		public void SnapToIndex(int index)
		{
			SnappedObject[] array = snappedObjects;
			foreach (SnappedObject snappedObject in array)
			{
				if (snappedObject != null && !(snappedObject.gameObject == null))
				{
					SnapItem snapItem = default(SnapItem);
					snapItem.index = index;
					snapItem.position = snappedObject.gameObject.transform.localPosition;
					snapItem.rotation = snappedObject.gameObject.transform.localRotation;
					snapData.SaveSnap(snapItem, snappedObject);
				}
			}
		}
	}
}

using System;
using UnityEngine;

namespace Hero.Respawning.Snap
{
	public class SnapScriptableObject : ScriptableObject
	{
		public string[] snapNames = new string[1];

		public SnapGroup[] groups;

		public void SaveSnap(SnapItem snapItem, SnappedObject snappedObject)
		{
			if (!TryGetBySnappedObject(snappedObject, out SnapGroup group))
			{
				Array.Resize(ref groups, groups.Length + 1);
				group = new SnapGroup();
				group.tid = snappedObject.tid;
				groups[groups.Length - 1] = group;
			}
			group.SaveSnap(snapItem);
		}

		public bool TryGetBySnappedObject(SnappedObject snappedObject, out SnapGroup group)
		{
			SnapGroup[] array = groups;
			foreach (SnapGroup snapGroup in array)
			{
				if (snapGroup.GetTidHash() == snappedObject.GetTidHash())
				{
					group = snapGroup;
					return true;
				}
			}
			group = null;
			return false;
		}
	}
}

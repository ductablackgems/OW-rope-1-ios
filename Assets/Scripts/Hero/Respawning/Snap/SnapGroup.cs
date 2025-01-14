using System;

namespace Hero.Respawning.Snap
{
	[Serializable]
	public class SnapGroup
	{
		public string tid;

		public SnapItem[] snaps = new SnapItem[0];

		private bool tidHashLoaded;

		private int tidHash;

		public int GetTidHash()
		{
			if (!tidHashLoaded)
			{
				tidHash = tid.GetHashCode();
				tidHashLoaded = true;
			}
			return tidHash;
		}

		public void SaveSnap(SnapItem snap)
		{
			if (TryGetByIndex(snap.index, out SnapItem _, out int arrayIndex))
			{
				snaps[arrayIndex] = snap;
				return;
			}
			Array.Resize(ref snaps, snaps.Length + 1);
			snaps[snaps.Length - 1] = snap;
		}

		public bool TryGetByIndex(int snapIndex, out SnapItem snap)
		{
			int arrayIndex;
			return TryGetByIndex(snapIndex, out snap, out arrayIndex);
		}

		private bool TryGetByIndex(int snapIndex, out SnapItem snap, out int arrayIndex)
		{
			int num = 0;
			SnapItem[] array = snaps;
			foreach (SnapItem snapItem in array)
			{
				if (snapItem.index == snapIndex)
				{
					snap = snapItem;
					arrayIndex = num;
					return true;
				}
				num++;
			}
			snap = default(SnapItem);
			arrayIndex = 0;
			return false;
		}
	}
}

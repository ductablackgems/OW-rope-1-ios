using System;
using System.Collections.Generic;
using UnityEngine;

namespace Forge3D
{
	[Serializable]
	public class F3DTurretScriptable : ScriptableObject
	{
		public List<TurretStructure> Turrets = new List<TurretStructure>();

		public List<GameObject> Bases = new List<GameObject>();

		public List<GameObject> Swivels = new List<GameObject>();

		public string SwivelPrefix = "*SOCKET_SWIVEL_";

		public List<GameObject> Heads = new List<GameObject>();

		public string HeadPrefix = "*SOCKET_HEAD_";

		public List<GameObject> Mounts = new List<GameObject>();

		public string MountPrefix = "*SOCKET_MOUNT_";

		public List<GameObject> Breeches = new List<GameObject>();

		public string BarrelPrefix = "*SOCKET_BARREL_";

		public List<GameObject> Barrels = new List<GameObject>();

		public int SelectedTurret;

		public string WeaponSocket = "*SOCKET_WEAPON_";
	}
}

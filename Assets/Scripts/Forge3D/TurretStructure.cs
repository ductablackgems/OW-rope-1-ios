using System;
using System.Collections.Generic;
using UnityEngine;

namespace Forge3D
{
	[Serializable]
	public class TurretStructure
	{
		public bool NeedLOD;

		public string Name = "Turret";

		public GameObject Base;

		public GameObject Swivel;

		public string SwivelPrefix = "*SOCKET_SWIVEL_";

		public GameObject Head;

		public string HeadPrefix = "*SOCKET_HEAD_";

		public GameObject Mount;

		public string MountPrefix = "*SOCKET_MOUNT_";

		public GameObject Breech;

		public string BarrelPrefix = "*SOCKET_";

		public List<string> WeaponSlotsNames = new List<string>();

		public List<GameObject> WeaponSlots = new List<GameObject>();

		public List<GameObject> WeaponBreeches = new List<GameObject>();

		public List<string> WeaponBarrelSockets = new List<string>();

		public List<GameObject> WeaponBarrels = new List<GameObject>();

		public bool HasTurretScript;
	}
}

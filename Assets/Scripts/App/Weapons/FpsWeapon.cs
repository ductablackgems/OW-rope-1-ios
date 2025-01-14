using UnityEngine;
using UnityEngine.Serialization;

namespace App.Weapons
{
	public class FpsWeapon : MonoBehaviour
	{
		[FormerlySerializedAs("Type")]
		public GunType type;

		[FormerlySerializedAs("FieldOfView")]
		public float fieldOfView = 30f;
	}
}

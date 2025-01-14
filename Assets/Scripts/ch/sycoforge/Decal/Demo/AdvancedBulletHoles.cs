using UnityEngine;

namespace ch.sycoforge.Decal.Demo
{
	public class AdvancedBulletHoles : MonoBehaviour
	{
		public EasyDecal DecalPrefab;

		public GameObject ImpactParticles;

		public float CastRadius = 0.25f;

		private void Start()
		{
			if (DecalPrefab == null)
			{
				UnityEngine.Debug.LogError("The AdvancedBulletHoles script has no decal prefab attached.");
			}
			EasyDecal.HideMesh = false;
		}

		private void Update()
		{
			if (!Input.GetMouseButtonUp(0))
			{
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
			if (!Physics.Raycast(ray, out RaycastHit hitInfo, 200f))
			{
				return;
			}
			GameObject gameObject = hitInfo.collider.gameObject;
			Vector3 point = hitInfo.point;
			RaycastHit[] array = Physics.SphereCastAll(ray, CastRadius, Vector3.Distance(Camera.main.transform.position, point) + 2f);
			Vector3 a = hitInfo.normal;
			if (array.Length != 0)
			{
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit = array2[i];
					UnityEngine.Debug.DrawLine(ray.origin, raycastHit.point, Color.red);
					a += raycastHit.normal;
				}
			}
			a /= array.Length + 1;
			EasyDecal.ProjectAt(DecalPrefab.gameObject, gameObject, point, a);
			if (ImpactParticles != null)
			{
				Quaternion rotation = Quaternion.FromToRotation(Vector3.up, a);
				Object.Instantiate(ImpactParticles, point, rotation);
			}
		}
	}
}

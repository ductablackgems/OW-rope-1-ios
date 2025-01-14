using UnityEngine;

namespace App.Weapons
{
	public class VirtualTarget : MonoBehaviour
	{
		private RaycastHit hit;

		private Transform crossHair;

		private UnityEngine.Camera _camera;

		public bool Hitting
		{
			get;
			private set;
		}

		public RaycastHit Hit => hit;

		protected void Start()
		{
			crossHair = ServiceLocator.GetGameObject("CrossHair").transform;
			_camera = ServiceLocator.GetGameObject("MainCamera").GetComponent<UnityEngine.Camera>();
		}

		protected void FixedUpdate()
		{
			Vector3 vector = new Vector3(crossHair.position.x, crossHair.position.y, _camera.nearClipPlane);
			Ray ray = _camera.ScreenPointToRay(vector);
			int layerMask = 1061889;
			if (Physics.Raycast(ray, out hit, 10000f, layerMask))
			{
				UnityEngine.Debug.DrawLine(vector, hit.point, Color.green);
				base.transform.position = hit.point;
				Hitting = true;
			}
			else
			{
				Vector3 vector2 = ray.origin + ray.direction * 1000f;
				UnityEngine.Debug.DrawLine(vector, vector2, Color.red);
				base.transform.position = vector2;
				Hitting = false;
			}
		}
	}
}

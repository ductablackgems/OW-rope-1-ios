using UnityEngine;

namespace App.Util
{
	public class ArrowGizmo : MonoBehaviour
	{
		private void OnDrawGizmos()
		{
			DrawArrow.ForGizmo(base.transform.position, base.transform.forward, 0.5f);
		}
	}
}

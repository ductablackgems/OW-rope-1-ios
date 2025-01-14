using UnityEngine;

namespace MagicalFXMagicalEffect
{
	public class RaisingWall : MonoBehaviour
	{
		public GameObject Skill;

		public float Offset = -7f;

		public float Distance = 2f;

		private void Start()
		{
			if (Skill != null)
			{
				FX_SpawnDirection component = Skill.GetComponent<FX_SpawnDirection>();
				if ((bool)component)
				{
					Offset = -(int)((float)component.Number / 2f);
				}
			}
			Raising();
		}

		private void Raising()
		{
			if (Skill != null)
			{
				Object.Instantiate(Skill, base.transform.position + base.transform.forward * Distance + base.transform.right * Offset, Skill.transform.rotation).transform.forward = base.transform.right;
			}
		}
	}
}

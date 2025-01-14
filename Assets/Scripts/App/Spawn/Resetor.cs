using UnityEngine;

namespace App.Spawn
{
	public class Resetor : MonoBehaviour
	{
		private IResetable[] resetables;

		public void ResetStates()
		{
			if (resetables == null)
			{
				resetables = GetComponentsInChildren<IResetable>(includeInactive: true);
			}
			IResetable[] array = resetables;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResetStates();
			}
		}
	}
}

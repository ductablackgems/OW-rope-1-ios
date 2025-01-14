using System.Collections.Generic;

namespace App.Spawn.Pooling
{
	public class HumanPooler : Pooler<Resetor>
	{
		public override Resetor Pop()
		{
			Resetor resetor = base.Pop();
			if (resetor == null)
			{
				return null;
			}
			resetor.ResetStates();
			return resetor;
		}

		public void GetSpawned(List<Resetor> list)
		{
			foreach (Resetor allItem in base.AllItems)
			{
				if (!(allItem.transform.parent != null))
				{
					list.Add(allItem);
				}
			}
		}
	}
}

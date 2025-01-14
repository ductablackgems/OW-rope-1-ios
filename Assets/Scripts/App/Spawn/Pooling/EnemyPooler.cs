using UnityEngine;

namespace App.Spawn.Pooling
{
	public class EnemyPooler : MonoBehaviour
	{
		public HumanPooler civilPooler;

		public HumanPooler policePooler;

		public HumanPooler rescuerPooler;

		private bool reserved;

		private EnemyType reservedType;

		public void ReserveItem(EnemyType type)
		{
			reserved = true;
			reservedType = type;
		}

		public void ClearReservation()
		{
			reserved = false;
		}

		public bool HasCiviles(bool includeReserved = false)
		{
			return HasAvailableItem(includeReserved, EnemyType.Civil, civilPooler);
		}

		public bool HasPolices(bool includeReserved = false)
		{
			return HasAvailableItem(includeReserved, EnemyType.Police, policePooler);
		}

		public bool HasRescuers(bool includeReserved = false)
		{
			return HasAvailableItem(includeReserved, EnemyType.Rescuer, rescuerPooler);
		}

		public Resetor Pop(EnemyType type = EnemyType.Civil)
		{
			HumanPooler pooler = GetPooler(type);
			Resetor resetor = (pooler != null) ? pooler.Pop() : null;
			if (resetor == null)
			{
				return null;
			}
			resetor.ResetStates();
			return resetor;
		}

		public void Push(Resetor enemyResetor)
		{
			if (policePooler.OwningItem(enemyResetor))
			{
				policePooler.Push(enemyResetor);
			}
			else if (rescuerPooler.OwningItem(enemyResetor))
			{
				rescuerPooler.Push(enemyResetor);
			}
			else
			{
				civilPooler.Push(enemyResetor);
			}
		}

		public HumanPooler GetPooler(EnemyType type)
		{
			switch (type)
			{
			case EnemyType.Civil:
				return civilPooler;
			case EnemyType.Police:
				return policePooler;
			case EnemyType.Rescuer:
				return rescuerPooler;
			default:
				UnityEngine.Debug.LogFormat("Not Supported EnemyType {0}", type);
				return null;
			}
		}

		private bool HasAvailableItem(bool includeReserved, EnemyType type, HumanPooler pooler)
		{
			if (pooler.Count() <= 1)
			{
				if (!pooler.IsEmpty())
				{
					if (!includeReserved && reserved)
					{
						return reservedType != type;
					}
					return true;
				}
				return false;
			}
			return true;
		}
	}
}

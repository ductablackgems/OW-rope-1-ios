using System.Collections.Generic;
using UnityEngine;

namespace App.Effects
{
	public class EffectsManager : MonoBehaviour
	{
		private class EffectCache
		{
			public Transform Parent;

			public GameplayEffect Prefab;

			public List<GameplayEffect> Used = new List<GameplayEffect>(4);

			private List<GameplayEffect> Cache = new List<GameplayEffect>(4);

			public GameplayEffect Get()
			{
				GameplayEffect gameplayEffect = null;
				if (Cache.Count > 0)
				{
					int index = Cache.Count - 1;
					gameplayEffect = Cache[index];
					Cache.RemoveAt(index);
				}
				else
				{
					gameplayEffect = UnityEngine.Object.Instantiate(Prefab, Parent);
					gameplayEffect.Initialize(Return);
				}
				Used.Add(gameplayEffect);
				return gameplayEffect;
			}

			private void Return(GameplayEffect effect)
			{
				Cache.Add(effect);
				Used.Remove(effect);
			}
		}

		public GameplayEffect objectiveEffectPrefab;

		public GameplayEffect arrowEffectPrefab;

		public GameplayEffect npcMarkEffectPrefab;

		private List<EffectCache> caches = new List<EffectCache>(4);

		public GameplayEffect GetEffect(GameplayEffect prefab)
		{
			EffectCache effectCache = caches.Find((EffectCache obj) => obj.Prefab == prefab);
			if (effectCache == null)
			{
				effectCache = new EffectCache
				{
					Parent = base.transform.parent,
					Prefab = prefab
				};
				caches.Add(effectCache);
			}
			return effectCache.Get();
		}
	}
}

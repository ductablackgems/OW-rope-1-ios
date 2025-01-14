using LlockhamIndustries.ExtensionMethods;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
	public class PoolItem
	{
		private ProjectionPool pool;

		private ProjectionRenderer renderer;

		public ProjectionPool Pool => pool;

		public ProjectionRenderer Renderer => renderer;

		private bool Valid
		{
			get
			{
				if (renderer == null)
				{
					if (pool.activePool != null)
					{
						pool.activePool.Remove(this);
					}
					if (pool.inactivePool != null)
					{
						pool.inactivePool.Remove(this);
					}
					return false;
				}
				return true;
			}
		}

		public PoolItem(ProjectionPool Pool)
		{
			pool = Pool;
			GameObject gameObject = new GameObject("Projection");
			gameObject.transform.SetParent(pool.Parent);
			gameObject.SetActive(value: false);
			renderer = gameObject.AddComponent<ProjectionRenderer>();
			renderer.PoolItem = this;
		}

		internal void Initialize(ProjectionRenderer Renderer = null, bool IncludeBehaviours = false)
		{
			if (!Valid)
			{
				return;
			}
			renderer.transform.SetParent(pool.Parent);
			if (Renderer != null)
			{
				renderer.Projection = Renderer.Projection;
				renderer.Tiling = Renderer.Tiling;
				renderer.Offset = Renderer.Offset;
				renderer.MaskMethod = Renderer.MaskMethod;
				renderer.MaskLayer1 = Renderer.MaskLayer1;
				renderer.MaskLayer2 = Renderer.MaskLayer2;
				renderer.MaskLayer3 = Renderer.MaskLayer3;
				renderer.MaskLayer4 = Renderer.MaskLayer4;
				renderer.Properties = Renderer.Properties;
				if (IncludeBehaviours)
				{
					MonoBehaviour[] components = Renderer.GetComponents<MonoBehaviour>();
					foreach (MonoBehaviour monoBehaviour in components)
					{
						if (!(monoBehaviour.GetType() == typeof(Transform)) && !(monoBehaviour.GetType() == typeof(ProjectionRenderer)))
						{
							renderer.gameObject.AddComponent(monoBehaviour).enabled = monoBehaviour.enabled;
						}
					}
				}
				renderer.transform.localScale = Renderer.transform.localScale;
				renderer.gameObject.layer = Renderer.gameObject.layer;
				renderer.gameObject.tag = Renderer.gameObject.tag;
			}
			else
			{
				renderer.transform.localScale = Vector3.one;
			}
			renderer.gameObject.SetActive(value: true);
		}

		internal void Terminate()
		{
			if (!Valid)
			{
				return;
			}
			renderer.gameObject.SetActive(value: false);
			Component[] components = renderer.gameObject.GetComponents<Component>();
			foreach (Component component in components)
			{
				if (!(component.GetType() == typeof(Transform)) && !(component.GetType() == typeof(ProjectionRenderer)))
				{
					UnityEngine.Object.Destroy(component);
				}
			}
			renderer.transform.SetParent(pool.Parent);
		}

		public void Return()
		{
			pool.activePool.Remove(this);
			Terminate();
			if (pool.inactivePool == null)
			{
				pool.inactivePool = new List<PoolItem>();
			}
			pool.inactivePool.Add(this);
		}
	}
}

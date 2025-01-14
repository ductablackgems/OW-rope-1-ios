using System.Collections.Generic;
using UnityEngine;

namespace App
{
	public class WhoIs
	{
		public static WhoIsEntities Entities;

		public static WhoIsMasks Masks;

		public static void Initialize()
		{
			Entities = new WhoIsEntities();
			Masks = new WhoIsMasks(Entities);
		}

		public static bool Compare(Collider collider, WhoIsItemDefinition definition, HashSet<Transform> usedTransforms = null)
		{
			return !Resolve(collider, definition, usedTransforms).IsEmpty;
		}

		public static bool Compare(Collider collider, WhoIsItemDefinition[] mask, HashSet<Transform> usedTransforms = null)
		{
			return !Resolve(collider, mask, usedTransforms).IsEmpty;
		}

		public static WhoIsResult Resolve(Collider collider, WhoIsItemDefinition definition, HashSet<Transform> usedTransforms = null)
		{
			if (definition.hasColliderOnRoot && collider.CompareTag(definition.rootTag))
			{
				return CreateResult(collider.transform, definition, usedTransforms);
			}
			if (definition.hasChild)
			{
				bool flag = false;
				for (int i = 0; i < definition.childTags.Length; i++)
				{
					if (collider.CompareTag(definition.childTags[i]))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					Transform parent = collider.transform.parent;
					while (parent != null)
					{
						if (parent.CompareTag(definition.rootTag))
						{
							return CreateResult(parent, definition, usedTransforms);
						}
						parent = parent.parent;
					}
				}
			}
			return default(WhoIsResult);
		}

		public static WhoIsResult Resolve(Collider collider, WhoIsItemDefinition[] mask, HashSet<Transform> usedTransforms = null)
		{
			for (int i = 0; i < mask.Length; i++)
			{
				WhoIsResult result = Resolve(collider, mask[i], usedTransforms);
				if (!result.IsEmpty)
				{
					return result;
				}
			}
			return default(WhoIsResult);
		}

		public static bool CompareCollision(Collision collision, WhoIsItemDefinition definition, HashSet<Transform> usedTransforms = null)
		{
			return !ResolveCollision(collision, definition, usedTransforms).IsEmpty;
		}

		public static bool CompareCollision(Collision collision, WhoIsItemDefinition[] mask, HashSet<Transform> usedTransforms = null)
		{
			return !ResolveCollision(collision, mask, usedTransforms).IsEmpty;
		}

		public static WhoIsResult ResolveCollision(Collision collision, WhoIsItemDefinition definition, HashSet<Transform> usedTransforms = null)
		{
			Transform transform = collision.transform;
			if (transform.CompareTag(definition.rootTag))
			{
				return CreateResult(transform, definition, usedTransforms);
			}
			return default(WhoIsResult);
		}

		public static WhoIsResult ResolveCollision(Collision collision, WhoIsItemDefinition[] mask, HashSet<Transform> usedTransforms = null)
		{
			Transform transform = collision.transform;
			foreach (WhoIsItemDefinition whoIsItemDefinition in mask)
			{
				if (transform.CompareTag(whoIsItemDefinition.rootTag))
				{
					return CreateResult(transform, whoIsItemDefinition, usedTransforms);
				}
			}
			return default(WhoIsResult);
		}

		public static bool CompareGameObject(GameObject gameObject, WhoIsItemDefinition definition, HashSet<Transform> usedTransforms = null)
		{
			return !ResolveGameObject(gameObject, definition, usedTransforms).IsEmpty;
		}

		public static bool CompareGameObject(GameObject gameObject, WhoIsItemDefinition[] mask, HashSet<Transform> usedTransforms = null)
		{
			return !ResolveGameObject(gameObject, mask, usedTransforms).IsEmpty;
		}

		public static WhoIsResult ResolveGameObject(GameObject gameObject, WhoIsItemDefinition definition, HashSet<Transform> usedTransforms = null)
		{
			Transform transform = gameObject.transform;
			if (transform.CompareTag(definition.rootTag))
			{
				return CreateResult(transform, definition, usedTransforms);
			}
			return default(WhoIsResult);
		}

		public static WhoIsResult ResolveGameObject(GameObject gameObject, WhoIsItemDefinition[] mask, HashSet<Transform> usedTransforms = null)
		{
			Transform transform = gameObject.transform;
			foreach (WhoIsItemDefinition whoIsItemDefinition in mask)
			{
				if (transform.CompareTag(whoIsItemDefinition.rootTag))
				{
					return CreateResult(transform, whoIsItemDefinition, usedTransforms);
				}
			}
			return default(WhoIsResult);
		}

		private static WhoIsResult CreateResult(Transform transform, WhoIsItemDefinition definition, HashSet<Transform> usedTransforms)
		{
			if (usedTransforms == null)
			{
				return new WhoIsResult(transform, definition);
			}
			if (usedTransforms.Contains(transform))
			{
				WhoIsResult result = new WhoIsResult(null, definition);
				result.skipped = true;
				return result;
			}
			usedTransforms.Add(transform);
			return new WhoIsResult(transform, definition);
		}
	}
}

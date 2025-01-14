using com.ootii.Graphics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Geometry
{
	public static class RaycastExt
	{
		public const int MAX_HITS = 40;

		public static RaycastHitDistanceComparer HitDistanceComparer = new RaycastHitDistanceComparer();

		public static RaycastHit EmptyHitInfo = default(RaycastHit);

		public static RaycastHit[] SharedHitArray = new RaycastHit[40];

		public static Collider[] SharedColliderArray = new Collider[40];

		public static bool SafeRaycast(Vector3 rRayStart, Vector3 rRayDirection, float rDistance = 1000f, int rLayerMask = -1, Transform rIgnore = null, List<Transform> rIgnoreList = null, bool rIgnoreTriggers = true)
		{
			if (rIgnore == null && rIgnoreList == null && rLayerMask != -1)
			{
				return Physics.Raycast(rRayStart, rRayDirection, rDistance, rLayerMask, rIgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
			}
			int num = 0;
			int num2 = SharedHitArray.Length;
			for (int i = 0; i < num2; i++)
			{
				SharedHitArray[i] = EmptyHitInfo;
			}
			num = ((rLayerMask == -1) ? Physics.RaycastNonAlloc(rRayStart, rRayDirection, SharedHitArray, rDistance) : Physics.RaycastNonAlloc(rRayStart, rRayDirection, SharedHitArray, rDistance, rLayerMask, rIgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide));
			switch (num)
			{
			case 0:
				return false;
			case 1:
			{
				if (rIgnoreTriggers && SharedHitArray[0].collider.isTrigger)
				{
					return false;
				}
				Transform transform2 = SharedHitArray[0].collider.transform;
				if (rIgnore != null && IsDescendant(rIgnore, transform2))
				{
					return false;
				}
				if (rIgnoreList != null)
				{
					for (int l = 0; l < rIgnoreList.Count; l++)
					{
						if (IsDescendant(rIgnoreList[l], transform2))
						{
							return false;
						}
					}
				}
				return true;
			}
			default:
				for (int j = 0; j < num; j++)
				{
					if (rIgnoreTriggers && SharedHitArray[j].collider.isTrigger)
					{
						continue;
					}
					Transform transform = SharedHitArray[j].collider.transform;
					if (rIgnore != null && IsDescendant(rIgnore, transform))
					{
						continue;
					}
					if (rIgnoreList != null)
					{
						bool flag = false;
						for (int k = 0; k < rIgnoreList.Count; k++)
						{
							if (IsDescendant(rIgnoreList[k], transform))
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							continue;
						}
					}
					return true;
				}
				return false;
			}
		}

		public static bool SafeRaycast(Vector3 rRayStart, Vector3 rRayDirection, out RaycastHit rHitInfo, float rDistance = 1000f, int rLayerMask = -1, Transform rIgnore = null, List<Transform> rIgnoreList = null, bool rIgnoreTriggers = true, bool rDebug = false)
		{
			if (rIgnore == null && rIgnoreList == null && rLayerMask != -1)
			{
				return Physics.Raycast(rRayStart, rRayDirection, out rHitInfo, rDistance, rLayerMask, rIgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
			}
			rHitInfo = EmptyHitInfo;
			int num = 0;
			int num2 = SharedHitArray.Length;
			for (int i = 0; i < num2; i++)
			{
				SharedHitArray[i] = EmptyHitInfo;
			}
			num = ((rLayerMask == -1) ? Physics.RaycastNonAlloc(rRayStart, rRayDirection, SharedHitArray, rDistance) : Physics.RaycastNonAlloc(rRayStart, rRayDirection, SharedHitArray, rDistance, rLayerMask, rIgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide));
			switch (num)
			{
			case 0:
				return false;
			case 1:
			{
				if (rIgnoreTriggers && SharedHitArray[0].collider.isTrigger)
				{
					return false;
				}
				Transform transform2 = SharedHitArray[0].collider.transform;
				if (rIgnore != null && IsDescendant(rIgnore, transform2))
				{
					return false;
				}
				if (rIgnoreList != null)
				{
					for (int l = 0; l < rIgnoreList.Count; l++)
					{
						if (IsDescendant(rIgnoreList[l], transform2))
						{
							return false;
						}
					}
				}
				rHitInfo = SharedHitArray[0];
				return true;
			}
			default:
				for (int j = 0; j < num; j++)
				{
					if (rIgnoreTriggers && SharedHitArray[j].collider.isTrigger)
					{
						continue;
					}
					Transform transform = SharedHitArray[j].collider.transform;
					if (rIgnore != null && IsDescendant(rIgnore, transform))
					{
						continue;
					}
					if (rIgnoreList != null)
					{
						bool flag = false;
						for (int k = 0; k < rIgnoreList.Count; k++)
						{
							if (IsDescendant(rIgnoreList[k], transform))
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							continue;
						}
					}
					if (rHitInfo.collider == null || SharedHitArray[j].distance < rHitInfo.distance)
					{
						rHitInfo = SharedHitArray[j];
					}
				}
				if (rHitInfo.collider != null)
				{
					return true;
				}
				return false;
			}
		}

		public static int SafeRaycastAll(Vector3 rRayStart, Vector3 rRayDirection, out RaycastHit[] rHitArray, float rDistance = 1000f, int rLayerMask = -1, Transform rIgnore = null, List<Transform> rIgnoreList = null, bool rIgnoreTriggers = true)
		{
			rHitArray = null;
			int num = 0;
			int num2 = SharedHitArray.Length;
			for (int i = 0; i < num2; i++)
			{
				SharedHitArray[i] = EmptyHitInfo;
			}
			num = ((rLayerMask == -1) ? Physics.RaycastNonAlloc(rRayStart, rRayDirection, SharedHitArray, rDistance) : Physics.RaycastNonAlloc(rRayStart, rRayDirection, SharedHitArray, rDistance, rLayerMask, rIgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide));
			switch (num)
			{
			case 0:
				return 0;
			case 1:
			{
				if (rIgnoreTriggers && SharedHitArray[0].collider.isTrigger)
				{
					return 0;
				}
				Transform transform2 = SharedHitArray[0].collider.transform;
				if (rIgnore != null && IsDescendant(rIgnore, transform2))
				{
					return 0;
				}
				if (rIgnoreList != null)
				{
					for (int m = 0; m < rIgnoreList.Count; m++)
					{
						if (IsDescendant(rIgnoreList[m], transform2))
						{
							return 0;
						}
					}
				}
				rHitArray = SharedHitArray;
				return 1;
			}
			default:
			{
				int num3 = 0;
				for (int j = 0; j < num; j++)
				{
					bool flag = false;
					Transform transform = SharedHitArray[j].collider.transform;
					if (rIgnoreTriggers && SharedHitArray[j].collider.isTrigger)
					{
						flag = true;
					}
					if (rIgnore != null && IsDescendant(rIgnore, transform))
					{
						flag = true;
					}
					if (rIgnoreList != null)
					{
						for (int k = 0; k < rIgnoreList.Count; k++)
						{
							if (IsDescendant(rIgnoreList[k], transform))
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						num--;
						for (int l = j; l < num; l++)
						{
							SharedHitArray[l] = SharedHitArray[l + 1];
						}
						j--;
					}
					else
					{
						num3++;
					}
				}
				rHitArray = SharedHitArray;
				return num3;
			}
			}
		}

		public static bool SafeSphereCast(Vector3 rRayStart, Vector3 rRayDirection, float rRadius, float rDistance = 1000f, int rLayerMask = -1, Transform rIgnore = null, List<Transform> rIgnoreList = null, bool rIgnoreTriggers = true)
		{
			RaycastHit hitInfo = default(RaycastHit);
			if (rIgnore == null && rIgnoreList == null && rLayerMask != -1)
			{
				return Physics.SphereCast(rRayStart, rRadius, rRayDirection, out hitInfo, rDistance, rLayerMask, rIgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
			}
			int num = 0;
			int num2 = SharedHitArray.Length;
			for (int i = 0; i < num2; i++)
			{
				SharedHitArray[i] = EmptyHitInfo;
			}
			num = ((rLayerMask == -1) ? Physics.SphereCastNonAlloc(rRayStart, rRadius, rRayDirection, SharedHitArray, rDistance) : Physics.SphereCastNonAlloc(rRayStart, rRadius, rRayDirection, SharedHitArray, rDistance, rLayerMask, rIgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide));
			switch (num)
			{
			case 0:
				return false;
			case 1:
			{
				if (rIgnoreTriggers && SharedHitArray[0].collider.isTrigger)
				{
					return false;
				}
				Transform transform2 = SharedHitArray[0].collider.transform;
				if (rIgnore != null && IsDescendant(rIgnore, transform2))
				{
					return false;
				}
				if (rIgnoreList != null)
				{
					for (int l = 0; l < rIgnoreList.Count; l++)
					{
						if (IsDescendant(rIgnoreList[l], transform2))
						{
							return false;
						}
					}
				}
				return true;
			}
			default:
				for (int j = 0; j < num; j++)
				{
					if (rIgnoreTriggers && SharedHitArray[j].collider.isTrigger)
					{
						continue;
					}
					Transform transform = SharedHitArray[j].collider.transform;
					if (rIgnore != null && IsDescendant(rIgnore, transform))
					{
						continue;
					}
					if (rIgnoreList != null)
					{
						bool flag = false;
						for (int k = 0; k < rIgnoreList.Count; k++)
						{
							if (IsDescendant(rIgnoreList[k], transform))
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							continue;
						}
					}
					return true;
				}
				return false;
			}
		}

		public static bool SafeSphereCast(Vector3 rRayStart, Vector3 rRayDirection, float rRadius, out RaycastHit rHitInfo, float rDistance = 1000f, int rLayerMask = -1, Transform rIgnore = null, List<Transform> rIgnoreList = null, bool rIgnoreTriggers = true)
		{
			if (rIgnore == null && rIgnoreList == null && rLayerMask != -1)
			{
				return Physics.SphereCast(rRayStart, rRadius, rRayDirection, out rHitInfo, rDistance, rLayerMask, rIgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
			}
			rHitInfo = EmptyHitInfo;
			int num = 0;
			int num2 = SharedHitArray.Length;
			for (int i = 0; i < num2; i++)
			{
				SharedHitArray[i] = EmptyHitInfo;
			}
			num = ((rLayerMask == -1) ? Physics.SphereCastNonAlloc(rRayStart, rRadius, rRayDirection, SharedHitArray, rDistance) : Physics.SphereCastNonAlloc(rRayStart, rRadius, rRayDirection, SharedHitArray, rDistance, rLayerMask, rIgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide));
			switch (num)
			{
			case 0:
				return false;
			case 1:
			{
				if (rIgnoreTriggers && SharedHitArray[0].collider.isTrigger)
				{
					return false;
				}
				Transform transform2 = SharedHitArray[0].collider.transform;
				if (rIgnore != null && IsDescendant(rIgnore, transform2))
				{
					return false;
				}
				if (rIgnoreList != null)
				{
					for (int l = 0; l < rIgnoreList.Count; l++)
					{
						if (IsDescendant(rIgnoreList[l], transform2))
						{
							return false;
						}
					}
				}
				rHitInfo = SharedHitArray[0];
				return true;
			}
			default:
				for (int j = 0; j < num; j++)
				{
					if (rIgnoreTriggers && SharedHitArray[j].collider.isTrigger)
					{
						continue;
					}
					Transform transform = SharedHitArray[j].collider.transform;
					if (rIgnore != null && IsDescendant(rIgnore, transform))
					{
						continue;
					}
					if (rIgnoreList != null)
					{
						bool flag = false;
						for (int k = 0; k < rIgnoreList.Count; k++)
						{
							if (IsDescendant(rIgnoreList[k], transform))
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							continue;
						}
					}
					if (rHitInfo.collider == null || SharedHitArray[j].distance < rHitInfo.distance)
					{
						rHitInfo = SharedHitArray[j];
					}
				}
				if (rHitInfo.collider != null)
				{
					return true;
				}
				return false;
			}
		}

		public static int SafeSphereCastAll(Vector3 rRayStart, Vector3 rRayDirection, float rRadius, out RaycastHit[] rHitArray, float rDistance = 1000f, int rLayerMask = -1, Transform rIgnore = null, List<Transform> rIgnoreList = null, bool rIgnoreTriggers = true)
		{
			rHitArray = null;
			int num = 0;
			int num2 = SharedHitArray.Length;
			for (int i = 0; i < num2; i++)
			{
				SharedHitArray[i] = EmptyHitInfo;
			}
			num = ((rLayerMask == -1) ? Physics.SphereCastNonAlloc(rRayStart, rRadius, rRayDirection, SharedHitArray, rDistance) : Physics.SphereCastNonAlloc(rRayStart, rRadius, rRayDirection, SharedHitArray, rDistance, rLayerMask, rIgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide));
			switch (num)
			{
			case 0:
				return 0;
			case 1:
			{
				if (rIgnoreTriggers && SharedHitArray[0].collider.isTrigger)
				{
					return 0;
				}
				Transform transform2 = SharedHitArray[0].collider.transform;
				if (rIgnore != null && IsDescendant(rIgnore, transform2))
				{
					return 0;
				}
				if (rIgnoreList != null)
				{
					for (int m = 0; m < rIgnoreList.Count; m++)
					{
						if (IsDescendant(rIgnoreList[m], transform2))
						{
							return 0;
						}
					}
				}
				rHitArray = SharedHitArray;
				return 1;
			}
			default:
			{
				int num3 = 0;
				for (int j = 0; j < num; j++)
				{
					bool flag = false;
					Transform transform = SharedHitArray[j].collider.transform;
					if (rIgnoreTriggers && SharedHitArray[j].collider.isTrigger)
					{
						flag = true;
					}
					if (rIgnore != null && IsDescendant(rIgnore, transform))
					{
						flag = true;
					}
					if (rIgnoreList != null)
					{
						for (int k = 0; k < rIgnoreList.Count; k++)
						{
							if (IsDescendant(rIgnoreList[k], transform))
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						num--;
						for (int l = j; l < num; l++)
						{
							SharedHitArray[l] = SharedHitArray[l + 1];
						}
						j--;
					}
					else
					{
						num3++;
					}
				}
				rHitArray = SharedHitArray;
				return num3;
			}
			}
		}

		public static int SafeOverlapSphere(Vector3 rPosition, float rRadius, out Collider[] rColliderArray, int rLayerMask = -1, Transform rIgnore = null, List<Transform> rIgnoreList = null, bool rIgnoreTriggers = true)
		{
			rColliderArray = null;
			int num = 0;
			num = ((rLayerMask == -1) ? Physics.OverlapSphereNonAlloc(rPosition, rRadius, SharedColliderArray) : Physics.OverlapSphereNonAlloc(rPosition, rRadius, SharedColliderArray, rLayerMask, rIgnoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide));
			switch (num)
			{
			case 0:
				return 0;
			case 1:
			{
				if (rIgnoreTriggers && SharedColliderArray[0].isTrigger)
				{
					return 0;
				}
				Transform transform2 = SharedColliderArray[0].transform;
				if (rIgnore != null && IsDescendant(rIgnore, transform2))
				{
					return 0;
				}
				if (rIgnoreList != null)
				{
					for (int l = 0; l < rIgnoreList.Count; l++)
					{
						if (IsDescendant(rIgnoreList[l], transform2))
						{
							return 0;
						}
					}
				}
				rColliderArray = SharedColliderArray;
				return 1;
			}
			default:
			{
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					bool flag = false;
					Transform transform = SharedColliderArray[i].transform;
					if (rIgnoreTriggers && SharedColliderArray[i].isTrigger)
					{
						flag = true;
					}
					if (rIgnore != null && IsDescendant(rIgnore, transform))
					{
						flag = true;
					}
					if (rIgnoreList != null)
					{
						for (int j = 0; j < rIgnoreList.Count; j++)
						{
							if (IsDescendant(rIgnoreList[j], transform))
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						num--;
						for (int k = i; k < num; k++)
						{
							SharedColliderArray[k] = SharedColliderArray[k + 1];
						}
						i--;
					}
					else
					{
						num2++;
					}
				}
				rColliderArray = SharedColliderArray;
				return num2;
			}
			}
		}

		public static bool SafeSpiralCast(Transform rRootTransform, out RaycastHit rHitInfo, float rRadius = 8f, float rDistance = 1000f, float rDegreesPerStep = 27f, int rLayerMask = -1, string rTag = null, Transform rIgnore = null, List<Transform> rIgnoreList = null, bool rIgnoreTriggers = true, bool rDebug = false)
		{
			rHitInfo = EmptyHitInfo;
			float num = 2f * (360f / rDegreesPerStep);
			float num2 = rRadius / num;
			float num3 = 0f;
			float num4 = 0f;
			Vector3 zero = Vector3.zero;
			float num5 = 1f / num;
			Color white = Color.white;
			num = num + 360f / rDegreesPerStep - 1f;
			int num6 = 0;
			for (num6 = 0; (float)num6 < num; num6++)
			{
				zero.x = num4 * Mathf.Cos(num3 * ((float)Math.PI / 180f));
				zero.y = num4 * Mathf.Sin(num3 * ((float)Math.PI / 180f));
				zero.z = rDistance;
				if (rDebug)
				{
					GraphicsManager.DrawLine(rRootTransform.position, rRootTransform.TransformPoint(zero), (num6 == 0) ? Color.red : white);
				}
				Vector3 normalized = (rRootTransform.TransformPoint(zero) - rRootTransform.position).normalized;
				if (SafeRaycast(rRootTransform.position, normalized, out RaycastHit rHitInfo2, rDistance, rLayerMask, rIgnore, rIgnoreList, rIgnoreTriggers, rDebug))
				{
					GameObject gameObject = rHitInfo2.collider.gameObject;
					if (!(gameObject.transform == rRootTransform) && !(rHitInfo2.collider is TerrainCollider) && (rTag == null || rTag.Length <= 0 || gameObject.CompareTag(rTag)))
					{
						rHitInfo = rHitInfo2;
						return true;
					}
					continue;
				}
				num3 += rDegreesPerStep;
				num4 = Mathf.Min(num4 + num2, rRadius);
				if (rDebug)
				{
					white.r -= num5;
					white.g -= num5;
				}
			}
			return false;
		}

		public static bool SafeCircularCast(Vector3 rRayStart, Vector3 rRayDirection, Vector3 rRayUp, out RaycastHit rHitInfo, float rDistance = 1000f, float rDegreesPerStep = 30f, int rLayerMask = -1, string rTag = null, Transform rIgnore = null, List<Transform> rIgnoreList = null, bool rIgnoreTriggers = true, bool rDebug = false)
		{
			for (float num = 0f; num <= 360f; num += rDegreesPerStep)
			{
				Vector3 vector = Quaternion.AngleAxis(num, rRayUp) * rRayDirection;
				if (rDebug)
				{
					GraphicsManager.DrawLine(rRayStart, rRayStart + vector * rDistance, Color.cyan, null, 5f);
				}
				if (SafeRaycast(rRayStart, vector, out RaycastHit rHitInfo2, rDistance, rLayerMask, rIgnore, rIgnoreList, rIgnoreTriggers, rDebug))
				{
					GameObject gameObject = rHitInfo2.collider.gameObject;
					if (rTag == null || rTag.Length <= 0 || gameObject.CompareTag(rTag))
					{
						rHitInfo = rHitInfo2;
						return true;
					}
				}
			}
			rHitInfo = EmptyHitInfo;
			return false;
		}

		public static bool GetForwardEdge(Transform rTransform, float rMaxDistance, float rMaxHeight, int rCollisionLayers, out RaycastHit rEdgeHitInfo)
		{
			rEdgeHitInfo = EmptyHitInfo;
			Vector3 vector = rTransform.position + rTransform.up * (rMaxHeight + 0.001f);
			Vector3 forward = rTransform.forward;
			float rDistance = rMaxDistance * 1.5f;
			if (SafeRaycast(vector, forward, out rEdgeHitInfo, rDistance, rCollisionLayers, rTransform))
			{
				return false;
			}
			vector += rTransform.forward * rMaxDistance;
			forward = -rTransform.up;
			if (!SafeRaycast(vector, forward, out rEdgeHitInfo, rMaxHeight, rCollisionLayers, rTransform))
			{
				return false;
			}
			float num = rMaxHeight + 0.001f - rEdgeHitInfo.distance;
			vector = rTransform.position + rTransform.up * (num - 0.001f);
			forward = rTransform.forward;
			if (!SafeRaycast(vector, forward, out rEdgeHitInfo, rMaxDistance, rCollisionLayers, rTransform))
			{
				return false;
			}
			return true;
		}

		public static bool GetForwardEdge(Transform rTransform, Vector3 rPosition, float rMinHeight, float rMaxHeight, float rMaxDepth, int rCollisionLayers, out RaycastHit rEdgeHitInfo)
		{
			rEdgeHitInfo = EmptyHitInfo;
			Vector3 vector = rPosition + rTransform.up * (rMaxHeight + 0.001f);
			Vector3 forward = rTransform.forward;
			if (SafeRaycast(vector, forward, out rEdgeHitInfo, rMaxDepth, rCollisionLayers, rTransform, null, rIgnoreTriggers: false, rDebug: true))
			{
				return false;
			}
			vector += rTransform.forward * rMaxDepth;
			forward = -rTransform.up;
			float rDistance = rMaxHeight - rMinHeight;
			if (!SafeRaycast(vector, forward, out rEdgeHitInfo, rDistance, rCollisionLayers, rTransform, null, rIgnoreTriggers: false, rDebug: true))
			{
				return false;
			}
			float num = rMaxHeight + 0.001f - rEdgeHitInfo.distance;
			vector = rPosition + rTransform.up * (num - 0.001f);
			forward = rTransform.forward;
			if (!SafeRaycast(vector, forward, out rEdgeHitInfo, rMaxDepth, rCollisionLayers, rTransform, null, rIgnoreTriggers: false, rDebug: true))
			{
				return false;
			}
			return true;
		}

		public static bool GetForwardEdge(Transform rTransform, float rMaxDistance, float rMaxHeight, float rMinHeight, int rCollisionLayers, out RaycastHit rEdgeHitInfo)
		{
			rEdgeHitInfo = EmptyHitInfo;
			Vector3 rRayStart = rTransform.position + rTransform.up * (rMinHeight + 0.001f);
			Vector3 forward = rTransform.forward;
			if (!SafeRaycast(rRayStart, forward, out rEdgeHitInfo, rMaxDistance, rCollisionLayers, rTransform))
			{
				return false;
			}
			float distance = rEdgeHitInfo.distance;
			rRayStart = rTransform.position + rTransform.up * (rMaxHeight + 0.001f);
			forward = rTransform.forward;
			if (SafeRaycast(rRayStart, forward, out rEdgeHitInfo, rMaxDistance, rCollisionLayers, rTransform) && rEdgeHitInfo.distance < distance + 0.1f)
			{
				return false;
			}
			rRayStart += rTransform.forward * (distance + 0.001f);
			forward = -rTransform.up;
			if (!SafeRaycast(rRayStart, forward, out rEdgeHitInfo, rMaxHeight, rCollisionLayers, rTransform))
			{
				return false;
			}
			float num = rMaxHeight + 0.001f - rEdgeHitInfo.distance;
			rRayStart = rTransform.position + rTransform.up * (num - 0.001f);
			forward = rTransform.forward;
			if (!SafeRaycast(rRayStart, forward, out rEdgeHitInfo, rMaxDistance, rCollisionLayers, rTransform))
			{
				return false;
			}
			return true;
		}

		public static bool GetForwardEdge2(Transform rTransform, float rMinHeight, float rMaxHeight, float rEdgeDepth, float rMaxDepth, int rCollisionLayers, out RaycastHit rEdgeHitInfo, bool rDebug = false)
		{
			return GetForwardEdge2(rTransform, rTransform.position, rTransform.forward, rTransform.up, rMinHeight, rMaxHeight, rEdgeDepth, rMaxDepth, rCollisionLayers, out rEdgeHitInfo, rDebug);
		}

		public static bool GetForwardEdge2(Transform rTransform, Vector3 rPosition, float rMinHeight, float rMaxHeight, float rEdgeDepth, float rMaxDepth, int rCollisionLayers, out RaycastHit rEdgeHitInfo, bool rDebug = false)
		{
			return GetForwardEdge2(rTransform, rPosition, rTransform.forward, rTransform.up, rMinHeight, rMaxHeight, rEdgeDepth, rMaxDepth, rCollisionLayers, out rEdgeHitInfo, rDebug);
		}

		public static bool GetForwardEdge2(Transform rTransform, Vector3 rPosition, Vector3 rForward, Vector3 rUp, float rMinHeight, float rMaxHeight, float rEdgeDepth, float rMaxDepth, int rCollisionLayers, out RaycastHit rEdgeHitInfo, bool rDebug = false)
		{
			rEdgeHitInfo = EmptyHitInfo;
			float num = 0f;
			float num2 = float.MaxValue;
			float num3 = float.MaxValue;
			if (SafeRaycast(rPosition + rUp * (rMaxHeight - 0.001f), rForward, out rEdgeHitInfo, rMaxDepth, rCollisionLayers, rTransform, null, rIgnoreTriggers: false, rDebug))
			{
				num2 = rEdgeHitInfo.distance;
				num3 = num2;
			}
			else if (SafeRaycast(rPosition + rUp * (rMinHeight + (rMaxHeight - rMinHeight) * 0.5f), rForward, out rEdgeHitInfo, rMaxDepth, rCollisionLayers, rTransform, null, rIgnoreTriggers: false, rDebug))
			{
				num2 = rEdgeHitInfo.distance;
			}
			if (num2 < float.MaxValue)
			{
				Vector3 rRayStart = rPosition + rForward * (num2 + 0.001f) + rUp * (rMaxHeight + 0.001f);
				Vector3 rRayDirection = -rUp;
				float rDistance = rMaxHeight - rMinHeight;
				if (!SafeRaycast(rRayStart, rRayDirection, out rEdgeHitInfo, rDistance, rCollisionLayers, rTransform, null, rDebug))
				{
					return false;
				}
				num = rMaxHeight - (rEdgeHitInfo.distance + 0.001f);
			}
			else
			{
				Vector3 rRayDirection = -rUp;
				float rDistance = rMaxHeight - rMinHeight;
				float num4 = (rEdgeDepth > 0f) ? rEdgeDepth : 0.05f;
				for (float num5 = num4; num5 <= rMaxDepth; num5 += num4 * 0.5f)
				{
					if (SafeRaycast(rPosition + rForward * num5 + rUp * (rMaxHeight + 0.001f), rRayDirection, out rEdgeHitInfo, rDistance, rCollisionLayers, rTransform, null, rIgnoreTriggers: false, rDebug))
					{
						num = rMaxHeight - (rEdgeHitInfo.distance + 0.001f);
						break;
					}
				}
			}
			if (num == 0f)
			{
				return false;
			}
			if (!SafeRaycast(rPosition + rUp * num, rForward, out rEdgeHitInfo, rMaxDepth, rCollisionLayers, rTransform, null, rIgnoreTriggers: false, rDebug))
			{
				return false;
			}
			if (num3 - rEdgeHitInfo.distance < rEdgeDepth)
			{
				return false;
			}
			return true;
		}

		public static void Sort(RaycastHit[] rHitArray, int rCount)
		{
			if (rHitArray == null || rHitArray.Length <= 1)
			{
				return;
			}
			if (rCount > rHitArray.Length)
			{
				rCount = rHitArray.Length;
			}
			int num = 0;
			for (int i = 1; i < rCount; i++)
			{
				num = i;
				RaycastHit raycastHit = rHitArray[i];
				while (num > 0 && rHitArray[num - 1].distance > raycastHit.distance)
				{
					rHitArray[num] = rHitArray[num - 1];
					num--;
				}
				rHitArray[num] = raycastHit;
			}
		}

		private static bool IsDescendant(Transform rParent, Transform rDescendant)
		{
			if (rParent == null)
			{
				return false;
			}
			Transform transform = rDescendant;
			while (transform != null)
			{
				if (transform == rParent)
				{
					return true;
				}
				transform = transform.parent;
			}
			return false;
		}
	}
}

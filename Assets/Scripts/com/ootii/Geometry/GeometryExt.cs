using UnityEngine;

namespace com.ootii.Geometry
{
	public class GeometryExt
	{
		public const float EPSILON = 0.0001f;

		public static Vector3 VECTOR3_HALF;

		public static float LineMeshStepFactor;

		public static Transform Ignore;

		public static Transform[] IgnoreArray;

		public static Vector3[] SphericalDirections;

		static GeometryExt()
		{
			VECTOR3_HALF = new Vector3(0.5f, 0.5f, 0.5f);
			LineMeshStepFactor = 0.2f;
			Ignore = null;
			IgnoreArray = null;
			SphericalDirections = null;
			SphericalDirections = new Vector3[14];
			SphericalDirections[0] = Vector3.forward;
			SphericalDirections[1] = Vector3.back;
			SphericalDirections[2] = Vector3.right;
			SphericalDirections[3] = Vector3.left;
			SphericalDirections[4] = Vector3.up;
			SphericalDirections[5] = Vector3.down;
			SphericalDirections[6] = Vector3.Normalize(new Vector3(1f, 1f, 1f));
			SphericalDirections[7] = Vector3.Normalize(new Vector3(-1f, 1f, 1f));
			SphericalDirections[8] = Vector3.Normalize(new Vector3(1f, -1f, 1f));
			SphericalDirections[9] = Vector3.Normalize(new Vector3(-1f, -1f, 1f));
			SphericalDirections[10] = Vector3.Normalize(new Vector3(1f, 1f, -1f));
			SphericalDirections[11] = Vector3.Normalize(new Vector3(-1f, 1f, -1f));
			SphericalDirections[12] = Vector3.Normalize(new Vector3(1f, -1f, -1f));
			SphericalDirections[13] = Vector3.Normalize(new Vector3(-1f, -1f, -1f));
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, Collider rCollider, int rCollisionLayers = -1)
		{
			if (rCollisionLayers > -1 && rCollider != null && rCollider.gameObject != null && ((1 << rCollider.gameObject.layer) & rCollisionLayers) == 0)
			{
				return Vector3Ext.Null;
			}
			Vector3 result = Vector3Ext.Null;
			if (rCollider is BoxCollider)
			{
				result = ClosestPoint(rPoint, (BoxCollider)rCollider);
			}
			else if (rCollider is SphereCollider)
			{
				result = ClosestPoint(rPoint, (SphereCollider)rCollider);
			}
			else if (rCollider is CapsuleCollider)
			{
				result = ClosestPoint(rPoint, (CapsuleCollider)rCollider);
			}
			else if (rCollider is CharacterController)
			{
				result = ClosestPoint(rPoint, (CharacterController)rCollider);
			}
			else if (rCollider is TerrainCollider)
			{
				result = ClosestPoint(rPoint, (TerrainCollider)rCollider, 4f, rCollisionLayers);
			}
			else if (rCollider is MeshCollider)
			{
				MeshCollider meshCollider = (MeshCollider)rCollider;
				if (!(meshCollider.sharedMesh == null))
				{
					if (meshCollider.sharedMesh.name == "Plane")
					{
						Transform transform = meshCollider.transform;
						Vector3 size = meshCollider.sharedMesh.bounds.size;
						size.y = 0.001f;
						result = ClosestPoint(rPoint, transform, Vector3.zero, size);
					}
					else if (!meshCollider.sharedMesh.isReadable)
					{
						result = meshCollider.ClosestPointOnBounds(rPoint);
						UnityEngine.Debug.LogWarning($"{meshCollider.name}'s mesh is not imported as 'Read/Write Enabled' and may not be accurate. For accurate collisions, check 'Read/Write Enabled' on the model's import settings.");
					}
					else
					{
						result = MeshExt.ClosestPoint(rPoint, 0f, rCollider.gameObject.transform, meshCollider.sharedMesh);
					}
				}
			}
			return result;
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, float rRadius, Collider rCollider, int rCollisionLayers = -1)
		{
			if (rCollisionLayers > -1 && rCollider != null && rCollider.gameObject != null && ((1 << rCollider.gameObject.layer) & rCollisionLayers) == 0)
			{
				return Vector3Ext.Null;
			}
			Vector3 result = Vector3Ext.Null;
			if (rCollider is BoxCollider)
			{
				result = ClosestPoint(rPoint, (BoxCollider)rCollider);
			}
			else if (rCollider is SphereCollider)
			{
				result = ClosestPoint(rPoint, (SphereCollider)rCollider);
			}
			else if (rCollider is CapsuleCollider)
			{
				result = ClosestPoint(rPoint, (CapsuleCollider)rCollider);
			}
			else if (rCollider is CharacterController)
			{
				result = ClosestPoint(rPoint, (CharacterController)rCollider);
			}
			else if (rCollider is TerrainCollider)
			{
				result = ClosestPoint(rPoint, (TerrainCollider)rCollider, rRadius, rCollisionLayers);
			}
			else if (rCollider is MeshCollider)
			{
				MeshCollider meshCollider = (MeshCollider)rCollider;
				if (!(meshCollider.sharedMesh == null))
				{
					if (meshCollider.sharedMesh.name == "Plane")
					{
						Transform transform = meshCollider.transform;
						Vector3 size = meshCollider.sharedMesh.bounds.size;
						size.y = 0.001f;
						result = ClosestPoint(rPoint, transform, Vector3.zero, size);
					}
					else if (!meshCollider.sharedMesh.isReadable)
					{
						result = meshCollider.ClosestPointOnBounds(rPoint);
						UnityEngine.Debug.LogWarning($"{meshCollider.name}'s mesh is not imported as 'Read/Write Enabled' and may not be accurate. For accurate collisions, check 'Read/Write Enabled' on the model's import settings.");
					}
					else
					{
						result = MeshExt.ClosestPoint(rPoint, rRadius, rCollider.gameObject.transform, meshCollider.sharedMesh);
					}
				}
			}
			return result;
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, Vector3 rLineStart, Vector3 rLineEnd)
		{
			Vector3 vector = rLineEnd - rLineStart;
			Vector3 vector2 = Vector3.Project(rPoint - rLineStart, vector);
			Vector3 result = vector2 + rLineStart;
			if (Vector3.Dot(vector2, vector) < 0f)
			{
				result = rLineStart;
			}
			else if (vector2.sqrMagnitude > vector.sqrMagnitude)
			{
				result = rLineEnd;
			}
			return result;
		}

		public static Vector3 ClosestPoint(ref Vector3 point, ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3)
		{
			Vector3 vector = vertex2 - vertex1;
			Vector3 vector2 = vertex3 - vertex1;
			Vector3 rhs = point - vertex1;
			float num = Vector3.Dot(vector, rhs);
			float num2 = Vector3.Dot(vector2, rhs);
			if (num <= 0f && num2 <= 0f)
			{
				return vertex1;
			}
			Vector3 rhs2 = point - vertex2;
			float num3 = Vector3.Dot(vector, rhs2);
			float num4 = Vector3.Dot(vector2, rhs2);
			if (num3 >= 0f && num4 <= num3)
			{
				return vertex2;
			}
			float num5 = num * num4 - num3 * num2;
			if (num5 <= 0f && num >= 0f && num3 <= 0f)
			{
				float d = num / (num - num3);
				return vertex1 + d * vector;
			}
			Vector3 rhs3 = point - vertex3;
			float num6 = Vector3.Dot(vector, rhs3);
			float num7 = Vector3.Dot(vector2, rhs3);
			if (num7 >= 0f && num6 <= num7)
			{
				return vertex3;
			}
			float num8 = num6 * num2 - num * num7;
			if (num8 <= 0f && num2 >= 0f && num7 <= 0f)
			{
				float d2 = num2 / (num2 - num7);
				return vertex1 + d2 * vector2;
			}
			float num9 = num3 * num7 - num6 * num4;
			if (num9 <= 0f && num4 - num3 >= 0f && num6 - num7 >= 0f)
			{
				float d3 = (num4 - num3) / (num4 - num3 + (num6 - num7));
				return vertex2 + d3 * (vertex3 - vertex2);
			}
			float num10 = 1f / (num9 + num8 + num5);
			float d4 = num8 * num10;
			float d5 = num5 * num10;
			return vertex1 + vector * d4 + vector2 * d5;
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, Vector3 rPosition, float rRadius)
		{
			Vector3 b = Vector3.Normalize(rPoint - rPosition) * rRadius;
			return rPosition + b;
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, Transform rTransform, Vector3 rCenter, Vector3 rColliderSize)
		{
			Vector3 vector = rColliderSize * 0.5f;
			Vector3 vector2 = (rTransform != null) ? rTransform.InverseTransformPoint(rPoint) : rPoint;
			if (vector2.x < 0f - vector.x || vector2.x > vector.x || vector2.y < 0f - vector.y || vector2.y > vector.y || vector2.z < 0f - vector.z || vector2.z > vector.z)
			{
				vector2 -= rCenter;
				vector2.x = Mathf.Clamp(vector2.x, 0f - vector.x, vector.x);
				vector2.y = Mathf.Clamp(vector2.y, 0f - vector.y, vector.y);
				vector2.z = Mathf.Clamp(vector2.z, 0f - vector.z, vector.z);
				vector2 += rCenter;
			}
			else
			{
				float num = vector.x - Mathf.Abs(vector2.x);
				float num2 = vector.y - Mathf.Abs(vector2.y);
				float num3 = vector.z - Mathf.Abs(vector2.z);
				if (num < num2 && num < num3)
				{
					vector2.x = ((vector2.x < 0f) ? (0f - vector.x) : vector.x);
				}
				else if (num2 < num && num2 < num3)
				{
					vector2.y = ((vector2.y < 0f) ? (0f - vector.y) : vector.y);
				}
				else
				{
					vector2.z = ((vector2.z < 0f) ? (0f - vector.z) : vector.z);
				}
			}
			if (rTransform == null)
			{
				return vector2;
			}
			return rTransform.TransformPoint(vector2);
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, Vector3 rLineStart, Vector3 rLineEnd, float rRadius)
		{
			Vector3 vector = rLineEnd - rLineStart;
			Vector3 vector2 = Vector3.Project(rPoint - rLineStart, vector);
			Vector3 vector3 = vector2 + rLineStart;
			if (Vector3.Dot(vector2, vector) < 0f)
			{
				vector3 = rLineStart;
			}
			else if (vector2.sqrMagnitude > vector.sqrMagnitude)
			{
				vector3 = rLineEnd;
			}
			return vector3 + (rPoint - vector3).normalized * rRadius;
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, SphereCollider rCollider)
		{
			Transform transform = rCollider.transform;
			Vector3 b = Vector3.Normalize(rPoint - (transform.position + rCollider.center)) * (rCollider.radius * transform.localScale.x);
			return transform.position + rCollider.center + b;
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, CapsuleCollider rCollider)
		{
			Vector3 zero = Vector3.zero;
			Transform transform = rCollider.transform;
			Vector3 vector = transform.InverseTransformPoint(rPoint);
			float num = rCollider.height * 0.5f;
			if (rCollider.direction == 0)
			{
				Vector3 b = Vector3.right * (num - rCollider.radius);
				if (vector.x > rCollider.center.x + num - rCollider.radius)
				{
					zero = ClosestPoint(vector, rCollider.center + b, rCollider.radius);
				}
				else if (vector.x < rCollider.center.x - num + rCollider.radius)
				{
					zero = ClosestPoint(vector, rCollider.center - b, rCollider.radius);
				}
				else
				{
					zero = ClosestPoint(vector, rCollider.center - b, rCollider.center + b);
					zero = ClosestPoint(vector, zero, rCollider.radius);
				}
				return transform.TransformPoint(zero);
			}
			if (rCollider.direction == 1)
			{
				Vector3 b2 = Vector3.up * (num - rCollider.radius);
				if (vector.y > rCollider.center.y + num - rCollider.radius)
				{
					zero = ClosestPoint(vector, rCollider.center + b2, rCollider.radius);
				}
				else if (vector.y < rCollider.center.y - num + rCollider.radius)
				{
					zero = ClosestPoint(vector, rCollider.center - b2, rCollider.radius);
				}
				else
				{
					zero = ClosestPoint(vector, rCollider.center - b2, rCollider.center + b2);
					zero = ClosestPoint(vector, zero, rCollider.radius);
				}
				return transform.TransformPoint(zero);
			}
			Vector3 b3 = Vector3.forward * (num - rCollider.radius);
			if (vector.z > rCollider.center.z + num - rCollider.radius)
			{
				zero = ClosestPoint(vector, rCollider.center + b3, rCollider.radius);
			}
			else if (vector.z < rCollider.center.z - num + rCollider.radius)
			{
				zero = ClosestPoint(vector, rCollider.center - b3, rCollider.radius);
			}
			else
			{
				zero = ClosestPoint(vector, rCollider.center - b3, rCollider.center + b3);
				zero = ClosestPoint(vector, zero, rCollider.radius);
			}
			return transform.TransformPoint(zero);
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, BoxCollider rCollider)
		{
			Transform transform = rCollider.transform;
			Matrix4x4 lhs = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			Matrix4x4 rhs = Matrix4x4.TRS(rCollider.center, Quaternion.identity, rCollider.size);
			Matrix4x4 matrix4x = lhs * rhs;
			Vector3 vector = matrix4x.inverse.MultiplyPoint(rPoint);
			if (vector.x < 0f - VECTOR3_HALF.x || vector.x > VECTOR3_HALF.x || vector.y < 0f - VECTOR3_HALF.y || vector.y > VECTOR3_HALF.y || vector.z < 0f - VECTOR3_HALF.z || vector.z > VECTOR3_HALF.z)
			{
				vector.x = Mathf.Clamp(vector.x, 0f - VECTOR3_HALF.x, VECTOR3_HALF.x);
				vector.y = Mathf.Clamp(vector.y, 0f - VECTOR3_HALF.y, VECTOR3_HALF.y);
				vector.z = Mathf.Clamp(vector.z, 0f - VECTOR3_HALF.z, VECTOR3_HALF.z);
			}
			else
			{
				float num = VECTOR3_HALF.x - Mathf.Abs(vector.x);
				float num2 = VECTOR3_HALF.y - Mathf.Abs(vector.y);
				float num3 = VECTOR3_HALF.z - Mathf.Abs(vector.z);
				if (num < num2 && num < num3)
				{
					vector.x = ((vector.x < 0f) ? (0f - VECTOR3_HALF.x) : VECTOR3_HALF.x);
				}
				else if (num2 < num && num2 < num3)
				{
					vector.y = ((vector.y < 0f) ? (0f - VECTOR3_HALF.y) : VECTOR3_HALF.y);
				}
				else
				{
					vector.z = ((vector.z < 0f) ? (0f - VECTOR3_HALF.z) : VECTOR3_HALF.z);
				}
			}
			return matrix4x.MultiplyPoint(vector);
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, TerrainCollider rCollider, float rRadius = 4f, int rCollisionLayers = -1)
		{
			RaycastHit raycastHit = default(RaycastHit);
			raycastHit.distance = float.MaxValue;
			for (int i = 0; i < SphericalDirections.Length; i++)
			{
				RaycastHit hitInfo;
				if (Physics.Raycast(rPoint, SphericalDirections[i], out hitInfo, rRadius + 0.05f, rCollisionLayers, QueryTriggerInteraction.Ignore) && hitInfo.collider == rCollider && hitInfo.distance < raycastHit.distance && !IgnoreCollider(hitInfo.collider))
				{
					raycastHit = hitInfo;
				}
			}
			if (raycastHit.distance < float.MaxValue)
			{
				return raycastHit.point;
			}
			return Vector3Ext.Null;
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, Vector3 rDirection, float rRadius, TerrainCollider rCollider, int rCollisionLayers = -1)
		{
			RaycastHit hitInfo;
			if (rDirection.sqrMagnitude > 0f && Physics.SphereCast(rPoint, rRadius * 0.5f, rDirection, out hitInfo, rRadius, rCollisionLayers, QueryTriggerInteraction.Ignore) && hitInfo.collider == rCollider)
			{
				if (Physics.Raycast(rPoint, hitInfo.point - rPoint, out RaycastHit hitInfo2, rRadius + 0.01f, rCollisionLayers, QueryTriggerInteraction.Ignore) && hitInfo2.collider == rCollider)
				{
					hitInfo = hitInfo2;
				}
				return hitInfo.point;
			}
			hitInfo = default(RaycastHit);
			hitInfo.distance = float.MaxValue;
			for (int i = 0; i < SphericalDirections.Length; i++)
			{
				RaycastHit hitInfo3;
				if (Physics.Raycast(rPoint, SphericalDirections[i], out hitInfo3, rRadius + 0.05f, rCollisionLayers, QueryTriggerInteraction.Ignore) && hitInfo3.collider == rCollider && hitInfo3.distance < hitInfo.distance && !IgnoreCollider(hitInfo3.collider))
				{
					hitInfo = hitInfo3;
				}
			}
			if (hitInfo.distance < float.MaxValue)
			{
				return hitInfo.point;
			}
			return Vector3Ext.Null;
		}

		public static Vector3 ClosestPoint(Vector3 rPoint, CharacterController rController)
		{
			Vector3 zero = Vector3.zero;
			Transform transform = rController.transform;
			Vector3 vector = rPoint - transform.position;
			Vector3 b = Vector3.up * (rController.height * 0.5f - rController.radius);
			if (vector.y > rController.height - rController.radius)
			{
				zero = ClosestPoint(vector, rController.center + b, rController.radius);
			}
			else if (vector.y < rController.radius)
			{
				zero = ClosestPoint(vector, rController.center - b, rController.radius);
			}
			else
			{
				zero = ClosestPoint(vector, rController.center - b, rController.center + b);
				zero = ClosestPoint(vector, zero, rController.radius);
			}
			return zero + transform.position;
		}

		public static void ClosestPoints(Vector3 rStart, Vector3 rEnd, float rRadius, Collider rCollider, ref Vector3 rLinePoint, ref Vector3 rColliderPoint, int rCollisionLayers = -1)
		{
			if (rCollisionLayers <= -1 || !(rCollider != null) || !(rCollider.gameObject != null) || ((1 << rCollider.gameObject.layer) & rCollisionLayers) != 0)
			{
				if (rCollider is BoxCollider)
				{
					ClosestPoints(rStart, rEnd, (BoxCollider)rCollider, ref rLinePoint, ref rColliderPoint);
				}
				else if (rCollider is SphereCollider)
				{
					ClosestPoints(rStart, rEnd, (SphereCollider)rCollider, ref rLinePoint, ref rColliderPoint);
				}
				else if (rCollider is CapsuleCollider)
				{
					ClosestPoints(rStart, rEnd, (CapsuleCollider)rCollider, ref rLinePoint, ref rColliderPoint);
				}
				else if (rCollider is CharacterController)
				{
					ClosestPoints(rStart, rEnd, (CharacterController)rCollider, ref rLinePoint, ref rColliderPoint);
				}
				else if (rCollider is TerrainCollider)
				{
					ClosestPoints(rStart, rEnd, (TerrainCollider)rCollider, ref rLinePoint, ref rColliderPoint, rRadius, rCollisionLayers);
				}
				else if (rCollider is MeshCollider)
				{
					ClosestPoints(rStart, rEnd, rRadius, (MeshCollider)rCollider, ref rLinePoint, ref rColliderPoint);
				}
			}
		}

		public static void ClosestPoints(Vector3 rStart1, Vector3 rEnd1, Vector3 rStart2, Vector3 rEnd2, ref Vector3 rLine1Point, ref Vector3 rLine2Point)
		{
			Vector3 vector = rEnd1 - rStart1;
			float num = vector.magnitude * 0.5f;
			Vector3 normalized = vector.normalized;
			Vector3 a = rStart1 + normalized * num;
			float num2 = 0f;
			Vector3 vector2 = rEnd2 - rStart2;
			float num3 = vector2.magnitude * 0.5f;
			Vector3 normalized2 = vector2.normalized;
			Vector3 vector3 = rStart2 + normalized2 * num3;
			float num4 = 0f;
			Vector3 rThis = a - vector3;
			float num5 = 0f - normalized.Dot(normalized2);
			float num6 = rThis.Dot(normalized);
			float num7 = 0f - rThis.Dot(normalized2);
			float num8 = Mathf.Abs(1f - num5 * num5);
			if (num8 >= 0.0001f)
			{
				num2 = num5 * num7 - num6;
				num4 = num5 * num6 - num7;
				float num9 = num * num8;
				float num10 = num3 * num8;
				if (num2 >= 0f - num9)
				{
					if (num2 <= num9)
					{
						if (num4 >= 0f - num10)
						{
							if (num4 <= num10)
							{
								float num11 = 1f / num8;
								num2 *= num11;
								num4 *= num11;
							}
							else
							{
								num4 = num3;
								float num12 = 0f - (num5 * num4 + num6);
								num2 = ((num12 < 0f - num) ? (0f - num) : ((!(num12 <= num)) ? num : num12));
							}
						}
						else
						{
							num4 = 0f - num3;
							float num13 = 0f - (num5 * num4 + num6);
							num2 = ((num13 < 0f - num) ? (0f - num) : ((!(num13 <= num)) ? num : num13));
						}
					}
					else if (num4 >= 0f - num10)
					{
						if (num4 <= num10)
						{
							num2 = num;
							float num14 = 0f - (num5 * num2 + num7);
							num4 = ((num14 < 0f - num3) ? (0f - num3) : ((!(num14 <= num3)) ? num3 : num14));
						}
						else
						{
							num4 = num3;
							float num15 = 0f - (num5 * num4 + num6);
							if (num15 < 0f - num)
							{
								num2 = 0f - num;
							}
							else if (num15 <= num)
							{
								num2 = num15;
							}
							else
							{
								num2 = num;
								float num16 = 0f - (num5 * num2 + num7);
								num4 = ((num16 < 0f - num3) ? (0f - num3) : ((!(num16 <= num3)) ? num3 : num16));
							}
						}
					}
					else
					{
						num4 = 0f - num3;
						float num17 = 0f - (num5 * num4 + num6);
						if (num17 < 0f - num)
						{
							num2 = 0f - num;
						}
						else if (num17 <= num)
						{
							num2 = num17;
						}
						else
						{
							num2 = num;
							float num18 = 0f - (num5 * num2 + num7);
							num4 = ((num18 > num3) ? num3 : ((!(num18 >= 0f - num3)) ? (0f - num3) : num18));
						}
					}
				}
				else if (num4 >= 0f - num10)
				{
					if (num4 <= num10)
					{
						num2 = 0f - num;
						float num19 = 0f - (num5 * num2 + num7);
						num4 = ((num19 < 0f - num3) ? (0f - num3) : ((!(num19 <= num3)) ? num3 : num19));
					}
					else
					{
						num4 = num3;
						float num20 = 0f - (num5 * num4 + num6);
						if (num20 > num)
						{
							num2 = num;
						}
						else if (num20 >= 0f - num)
						{
							num2 = num20;
						}
						else
						{
							num2 = 0f - num;
							float num21 = 0f - (num5 * num2 + num7);
							num4 = ((num21 < 0f - num3) ? (0f - num3) : ((!(num21 <= num3)) ? num3 : num21));
						}
					}
				}
				else
				{
					num4 = 0f - num3;
					float num22 = 0f - (num5 * num4 + num6);
					if (num22 > num)
					{
						num2 = num;
					}
					else if (num22 >= 0f - num)
					{
						num2 = num22;
					}
					else
					{
						num2 = 0f - num;
						float num23 = 0f - (num5 * num2 + num7);
						num4 = ((num23 < 0f - num3) ? (0f - num3) : ((!(num23 <= num3)) ? num3 : num23));
					}
				}
			}
			else
			{
				float num24 = num + num3;
				float num25 = (num5 > 0f) ? (-1f) : 1f;
				float num26 = 0f - (num6 - num25 * num7) * 0.5f;
				if (num26 < 0f - num24)
				{
					num26 = 0f - num24;
				}
				else if (num26 > num24)
				{
					num26 = num24;
				}
				num4 = (0f - num25) * num26 * num3 / num24;
				num2 = num26 + num25 * num4;
			}
			rLine1Point = a + normalized * num2;
			rLine2Point = vector3 + normalized2 * num4;
		}

		public static void ClosestPoints(Vector3 rStart, Vector3 rEnd, Vector3 rPosition, float rRadius, ref Vector3 rLinePoint, ref Vector3 rColliderPoint)
		{
			Vector3 vector = rEnd - rStart;
			Vector3 normalized = vector.normalized;
			Vector3 lhs = rPosition - rStart;
			float a = Vector3.Dot(lhs, normalized);
			float d = Mathf.Min((normalized * Mathf.Max(a, 0f)).magnitude, vector.magnitude);
			rLinePoint = rStart + normalized * d;
			lhs = rPosition - rLinePoint;
			rColliderPoint = rLinePoint + lhs.normalized * (lhs.magnitude - rRadius);
		}

		public static void ClosestPoints(Vector3 rStart, Vector3 rEnd, Transform rTransform, Vector3 rCenter, Vector3 rColliderSize, ref Vector3 rLinePoint, ref Vector3 rColliderPoint)
		{
			Vector3 vector = rEnd - rStart;
			Vector3 normalized = vector.normalized;
			Vector3 a = rStart + normalized * (vector.magnitude * 0.5f);
			Vector3 rThis = a - rTransform.position;
			Vector3 right = rTransform.right;
			Vector3 up = rTransform.up;
			Vector3 forward = rTransform.forward;
			Vector3 rBoxPoint = new Vector3(rThis.Dot(right), rThis.Dot(up), rThis.Dot(forward));
			Vector3 rBoxDirection = new Vector3(normalized.Dot(right), normalized.Dot(up), normalized.Dot(forward));
			Vector3 rBoxExtents = rColliderSize * 0.5f;
			rBoxExtents.x *= rTransform.lossyScale.x;
			rBoxExtents.y *= rTransform.lossyScale.y;
			rBoxExtents.z *= rTransform.lossyScale.z;
			if (rBoxDirection.x < 0f)
			{
				rBoxPoint.x = 0f - rBoxPoint.x;
				rBoxDirection.x = 0f - rBoxDirection.x;
			}
			if (rBoxDirection.y < 0f)
			{
				rBoxPoint.y = 0f - rBoxPoint.y;
				rBoxDirection.y = 0f - rBoxDirection.y;
			}
			if (rBoxDirection.z < 0f)
			{
				rBoxPoint.z = 0f - rBoxPoint.z;
				rBoxDirection.z = 0f - rBoxDirection.z;
			}
			float rLineDistance = 0f;
			if (rBoxDirection.x > 0f)
			{
				if (rBoxDirection.y > 0f)
				{
					if (rBoxDirection.z > 0f)
					{
						GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, ref rLineDistance);
					}
					else
					{
						GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 0, 1, ref rLineDistance);
					}
				}
				else if (rBoxDirection.z > 0f)
				{
					GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 0, 2, ref rLineDistance);
				}
				else
				{
					GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 0, ref rLineDistance);
				}
			}
			else if (rBoxDirection.y > 0f)
			{
				if (rBoxDirection.z > 0f)
				{
					GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 1, 2, ref rLineDistance);
				}
				else
				{
					GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 1, ref rLineDistance);
				}
			}
			else if (rBoxDirection.z > 0f)
			{
				GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 2, ref rLineDistance);
			}
			else
			{
				rLineDistance = 0f;
			}
			rLinePoint = a + normalized * rLineDistance;
			float num = vector.magnitude / 2f;
			if (rLineDistance >= 0f - num)
			{
				if (rLineDistance > num)
				{
					rLinePoint = rEnd;
				}
			}
			else
			{
				rLinePoint = rStart;
			}
			rColliderPoint = ClosestPoint(rLinePoint, rTransform, rCenter, rColliderSize);
		}

		public static void ClosestPoints(Vector3 rStart, Vector3 rEnd, SphereCollider rCollider, ref Vector3 rLinePoint, ref Vector3 rColliderPoint)
		{
			Transform transform = rCollider.transform;
			float num = rCollider.radius * transform.lossyScale.x;
			Vector3 vector = rEnd - rStart;
			Vector3 normalized = vector.normalized;
			Vector3 lhs = transform.position + rCollider.center - rStart;
			float a = Vector3.Dot(lhs, normalized);
			float d = Mathf.Min((normalized * Mathf.Max(a, 0f)).magnitude, vector.magnitude);
			rLinePoint = rStart + normalized * d;
			lhs = transform.position + rCollider.center - rLinePoint;
			rColliderPoint = rLinePoint + lhs.normalized * (lhs.magnitude - num);
		}

		public static void ClosestPoints(Vector3 rStart, Vector3 rEnd, CapsuleCollider rCollider, ref Vector3 rLinePoint, ref Vector3 rColliderPoint)
		{
			Transform transform = rCollider.transform;
			float height = rCollider.height;
			float radius = rCollider.radius;
			Vector3 zero = Vector3.zero;
			zero[rCollider.direction] = height / 2f - radius;
			Vector3 rStart2 = transform.TransformPoint(rCollider.center - zero);
			Vector3 rEnd2 = transform.TransformPoint(rCollider.center + zero);
			ClosestPoints(rStart, rEnd, rStart2, rEnd2, ref rLinePoint, ref rColliderPoint);
			Vector3 vector = rLinePoint - rColliderPoint;
			rColliderPoint += vector.normalized * (radius * transform.lossyScale.x);
		}

		public static void ClosestPoints(Vector3 rStart, Vector3 rEnd, BoxCollider rCollider, ref Vector3 rLinePoint, ref Vector3 rColliderPoint)
		{
			Transform transform = rCollider.transform;
			Matrix4x4 lhs = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			Matrix4x4 rhs = Matrix4x4.TRS(rCollider.center, Quaternion.identity, rCollider.size);
			Matrix4x4 rMatrix = lhs * rhs;
			Vector3 vector = rEnd - rStart;
			Vector3 normalized = vector.normalized;
			Vector3 a = rStart + normalized * (vector.magnitude * 0.5f);
			Vector3 rThis = a - rMatrix.Position();
			Vector3 right = transform.right;
			Vector3 up = transform.up;
			Vector3 forward = transform.forward;
			Vector3 rBoxPoint = new Vector3(rThis.Dot(right), rThis.Dot(up), rThis.Dot(forward));
			Vector3 rBoxDirection = new Vector3(normalized.Dot(right), normalized.Dot(up), normalized.Dot(forward));
			Vector3 rBoxExtents = VECTOR3_HALF;
			rBoxExtents.x *= rMatrix.Scale().x;
			rBoxExtents.y *= rMatrix.Scale().y;
			rBoxExtents.z *= rMatrix.Scale().z;
			if (rBoxDirection.x < 0f)
			{
				rBoxPoint.x = 0f - rBoxPoint.x;
				rBoxDirection.x = 0f - rBoxDirection.x;
			}
			if (rBoxDirection.y < 0f)
			{
				rBoxPoint.y = 0f - rBoxPoint.y;
				rBoxDirection.y = 0f - rBoxDirection.y;
			}
			if (rBoxDirection.z < 0f)
			{
				rBoxPoint.z = 0f - rBoxPoint.z;
				rBoxDirection.z = 0f - rBoxDirection.z;
			}
			float rLineDistance = 0f;
			if (rBoxDirection.x > 0f)
			{
				if (rBoxDirection.y > 0f)
				{
					if (rBoxDirection.z > 0f)
					{
						GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, ref rLineDistance);
					}
					else
					{
						GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 0, 1, ref rLineDistance);
					}
				}
				else if (rBoxDirection.z > 0f)
				{
					GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 0, 2, ref rLineDistance);
				}
				else
				{
					GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 0, ref rLineDistance);
				}
			}
			else if (rBoxDirection.y > 0f)
			{
				if (rBoxDirection.z > 0f)
				{
					GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 1, 2, ref rLineDistance);
				}
				else
				{
					GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 1, ref rLineDistance);
				}
			}
			else if (rBoxDirection.z > 0f)
			{
				GetLineDistanceFromBoxExtent(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, 2, ref rLineDistance);
			}
			else
			{
				rLineDistance = 0f;
			}
			rLinePoint = a + normalized * rLineDistance;
			float num = vector.magnitude / 2f;
			if (rLineDistance >= 0f - num)
			{
				if (rLineDistance > num)
				{
					rLinePoint = rEnd;
				}
			}
			else
			{
				rLinePoint = rStart;
			}
			rColliderPoint = ClosestPoint(rLinePoint, rCollider);
		}

		public static void ClosestPoints(Vector3 rStart, Vector3 rEnd, CharacterController rController, ref Vector3 rLinePoint, ref Vector3 rColliderPoint)
		{
			Transform transform = rController.transform;
			Vector3 zero = Vector3.zero;
			zero[1] = rController.height / 2f - rController.radius;
			Vector3 rStart2 = transform.TransformPoint(rController.center - zero);
			Vector3 rEnd2 = transform.TransformPoint(rController.center + zero);
			ClosestPoints(rStart, rEnd, rStart2, rEnd2, ref rLinePoint, ref rColliderPoint);
			Vector3 vector = rColliderPoint - rLinePoint;
			rColliderPoint = rLinePoint + vector.normalized * (vector.magnitude - rController.radius);
		}

		public static void ClosestPoints(Vector3 rStart, Vector3 rEnd, TerrainCollider rCollider, ref Vector3 rLinePoint, ref Vector3 rColliderPoint, float rRadius = 4f, int rCollisionLayers = -1)
		{
			float num = float.MaxValue;
			float magnitude = (rEnd - rStart).magnitude;
			Vector3 normalized = (rEnd - rStart).normalized;
			for (float num2 = 0f; num2 < magnitude + rRadius; num2 += rRadius)
			{
				if (num2 > magnitude)
				{
					num2 = magnitude;
				}
				Vector3 vector = rStart + normalized * num2;
				Vector3 vector2 = ClosestPoint(vector, rRadius, rCollider, rCollisionLayers);
				if (vector2 != Vector3Ext.Null)
				{
					float sqrMagnitude = (vector2 - vector).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						rLinePoint = vector;
						rColliderPoint = vector2;
						num = sqrMagnitude;
					}
				}
				if (num2 == magnitude)
				{
					break;
				}
			}
		}

		public static void ClosestPoints(Vector3 rStart, Vector3 rEnd, Vector3 rMovement, TerrainCollider rCollider, ref Vector3 rLinePoint, ref Vector3 rColliderPoint, float rRadius = 4f, int rCollisionLayers = -1)
		{
			if (rMovement.sqrMagnitude > 0f)
			{
				Vector3 normalized = rMovement.normalized;
				float magnitude = rMovement.magnitude;
				if (Physics.CapsuleCast(rStart, rEnd, rRadius, normalized, out RaycastHit hitInfo, magnitude, rCollisionLayers) && hitInfo.collider == rCollider)
				{
					rColliderPoint = hitInfo.point;
					rLinePoint = ClosestPoint(rColliderPoint, rStart, rEnd);
					return;
				}
			}
			float num = float.MaxValue;
			float magnitude2 = (rEnd - rStart).magnitude;
			Vector3 normalized2 = (rEnd - rStart).normalized;
			for (float num2 = 0f; num2 < magnitude2 + rRadius; num2 += rRadius)
			{
				if (num2 > magnitude2)
				{
					num2 = magnitude2;
				}
				Vector3 vector = rStart + normalized2 * num2;
				Vector3 vector2 = ClosestPoint(vector, rMovement, rRadius, rCollider, rCollisionLayers);
				if (vector2 != Vector3Ext.Null)
				{
					float sqrMagnitude = (vector2 - vector).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						rLinePoint = vector;
						rColliderPoint = vector2;
						num = sqrMagnitude;
					}
				}
				if (num2 == magnitude2)
				{
					break;
				}
			}
		}

		public static void ClosestPoints(Vector3 rStart, Vector3 rEnd, float rRadius, MeshCollider rCollider, ref Vector3 rLinePoint, ref Vector3 rColliderPoint)
		{
			if (rCollider == null || rCollider.sharedMesh == null)
			{
				return;
			}
			if (rCollider.sharedMesh.name == "Plane")
			{
				Transform transform = rCollider.transform;
				Vector3 size = rCollider.sharedMesh.bounds.size;
				size.y = 0.001f;
				ClosestPoints(rStart, rEnd, transform, Vector3.zero, size, ref rLinePoint, ref rColliderPoint);
				return;
			}
			Vector3 vector = rEnd - rStart;
			float magnitude = vector.magnitude;
			if (magnitude == 0f)
			{
				rLinePoint = rStart;
				if (!rCollider.sharedMesh.isReadable)
				{
					rColliderPoint = rCollider.ClosestPointOnBounds(rStart);
					UnityEngine.Debug.LogWarning($"{rCollider.name}'s mesh is not imported as 'Read/Write Enabled' and may not be accurate. For accurate collisions, check 'Read/Write Enabled' on the model's import settings.");
				}
				else
				{
					rColliderPoint = MeshExt.ClosestPoint(rStart, rRadius, rCollider.gameObject.transform, rCollider.sharedMesh);
				}
				return;
			}
			float num = rRadius * LineMeshStepFactor;
			float num2 = float.MaxValue;
			Vector3 normalized = vector.normalized;
			for (float num3 = 0f; num3 < magnitude + num; num3 += num)
			{
				if (num3 > magnitude)
				{
					num3 = magnitude;
				}
				Vector3 vector2 = rStart + normalized * num3;
				Vector3 vector3;
				if (!rCollider.sharedMesh.isReadable)
				{
					vector3 = rCollider.ClosestPointOnBounds(vector2);
					UnityEngine.Debug.LogWarning($"{rCollider.name}'s mesh is not imported as 'Read/Write Enabled' and may not be accurate. For accurate collisions, check 'Read/Write Enabled' on the model's import settings.");
				}
				else
				{
					vector3 = MeshExt.ClosestPoint(vector2, rRadius, rCollider.gameObject.transform, rCollider.sharedMesh);
				}
				if (vector3 != Vector3Ext.Null)
				{
					float sqrMagnitude = (vector3 - vector2).sqrMagnitude;
					if (sqrMagnitude < num2)
					{
						rLinePoint = vector2;
						rColliderPoint = vector3;
						num2 = sqrMagnitude;
					}
				}
				if (num3 == magnitude)
				{
					break;
				}
			}
		}

		public static bool RaySphereIntersect(Vector3 rRayStart, Vector3 rRayDirection, Vector3 rSphereCenter, float rSphereRadius)
		{
			Vector3 vector = rSphereCenter - rRayStart;
			if (Vector3.Dot(rRayDirection, vector) <= 0f)
			{
				return false;
			}
			Vector3 b = Vector3.Project(vector, rRayDirection);
			if (Vector3.Distance(vector, b) > rSphereRadius)
			{
				return false;
			}
			return true;
		}

		public static bool LinePlaneIntersect(Vector3 rLineStart, Vector3 rLineEnd, Plane rPlane)
		{
			Vector3 rhs = rLineEnd - rLineStart;
			float num = Vector3.Dot(rPlane.normal, rhs);
			if (Mathf.Abs(num) > 0.0001f)
			{
				float num2 = (0f - (Vector3.Dot(rPlane.normal, rLineStart) + rPlane.distance)) / num;
				if (num2 < 0f || num2 > 1f)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public static bool LineSphereIntersect(Vector3 rLineStart, Vector3 rLineEnd, Vector3 rSphereCenter, float rSphereRadius)
		{
			Vector3 vector = rLineEnd - rLineStart;
			float magnitude = vector.magnitude;
			Vector3 normalized = vector.normalized;
			Vector3 vector2 = rSphereCenter - rLineStart;
			if (Vector3.Dot(normalized, vector2) <= 0f)
			{
				return false;
			}
			Vector3 vector3 = Vector3.Project(vector2, normalized);
			float num = Vector3.Distance(vector2, vector3);
			if (num > rSphereRadius)
			{
				return false;
			}
			if (Mathf.Abs(num - rSphereRadius) < 0.0001f)
			{
				if (Vector3.Distance(vector3, rLineStart) > magnitude)
				{
					return false;
				}
			}
			else
			{
				float num2 = Mathf.Sqrt(rSphereRadius * rSphereRadius - num * num);
				if (vector3.magnitude - num2 > magnitude)
				{
					return false;
				}
			}
			return true;
		}

		public static bool LineCylinderIntersect(Vector3 rLineStart, Vector3 rLineEnd, Transform rTransform, float rHeight, float rRadius)
		{
			Matrix4x4 inverse = Matrix4x4.TRS(rTransform.position, rTransform.rotation, Vector3.one).inverse;
			Vector3 vector = inverse.MultiplyPoint(rLineStart);
			Vector3 vector2 = (inverse.MultiplyPoint(rLineEnd) - vector) * 0.5f;
			Vector3 vector3 = new Vector3(Mathf.Abs(vector2.x), Mathf.Abs(vector2.y), Mathf.Abs(vector2.z));
			Vector3 vector4 = new Vector3(rRadius, rHeight * 0.5f, rRadius);
			Vector3 vector5 = -vector4;
			Vector3 vector6 = (vector4 - vector5) * 0.5f;
			Vector3 vector7 = vector + vector2 - (vector5 + vector4) * 0.5f;
			if (Mathf.Abs(vector7.x) > vector6.x + vector3.x)
			{
				return false;
			}
			if (Mathf.Abs(vector7.y) > vector6.y + vector3.y)
			{
				return false;
			}
			if (Mathf.Abs(vector7.z) > vector6.z + vector3.z)
			{
				return false;
			}
			if (Mathf.Abs(vector2.y * vector7.z - vector2.z * vector7.y) > vector6.y * vector3.z + vector6.z * vector3.y + 0.0001f)
			{
				return false;
			}
			if (Mathf.Abs(vector2.z * vector7.x - vector2.x * vector7.z) > vector6.z * vector3.x + vector6.x * vector3.z + 0.0001f)
			{
				return false;
			}
			if (Mathf.Abs(vector2.x * vector7.y - vector2.y * vector7.x) > vector6.x * vector3.y + vector6.y * vector3.x + 0.0001f)
			{
				return false;
			}
			return true;
		}

		public static bool LineCylinderFromBaseIntersect(Vector3 rLineStart, Vector3 rLineEnd, Transform rTransform, float rHeight, float rRadius)
		{
			Matrix4x4 inverse = Matrix4x4.TRS(rTransform.position + rTransform.up * (0f - rHeight / 2f), rTransform.rotation, Vector3.one).inverse;
			Vector3 vector = inverse.MultiplyPoint(rLineStart);
			Vector3 vector2 = (inverse.MultiplyPoint(rLineEnd) - vector) * 0.5f;
			Vector3 vector3 = new Vector3(Mathf.Abs(vector2.x), Mathf.Abs(vector2.y), Mathf.Abs(vector2.z));
			Vector3 vector4 = new Vector3(rRadius, rHeight * 0.5f, rRadius);
			Vector3 vector5 = -vector4;
			Vector3 vector6 = (vector4 - vector5) * 0.5f;
			Vector3 vector7 = vector + vector2 - (vector5 + vector4) * 0.5f;
			if (Mathf.Abs(vector7.x) > vector6.x + vector3.x)
			{
				return false;
			}
			if (Mathf.Abs(vector7.y) > vector6.y + vector3.y)
			{
				return false;
			}
			if (Mathf.Abs(vector7.z) > vector6.z + vector3.z)
			{
				return false;
			}
			if (Mathf.Abs(vector2.y * vector7.z - vector2.z * vector7.y) > vector6.y * vector3.z + vector6.z * vector3.y + 0.0001f)
			{
				return false;
			}
			if (Mathf.Abs(vector2.z * vector7.x - vector2.x * vector7.z) > vector6.z * vector3.x + vector6.x * vector3.z + 0.0001f)
			{
				return false;
			}
			if (Mathf.Abs(vector2.x * vector7.y - vector2.y * vector7.x) > vector6.x * vector3.y + vector6.y * vector3.x + 0.0001f)
			{
				return false;
			}
			return true;
		}

		public static bool LineBoxIntersect(Vector3 rLineStart, Vector3 rLineEnd, Transform rTransform, float rWidth, float rHeight, float rDepth)
		{
			Matrix4x4 inverse = Matrix4x4.TRS(rTransform.position, rTransform.rotation, Vector3.one).inverse;
			Vector3 vector = inverse.MultiplyPoint(rLineStart);
			Vector3 vector2 = (inverse.MultiplyPoint(rLineEnd) - vector) * 0.5f;
			Vector3 vector3 = new Vector3(Mathf.Abs(vector2.x), Mathf.Abs(vector2.y), Mathf.Abs(vector2.z));
			Vector3 vector4 = new Vector3(rWidth * 0.5f, rHeight * 0.5f, rDepth * 0.5f);
			Vector3 vector5 = -vector4;
			Vector3 vector6 = (vector4 - vector5) * 0.5f;
			Vector3 vector7 = vector + vector2 - (vector5 + vector4) * 0.5f;
			if (Mathf.Abs(vector7.x) > vector6.x + vector3.x)
			{
				return false;
			}
			if (Mathf.Abs(vector7.y) > vector6.y + vector3.y)
			{
				return false;
			}
			if (Mathf.Abs(vector7.z) > vector6.z + vector3.z)
			{
				return false;
			}
			if (Mathf.Abs(vector2.y * vector7.z - vector2.z * vector7.y) > vector6.y * vector3.z + vector6.z * vector3.y + 0.0001f)
			{
				return false;
			}
			if (Mathf.Abs(vector2.z * vector7.x - vector2.x * vector7.z) > vector6.z * vector3.x + vector6.x * vector3.z + 0.0001f)
			{
				return false;
			}
			if (Mathf.Abs(vector2.x * vector7.y - vector2.y * vector7.x) > vector6.x * vector3.y + vector6.y * vector3.x + 0.0001f)
			{
				return false;
			}
			return true;
		}

		public static bool LineBoxFromBaseIntersect(Vector3 rLineStart, Vector3 rLineEnd, Vector3 rPosition, Quaternion rRotation, float rWidth, float rHeight, float rDepth)
		{
			Matrix4x4 inverse = Matrix4x4.TRS(rPosition, rRotation, Vector3.one).inverse;
			Vector3 vector = inverse.MultiplyPoint(rLineStart);
			Vector3 vector2 = (inverse.MultiplyPoint(rLineEnd) - vector) * 0.5f;
			Vector3 vector3 = new Vector3(Mathf.Abs(vector2.x), Mathf.Abs(vector2.y), Mathf.Abs(vector2.z));
			Vector3 vector4 = new Vector3(rWidth * 0.5f, rHeight * 0.5f, rDepth);
			Vector3 vector5 = new Vector3(rWidth * -0.5f, rHeight * -0.5f, 0f);
			Vector3 vector6 = (vector4 - vector5) * 0.5f;
			Vector3 vector7 = vector + vector2 - (vector5 + vector4) * 0.5f;
			if (Mathf.Abs(vector7.x) > vector6.x + vector3.x)
			{
				return false;
			}
			if (Mathf.Abs(vector7.y) > vector6.y + vector3.y)
			{
				return false;
			}
			if (Mathf.Abs(vector7.z) > vector6.z + vector3.z)
			{
				return false;
			}
			if (Mathf.Abs(vector2.y * vector7.z - vector2.z * vector7.y) > vector6.y * vector3.z + vector6.z * vector3.y + 0.0001f)
			{
				return false;
			}
			if (Mathf.Abs(vector2.z * vector7.x - vector2.x * vector7.z) > vector6.z * vector3.x + vector6.x * vector3.z + 0.0001f)
			{
				return false;
			}
			if (Mathf.Abs(vector2.x * vector7.y - vector2.y * vector7.x) > vector6.x * vector3.y + vector6.y * vector3.x + 0.0001f)
			{
				return false;
			}
			return true;
		}

		public static bool CylinderContainsPoint(Vector3 pt1, Vector3 pt2, float rRadius, Vector3 testpt)
		{
			float num = rRadius * rRadius;
			float sqrMagnitude = (pt2 - pt1).sqrMagnitude;
			float num2 = pt2.x - pt1.x;
			float num3 = pt2.y - pt1.y;
			float num4 = pt2.z - pt1.z;
			float num5 = testpt.x - pt1.x;
			float num6 = testpt.y - pt1.y;
			float num7 = testpt.z - pt1.z;
			float num8 = num5 * num2 + num6 * num3 + num7 * num4;
			if (num8 < 0f || num8 > sqrMagnitude)
			{
				return false;
			}
			if (num5 * num5 + num6 * num6 + num7 * num7 - num8 * num8 / sqrMagnitude > num)
			{
				return false;
			}
			return true;
		}

		public static bool ContainsPoint(Vector3 rPoint, BoxCollider rCollider)
		{
			Transform transform = rCollider.transform;
			Matrix4x4 lhs = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
			Matrix4x4 rhs = Matrix4x4.TRS(rCollider.center, Quaternion.identity, rCollider.size);
			Vector3 vector = (lhs * rhs).inverse.MultiplyPoint(rPoint);
			if (vector.x < 0f - VECTOR3_HALF.x || vector.x > VECTOR3_HALF.x || vector.y < 0f - VECTOR3_HALF.y || vector.y > VECTOR3_HALF.y || vector.z < 0f - VECTOR3_HALF.z || vector.z > VECTOR3_HALF.z)
			{
				return false;
			}
			return true;
		}

		private static bool IgnoreCollider(Collider rCollider)
		{
			if (rCollider == null || rCollider.transform == null)
			{
				return true;
			}
			if (rCollider.isTrigger)
			{
				return true;
			}
			if (Ignore != null)
			{
				if (rCollider.transform == Ignore)
				{
					return true;
				}
				if (IsDescendant(Ignore, rCollider.transform))
				{
					return true;
				}
			}
			if (IgnoreArray != null)
			{
				for (int i = 0; i < IgnoreArray.Length; i++)
				{
					if (rCollider.transform == IgnoreArray[i])
					{
						return true;
					}
					if (IsDescendant(IgnoreArray[i], rCollider.transform))
					{
						return true;
					}
				}
			}
			return false;
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

		private static void GetLineDistanceFromBoxFace(ref Vector3 rBoxExtents, ref Vector3 rBoxPoint, ref Vector3 rBoxDirection, ref Vector3 rExtentToPoint, int rIndex0, int rIndex1, int rIndex2, ref float mLineDistance)
		{
			Vector3 vector = default(Vector3);
			vector[rIndex1] = rBoxPoint[rIndex1] + rBoxExtents[rIndex1];
			vector[rIndex2] = rBoxPoint[rIndex2] + rBoxExtents[rIndex2];
			if (rBoxDirection[rIndex0] * vector[rIndex1] >= rBoxDirection[rIndex1] * rExtentToPoint[rIndex0])
			{
				if (rBoxDirection[rIndex0] * vector[rIndex2] >= rBoxDirection[rIndex2] * rExtentToPoint[rIndex0])
				{
					float num = 1f / rBoxDirection[rIndex0];
					mLineDistance = (0f - rExtentToPoint[rIndex0]) * num;
					return;
				}
				float num2 = rBoxDirection[rIndex0] * rBoxDirection[rIndex0] + rBoxDirection[rIndex2] * rBoxDirection[rIndex2];
				float num3 = num2 * vector[rIndex1] - rBoxDirection[rIndex1] * (rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex2] * vector[rIndex2]);
				if (num3 <= 2f * num2 * rBoxExtents[rIndex1])
				{
					float num4 = num3 / num2;
					num2 += rBoxDirection[rIndex1] * rBoxDirection[rIndex1];
					num3 = vector[rIndex1] - num4;
					float num5 = rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex1] * num3 + rBoxDirection[rIndex2] * vector[rIndex2];
					mLineDistance = (0f - num5) / num2;
				}
				else
				{
					num2 += rBoxDirection[rIndex1] * rBoxDirection[rIndex1];
					float num6 = rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex1] * rExtentToPoint[rIndex1] + rBoxDirection[rIndex2] * vector[rIndex2];
					mLineDistance = (0f - num6) / num2;
				}
				return;
			}
			if (rBoxDirection[rIndex0] * vector[rIndex2] >= rBoxDirection[rIndex2] * rExtentToPoint[rIndex0])
			{
				float num7 = rBoxDirection[rIndex0] * rBoxDirection[rIndex0] + rBoxDirection[rIndex1] * rBoxDirection[rIndex1];
				float num8 = num7 * vector[rIndex2] - rBoxDirection[rIndex2] * (rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex1] * vector[rIndex1]);
				if (num8 <= 2f * num7 * rBoxExtents[rIndex2])
				{
					float num9 = num8 / num7;
					num7 += rBoxDirection[rIndex2] * rBoxDirection[rIndex2];
					num8 = vector[rIndex2] - num9;
					float num10 = rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex1] * vector[rIndex1] + rBoxDirection[rIndex2] * num8;
					mLineDistance = (0f - num10) / num7;
				}
				else
				{
					num7 += rBoxDirection[rIndex2] * rBoxDirection[rIndex2];
					float num11 = rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex1] * vector[rIndex1] + rBoxDirection[rIndex2] * rExtentToPoint[rIndex2];
					mLineDistance = (0f - num11) / num7;
				}
				return;
			}
			float num12 = 0f;
			float num13 = rBoxDirection[rIndex0] * rBoxDirection[rIndex0] + rBoxDirection[rIndex2] * rBoxDirection[rIndex2];
			float num14 = num13 * vector[rIndex1] - rBoxDirection[rIndex1] * (rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex2] * vector[rIndex2]);
			if (num14 >= 0f)
			{
				if (num14 <= 2f * num13 * rBoxExtents[rIndex1])
				{
					float num15 = num14 / num13;
					num13 += rBoxDirection[rIndex1] * rBoxDirection[rIndex1];
					num14 = vector[rIndex1] - num15;
					num12 = rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex1] * num14 + rBoxDirection[rIndex2] * vector[rIndex2];
					mLineDistance = (0f - num12) / num13;
				}
				else
				{
					num13 += rBoxDirection[rIndex1] * rBoxDirection[rIndex1];
					num12 = rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex1] * rExtentToPoint[rIndex1] + rBoxDirection[rIndex2] * vector[rIndex2];
					mLineDistance = (0f - num12) / num13;
				}
				return;
			}
			num13 = rBoxDirection[rIndex0] * rBoxDirection[rIndex0] + rBoxDirection[rIndex1] * rBoxDirection[rIndex1];
			num14 = num13 * vector[rIndex2] - rBoxDirection[rIndex2] * (rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex1] * vector[rIndex1]);
			if (num14 >= 0f)
			{
				if (num14 <= 2f * num13 * rBoxExtents[rIndex2])
				{
					float num16 = num14 / num13;
					num13 += rBoxDirection[rIndex2] * rBoxDirection[rIndex2];
					num14 = vector[rIndex2] - num16;
					num12 = rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex1] * vector[rIndex1] + rBoxDirection[rIndex2] * num14;
					mLineDistance = (0f - num12) / num13;
				}
				else
				{
					num13 += rBoxDirection[rIndex2] * rBoxDirection[rIndex2];
					num12 = rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex1] * vector[rIndex1] + rBoxDirection[rIndex2] * rExtentToPoint[rIndex2];
					mLineDistance = (0f - num12) / num13;
				}
			}
			else
			{
				num13 += rBoxDirection[rIndex2] * rBoxDirection[rIndex2];
				num12 = rBoxDirection[rIndex0] * rExtentToPoint[rIndex0] + rBoxDirection[rIndex1] * vector[rIndex1] + rBoxDirection[rIndex2] * vector[rIndex2];
				mLineDistance = (0f - num12) / num13;
			}
		}

		private static void GetLineDistanceFromBoxExtent(ref Vector3 rBoxExtents, ref Vector3 rBoxPoint, ref Vector3 rBoxDirection, ref float rLineDistance)
		{
			Vector3 rExtentToPoint = new Vector3(rBoxPoint.x - rBoxExtents[0], rBoxPoint.y - rBoxExtents[1], rBoxPoint.z - rBoxExtents[2]);
			if (rBoxDirection.y * rExtentToPoint.x >= rBoxDirection.x * rExtentToPoint.y)
			{
				if (rBoxDirection.z * rExtentToPoint.x >= rBoxDirection.x * rExtentToPoint.z)
				{
					GetLineDistanceFromBoxFace(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, ref rExtentToPoint, 0, 1, 2, ref rLineDistance);
				}
				else
				{
					GetLineDistanceFromBoxFace(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, ref rExtentToPoint, 2, 0, 1, ref rLineDistance);
				}
			}
			else if (rBoxDirection.z * rExtentToPoint.y >= rBoxDirection.y * rExtentToPoint.z)
			{
				GetLineDistanceFromBoxFace(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, ref rExtentToPoint, 1, 2, 0, ref rLineDistance);
			}
			else
			{
				GetLineDistanceFromBoxFace(ref rBoxExtents, ref rBoxPoint, ref rBoxDirection, ref rExtentToPoint, 2, 0, 1, ref rLineDistance);
			}
		}

		private static void GetLineDistanceFromBoxExtent(ref Vector3 rBoxExtents, ref Vector3 rBoxPoint, ref Vector3 rBoxDirection, int rIndex0, int rIndex1, ref float rLineDistance)
		{
			float num = rBoxPoint[rIndex0] - rBoxExtents[rIndex0];
			float num2 = rBoxPoint[rIndex1] - rBoxExtents[rIndex1];
			float num3 = rBoxDirection[rIndex1] * num;
			float num4 = rBoxDirection[rIndex0] * num2;
			float num5 = 0f;
			if (num3 >= num4)
			{
				rBoxPoint[rIndex0] = rBoxExtents[rIndex0];
				num2 = rBoxPoint[rIndex1] + rBoxExtents[rIndex1];
				if (num3 - rBoxDirection[rIndex0] * num2 >= 0f)
				{
					num5 = 1f / (rBoxDirection[rIndex0] * rBoxDirection[rIndex0] + rBoxDirection[rIndex1] * rBoxDirection[rIndex1]);
					rLineDistance = (0f - (rBoxDirection[rIndex0] * num + rBoxDirection[rIndex1] * num2)) * num5;
				}
				else
				{
					num5 = 1f / rBoxDirection[rIndex0];
					rLineDistance = (0f - num) * num5;
				}
			}
			else
			{
				rBoxPoint[rIndex1] = rBoxExtents[rIndex1];
				num = rBoxPoint[rIndex0] + rBoxExtents[rIndex0];
				if (num4 - rBoxDirection[rIndex1] * num >= 0f)
				{
					num5 = 1f / (rBoxDirection[rIndex0] * rBoxDirection[rIndex0] + rBoxDirection[rIndex1] * rBoxDirection[rIndex1]);
					rLineDistance = (0f - (rBoxDirection[rIndex0] * num + rBoxDirection[rIndex1] * num2)) * num5;
				}
				else
				{
					num5 = 1f / rBoxDirection[rIndex1];
					rLineDistance = (0f - num2) * num5;
				}
			}
		}

		private static void GetLineDistanceFromBoxExtent(ref Vector3 rBoxExtents, ref Vector3 rBoxPoint, ref Vector3 rBoxDirection, int rIndex0, ref float mLineDistance)
		{
			mLineDistance = (rBoxExtents[rIndex0] - rBoxPoint[rIndex0]) / rBoxDirection[rIndex0];
		}

		private static void GetClosestPointFromTerrain(TerrainCollider rCollider, Vector3 rStart, Vector3 rEnd)
		{
			Vector3 position = rCollider.transform.position;
			TerrainData terrainData = rCollider.terrainData;
			int heightmapWidth = terrainData.heightmapResolution;
			int heightmapHeight = terrainData.heightmapResolution;
			Vector3 size = terrainData.size;
			size = new Vector3(size.x / (float)(heightmapWidth - 1), size.y, size.z / (float)(heightmapHeight - 1));
			float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);
			Vector3[] array = new Vector3[heightmapWidth * heightmapHeight];
			for (int i = 0; i < heightmapHeight; i++)
			{
				for (int j = 0; j < heightmapWidth; j++)
				{
					array[i * heightmapWidth + j] = Vector3.Scale(size, new Vector3(-i, heights[j, i], j)) + position;
				}
			}
			int[] array2 = new int[(heightmapWidth - 1) * (heightmapHeight - 1) * 6];
			int num = 0;
			for (int k = 0; k < heightmapHeight - 1; k++)
			{
				for (int l = 0; l < heightmapWidth - 1; l++)
				{
					array2[num++] = k * heightmapWidth + l;
					array2[num++] = (k + 1) * heightmapWidth + l;
					array2[num++] = k * heightmapWidth + l + 1;
					array2[num++] = (k + 1) * heightmapWidth + l;
					array2[num++] = (k + 1) * heightmapWidth + l + 1;
					array2[num++] = k * heightmapWidth + l + 1;
				}
			}
			float num8 = float.MaxValue;
			for (num = 0; num < array2.Length; num += 3)
			{
				Vector3.SqrMagnitude(ClosestPoint(ref rEnd, ref array[array2[num]], ref array[array2[num + 1]], ref array[array2[num + 2]]) - rEnd);
			}
		}
	}
}

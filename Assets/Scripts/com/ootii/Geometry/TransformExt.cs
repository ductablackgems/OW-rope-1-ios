using System.Collections.Generic;
using UnityEngine;

namespace com.ootii.Geometry
{
	public static class TransformExt
	{
		public static Transform FindTransform(this Transform rThis, HumanBodyBones rBone)
		{
			Animator component = rThis.gameObject.GetComponent<Animator>();
			if (component != null)
			{
				return component.GetBoneTransform(rBone);
			}
			return null;
		}

		public static Transform FindTransform(this Transform rThis, string rName)
		{
			return FindChildTransform(rThis, rName);
		}

		public static Transform FindChildTransform(Transform rParent, string rName)
		{
			string name = rParent.name;
			if (string.Compare(name, rName, ignoreCase: true) == 0)
			{
				return rParent;
			}
			int num = name.IndexOf(':');
			if (num >= 0)
			{
				name = name.Substring(num + 1);
				if (string.Compare(name, rName, ignoreCase: true) == 0)
				{
					return rParent;
				}
			}
			for (int i = 0; i < rParent.transform.childCount; i++)
			{
				Transform transform = FindChildTransform(rParent.transform.GetChild(i), rName);
				if (transform != null)
				{
					return transform;
				}
			}
			return null;
		}

		public static void FindTransformChain(Transform rParent, string rName, ref List<Transform> rList)
		{
			Transform transform = rParent.FindTransform(rName);
			rList.Clear();
			while (transform != null)
			{
				rList.Add(transform);
				transform = ((transform.childCount <= 0) ? null : transform.GetChild(0));
			}
		}

		public static void Reset(this Transform rTransform)
		{
			if (!(rTransform == null))
			{
				rTransform.localPosition = Vector3.zero;
				rTransform.localRotation = Quaternion.identity;
				rTransform.localScale = Vector3.one;
			}
		}

		public static void ResetRect(this Transform rTransform)
		{
			if (!(rTransform == null))
			{
				rTransform.localPosition = Vector3.zero;
				rTransform.localRotation = Quaternion.identity;
				rTransform.localScale = Vector3.one;
				RectTransform component = rTransform.GetComponent<RectTransform>();
				if (component != null)
				{
					component.anchoredPosition = Vector2.zero;
				}
			}
		}
	}
}

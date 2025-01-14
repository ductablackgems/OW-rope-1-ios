using com.ootii.Data.Serializers;
using System;
using UnityEngine;

namespace com.ootii.Base
{
	[Serializable]
	public class BaseScriptableObject : ScriptableObject, IBaseObject
	{
		public GUIDChangedDelegate GUIDChangedEvent;

		[HideInInspector]
		public string _GUID = "";

		[SerializationIgnore]
		public virtual string GUID
		{
			get
			{
				if (_GUID.Length == 0)
				{
					GenerateGUID();
				}
				return _GUID;
			}
			set
			{
				if (value.Length != 0)
				{
					string gUID = _GUID;
					_GUID = value;
					if (gUID.Length > 0 && value != gUID)
					{
						OnGUIDChanged(gUID, _GUID);
					}
				}
			}
		}

		[SerializationIgnore]
		public virtual string Name
		{
			get
			{
				return base.name;
			}
			set
			{
				base.name = value;
			}
		}

		public string GenerateGUID()
		{
			_GUID = Guid.NewGuid().ToString();
			return _GUID;
		}

		public virtual void OnGUIDChanged(string rOldGUID, string rNewGUID)
		{
			if (GUIDChangedEvent != null)
			{
				GUIDChangedEvent(rOldGUID, rNewGUID);
			}
		}
	}
}

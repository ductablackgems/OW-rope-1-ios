using System;

namespace com.ootii.Base
{
	[Serializable]
	public class BaseObject : IBaseObject
	{
		public GUIDChangedDelegate GUIDChangedEvent;

		public string _GUID = "";

		public string _Name = "";

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

		public virtual string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}

		public BaseObject()
		{
		}

		public BaseObject(string rGUID)
		{
			_GUID = rGUID;
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

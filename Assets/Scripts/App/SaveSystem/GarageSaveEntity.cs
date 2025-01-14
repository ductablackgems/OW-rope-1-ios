using UnityEngine;

namespace App.SaveSystem
{
	public class GarageSaveEntity : AbstractSaveEntity
	{
		public bool vehicleInside;

		public string prefabTid;

		public GarageSaveEntity(string parentKey, string key)
		{
			GenerateEntityKey(parentKey, key);
		}

		protected override void LoadData()
		{
			vehicleInside = GetBool("vehicleInside");
			prefabTid = GetString("prefabTid");
		}

		protected override void SaveData(bool includeChildren)
		{
			SaveParam("vehicleInside", vehicleInside);
			SaveParam("prefabTid", prefabTid);
		}

		public override void Delete()
		{
			DeleteParam("vehicleInside");
			DeleteParam("prefabTid");
		}

		public override void Dump()
		{
			UnityEngine.Debug.Log($"{base.EntityKey} vehicleInside: {vehicleInside}, prefabTid: {prefabTid}");
		}
	}
}

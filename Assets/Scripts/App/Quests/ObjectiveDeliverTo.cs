namespace App.Quests
{
	public class ObjectiveDeliverTo : ObjectiveTalkTo
	{
		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			RemoveItem();
		}

		private void RemoveItem()
		{
		}
	}
}

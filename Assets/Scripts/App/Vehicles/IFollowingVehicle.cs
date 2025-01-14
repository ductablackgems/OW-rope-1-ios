namespace App.Vehicles
{
	public interface IFollowingVehicle
	{
		bool FollowingPlayer
		{
			get;
		}

		void SetFollowingPlayer(bool followingPlayer);
	}
}

using App.Player;
using UnityEngine;

namespace App.GUI
{
	public class CrimeMeter : MonoBehaviour
	{
		public GameObject[] stars;

		private CrimeManager crimeManager;

		private void Awake()
		{
			crimeManager = ServiceLocator.Get<CrimeManager>();
		}

		private void Update()
		{
			for (int i = 0; i < stars.Length; i++)
			{
				stars[i].SetActive(i < crimeManager.StarCount);
			}
		}
	}
}

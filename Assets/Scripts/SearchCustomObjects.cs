using App.Shop;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SearchCustomObjects : MonoBehaviour
{
	private ShoppingZone shoppingZone;

	public string[] Find;

	public List<GameObject> FindObjects;

	private void Start()
	{
		for (int i = 0; i < Find.Length; i++)
		{
			FindObjects.Add(GameObject.Find(Find[i]));
		}
	}
}

using System;
using UnityEngine;

public class MaterialType : MonoBehaviour
{
	[Serializable]
	public enum MaterialTypeEnum
	{
		Plaster,
		Metall,
		Folliage,
		Rock,
		Wood,
		Brick,
		Concrete,
		Dirt,
		Glass,
		Water
	}

	public MaterialTypeEnum TypeOfMaterial;
}

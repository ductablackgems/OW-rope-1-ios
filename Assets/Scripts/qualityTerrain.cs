using App;
using App.SaveSystem;
using UnityEngine;

public class qualityTerrain : MonoBehaviour
{
	private int qualityLevel = 1;

	private SettingsSaveEntity settingsSave;

	private Terrain terrain;

	private void Start()
	{
		settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
		qualityLevel = (int)settingsSave.graphicQuality;
		terrain = GetComponent<Terrain>();
		switch (qualityLevel)
		{
		case 3:
			terrain.detailObjectDistance = 80f;
			terrain.detailObjectDensity = 0.1f;
			terrain.treeBillboardDistance = 50f;
			terrain.heightmapPixelError = 20f;
			break;
		case 2:
			terrain.detailObjectDistance = 40f;
			terrain.detailObjectDensity = 0.05f;
			terrain.treeBillboardDistance = 25f;
			terrain.heightmapPixelError = 50f;
			break;
		case 1:
			terrain.detailObjectDistance = 20f;
			terrain.detailObjectDensity = 0.01f;
			terrain.treeBillboardDistance = 0f;
			terrain.heightmapPixelError = 100f;
			break;
		case 0:
			terrain.drawTreesAndFoliage = false;
			terrain.heightmapPixelError = 150f;
			break;
		default:
			terrain.drawTreesAndFoliage = false;
			terrain.heightmapPixelError = 150f;
			break;
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
	public GameObject CarModel;

	public Car PlayerCar;

	public GameObject CubeModel;

	public GameObject SphereModel;

	public bool ColorDeformationActive;

	public Color ColorDeformation;

	public float ColorDeformationItensity;

	public bool ColorDeformationUseCarColor;

	private List<ImpactDeformable> Deformables = new List<ImpactDeformable>();

	private void Start()
	{
		Deformables.Add(PlayerCar.CarHullDeformable);
		CreateAICar(Color.red, 1);
		CreateAICar(Color.green, 2);
		CreateAICar(Color.yellow, 3);
		CreateAICar(Color.white, 4);
		CreateAICar(Color.magenta, 5);
		CreateAICar(Color.cyan, 6);
		CreateAICar(Color.gray, 7);
	}

	private Car CreateAICar(Color color, int pos)
	{
		Car car = UnityEngine.Object.Instantiate(CarModel.GetComponent<Car>());
		UnityEngine.Object.Destroy(car.GetComponent<PlayerControl>());
		UnityEngine.Object.Destroy(car.transform.Find("CarCam").gameObject);
		car.Color = color;
		car.transform.position = Quaternion.Euler(0f, pos * 45, 0f) * new Vector3(0f, 0f, -15f);
		car.transform.forward = -car.transform.position;
		car.transform.parent = base.transform;
		car.name = "AI Car";
		car.gameObject.AddComponent<AIControl>();
		Deformables.Add(car.CarHullDeformable);
		return car;
	}

	private void Update()
	{
		if (!Input.GetMouseButton(0))
		{
			return;
		}
		Camera[] allCameras = Camera.allCameras;
		for (int i = 0; i < allCameras.Length; i++)
		{
			Ray ray = allCameras[i].ScreenPointToRay(UnityEngine.Input.mousePosition);
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Raycast(ray, out hitInfo))
			{
				ImpactDeformable component = hitInfo.collider.GetComponent<ImpactDeformable>();
				if (component != null)
				{
					component.Repair(0.05f, hitInfo.point, 0.75f);
				}
			}
		}
	}

	public void CreateCube()
	{
		CreateDeforambleObject(CubeModel);
	}

	public void CreateSphere()
	{
		CreateDeforambleObject(SphereModel);
	}

	public void CreateDeforambleObject(GameObject model)
	{
		Vector3 vector = UnityEngine.Random.insideUnitCircle * 20f;
		vector.z = vector.y;
		vector.y = 15f;
		GameObject gameObject = UnityEngine.Object.Instantiate(model, vector, UnityEngine.Random.rotation);
		gameObject.transform.parent = base.transform;
		Deformables.Add(gameObject.GetComponent<ImpactDeformable>());
		ApplyColorDeformation();
	}

	public void RepairAll()
	{
		Deformables.ForEach(delegate(ImpactDeformable i)
		{
			i.Repair(1f);
		});
	}

	public void RandomDamageAll()
	{
		Deformables.ForEach(delegate(ImpactDeformable i)
		{
			Collider component = i.GetComponent<Collider>();
			if (!(component == null))
			{
				Vector3 vector = UnityEngine.Random.onUnitSphere * 5f + i.transform.position;
				Ray ray = new Ray(vector, i.transform.position - vector);
				if (component.Raycast(ray, out RaycastHit hitInfo, 10f))
				{
					i.Deform(hitInfo.point, ray.direction.normalized * 0.3f);
				}
			}
		});
	}

	public void ApplyColorDeformation()
	{
		Deformables.ForEach(delegate(ImpactDeformable impDef)
		{
			if (!(impDef == null))
			{
				impDef.ColorDeformationDistance = (ColorDeformationActive ? Mathf.Lerp(0.4f, 0.01f, ColorDeformationItensity) : 0f);
				impDef.DeformedColor = ColorDeformation;
				impDef.OnGetDeformationColor -= this.OnGetDeformationColor;
				if (ColorDeformationUseCarColor)
				{
					impDef.OnGetDeformationColor += this.OnGetDeformationColor;
				}
			}
		});
	}

	private bool OnGetDeformationColor(ImpactDeformable impDef, ref Color deformationColor)
	{
		if (impDef.LastContact == null)
		{
			return false;
		}
		Car component = impDef.LastContact.gameObject.GetComponent<Car>();
		if (component == null)
		{
			return false;
		}
		deformationColor = component.Color;
		return true;
	}
}

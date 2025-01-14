using UnityEngine;

namespace MagicalFXMagicalEffect
{
	public class Wizard : MonoBehaviour
	{
		public GameObject[] Skills;

		private Vector3 positionLook;

		public int Index;

		public bool Showtime;

		public float Delay = 1f;

		public float RandomSize = 10f;

		public bool RandomSkill;

		private float timeTemp;

		private void Start()
		{
			timeTemp = Time.time;
		}

		private void Update()
		{
			if (Showtime)
			{
				if (Time.time >= timeTemp + Delay)
				{
					if (Physics.Raycast(new Ray(base.transform.position + new Vector3(UnityEngine.Random.Range(0f - RandomSize, RandomSize), 0f, UnityEngine.Random.Range(0f - RandomSize, RandomSize)), -Vector3.up), out RaycastHit hitInfo, 100f))
					{
						positionLook = hitInfo.point;
					}
					Quaternion rotation = Quaternion.LookRotation((positionLook - base.transform.position).normalized);
					rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
					base.transform.rotation = rotation;
					if (RandomSkill)
					{
						Index = UnityEngine.Random.Range(0, Skills.Length);
					}
					else
					{
						Index++;
					}
					Deploy();
					timeTemp = Time.time;
				}
			}
			else
			{
				Aim();
				if (Input.GetMouseButtonDown(0))
				{
					Deploy();
				}
			}
			KeyUpdate();
		}

		private void KeyUpdate()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.A))
			{
				Index--;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.D))
			{
				Index++;
			}
			if (Index < 0)
			{
				Index = Skills.Length - 1;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
			{
				if (Time.timeScale > 0.1f)
				{
					Time.timeScale = 0.01f;
				}
				else
				{
					Time.timeScale = 1f;
				}
			}
		}

		private void Deploy()
		{
			if (Index >= Skills.Length || Index < 0)
			{
				Index = 0;
			}
			FX_Position component = Skills[Index].GetComponent<FX_Position>();
			if ((bool)component)
			{
				if (component.Mode == SpawnMode.Static)
				{
					Place(Skills[Index]);
				}
				if (component.Mode == SpawnMode.OnDirection)
				{
					PlaceDirection(Skills[Index]);
				}
			}
			else
			{
				Shoot(Skills[Index]);
			}
		}

		private void Aim()
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition), out RaycastHit hitInfo, 100f))
			{
				positionLook = hitInfo.point;
			}
			Quaternion b = Quaternion.LookRotation((positionLook - base.transform.position).normalized);
			b.eulerAngles = new Vector3(0f, b.eulerAngles.y, 0f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, 0.5f);
		}

		private void Shoot(GameObject skill)
		{
			Object.Instantiate(skill, base.transform.position + Vector3.up * 0.5f + base.transform.forward, skill.transform.rotation).transform.forward = (positionLook - base.transform.position).normalized;
		}

		private void Place(GameObject skill)
		{
			Object.Instantiate(skill, positionLook, skill.transform.rotation);
		}

		private void PlaceDirection(GameObject skill)
		{
			FX_Position component = UnityEngine.Object.Instantiate(skill, base.transform.position + base.transform.forward, skill.transform.rotation).GetComponent<FX_Position>();
			if (component.Mode == SpawnMode.OnDirection)
			{
				component.transform.forward = base.transform.forward;
			}
		}

		private void OnGUI()
		{
			string text = "";
			if (Index >= 0 && Index < Skills.Length && Skills.Length != 0)
			{
				text = Skills[Index].name;
			}
			GUI.Label(new Rect(30f, 30f, Screen.width, 100f), text ?? "");
			if (GUI.Button(new Rect(30f, Screen.height - 40, 100f, 30f), "Prev"))
			{
				Index--;
			}
			if (GUI.Button(new Rect(140f, Screen.height - 40, 100f, 30f), "Next"))
			{
				Index++;
			}
			if (GUI.Button(new Rect(250f, Screen.height - 40, 100f, 30f), "Show time"))
			{
				Showtime = !Showtime;
			}
			if (Index < 0)
			{
				Index = Skills.Length - 1;
			}
		}
	}
}

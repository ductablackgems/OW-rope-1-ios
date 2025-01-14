using UnityEngine;

public class limitFramerate : MonoBehaviour
{
	private void Awake()
	{
		Application.targetFrameRate = 60;
	}
}

using System.Collections;
using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
	public class Distances : MonoBehaviour
	{
		public CurvySpline Spline;

		public Transform Cube;

		public int Amount = 10;

		public float Speed = 1f;

		private Transform[] cubes;

		private float[] tf;

		private int[] dir;

		private IEnumerator Start()
		{
			if ((bool)Spline && (bool)Cube)
			{
				while (!Spline.IsInitialized)
				{
					yield return null;
				}
				cubes = new Transform[Amount];
				tf = new float[Amount];
				dir = new int[Amount];
				cubes[0] = Cube;
				tf[0] = 0f;
				dir[0] = ((Speed >= 0f) ? 1 : (-1));
				float num = Spline.Length / (float)Amount;
				Cube.localScale = new Vector3(num * 0.7f, num * 0.7f, num * 0.7f);
				Cube.position = Spline.InterpolateByDistance(0f);
				for (int i = 1; i < Amount; i++)
				{
					tf[i] = Spline.DistanceToTF((float)i * num);
					cubes[i] = getCube();
					cubes[i].position = Spline.Interpolate(tf[i]);
					cubes[i].rotation = Spline.GetOrientationFast(tf[i]);
					dir[i] = ((Speed >= 0f) ? 1 : (-1));
				}
				Speed = Mathf.Abs(Speed);
			}
		}

		private void Update()
		{
			if (cubes != null && Spline.IsInitialized)
			{
				for (int i = 0; i < cubes.Length; i++)
				{
					cubes[i].position = Spline.MoveBy(ref tf[i], ref dir[i], Speed * Time.deltaTime, CurvyClamping.Loop);
					cubes[i].rotation = Spline.GetOrientationFast(tf[i]);
				}
			}
		}

		private Transform getCube()
		{
			return UnityEngine.Object.Instantiate(Cube);
		}
	}
}

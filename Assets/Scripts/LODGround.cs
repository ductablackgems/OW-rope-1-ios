using UnityEngine;

public class LODGround : MonoBehaviour
{
	public float dist_near = 600f;

	public float dist_far = 2000000f;

	private float obj_xpos;

	private float obj_zpos;

	private float dist_near_quad;

	private float dist_far_quad;

	private void Start()
	{
		Bounds bounds = GetComponent<Renderer>().bounds;
		obj_xpos = bounds.center.x;
		obj_zpos = bounds.center.z;
		dist_near_quad = dist_near * dist_near;
		dist_far_quad = dist_far * dist_far;
	}

	private void Update()
	{
		float num = (obj_xpos - Camera.main.transform.position.x) * (obj_xpos - Camera.main.transform.position.x) + (obj_zpos - Camera.main.transform.position.z) * (obj_zpos - Camera.main.transform.position.z);
		GetComponent<Renderer>().enabled = (num >= dist_near_quad && num < dist_far_quad);
	}
}

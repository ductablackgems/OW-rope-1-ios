using UnityEngine;

public class jiggleBones : MonoBehaviour
{
	public bool debugMode = true;

	public Transform targetBone;

	private Vector3 targetPos;

	private Vector3 dynamicPos;

	public Vector3 boneAxis = new Vector3(0f, 0f, 1f);

	public float targetDistance = 22f;

	public float bStiffness = 0.2f;

	public float bMass = 0.9f;

	public float bDamping = 0.75f;

	public float bGravity = 0.75f;

	private Vector3 force;

	private Vector3 acc;

	private Vector3 vel;

	public bool SquashAndStretch;

	public float sideStretch = 0.15f;

	public float frontStretch = 0.2f;

	private void Awake()
	{
		Vector3 vector = dynamicPos = targetBone.position + targetBone.TransformDirection(new Vector3(boneAxis.x * targetDistance, boneAxis.y * targetDistance, boneAxis.z * targetDistance));
	}

	private void LateUpdate()
	{
		targetBone.rotation = default(Quaternion);
		Vector3 dir = targetBone.TransformDirection(new Vector3(boneAxis.x * targetDistance, boneAxis.y * targetDistance, boneAxis.z * targetDistance));
		Vector3 vector = targetBone.TransformDirection(new Vector3(0f, 1f, 0f));
		Vector3 vector2 = targetBone.position + targetBone.TransformDirection(new Vector3(boneAxis.x * targetDistance, boneAxis.y * targetDistance, boneAxis.z * targetDistance));
		force.x = (vector2.x - dynamicPos.x) * bStiffness;
		acc.x = force.x / bMass;
		vel.x += acc.x * (1f - bDamping);
		force.y = (vector2.y - dynamicPos.y) * bStiffness;
		force.y -= bGravity / 10f;
		acc.y = force.y / bMass;
		vel.y += acc.y * (1f - bDamping);
		force.z = (vector2.z - dynamicPos.z) * bStiffness;
		acc.z = force.z / bMass;
		vel.z += acc.z * (1f - bDamping);
		dynamicPos += vel + force;
		targetBone.LookAt(dynamicPos, vector);
		if (SquashAndStretch)
		{
			float magnitude = (dynamicPos - vector2).magnitude;
			float x = (boneAxis.x != 0f) ? (1f + magnitude * frontStretch) : (1f + (0f - magnitude) * sideStretch);
			float y = (boneAxis.y != 0f) ? (1f + magnitude * frontStretch) : (1f + (0f - magnitude) * sideStretch);
			float z = (boneAxis.z != 0f) ? (1f + magnitude * frontStretch) : (1f + (0f - magnitude) * sideStretch);
			targetBone.localScale = new Vector3(x, y, z);
		}
		if (debugMode)
		{
			UnityEngine.Debug.DrawRay(targetBone.position, dir, Color.blue);
			UnityEngine.Debug.DrawRay(targetBone.position, vector, Color.green);
			UnityEngine.Debug.DrawRay(vector2, Vector3.up * 0.2f, Color.yellow);
			UnityEngine.Debug.DrawRay(dynamicPos, Vector3.up * 0.2f, Color.red);
		}
	}
}

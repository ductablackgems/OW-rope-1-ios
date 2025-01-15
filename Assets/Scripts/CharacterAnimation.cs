using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
	private CharacterController cc;

	private Animation anim;

	private void Start()
	{
		cc = GetComponentInChildren<CharacterController>();
		anim = GetComponentInChildren<Animation>();
	}

	private void LateUpdate()
	{
		if (cc.isGrounded && ETCInput.GetAxis("Vertical") != 0f)
		{
			anim.CrossFade("soldierRun");
		}
		if (cc.isGrounded && ETCInput.GetAxis("Vertical") == 0f && ETCInput.GetAxis("Horizontal") == 0f)
		{
			anim.CrossFade("soldierIdleRelaxed");
		}
		if (!cc.isGrounded)
		{
			anim.CrossFade("soldierFalling");
		}
		if (cc.isGrounded && ETCInput.GetAxis("Vertical") == 0f && ETCInput.GetAxis("Horizontal") > 0f)
		{
			anim.CrossFade("soldierSpinRight");
		}
		if (cc.isGrounded && ETCInput.GetAxis("Vertical") == 0f && ETCInput.GetAxis("Horizontal") < 0f)
		{
			anim.CrossFade("soldierSpinLeft");
		}
	}
}

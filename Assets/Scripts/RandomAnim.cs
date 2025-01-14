using UnityEngine;

public class RandomAnim : MonoBehaviour
{
	private Animator m_Animator;

	private int random;

	private bool startAnim;

	private void Start()
	{
		m_Animator = GetComponent<Animator>();
		randomAnimPlay();
	}

	private void Update()
	{
		startAnim = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Start");
		if (startAnim)
		{
			randomAnimPlay();
			startAnim = false;
		}
	}

	private void randomAnimPlay()
	{
		random = UnityEngine.Random.Range(0, 7);
		m_Animator.SetInteger("RandomIdle", random);
	}
}

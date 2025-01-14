using UnityEngine;

[AddComponentMenu("NGUI/Examples/Chat Participant")]
public class ChatParticipant : MonoBehaviour
{
	public GameObject prefab;

	public Transform lookAt;

	private HUDText mText;

	public HUDText hudText => mText;

	private void Start()
	{
		if (HUDRoot.go == null)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		GameObject gameObject = HUDRoot.go.AddChild(prefab);
		mText = gameObject.GetComponentInChildren<HUDText>();
		gameObject.AddComponent<UIFollowTarget>().target = base.transform;
		if (ChatManager.instance != null)
		{
			ChatManager.instance.AddParticipant(this);
		}
	}
}

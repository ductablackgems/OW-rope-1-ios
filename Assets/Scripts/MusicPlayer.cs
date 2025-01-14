using App;
using App.SaveSystem;
using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
	private static MusicPlayer _instance;

	private AudioSource MpPlayer;

	public AudioClip[] Clips;

	private SettingsSaveEntity settingsSave;

	public int num;

	private void Awake()
	{
		if (!_instance)
		{
			_instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		MpPlayer = GetComponent<AudioSource>();
		settingsSave = ServiceLocator.Get<SaveEntities>().SettingsSave;
	}

	private IEnumerator WaitForTrackTOend()
	{
		while (MpPlayer.isPlaying)
		{
			yield return new WaitForSeconds(0.5f);
		}
		num++;
		if (num > Clips.Length - 1)
		{
			num = UnityEngine.Random.Range(0, Clips.Length);
		}
		MpPlayer.clip = Clips[num];
		MpPlayer.Play();
		StartCoroutine(WaitForTrackTOend());
	}

	private void OnLevelWasLoaded(int level)
	{
		if (level == 2 && !(settingsSave.memoryRam < 1024f) && !MpPlayer.isPlaying)
		{
			num = 11;
			MpPlayer.clip = Clips[num];
			MpPlayer.loop = false;
			MpPlayer.Play();
			StartCoroutine(WaitForTrackTOend());
		}
	}
}

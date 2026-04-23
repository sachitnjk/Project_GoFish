using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct AudioConfig
{
	public AudioClip clip;
	public AudioMixerGroup group;
	public bool isLooping;
}

public class SoundManager : MonoBehaviour
{
	[SerializeField] private AudioConfig castLineSFX_Config;
	[SerializeField] private AudioConfig onBiteSFX_Config;
	[SerializeField] private AudioConfig biteSuccessHitSFX_Config;
	[SerializeField] private AudioConfig fishCaughtSFXConfig;
	[SerializeField] private AudioConfig resultsSFX_Config;
	[SerializeField] private AudioConfig baseBGM_Config;
	[SerializeField] private AudioConfig buttonSFX_Config;

	[SerializeField] private AudioMixer audioMixer;

	[SerializeField] private AudioSource bgmSource;
	[SerializeField] private AudioSource sfxSource;

	private float masterVolume;
	private float bgmVolume;
	private float sfxVolume;

	public static SoundManager instance;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
		
		DontDestroyOnLoad(gameObject);
		bgmSource.loop = true;

		SetMasterVolume(1f);
		SetBGMVolume(1f);
		SetSFXVolume(1f);

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{

		PlayBGM();
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SetMasterVolume(masterVolume);
		SetBGMVolume(bgmVolume);
		SetSFXVolume(sfxVolume);
	}

	public void PlayBGM()
	{
		AudioClip clip = baseBGM_Config.clip;
		if (clip == null) return;

		bgmSource.clip = clip;
		bgmSource.outputAudioMixerGroup = baseBGM_Config.group;
		bgmSource.Play();
	}

	public void PlayButtonSFX()
	{
		PlaySFX(buttonSFX_Config);
	}

	public void PlayCastLineSFX()
	{
		PlaySFX(castLineSFX_Config);
	}

	public void PlayOnBiteSFX()
	{
		PlaySFX(onBiteSFX_Config);
	}

	public void PlayOnBiteSuccessSFX()
	{
		PlaySFX(biteSuccessHitSFX_Config);
	}

	public void PlayFishCaughtSFX()
	{
		PlaySFX(fishCaughtSFXConfig);
	}

	public void PlayResultsSFX()
	{
		PlaySFX(resultsSFX_Config);
	}

	private void PlaySFX(AudioConfig audioConfig)
	{
		if (audioConfig.clip == null) return;

		sfxSource.outputAudioMixerGroup = audioConfig.group;

		sfxSource.PlayOneShot(audioConfig.clip);
	}

	#region Audio Slider control

	public void SetMasterVolume(float value)
	{
		masterVolume = value;
		audioMixer.SetFloat("MasterVolume", ConbvertToDB(value));
	}

	public void SetBGMVolume(float value)
	{
		bgmVolume = value;
		audioMixer.SetFloat("BGMVolume", ConbvertToDB(value));
	}

	public void SetSFXVolume(float value)
	{
		sfxVolume = value;
		audioMixer.SetFloat("SFXVolume", ConbvertToDB(value));
	}

	private float ConbvertToDB(float value)
	{
		value = Mathf.Clamp(value, 0.0001f, 1f);
		return Mathf.Log10(value) * 20f;
	}

	#endregion

	public bool TryGetMixerValue(string param, out float value)
	{
		return audioMixer.GetFloat(param, out value);
	}

	public static float DBToLinear(float db)
	{
		return Mathf.Pow(10f, db / 20);
	}
}

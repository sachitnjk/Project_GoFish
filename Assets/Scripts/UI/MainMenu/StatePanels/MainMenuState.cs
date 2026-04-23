using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : UIState
{
	[SerializeField] private GameObject panel;

	public override void OnEnter()
	{
		panel.SetActive(true);
	}

	public override void OnExit()
	{
		panel.SetActive(false);
	}

	public void OnPlayPressed()
	{
		PlayButtonSound();

		LoadingManager.TargetScene = "FishingGame";
		SceneManager.LoadScene("LoadingScene");

		UIManager.Instance.UI_StateMachine.Pop();
	}

	public void OnSettingsPressed()
	{
		PlayButtonSound();

		SettingsState settings = (FindFirstObjectByType<SettingsState>(FindObjectsInactive.Include));

		UIManager.Instance.UI_StateMachine.Push(settings);
	}

	public void OnCreditsPressed()
	{
		PlayButtonSound();

		CreditsState credits = (FindFirstObjectByType<CreditsState>(FindObjectsInactive.Include));

		UIManager.Instance.UI_StateMachine.Push(credits);
	}

	public void OnExitPressed()
	{
		PlayButtonSound();

		Application.Quit();
	}
}

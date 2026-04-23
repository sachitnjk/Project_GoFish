using UnityEngine;
using UnityEngine.SceneManagement;

public class EndState : UIState
{
	[SerializeField] private GameObject currentPanel;

	public override void OnEnter()
	{
		currentPanel.SetActive(true);
	}

	public override void OnExit()
	{
		currentPanel.SetActive(false);
	}

	public void OnBackToMenuTriggered()
	{
		GameManager.Instance.ResetScore();

		LoadingManager.TargetScene = "MainMenu";
		SceneManager.LoadScene("LoadingScene");

		currentPanel.SetActive(false);
	}
}

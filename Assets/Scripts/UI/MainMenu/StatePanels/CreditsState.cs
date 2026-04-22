using UnityEngine;

public class CreditsState : UIState
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

	public void OnBackPressed()
	{
		UIManager.Instance.UI_StateMachine.Pop();

		PlayButtonSound();
	}

}

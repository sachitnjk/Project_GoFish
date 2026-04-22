using UnityEngine;

public class UIBootstrap : MonoBehaviour
{
	[SerializeField] private MainMenuState mainMenuState;

	private void Start()
	{
		UIManager.Instance.UI_StateMachine.Push(mainMenuState);
	}
}

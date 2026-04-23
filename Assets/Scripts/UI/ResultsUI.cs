using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ResultsUI : MonoBehaviour
{

	[SerializeField] private TextMeshProUGUI rarity_TextBox;
	[SerializeField] private Image fishSprite_Image;
	[SerializeField] private TextMeshProUGUI fishName_TextBox;
	[SerializeField] private TextMeshProUGUI fishScore_TextBox;
	[SerializeField] private TextMeshProUGUI totalScoreDisplay_TextBox;
	[SerializeField] private Button continue_Button;

	private void Start()
	{
		continue_Button.onClick.AddListener(HandleOnContinue);
	}

	public void PopulateData(Rarity rarity, Sprite fishSprite, FishType fishName, int fishScoreToAdd)
	{
		rarity_TextBox.text = rarity.ToString();
		fishSprite_Image.sprite = fishSprite;
		fishName_TextBox.text = fishName.ToString();
		fishScore_TextBox.text = "Score of fish caught: " + fishScoreToAdd.ToString();
		totalScoreDisplay_TextBox.text = "Total score: " + GameManager.Instance.TotalScore.ToString() + "+" + fishScoreToAdd.ToString();
	}

	public void HandleOnContinue()
	{
		GameManager.Instance.IsResultsLock = false;

		if(GameManager.Instance.IsReadyToEnd())
		{
			EndState endState = (FindFirstObjectByType<EndState>(FindObjectsInactive.Include));
			UIManager.Instance.UI_StateMachine.Push(endState);
		}

		Destroy(this.gameObject);
	}
}

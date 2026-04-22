using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ResultsUI : MonoBehaviour
{

	[SerializeField] private TextMeshProUGUI rarity_TextBox;
	[SerializeField] private Image fishSprite_Image;
	[SerializeField] private TextMeshProUGUI fishName_TextBox;
	[SerializeField] private Button continue_Button;

	private void Start()
	{
		continue_Button.onClick.AddListener(HandleOnContinue);
	}

	public void PopulateData(Rarity rarity, Sprite fishSprite, FishType fishName)
	{
		rarity_TextBox.text = rarity.ToString();
		fishSprite_Image.sprite = fishSprite;
		fishName_TextBox.text = fishName.ToString();
	}

	public void HandleOnContinue()
	{
		GameManager.Instance.IsResultsLock = false;

		Destroy(this.gameObject);
	}
}

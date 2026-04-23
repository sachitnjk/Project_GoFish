using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	[Header("Reeling related references")]
	[SerializeField] private GameObject reelingSlider_Object;
	[SerializeField] private RectTransform trackArea;
	[SerializeField] private Slider reelingProgress_Slider;
	[SerializeField] private RectTransform playerReelingBar_UI;
	[SerializeField] private RectTransform fishBar_UI;
	[SerializeField] private Image fishBarSprite_Image;
	[SerializeField] private ResultsUI resultsUIPrefab;

	[field: SerializeField] public UIStateMachine UI_StateMachine { get; private set; }

	public ResultsUI currentInstantiatedResultsUI { get; private set; }

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	public void ToggleReelingUI(bool value)
	{
		reelingSlider_Object.SetActive(value);
	}

	public void UpdateReelingUI(FishRuntimeData fishRuntimeData)
	{
		fishBarSprite_Image.sprite = fishRuntimeData.fishData.fishSprite;
	}

	public void SetReelingValues(float reelPorgress, float playerBarPosition, float fishBarPosition)
	{
		float trackHeight = trackArea.rect.height;

		reelingProgress_Slider.value = reelPorgress;

		Vector2 playerBarPos = playerReelingBar_UI.anchoredPosition;
		float barHeight = playerReelingBar_UI.rect.height;
		float yPlayerBarPos = playerBarPosition * (trackHeight - barHeight);
		playerBarPos.y = yPlayerBarPos;
		playerReelingBar_UI.anchoredPosition = playerBarPos;

		Vector2 fishBarPos = fishBar_UI.anchoredPosition;
		float fishHeight = fishBar_UI.rect.height;
		float y = fishBarPosition * (trackHeight - fishHeight);
		fishBarPos.y = y;
		fishBar_UI.anchoredPosition = fishBarPos;
	}

	public void SpawnResults(Rarity rarity, Sprite fishSprite, FishType fishName)
	{
		if(currentInstantiatedResultsUI != null)
		{
			return;
		}

		currentInstantiatedResultsUI = Instantiate(resultsUIPrefab, this.transform);
		currentInstantiatedResultsUI.PopulateData(rarity, fishSprite, fishName);
	}
}

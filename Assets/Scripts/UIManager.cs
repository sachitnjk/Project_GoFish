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
	}

	public void ToggleReelingUI(bool value)
	{
		reelingSlider_Object.SetActive(value);
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
}

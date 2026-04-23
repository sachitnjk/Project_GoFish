using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public bool IsResultsLock;

	[field: SerializeField] public int maxScore { get; private set; } = 40;
	public int TotalScore { get; private set; }

	[Tooltip("Needs on scene reference")]
	[field: SerializeField] public WaterRippleController RippleController { get; private set; }

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(Instance.gameObject);
		}
	}

	public void AddToScore(int scoreToAdd)
	{
		int cachedScore = TotalScore;

		TotalScore += scoreToAdd;

		if(cachedScore == 0 && scoreToAdd > 0)
		{
			UIManager.Instance.ToggleTotalScoreUI(true);
		}
		else
		{
			UIManager.Instance.UpdateTotalScoreUI();
		}
	}

	public void ResetScore()
	{
		TotalScore = 0;
		UIManager.Instance.ToggleTotalScoreUI(true);
	}

	public bool IsReadyToEnd()
	{
		if (TotalScore >= maxScore)
		{
			return true;
		}

		return false;
	}
}

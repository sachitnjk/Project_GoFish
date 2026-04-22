using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public bool IsResultsLock;

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
}

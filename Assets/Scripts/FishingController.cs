using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


[Serializable]
public enum FishingState
{
	Idle,
	Waiting,
	Bite,
	Reeling,
	Result
}

[Serializable]
public enum FishType
{
	GoldenMackarel,
	MajiliSnapper,
	CoastalCatfish,
	Tuna,
	Sardine,
	Bream,
	Anchovy,
	Herring
}

[Serializable]
public enum Rarity
{
	Common,
	Uncommon,
	Rare
}

[System.Serializable]
public struct FishRuntimeData
{
	public FishData fishData;
	public Rarity rarity;
	//difficulty can be added in here

	public FishRuntimeData(FishData fishData, Rarity rarity)
	{
		this.fishData = fishData;
		this.rarity = rarity;
	}
}

public class FishingController : MonoBehaviour
{

	private FishingState currentState;
	private float stateTimer;

	[SerializeField] private FishDataComposite fishDataComposite;
	[SerializeField] private GameObject FishPrefab;

	[Header("STATE REFERENCES")]
	[Header("Waiting state references")]
	[Range(1.0f, 3.0f)]
	[SerializeField] private float waitingStateMinTime = 2.0f;
	[Range(4.0f, 8.0f)]
	[SerializeField] private float waitingStateMaxTime = 6.0f;
	[Header("Biting state references")]
	[SerializeField] private float biteStateTime = 1.5f;
	[Header("Result state references")]
	[SerializeField] private float resultStateTime = 2f;

	[Header("Reeling related references")]
	[SerializeField] private float reelProgress = 0f;
	[SerializeField] private float reelSpeed = 0.4f;
	[SerializeField] private float failSpeed = 0.2f;

	[SerializeField] private float playerBarPosition = 0f;
	[SerializeField] private float playerBarSpeed = 1.5f;
	[SerializeField] private float gravity = 2f;

	[SerializeField] private float fishPosition = 0f;
	[SerializeField] private float fishMoveSpeed = 1f;

	[SerializeField] private float catchThreshold = 1f;

	private bool isReeling = false;

	public FishingState CurrentState => currentState;

	private PlayerController playerController;
	private FishRuntimeData currentFishRuntimeData;
	private GameObject currentSpawnedFish;

	private void Start()
	{
		playerController = this.gameObject.AddComponent<PlayerController>();
	}

	private void OnDestroy()
	{
		
	}

	private void Update()
	{
		switch(currentState)
		{
			case FishingState.Idle:
				UpdateIdle();
				break;
			case FishingState.Waiting:
				UpdateWaiting();
				break;
			case FishingState.Bite:
				UpdateBite();
				break;
			case FishingState.Reeling:
				UpdateReeling();
				break;
			case FishingState.Result:
				UpdateResult();
				break;
		}
	}

	public void SetState(FishingState newState)
	{
		ExitState(currentState);

		currentState = newState;

		EnterState(newState);
	}

	public void EnterState(FishingState state)
	{
		switch(state)
		{
			case FishingState.Idle:
				break;
			case FishingState.Waiting:
				stateTimer = UnityEngine.Random.Range(waitingStateMinTime, waitingStateMaxTime);
				break;
			case FishingState.Bite:
				stateTimer = biteStateTime;
				break;
			case FishingState.Reeling:
				StartReeling();
				break;
			case FishingState.Result:
				stateTimer = resultStateTime;
				break;
		}
	}

	public void ExitState(FishingState state)
	{

	}

	#region State Transitions

	public void StartFishing()
	{
		SetState(FishingState.Waiting);
	}

	public void StartReeling()
	{
		reelProgress = 0f;
		playerBarPosition = 0.5f;
		fishPosition = UnityEngine.Random.value;

		isReeling = true;
	}

	public void OnBiteHit(Vector3 castEndPoint)
	{
		//Spawn random fish
		if(fishDataComposite != null)
		{
			FishData randomFishData = fishDataComposite.GetRandomFishData();

			int randomIndex = UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(Rarity)).Length);
			Rarity randomizedRarity = (Rarity)Enum.GetValues(typeof(Rarity)).GetValue(randomIndex);

			currentFishRuntimeData = new FishRuntimeData(randomFishData, randomizedRarity);
			SpawnFish(castEndPoint);

			SetState(FishingState.Reeling);
		}
	}

	private void SpawnFish(Vector3 castEndPoint)
	{
		currentSpawnedFish = Instantiate(FishPrefab, castEndPoint, Quaternion.identity);
		FishBase instantiatedFish = currentSpawnedFish.gameObject.GetComponent<FishBase>();
		instantiatedFish.PopulateData(currentFishRuntimeData);
	}

	#endregion

	#region Update states

	private void UpdateIdle()
	{
		return;
	}

	private void UpdateWaiting()
	{
		stateTimer -= Time.deltaTime;

		if (stateTimer <= 0f)
		{
			SetState(FishingState.Bite);
		}
	}

	private void UpdateBite()
	{
		stateTimer -= Time.deltaTime;

		if (stateTimer <= 0f)
		{
			SetState(FishingState.Idle);
		}
	}

	private void UpdateReeling()
	{
		float deltaTime = Time.deltaTime;

		if(playerController.isCastHeld)
		{
			playerBarPosition += playerBarSpeed * deltaTime;
		}
		else
		{
			playerBarPosition -= gravity * deltaTime;
		}

		playerBarPosition = Mathf.Clamp01(playerBarPosition);

		fishPosition += Mathf.Sin(Time.time * fishMoveSpeed) * deltaTime;
		fishPosition = Mathf.Clamp01(fishPosition);

		float barSize = 0.2f;

		bool isInside = playerBarPosition > fishPosition - barSize && playerBarPosition < fishPosition + barSize;

		if(isInside)
		{
			reelProgress += reelSpeed * deltaTime;
		}
		else
		{
			reelSpeed -= failSpeed * deltaTime;
		}

		reelProgress = Mathf.Clamp01(reelProgress);

		//Succcues scenario
		if(reelProgress >= catchThreshold)
		{
			isReeling = false;
			EnterState(FishingState.Result);
		}

		//fail Scenariao
		if(reelProgress <= 0f)
		{
			isReeling = false;
			EnterState(FishingState.Idle);
		}

		Debug.Log($"Fish: {fishPosition:F2} | Player: {playerBarPosition:F2} | Progress: {reelProgress:F2}");
	}

	private void UpdateResult()
	{
		stateTimer -= Time.deltaTime;

		if (stateTimer <= 0f)
		{
			SetState(FishingState.Idle);
		}
	}

	#endregion

}

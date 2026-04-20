using System;
using UnityEditor;
using UnityEngine;


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
	
	public FishingState CurrentState => currentState;

	private FishRuntimeData currentFishRuntimeData;
	private GameObject currentSpawnedFish;

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

	public void OnBiteHit()
	{
		//Spawn random fish
		if(fishDataComposite != null)
		{
			FishData randomFishData = fishDataComposite.GetRandomFishData();

			int randomIndex = UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(Rarity)).Length);
			Rarity randomizedRarity = (Rarity)Enum.GetValues(typeof(Rarity)).GetValue(randomIndex);

			currentFishRuntimeData = new FishRuntimeData(randomFishData, randomizedRarity);
			SpawnFish();

			SetState(FishingState.Reeling);
		}
	}

	private void SpawnFish()
	{
		currentSpawnedFish = Instantiate(FishPrefab);
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

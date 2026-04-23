using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;


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
	Bream,
	Sturgeon
}

[Serializable]
public enum Rarity
{
	Common,
	Uncommon,
	Rare
}

public enum FishBehaviour
{
	Smooth,
	Mixed,
	Erratic
}

[System.Serializable]
public struct FishRuntimeData
{
	public FishData fishData;
	public Rarity rarity;
	public FishBehaviour fishBehaviour;
	public float reelSpeed;
	public float failSpeed;

	public FishRuntimeData(FishData fishData, Rarity rarity, FishBehaviour fishBehaviour, float reelSpeed, float failSpeed)
	{
		this.fishData = fishData;
		this.rarity = rarity;
		this.fishBehaviour = fishBehaviour;
		this.reelSpeed = reelSpeed;
		this.failSpeed = failSpeed;
	}
}

public class FishingController : MonoBehaviour
{

	private FishingState currentState;
	private float stateTimer;

	[SerializeField] private FishDataComposite fishDataComposite;
	[SerializeField] private GameObject FishPrefab;

	[Header("STATE REFERENCES")]
	[SerializeField] private TextMeshProUGUI currentState_TextBox;
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
	//[SerializeField] private float reelSpeed = 0.4f;
	//[SerializeField] private float failSpeed = 0.2f;

	[SerializeField] private float playerBarPosition = 0f;
	[SerializeField] private float playerBarSpeed = 0.4f;
	[SerializeField] private float gravity = 2f;

	[SerializeField] private float fishPosition = 0f;
	//[SerializeField] private float fishMoveSpeed = 1f;

	[SerializeField] private float catchThreshold = 1f;

	private RectTransform currentStateTextBox_RectTransform;

	private float fishTarget;
	private float behaviourTimer;
	private float fishVelocity;

	private bool isReeling = false;

	public FishingState CurrentState => currentState;

	private PlayerController playerController;
	private FishRuntimeData currentFishRuntimeData;
	private GameObject currentSpawnedFish;

	private void Start()
	{
		playerController = GetComponent<PlayerController>();

		currentState_TextBox.text = currentState.ToString();
		currentStateTextBox_RectTransform = currentState_TextBox.GetComponent<RectTransform>();
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

	private void LateUpdate()
	{
		AlignTextToCam();
	}

	public void SetState(FishingState newState)
	{
		ExitState(currentState);

		currentState = newState;
		currentState_TextBox.text = newState.ToString();

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
				SoundManager.instance.PlayOnBiteSFX();
				stateTimer = biteStateTime;
				break;
			case FishingState.Reeling:
				StartReeling();
				break;
			case FishingState.Result:
				SoundManager.instance.PlayResultsSFX();
				GameManager.Instance.IsResultsLock = true;
				ShowResult();
				//stateTimer = resultStateTime;
				break;
		}
	}

	public void ExitState(FishingState state)
	{
		switch(state)
		{
			case FishingState.Idle:
				break;
			case FishingState.Waiting:
				break;
			case FishingState.Bite:
				break;
			case FishingState.Reeling:
				UIManager.Instance.ToggleReelingUI(false);
				break;
			case FishingState.Result:
				break;
		}
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
		fishPosition = UnityEngine.Random.Range(0.2f, 0.8f);
		fishTarget = fishPosition;

		behaviourTimer = 0f;
		fishVelocity = 0f;

		isReeling = true;
	}

	public void OnBiteHit(Vector3 castEndPoint)
	{
		//Spawn random fish
		if(fishDataComposite != null)
		{
			SoundManager.instance.PlayOnBiteSuccessSFX();

			FishData randomFishData = fishDataComposite.GetRandomFishData();

			int randomIndex = UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(Rarity)).Length);
			Rarity randomizedRarity = (Rarity)Enum.GetValues(typeof(Rarity)).GetValue(randomIndex);
			FishBehaviour fishBehaviour = GetFishBehaviour(randomizedRarity);
			float reelSpeed = GetReelMultiplier(fishBehaviour);
			float failSpeed = GetFailMultiplier(fishBehaviour);

			currentFishRuntimeData = new FishRuntimeData(randomFishData, randomizedRarity, fishBehaviour, reelSpeed, failSpeed);
			SpawnFish(castEndPoint);
			UIManager.Instance.UpdateReelingUI(currentFishRuntimeData);

			SetState(FishingState.Reeling);
		}
	}


	#endregion

	#region Helpers

	private void AlignTextToCam()
	{
		Vector3 dir = currentStateTextBox_RectTransform.position - Camera.main.gameObject.transform.position;
		dir.y = 0f;

		if (dir.sqrMagnitude > 0.001f)
			currentStateTextBox_RectTransform.rotation = Quaternion.LookRotation(dir);
	}

	private FishBehaviour GetFishBehaviour(Rarity rarity)
	{
		FishBehaviour behaviour = FishBehaviour.Smooth;
		switch (rarity)
		{
			case Rarity.Common:
				behaviour = FishBehaviour.Smooth;
				break;
			case Rarity.Uncommon:
				behaviour = FishBehaviour.Mixed;
				break;
			case Rarity.Rare:
				behaviour = FishBehaviour.Erratic;
				break;
		}

		return behaviour;
	}

	private float GetReelMultiplier(FishBehaviour behaviour)
	{
		switch (behaviour)
		{
			case FishBehaviour.Smooth: return 0.4f;
			case FishBehaviour.Mixed: return 0.26f;
			case FishBehaviour.Erratic: return 0.2f;
		}

		return 0.26f;
	}

	private float GetFailMultiplier(FishBehaviour behaviour)
	{
		switch (currentFishRuntimeData.fishBehaviour)
		{
			case FishBehaviour.Smooth: return 0.2f;
			case FishBehaviour.Mixed: return 0.35f;
			case FishBehaviour.Erratic: return 0.45f;
		}

		return 0.35f;
	}

	private void SpawnFish(Vector3 castEndPoint)
	{
		currentSpawnedFish = Instantiate(FishPrefab, castEndPoint, Quaternion.identity);
		FishBase instantiatedFish = currentSpawnedFish.gameObject.GetComponent<FishBase>();
		instantiatedFish.PopulateVisualData(currentFishRuntimeData);
	}

	private void UpdateFishMovement(float dt)
	{
		switch (currentFishRuntimeData.fishBehaviour)
		{
			case FishBehaviour.Smooth:
				UpdateSmooth(dt);
				break;

			case FishBehaviour.Mixed:
				UpdateMixed(dt);
				break;

			case FishBehaviour.Erratic:
				UpdateErratic(dt);
				break;
		}

		fishPosition = Mathf.Clamp01(fishPosition);
	}

	private void HandleOnReeledIn()
	{
		isReeling = false;
		StartCoroutine(PullFishToPlayer());
	}

	private IEnumerator PullFishToPlayer()
	{
		if (currentSpawnedFish == null) yield break;

		FishingRodController fishingRodController = playerController.FishingRodController;

		Transform fish = currentSpawnedFish.transform;
		Vector3 start = fish.position;
		Vector3 end = fishingRodController.CastLineStartPoint.position;

		float duration = 0.5f;
		float t = 0f;

		while (t < 1f)
		{
			if(fish != null)
			{
				t += Time.deltaTime / duration;

				Vector3 pos = Vector3.Lerp(start, end, t);

				float height = Mathf.Sin(t * Mathf.PI) * 1.5f;
				pos.y += height;

				fish.position = pos;

				fishingRodController.UpdateLineEndPoint(pos);
			}

			yield return null;
		}

		if(fish != null)
		{
			fish.position = end;
			SoundManager.instance.PlayFishCaughtSFX();
		}

		Destroy(currentSpawnedFish);
		fishingRodController.DisableLine();

		SetState(FishingState.Result);
		//EnterState(FishingState.Result);
	}

	private void ShowResult()
	{
		UIManager.Instance.SpawnResults(currentFishRuntimeData.rarity, currentFishRuntimeData.fishData.fishSprite, currentFishRuntimeData.fishData.fishType);
	}

	#endregion

	#region Fish Behvaiours

	private void UpdateSmooth(float dt)
	{
		if (Mathf.Abs(fishPosition - fishTarget) < 0.05f)
		{
			fishTarget = UnityEngine.Random.Range(0.2f, 0.8f);
		}

		float speed = currentFishRuntimeData.fishData.fishSpeed * 0.5f;

		fishPosition = Mathf.MoveTowards(fishPosition, fishTarget, dt * speed);
	}

	private void UpdateMixed(float dt)
	{
		behaviourTimer -= dt;

		if (behaviourTimer <= 0f)
		{
			fishTarget = UnityEngine.Random.Range(0f, 1f);
			behaviourTimer = UnityEngine.Random.Range(0.5f, 1.5f);
		}

		float speed = currentFishRuntimeData.fishData.fishSpeed;

		fishPosition = Mathf.MoveTowards(fishPosition, fishTarget, dt * speed);
	}

	private void UpdateErratic(float dt)
	{
		behaviourTimer -= dt;

		if (behaviourTimer <= 0f)
		{
			fishVelocity = UnityEngine.Random.Range(-1.5f, 1.5f) * currentFishRuntimeData.fishData.fishSpeed;
			behaviourTimer = UnityEngine.Random.Range(0.1f, 0.4f);
		}

		fishPosition += fishVelocity * dt;

		//Added little bounce for fun element
		if (fishPosition <= 0f || fishPosition >= 1f)
		{
			fishVelocity *= -1f;
		}
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
		UIManager.Instance.ToggleReelingUI(true);

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

		//fishTimer += deltaTime;
		//fishPosition = Mathf.PingPong(fishTimer * currentFishRuntimeData.fishData.fishSpeed, 1f);
		UpdateFishMovement(deltaTime);
		fishPosition = Mathf.Clamp01(fishPosition);

		float barSize = 0.2f;

		bool isInside = playerBarPosition > fishPosition - barSize && playerBarPosition < fishPosition + barSize;

		if(isInside)
		{
			reelProgress += currentFishRuntimeData.reelSpeed * deltaTime;
		}
		else
		{
			reelProgress -= currentFishRuntimeData.failSpeed * deltaTime;
		}

		reelProgress = Mathf.Clamp01(reelProgress);

		//Succcues scenario
		if(reelProgress >= catchThreshold)
		{
			isReeling = false;

			//Trigger fish being pulled out of water.
			HandleOnReeledIn();

			//SetState(FishingState.Result);
		}

		//fail Scenariao
		if(reelProgress <= 0f)
		{
			isReeling = false;
			EnterState(FishingState.Idle);
		}

		UIManager.Instance.SetReelingValues(reelProgress, playerBarPosition, fishPosition);
	}

	private void UpdateResult()
	{
		if(!GameManager.Instance.IsResultsLock)
		{
			SetState(FishingState.Idle);
		}
		return;
	}

	#endregion

}

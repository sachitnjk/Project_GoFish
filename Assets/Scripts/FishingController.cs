using System;
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

public class FishingController : MonoBehaviour
{

	private FishingState currentState;
	private float stateTimer;
	
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
		SetState(FishingState.Reeling);
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

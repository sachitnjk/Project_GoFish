using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private PlayerInput playerInput;
	private InputAction CastAction;

	[SerializeField] private FishingController fishingController;
	[SerializeField] private FishingRodController fishingRodController;

	public FishingRodController FishingRodController => fishingRodController;

	public bool isCastHeld { get; private set; } = false;

	private void Start()
	{
		playerInput = PlayerInputProvider.Instance.PlayerInput;
		CastAction = playerInput.actions["Interact"];

		CastAction.performed += HandleOnCastAction;
		CastAction.canceled += HandleOnCastActionReleased;
	}

	private void OnDestroy()
	{
		CastAction.performed -= HandleOnCastAction;
		CastAction.canceled -= HandleOnCastActionReleased;
	}

	private void HandleOnCastAction(InputAction.CallbackContext context)
	{
		isCastHeld = true;

		if (fishingController.CurrentState == FishingState.Bite && fishingRodController.CastEndPoint != null)
		{
			fishingController.OnBiteHit(fishingRodController.CastEndPoint);
		}
	}

	private void HandleOnCastActionReleased(InputAction.CallbackContext context)
	{
		isCastHeld = false;

		if (fishingController.CurrentState != FishingState.Idle)
		{
			return;
		}


		fishingRodController.ReleaseCastWindup(() =>
		{
			if(fishingController.CurrentState == FishingState.Idle)
			{
				fishingController.StartFishing();
			}
		});
	}
}

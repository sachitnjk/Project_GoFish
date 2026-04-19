using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private PlayerInput playerInput;
	private InputAction CastAction;

	[SerializeField] private FishingController fishingController;
	[SerializeField] private FishingRodController fishingRodController;

	private bool isCastHeld = false;

	private void Start()
	{
		playerInput = PlayerInputProvider.Instance.PlayerInput;
		CastAction = playerInput.actions["Cast"];

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

		if (fishingController.CurrentState == FishingState.Bite)
		{
			fishingController.OnBiteHit();
		}
	}

	private void HandleOnCastActionReleased(InputAction.CallbackContext context)
	{
		isCastHeld = false;

		fishingRodController.ReleaseCastWindup(() =>
		{
			if(fishingController.CurrentState == FishingState.Idle)
			{
				fishingController.StartFishing();
			}
		});
	}
}

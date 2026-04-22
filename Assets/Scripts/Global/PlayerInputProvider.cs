using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputProvider : MonoBehaviour
{
	public static PlayerInputProvider Instance;

	[SerializeField] private PlayerInput playerInput;

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

	public PlayerInput PlayerInput => playerInput;
}

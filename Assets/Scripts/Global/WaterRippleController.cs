using UnityEngine;

public class WaterRippleController : MonoBehaviour
{
	private Material waterMaterial;

	[Header("Ripple Settings")]
	[SerializeField] private float rippleStrength = 0.2f;
	[SerializeField] private float rippleRadius = 3f;

	private void Awake()
	{
		Renderer renderer = GetComponent<Renderer>();
		waterMaterial = renderer.material;
	}

	public void TriggerRipple(Vector3 worldPos)
	{
		waterMaterial.SetVector("_ImpactPos", worldPos);
		waterMaterial.SetFloat("_ImpactTime", Time.time);

		waterMaterial.SetFloat("_RippleStrength", rippleStrength);
		waterMaterial.SetFloat("_RippleRadius", rippleRadius);
	}
}

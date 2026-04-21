using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "Scriptable/FishData", order = 1)]
public class FishData : ScriptableObject
{
	public FishType fishType;
	public Sprite fishSprite;
}

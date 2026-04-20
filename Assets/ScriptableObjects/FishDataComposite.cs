using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FishDataComposite : ScriptableObject
{
	public List<FishData> AllFishData = new List<FishData>();

	public FishData GetRandomFishData()
	{
		int randomIndex = UnityEngine.Random.Range(0, AllFishData.Count);
		return AllFishData[randomIndex];
	}
}

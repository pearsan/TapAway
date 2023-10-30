using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalFortuneUIManager : StandardizedUIManager
{
    [System.Serializable]
    public class RewardPrefab
    {
        public string ID;
        public GameObject Reward;
    }    

    [Tooltip("Recognize by ID")]
    public List<RewardPrefab> RewardPrefabs;

    protected override void Initialize()
    {
        
    }
}

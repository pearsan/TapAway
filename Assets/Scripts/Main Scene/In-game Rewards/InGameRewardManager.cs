using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameRewardManager : MonoBehaviour
{
    [HideInInspector] public int GoldReward;
    public static InGameRewardManager Instance;

    [SerializeField] private HorizontalFortuneBehaviourManager _horizontalFortuneBehaviourManager;
    private void Awake()
    {
        Instance = this;
    }

    public void GenerateNewGoldReward()
    {
        GoldReward = Random.Range(20, 50);
    }

    public void EnableFortune()
    {
        _horizontalFortuneBehaviourManager.SetData("CanSelectNewSlot", true);
    }    

    public int GetMultiplyEfficient()
    {
        string multiplyID = _horizontalFortuneBehaviourManager.InstantiateRules[(int)_horizontalFortuneBehaviourManager.GetData("SelectingSlotIndex")];
        switch(multiplyID)
        {
            case "x2": return 2;
            case "x3": return 3;
            case "x4": return 4;
            case "x5": return 5;
        }
        return 2;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRewardManager : MonoBehaviour
{
    public static LevelRewardManager Instance;

    private LevelRewardSheet[] LevelRewardDataSO;

    private int _totalLevelToClaim;
    private int _totalLevelCompleted;

    private int _currentReward;

    [HideInInspector] public bool IsRewardClaim = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        LevelRewardDataSO = Resources.Load<SerializableSet>("Level Rewards/SerializableSet").LevelRewards;
        UpdateRewardProgress();
    }

    private void Update()
    {
        UpdateRewardProgress();
    }

    public (int, int) GetData()
    {
        return (_totalLevelCompleted, _totalLevelToClaim);
    }    

    public bool OnValidateTriggerClaimRewardEvent()
    {
        Debug.Log(LevelRewardDataSO[_currentReward].Reward);
        return _totalLevelCompleted + 1 == _totalLevelToClaim;
    }

    public void OnChooseRewardDependExcel()
    {
        ShopGUIManager.Instance.ChooseNewRandomSkin();
        ShopManager.Instance.MarkShopItemIsUnlock();
    }    

    /// <summary>
    /// Example: 1, 4, 9 are the levels with rewards. If the player's current level is 5, total: 9 - 4 = 5, completed: 5 - 4 = 1; if current is 3, total: 4-1=3, completed: 3-1=2
    /// </summary>
    /// <returns></returns>
    private void UpdateRewardProgress()
    {
        for (int i = 0; i < LevelRewardDataSO.Length - 1; i++)
        {
            if (LevelRewardDataSO[i].Level <= GameplayManager.Instance.GetCurrentStage() && GameplayManager.Instance.GetCurrentStage() < LevelRewardDataSO[i + 1].Level)
            {
                _totalLevelCompleted = GameplayManager.Instance.GetCurrentStage() - LevelRewardDataSO[i].Level;
                _totalLevelToClaim = LevelRewardDataSO[i + 1].Level - LevelRewardDataSO[i].Level;
                _currentReward = i + 1;
                return;
            }
        }
        //in case player never reach first reward, that's alway first progress of reward system
        _totalLevelCompleted = GameplayManager.Instance.GetCurrentStage();
        _totalLevelToClaim = LevelRewardDataSO[0].Level;
    }    
}
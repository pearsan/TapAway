using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class InGameRewardUIManager : MonoBehaviour
{
    public static InGameRewardUIManager Instance;

    [Header("Panels")]
    [SerializeField] private GameObject InGameRewardPanel;

    [Header("Texts")]
    [SerializeField] private TMP_Text _goldEarnText;
    [SerializeField] private TMP_Text _goldEarnByAdsText;

    private int goldEarned;

    private void Awake()
    {
        Instance = this;
        InGameRewardPanel.SetActive(true);
        InGameRewardPanel.SetActive(false);
    }

    private void Update()
    {
        UpdateGoldText();
    }

    #region Button behaviours
    public void ClaimGoldWithoutADsButton()
    {
        GameUIManager.Instance.OnAddGoldFeedbackAnimation(InGameRewardManager.Instance.GoldReward);
    }

    public void ClaimGoldWithADSButton()
    {
        ISHandler.Instance.ShowRewardedVideo("In-game ads to multiply gold cube"
            , () => 
            { 
                goldEarned = InGameRewardManager.Instance.GoldReward * InGameRewardManager.Instance.GetMultiplyEfficient();
                DOTween.Restart("Scale_Down_In-game_Reward_Panel");
            }
            , () => { });
    }    
    #endregion

    public void OnAddGold()
    {

        GameUIManager.Instance.OnAddGoldFeedbackAnimation(goldEarned);
    }

    #region Text behaviours
    private void UpdateGoldText()
    {
        if (InGameRewardPanel.activeSelf)
        {
            _goldEarnText.text = "+" + InGameRewardManager.Instance.GoldReward;
            _goldEarnByAdsText.text = "Claim " + InGameRewardManager.Instance.GoldReward * InGameRewardManager.Instance.GetMultiplyEfficient();
        }
    }
    #endregion

    #region Panel behaviours
    public void OnEnterInGameRewardPanel()
    {
        InGameRewardManager.Instance.GenerateNewGoldReward();
        InGameRewardManager.Instance.EnableFortune();

        InGameRewardPanel.SetActive(true);
        DOTween.Restart("Scale_Up_In-game_Reward_Panel");
    }
    #endregion
}

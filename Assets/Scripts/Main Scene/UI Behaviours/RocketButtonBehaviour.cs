using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketButtonBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject GoldDescription;
    [SerializeField] private GameObject AdsDescription;

    [Space(10f)]
    [SerializeField] private CameraBehaviour CameraBehaviour;

    private void Update()
    {
        UpdateDescriptionDependGold();
    }

    public void RocketButton()
    {
        if (!CameraBehaviour.CanRocketMode()) return;

        if (GoldManager.Instance.GetGold() >= 100)
        {
            GoldManager.Instance.ModifyGoldValue(-100);
            CameraBehaviour.Instance.OnRocketMode();
        }
        else
        {
            ISHandler.Instance.ShowRewardedVideo("Buy rocket by ads"
                , () => {            
                    CameraBehaviour.Instance.OnRocketMode();
                }
                , () => { });
        }

    }

    private void UpdateDescriptionDependGold()
    {
        GoldDescription.SetActive(GoldManager.Instance.GetGold() >= 100);
        AdsDescription.SetActive(GoldManager.Instance.GetGold() < 100);
    }
}

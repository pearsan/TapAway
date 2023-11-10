using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombButtonBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject GoldDescription;
    [SerializeField] private GameObject AdsDescription;

    [Space(10f)]
    [SerializeField] private CameraBehaviour CameraBehaviour;

    private void Update()
    {
        UpdateDescriptionDependGold();   
    }

    public void BombButton()
    {
        if (!CameraBehaviour.CanBombMode()) return;

        if(GoldManager.Instance.GetGold() >= 100)
        {
            GoldManager.Instance.ModifyGoldValue(-100);
            CameraBehaviour.Instance.OnBombMode();
        }
        else
        {
            ISHandler.Instance.ShowRewardedVideo("Buy bomb by ads"
                , () => {
                    CameraBehaviour.Instance.OnBombMode();
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

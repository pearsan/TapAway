using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketButtonBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject GoldDescription;
    [SerializeField] private GameObject AdsDescription;

    private void Update()
    {
        UpdateDescriptionDependGold();
    }

    public void RocketButton()
    {
        if (GoldManager.Instance.GetGold() >= 100)
        {
            GoldManager.Instance.ModifyGoldValue(-100);
            Debug.Log("Rocket event in there");
        }
        else
        {
            ISHandler.Instance.ShowRewardedVideo("Buy bomb by ads"
                , () => { Debug.Log("Rocket event in there"); }
                , () => { });
        }

    }

    private void UpdateDescriptionDependGold()
    {
        GoldDescription.SetActive(GoldManager.Instance.GetGold() >= 100);
        AdsDescription.SetActive(GoldManager.Instance.GetGold() < 100);
    }
}

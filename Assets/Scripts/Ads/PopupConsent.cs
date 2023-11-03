using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core.Easing;

public class PopupConsent : MonoBehaviour
{
    private const string AlreadyConfirmPopupConsent = "already_confirm_popup_consent";
    [SerializeField] private GameObject panelConsent;

    private void Start()
    {
        var consentNum = PlayerPrefs.GetInt(AlreadyConfirmPopupConsent, 0);

        if (consentNum == 0)
        {
            panelConsent.SetActive(true);
            return;
        }

        panelConsent.SetActive(false);
        ISHandler.Instance.Init(consentNum == 1);
    }

    public void OnClickBtnYes()
    {
        panelConsent.SetActive(false);
        AudioManager.Instance.ClickButton();
        PlayerPrefs.SetInt(AlreadyConfirmPopupConsent, 1);
        ISHandler.Instance.Init(true);
    }


    public void OnClickBtnNo()
    {
        panelConsent.SetActive(false);
        AudioManager.Instance.ClickButton();
        PlayerPrefs.SetInt(AlreadyConfirmPopupConsent, 2);
        ISHandler.Instance.Init(false);
    }
}
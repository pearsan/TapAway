using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FalconPopupForceUpdate : MonoBehaviour
{
    public GameObject groupOkCancel;
    public GameObject groupOk;

    public Text textTitle;
    public Text textUpdate;
    public Text textUpdate1;
    public Text textCancel;

    private void OnEnable()
    {
        Debug.LogWarning("lmao");
    }

    public void ShowOkCancel()
    {
        groupOkCancel.SetActive(true);
        groupOk.SetActive(false);
    }

    public void ShowOkOnly()
    {
        groupOkCancel.SetActive(false);
        groupOk.SetActive(true);
    }

    public void ButtonUpdate()
    {
        ForceUpdateConfig config = FalconConfig.Instance<ForceUpdateConfig>();
#if UNITY_ANDROID
        if (config.f_core_popupUpdate_url_store_android == "")
        {
            Application.OpenURL("market://details?id=" + Application.identifier);
        }
        else
        {
            Application.OpenURL(config.f_core_popupUpdate_url_store_android);
        }
#elif UNITY_IOS
        if (config.f_core_popupUpdate_url_store_ios == "")
        {
            Application.OpenURL("itms-apps://itunes.apple.com/app/" + Application.identifier);
        }
        else
        {
            Application.OpenURL(config.f_core_popupUpdate_url_store_ios);
        }
#endif
    }

    public void ButtonCancel()
    {
        gameObject.SetActive(false);
    }

    public void UpdateUI(ForceUpdateConfig config)
    {
        textTitle.text = config.f_core_popupUpdate_title;
        textUpdate.text = config.f_core_popupUpdate_button_update;
        textUpdate1.text = config.f_core_popupUpdate_button_update;
        textCancel.text = config.f_core_popupUpdate_button_cancel;
    }
}

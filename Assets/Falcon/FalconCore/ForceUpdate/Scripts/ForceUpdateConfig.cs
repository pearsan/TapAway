using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;

public class ForceUpdateConfig : FalconConfig
{
    public string f_core_popupUpdate_url_store_android = "";
    public string f_core_popupUpdate_url_store_ios = "";

    public string f_core_popupUpdate_minVersion_android = "";
    public string f_core_popupUpdate_minVersion_ios = "";
    public string f_core_popupUpdate_targetVersion_android = "";
    public string f_core_popupUpdate_targetVersion_ios = "";

    public string f_core_popupUpdate_title = "You are using an old version, please update!";
    public string f_core_popupUpdate_button_cancel = "Cancel";
    public string f_core_popupUpdate_button_update = "Update";
}

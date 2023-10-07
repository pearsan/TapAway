using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaEffect : MonoBehaviour
{
    [SerializeField] private CameraBehaviour gachaCubeRotater;

    private GameObject _obtainedObject;

    public void OnGachaEffect (ShopItemSO obtainedObject)
    {
        if (_obtainedObject != null)
            Destroy(_obtainedObject);
    }    
}

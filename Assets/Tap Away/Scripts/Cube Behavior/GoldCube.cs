using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCube : MonoBehaviour, ITappable
{
    public void Tap()
    {
        transform.parent = null;
        Destroy(gameObject);
    }
}

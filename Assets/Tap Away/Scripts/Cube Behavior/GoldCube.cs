using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCube : MonoBehaviour, ITappable
{
    public void Tap()
    {
        Destroy(gameObject);
    }
}

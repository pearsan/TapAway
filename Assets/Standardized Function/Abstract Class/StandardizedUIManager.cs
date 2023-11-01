using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StandardizedUIManager : MonoBehaviour
{
    [SerializeField] protected StandardizedBehaviourManager _standardizedBehaviourManager;

    protected virtual void Start()
    {
        _standardizedBehaviourManager.Subcribe(this);
        Initialize();
    }

    protected abstract void Initialize();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StandardizedBehaviourManager : MonoBehaviour
{
    //Neither have another class in this script (Full loose coupling)   
    public Dictionary<string, object> DataDictionary;

    [HideInInspector] public StandardizedUIManager Subcriber; //Use "Observer pattern" idea

    protected virtual void Start()
    {
        Initialize();
    }
    protected abstract void Initialize();

    public void Subcribe(StandardizedUIManager subcriber)
    {
        Subcriber = subcriber;
    }
    public abstract void SetData();
    public abstract object GetData(string key);
}

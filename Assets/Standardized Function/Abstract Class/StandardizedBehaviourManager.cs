using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StandardizedBehaviourManager : MonoBehaviour
{
    //Neither have another class in this script (Full loose coupling)   
    public Dictionary<string, object> DataDictionary;

    [HideInInspector] public StandardizedUIManager Subcriber; //Use "Observer pattern" idea


    protected virtual void Awake()
    {
        DataDictionary = new Dictionary<string, object>();
        Initialize();
    }
    protected virtual void Initialize()
    {
        
    }    

    public void Subcribe(StandardizedUIManager subcriber)
    {
        Subcriber = subcriber;
    }
    public abstract void SetData(string key, object value);
    public abstract object GetData(string key);
}

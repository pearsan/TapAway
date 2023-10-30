using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalFortuneBehaviourManager : StandardizedBehaviourManager
{
    [Tooltip("The following array dictates how to generate items and their order, with IDs taken from the UIManager.")]
    public List<string> InstantiateRules;

    protected override void Initialize()
    {
        DataDictionary.Add("InstantiateRules", InstantiateRules);
    }

    public override void SetData()
    {
        
    }

    public override object GetData(string key)
    {
        object returnObject;
        DataDictionary.TryGetValue(key, out returnObject);
        return returnObject;
    }
}

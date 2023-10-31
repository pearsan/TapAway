using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalFortuneBehaviourManager : StandardizedBehaviourManager
{
    [Tooltip("The following array dictates how to generate items and their order, with IDs taken from the UIManager.")]
    public List<string> InstantiateRules;

    [HideInInspector] public List<GameObject> FortuneSlots; //This list save all slot in fortune (UI in game but in hierachy)

    [Tooltip("Time between select new slot in fortune")]
    [SerializeField] private float timeInterval;

    private int _selectingSlotIndex = 0;
    private float timer = 0;

    protected override void Initialize()
    {
        DataDictionary.Add("InstantiateRules", InstantiateRules);
        DataDictionary.Add("SelectingSlotIndex", _selectingSlotIndex);
        DataDictionary.Add("CanSelectNewSlot", true);
        FortuneSlots = new List<GameObject>();

        base.Initialize();
    }

    private void Update()
    {
        SelectNewSlot();
    }

    public override void SetData(string key, object value)
    {
        DataDictionary[key] = value;
    }

    public override object GetData(string key)
    {
        object returnObject = new object();
        DataDictionary.TryGetValue(key, out returnObject);
        return returnObject;
    }

    private void SelectNewSlot()
    {
        if (! (bool)DataDictionary["CanSelectNewSlot"] ) return;

        timer += Time.deltaTime;

        if (timer >= timeInterval)
        {
            timer = 0;
            _selectingSlotIndex++;

            if (_selectingSlotIndex >= FortuneSlots.Count)
            {
                _selectingSlotIndex = 0;
            }

            DataDictionary["SelectingSlotIndex"] = _selectingSlotIndex;
        }
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class HorizontalFortuneUIManager : StandardizedUIManager
{
    [System.Serializable]
    public class RewardPrefab
    {
        public string ID;
        public Color Color;
    }    

    [Tooltip("Recognize by ID")]
    public List<RewardPrefab> RewardPrefabs;
    public GameObject FortuneSlotPrefab;

    [SerializeField] private Transform InstantiateTransform;

    private List<string> _instantiateRules;

    private const float SELECTING_SLOT_FILTER = 0f;
    private const float UNSELECTING_SLOT_FILTER = 100f / 255f;

    protected override void Initialize()
    {
        _instantiateRules = (List<string>)_standardizedBehaviourManager.GetData("InstantiateRules");

        CreateNewFortune();
    }

    private void Update()
    {
        UpdateSlotFilter();
    }

    public void CreateNewFortune()
    {
        for (int i = 0; i < _instantiateRules.Count; i++) 
        {
            GameObject reward = Instantiate<GameObject>(FortuneSlotPrefab, InstantiateTransform);
            ((HorizontalFortuneBehaviourManager)_standardizedBehaviourManager).FortuneSlots.Add(reward);

            reward.name = _instantiateRules[i];
            reward.GetComponentInChildren<TMP_Text>().text = reward.name ;
            reward.GetComponent<Image>().color = EditCube(_instantiateRules[i]);
        }    
    }

    #region Button behaviours
    public void OnStopFortuneWheel()
    {
        _standardizedBehaviourManager.SetData("CanSelectNewSlot", false);
    }    
    #endregion

    private Color EditCube(string id)
    {
        return RewardPrefabs
            .Find(x => x.ID == id).Color;
    }    

    private void UpdateSlotFilter()
    {
        List<GameObject> slots = ((HorizontalFortuneBehaviourManager)_standardizedBehaviourManager).FortuneSlots;
        foreach(var slot in slots)
        {
            slot.transform.Find("Filter").GetComponent<Image>().DOFade(UNSELECTING_SLOT_FILTER, 0f);
        }
        slots[(int)_standardizedBehaviourManager.GetData("SelectingSlotIndex")].transform.Find("Filter").GetComponent<Image>().DOFade(SELECTING_SLOT_FILTER, 0f);
    }
}

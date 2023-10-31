using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

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

    protected override void Initialize()
    {
        _instantiateRules = (List<string>)_standardizedBehaviourManager.GetData("InstantiateRules");

        CreateNewFortune();
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

    private Color EditCube(string id)
    {
        return RewardPrefabs
            .Find(x => x.ID == id).Color;
    }    
}

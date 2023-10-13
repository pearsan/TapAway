using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Layers")]
    [SerializeField] private GameObject MainLayer;
    [SerializeField] private GameObject ShopLayer;
    [SerializeField] private GachaEffect GachaLayer;

    [Header("Gold feedbacks")]
    [SerializeField] private GameObject GoldFeedbackPrefab;
    [SerializeField] private Transform GoldParentTransform;
    [SerializeField] private RectTransform StartPosition;
    [SerializeField] private RectTransform EndPosition;

    [Header("Level feedbacks")]
    [SerializeField] private List<TMP_Text> LevelText;

    [Header("Texts")]
    [SerializeField] private TMP_Text MoveAttemptText;
    private const float TIME_TO_FEEDBACK = 0.5f;

    [Header("Panels")]
    [SerializeField] private GameObject WinPanel;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        MainLayer.GetComponent<CanvasGroup>().alpha = 1;
        ShopLayer.GetComponent<CanvasGroup>().alpha = 0;
        GachaLayer.GetComponent<CanvasGroup>().alpha = 0;

        ShopLayer.transform.DOLocalMoveX(1080, 0f);
    }

    private void Update()
    {
        UpdateTextLevel();
    }

    #region Button behaviours
    public void OnEnterShopAnimation()
    {
        SetCanvasGroupValue(0, 1, 0);

        ShopLayer.transform.DOLocalMoveX(1080, 0f);
        ShopLayer.transform.DOLocalMoveX(0, 0.1f);
    }    

    public void OnExitShopAnimation()
    {
        SetCanvasGroupValue(1, 0, 0);

        MainLayer.GetComponent<CanvasGroup>().alpha = 1;
        ShopLayer.transform.DOLocalMoveX(1080, 0.1f).OnComplete(() => { ShopLayer.GetComponent<CanvasGroup>().alpha = 0f; });
    }

    public void OnExitGachaAnimation()
    {
        SetCanvasGroupValue(0,1,0);
        GachaLayer.OnExitGachaAnimation();
    }

    public void OnNextLevelWithAdsButton()
    {
        ISHandler.Instance.ShowRewardedVideo("x4 reward button after pass level", 
            () => { OnAddGoldFeedbackAnimation(800); OnTriggerExitWinPanel(); }
            , () => { });
    }    

    public void OnNextLevelWithoutAdsButton()
    {
        OnAddGoldFeedbackAnimation(400); 
        OnTriggerExitWinPanel();
    }    
    #endregion

    #region Script behaviours
    private void SetCanvasGroupValue(float MainLayerValue, float ShopLayerValue, float GachaLayerValue)
    {
        MainLayer.GetComponent<CanvasGroup>().alpha = MainLayerValue;
        ShopLayer.GetComponent<CanvasGroup>().alpha = ShopLayerValue;
        GachaLayer.GetComponent<CanvasGroup>().alpha = GachaLayerValue;

        MainLayer.GetComponent<CanvasGroup>().blocksRaycasts = (MainLayerValue == 1) ? true : false;
        ShopLayer.GetComponent<CanvasGroup>().blocksRaycasts = (ShopLayerValue == 1) ? true : false;
        GachaLayer.GetComponent<CanvasGroup>().blocksRaycasts = (GachaLayerValue == 1) ? true : false;
    }

    private IEnumerator WaitForNextLevel(float timer)
    {
        yield return new WaitForSeconds(timer);
        GameplayManager.Instance.OnTriggerNextStage();
    }    
    #endregion

    #region UI Behaviours
    private void UpdateTextLevel()
    {
        foreach(var text in LevelText)
        {
            text.text = "Level " + (GameplayManager.Instance.GetCurrentStage() + 1);
        }    
    }

    public void OnTriggerEnterWinPanel()
    {
        Debug.Log("Really?");
        WinPanel.SetActive(true);
    }

    private void OnTriggerExitWinPanel()
    {
        Debug.Log("Run");
        WinPanel.SetActive(false);
    }    
    #endregion

    #region Feedback Effect Behaviours
    public void OnAddGoldFeedbackAnimation(int goldAddValue)
    {
        for (int i = 1; i <= 4; i++)
        {
            GameObject gold = Instantiate<GameObject>(GoldFeedbackPrefab, GoldParentTransform);
            gold.GetComponent<RectTransform>().anchoredPosition = new Vector3(150, -540);
            //gold.GetComponent<SpriteTrail>().Direction = (new Vector3(150, -540) - (Vector3) EndPosition.anchoredPosition).normalized;
            Vector2 offset = new Vector2 (Random.Range(-200, 200), Random.Range(-200,200));
            gold.GetComponent<RectTransform>().DOAnchorPos( gold.GetComponent<RectTransform>().anchoredPosition + offset, 0.5f).SetEase(Ease.InOutSine)
                .OnComplete(()=> {
                    StartCoroutine(gold.GetComponent<SpriteTrail>().SpawnSpriteTrail());
                    gold.GetComponent<RectTransform>().DOAnchorPos(EndPosition.anchoredPosition, 0.5f)
                .OnComplete(() => { StartCoroutine(AddGoldAnimation(goldAddValue)); Destroy(gold); }) ; 
                });
        }
    }
    
    private IEnumerator AddGoldAnimation(int goldAddValue)
    {
        for(int i = 1; i <= goldAddValue / 4;i++)
        {
            GoldManager.Instance.ModifyGoldValue(1);
            yield return new WaitForSeconds(0.01f * 200 / goldAddValue);
        }    
    }

    public void OnGachaFeedbackAnimation(ShopItemSO target)
    {
        SetCanvasGroupValue(0, 0, 1);
        StartCoroutine(GachaLayer.OnTriggerGachaAnimation(target));
    }    
    #endregion
}

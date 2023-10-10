using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    private const float TIME_TO_FEEDBACK = 0.5f;

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

    #region Button behaviours
    public void OnEnterShopAnimation()
    {
        SetCanvasGroupValue(0, 1, 0);

        ShopLayer.transform.DOLocalMoveX(1080, 0f);
        ShopLayer.transform.DOLocalMoveX(0, 0.1f);
    }    

    public void OnExitShopAnimation()
    {
        SetCanvasGroupValue(0, 1, 0);

        MainLayer.GetComponent<CanvasGroup>().alpha = 1;
        ShopLayer.transform.DOLocalMoveX(1080, 0.1f).OnComplete(() => { ShopLayer.GetComponent<CanvasGroup>().alpha = 0f; });
    }

    public void OnExitGachaAnimation()
    {
        SetCanvasGroupValue(0,1,0);
        GachaLayer.OnExitGachaAnimation();
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
    #endregion

    #region Feedback Effect Behaviours
    public void OnAddGoldFeedbackAnimation()
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
                .OnComplete(() => { StartCoroutine(AddGoldAnimation()); Destroy(gold); }) ; 
                });
        }
    }
    
    private IEnumerator AddGoldAnimation()
    {
        for(int i = 1; i <= GoldManager.Instance.GOLD_EARN_EACH_ADS / 4;i++)
        {
            GoldManager.Instance.ModifyGoldValue(1);
            yield return new WaitForSeconds(0.01f);
        }    
    }    

    public void OnGachaFeedbackAnimation(ShopItemSO target)
    {
        SetCanvasGroupValue(0, 0, 1);
        GachaLayer.OnGachaEffect(target);
    }    
    #endregion
}

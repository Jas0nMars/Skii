using System;
using System.Collections;
using System.Numerics;
using DG.Tweening;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UIMainPanel : MonoBehaviour
    {
        public Button settingBtn;
        public Toggle musicToggle;
        public Toggle tipToggle;
        public Button startMaskBtn;
        public Button rebornBtn;
        public Text curLvTxt;
        public Text nextLvTxt;
        public Text curTipTxt;
        public Text maxTipTxt;
        public RectTransform curTipTrans;
        public RectTransform curProgressTrans;
        public RectTransform toggleGroupTrans;
        public GameObject introObj;
        public Image rebornRoot;
        public Text rebornTimeTxt;
        public Slider curProgressSilder;
        public Slider historyProgressSlider;

        public TextMeshProUGUI tipsTxt;
        public Text curScoreTxt;
        public Text maxScoreTxt;
        public TextMeshProUGUI finishTipsTxt;
        public Image[] themeImgArr;

        private int settingToggleGroupOpenPosY = 0;
        private int settingToggleGroupClosePosY = 120;
        private bool isSettingBtnOpen = false;
        private int curLeftTime = 3;
        private Coroutine countDownHandler;
        private float originH = 0f;

        private UI_FadeInOut rebornRootSc;



        private void Awake()
        {
            rebornRootSc = rebornRoot.gameObject.GetComponent<UI_FadeInOut>();
            settingBtn.onClick.AddListener(OnClickSettingBtn);
            musicToggle.onValueChanged.AddListener(OnClickMusicBtn);
            tipToggle.onValueChanged.AddListener(OnClickTipBtn);
            // startMaskBtn.onClick.AddListener(OnClickStartBtn);
            rebornBtn.onClick.AddListener(OnClickRebornBtn);
            EventTriggerListener.Get(startMaskBtn.gameObject).OnBtnDown = OnClickMaskBtnDown;
            EventTriggerListener.Get(startMaskBtn.gameObject).OnBtnUp = OnClickMaskBtnUp;

            EventManager.Instance.RegisterEvent(EventKey.PlayerDie, PlayerDie);
            EventManager.Instance.RegisterEvent(EventKey.PlayerReborn, PlayerReborn);
            EventManager.Instance.RegisterEvent(EventKey.UpdateProgress, UpdateProgress);
            EventManager.Instance.RegisterEvent(EventKey.PlayerDie, PlayerDie);
            EventManager.Instance.RegisterEvent(EventKey.ShowTips, ShowTips);
            EventManager.Instance.RegisterEvent(EventKey.UpdateScore, UpdateScore);
            EventManager.Instance.RegisterEvent(EventKey.LevelFinish, ShowFinishUI);
            EventManager.Instance.RegisterEvent(EventKey.ResetLevel, ResetLevel);
        }

        private void Start()
        {
            SetLevelInfo();
        }
        private string[] tips = { "干得好！", "厉害！", "漂亮！", "帅气！" };
        private int index = 0;
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.A))
        //     {
        //         Debug.Log("ShowTips: " + index);
        //         ShowTips(tips[index++ % 3]);
        //     }
        // }


        private Sequence tipsSequence;
        private void ShowTips(object arg0)
        {
            if (tipsSequence != null) tipsSequence.Kill();
            tipsSequence = DOTween.Sequence();
            tipsTxt.text = (string)arg0;
            Color c = tipsTxt.color;
            c.a = 1;
            tipsTxt.color = c;
            // tipsTxt.DOFade(1, 0f);
            tipsTxt.transform.localScale = UnityEngine.Vector3.zero;
            // 添加放大到1.1的动画
            tipsSequence.Append(tipsTxt.transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack));

            // 添加缩小回1的动画
            tipsSequence.Append(tipsTxt.transform.DOScale(1f, 0.1f).SetEase(Ease.OutQuad));

            tipsSequence.Append(tipsTxt.DOFade(0, 0.5f).SetDelay(1));
        }

        private void SetLevelInfo()
        {
            SetThemeColor();
            if (PlayerPrefs.GetInt("NeedPlayMusic", 1) == 1)
            {
                AudioManager.instance.PlayMusic();
                musicToggle.isOn = true;
            }
            if (PlayerPrefs.GetInt("NeedShowTips", 1) == 1)
            {
                // AudioManager.instance.PlayMusic();
                tipToggle.isOn = true;
                tipsTxt.gameObject.SetActive(true);
            }
            else
            {
                tipsTxt.gameObject.SetActive(false);
            }
            curLvTxt.text = GameManager.Instance.CurLevel.ToString();
            nextLvTxt.text = (GameManager.Instance.CurLevel + 1).ToString();
            maxScoreTxt.text = GameManager.Instance.MaxScore.ToString();
            curScoreTxt.text = "0";
            curTipTxt.text = "0%";
            maxTipTxt.text = GameManager.Instance.HistoryProgress + "%";
            curProgressSilder.value = 0;
            finishTipsTxt.gameObject.SetActive(false);
            if (GameManager.Instance.HistoryProgress != 0)
            {
                historyProgressSlider.gameObject.SetActive(true);
                historyProgressSlider.value = GameManager.Instance.HistoryProgress / 100f;
            }
            else
            {
                historyProgressSlider.gameObject.SetActive(false);
            }
            ShowAppearAnim();

        }

        private void SetThemeColor()
        {
            Color color = GameManager.Instance.CurThemeColor;
            foreach (Image img in themeImgArr)
            {
                img.color = color;
            }
            nextLvTxt.color = color;
        }

        private void ShowAppearAnim(bool ShowIntroRoot = false)
        {
            // settingBtn.transform.localScale = Vector3.zero;
            // 创建动画序列
            Sequence mySequence = DOTween.Sequence();

            // 添加放大到1.1的动画
            mySequence.Append(settingBtn.transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack));

            // 添加缩小回1的动画
            mySequence.Append(settingBtn.transform.DOScale(1f, 0.1f).SetEase(Ease.OutQuad)).onComplete = () =>
            {
                if (ShowIntroRoot)
                {
                    introObj.GetComponent<UI_FadeInOut>().UI_FadeIn_Event();
                }
            };
            //记录
            maxScoreTxt.GetComponent<CanvasGroup>().alpha = 1;
            var color = curScoreTxt.color;
            color.a = 0;
            curScoreTxt.color = color;
        }

        private void ShowDisappearAnim()
        {
            // 创建动画序列
            Sequence mySequence = DOTween.Sequence();

            // 添加放大到1.1的动画
            mySequence.Append(settingBtn.transform.DOScale(1.1f, 0.1f).SetEase(Ease.OutBack));

            // 添加缩小回1的动画
            mySequence.Append(settingBtn.transform.DOScale(0f, 0.2f).SetEase(Ease.OutQuad));

            introObj.GetComponent<UI_FadeInOut>().UI_FadeOut_Event();

            //记录切换
            maxScoreTxt.GetComponent<UI_FadeInOut>().UI_FadeOut_Event();
            curScoreTxt.DOFade(1, 0.5f);
        }




        private void PlayerDie(object arg0)
        {
            StartCoroutine(ResetGameAnim(false));
        }

        private void ResetLevel(object arg0)
        {
            StartCoroutine(ResetGameAnim(true));
        }

        private void PlayerReborn(object arg0)
        {
            //弹出复活界面
            curLeftTime = 3;
            rebornTimeTxt.text = curLeftTime + "";
            rebornRoot.gameObject.SetActive(true);
            // Color c = rebornRoot.color;
            // c.a = 0;
            // rebornRoot.color = c;
            // rebornRoot.DOFade(225/255f , 0.5f);
            rebornRootSc.UI_FadeIn_Event();
            countDownHandler = StartCoroutine(CountDown());
        }

        private IEnumerator CountDown()
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(1);
                curLeftTime--;
                rebornTimeTxt.text = curLeftTime + "";
            }
            StartCoroutine(ResetGameAnim(false));
        }

        private IEnumerator ResetGameAnim(bool isFinish)
        {
            Image img = startMaskBtn.GetComponent<Image>();
            //渐隐，然后整个屏幕渐隐并向上
            yield return new WaitForSeconds(0.7f);
            var cameraTrans = Camera.main.transform;
            cameraTrans.DOMoveY(cameraTrans.position.y + 0.5f, 0.5f);
            if (rebornRootSc.gameObject.activeSelf) rebornRootSc.UI_FadeOut_Event();
            DOTween.Kill(img);
            img.DOFade(1, 0.5f).onComplete = () =>
            {
                GameManager.Instance.ResetGame(isFinish);
            };
            //重置Player enemy，game状态
            yield return new WaitForSeconds(0.6f);
            ShowAppearAnim(true);
            img.DOFade(0, 0.5f).onComplete = ()=>{
                SetLevelInfo();
            };
        }


        public void OnClickSettingBtn()
        {
            if (isSettingBtnOpen)
            {
                toggleGroupTrans.DOLocalMoveY(settingToggleGroupClosePosY, 0.2f);
            }
            else
            {
                toggleGroupTrans.DOLocalMoveY(settingToggleGroupOpenPosY, 0.4f).SetEase(Ease.OutCubic);
            }

            isSettingBtnOpen = !isSettingBtnOpen;
        }

        public void OnClickMaskBtnUp(GameObject go)
        {
            if (GameManager.Instance.CurState == GameManager.GameState.BeforeStart)
            {
                GameManager.Instance.CurState = GameManager.GameState.Running;
                // introObj.gameObject.SetActive(false);
                ShowDisappearAnim();
            }
            else if (GameManager.Instance.CurState == GameManager.GameState.Running)
            {
                GameManager.Instance.player.StopAccelerate();
            }
        }

        public void OnClickMaskBtnDown(GameObject go)
        {
            if (GameManager.Instance.CurState == GameManager.GameState.Running)
            {
                GameManager.Instance.player.StartAccelerate();
                GameManager.Instance.player.ChangeDir();
            }
        }

        public void OnClickStartBtn()
        {

        }

        public void OnClickMusicBtn(bool state)
        {
            if (state)
            {
                AudioManager.instance.Mute(false);
                AudioManager.instance.PlayMusic();
                PlayerPrefs.SetInt("NeedPlayMusic", 1);
            }
            else
            {
                AudioManager.instance.Mute(true);
                PlayerPrefs.SetInt("NeedPlayMusic", 0);
            }
        }

        public void OnClickTipBtn(bool state)
        {
            if (state)
            {
                PlayerPrefs.SetInt("NeedShowTips", 1);
                GameManager.Instance.NeedShowTips = true;
            }
            else
            {
                PlayerPrefs.SetInt("NeedShowTips", 0);
                GameManager.Instance.NeedShowTips = false;
            }
        }

        public void OnClickRebornBtn()
        {
            if (countDownHandler != null) StopCoroutine(countDownHandler);
            rebornRoot.gameObject.SetActive(false);
            GameManager.Instance.GameReborn();
        }

        private void UpdateProgress(object arg0)
        {
            int curProgress = GameManager.Instance.CurProgress > 100 ? 100 : GameManager.Instance.CurProgress;
            curTipTxt.text = curProgress + "%";
            curProgressSilder.value = curProgress / 100f;
            curScoreTxt.text = GameManager.Instance.CurScore.ToString();
        }

        private void UpdateScore(object arg0)
        {
            curScoreTxt.text = GameManager.Instance.CurScore.ToString();

        }

        private void ShowFinishUI(object arg0)
        {
            finishTipsTxt.gameObject.SetActive(true);
            finishTipsTxt.text = string.Format("关卡{0}完成！", GameManager.Instance.CurLevel);
        }

    }
}
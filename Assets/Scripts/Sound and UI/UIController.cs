using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public float time = 0.5f;
    [SerializeField] private Transform MusicButton;
    [SerializeField] private Transform SFXButton;
    [SerializeField] private Transform ResetButton;
    [SerializeField] private Transform SettingsButton;
    [SerializeField] private float ySettingsMoving;
    [Space]
    [SerializeField] private Image Logo;
    [SerializeField] private Text WinLoseText;
    [SerializeField] private Text TapToContinuteText;
    [SerializeField] private Image Panel;
    [SerializeField] private Image BlackFade;
    [SerializeField] private Image WhiteFade;

    private Image SettingsImage;
    private Image MusicImage;
    private Image SFXImage;
    private Image ResetImage;
    private bool isMoving;
    private bool stateOfSettings;
    private Camera camera;
    private float yOffset;

    private void Start()
    {
        instance = this;

        SettingsImage = SettingsButton.GetComponent<Image>();
        MusicImage = MusicButton.GetComponent<Image>();
        ResetImage = ResetButton.GetComponent<Image>();
        SFXImage = SFXButton.GetComponent<Image>();
        
        camera = Camera.main;
        yOffset = (float)Screen.height / 1280f;

        StartCoroutine(FadeBackgroundOut());
    }

    public IEnumerator ChangeResetActivity(int state)
    {
        bool boolState = state == 1 ? true : false;
        ResetButton.GetComponent<Button>().enabled = boolState;

        if (boolState)
            ResetButton.gameObject.SetActive(boolState);
        yield return ResetImage.DOFade(state, time).WaitForCompletion();
        if (!boolState)
            ResetButton.gameObject.SetActive(boolState);
    }

    public void MoveSettingsButtons()
    {
        if (!isMoving)
            StartCoroutine(MoveButtonsRoutine());
    }

    private IEnumerator MoveButtonsRoutine()
    {
        isMoving = true;

        if (!stateOfSettings)
        {
            MusicButton.gameObject.SetActive(true);
            SFXButton.gameObject.SetActive(true);
            //ResetButton.gameObject.SetActive(true);

            MusicImage.DOFade(1, time);
            SFXImage.DOFade(1, time);
            //ResetImage.DOFade(1, time);
            
            SFXButton.DOMoveY(SettingsButton.position.y - yOffset * ySettingsMoving, time);
            //ResetButton.DOMoveY(SettingsButton.position.y - (ySettingsMoving * yOffset) * 3, time);
            yield return MusicButton.DOMoveY(SettingsButton.position.y - (ySettingsMoving * yOffset) * 2, time).WaitForCompletion();
        }
        else
        {
            MusicImage.DOFade(0, time);
            SFXImage.DOFade(0, time);
            //ResetImage.DOFade(0, time);

            //ResetButton.DOMoveY(SettingsButton.position.y, time);
            SFXButton.DOMoveY(SettingsButton.position.y, time);
            yield return MusicButton.DOMoveY(SettingsButton.position.y, time).WaitForCompletion();
            
            MusicButton.gameObject.SetActive(false);
            SFXButton.gameObject.SetActive(false);
            //ResetButton.gameObject.SetActive(false);
        }
        
        stateOfSettings = !stateOfSettings;
        isMoving = false;
    }

    public IEnumerator EndLevel(string text)
    {
        SettingsImage.DOFade(0, time);
        if (stateOfSettings)
            StartCoroutine(MoveButtonsRoutine());
        
        BlackFade.gameObject.SetActive(true);
        WinLoseText.gameObject.SetActive(true);
        TapToContinuteText.gameObject.SetActive(true);
        //Panel.gameObject.SetActive(true);
        WhiteFade.gameObject.SetActive(true);
        WinLoseText.text = text;
        
        
        WinLoseText.DOFade(1, time);
        BlackFade.DOFade(0.45f, time);
        yield return TapToContinuteText.DOFade(1, time).WaitForCompletion();
        //Panel.DOFade(0.1f, time);
        //yield return Logo.DOFade(1, time * 2).WaitForCompletion();
        
        SettingsButton.gameObject.SetActive(false);
    }

    public IEnumerator Reset(bool reset = false)
    {
        yield return WhiteFade.DOFade(1, time).WaitForCompletion();
        yield return new WaitForSeconds(1f);

        if (!reset)
        {
            GameManager.instance.ResetGame();

            BlackFade.color = new Color(0, 0, 0, 0);
            //Logo.color = new Color(1, 1, 1, 0);
            //Panel.color = new Color(Panel.color.r, Panel.color.g, Panel.color.b, 0);
            WinLoseText.color = new Color(1, 1, 1, 0);
            TapToContinuteText.color = new Color(1, 1, 1, 0);

            BlackFade.gameObject.SetActive(false);
            WinLoseText.gameObject.SetActive(false);
            TapToContinuteText.gameObject.SetActive(false);
            //Panel.gameObject.SetActive(false);
            
            SettingsButton.gameObject.SetActive(true);
            SettingsImage.color = Color.white;

            StartCoroutine(FadeBackgroundOut());
        }
        else
            GameManager.instance.LoadNextLevel();
    }

    public IEnumerator FadeBackgroundOut()
    {
        yield return WhiteFade.DOFade(0, time).WaitForCompletion();
        WhiteFade.gameObject.SetActive(false);
    }

    public void KillAllTweens()
    {
        DOTween.KillAll();
    }
}
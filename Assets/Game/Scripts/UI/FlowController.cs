using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FlowController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup introPanel;
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private CanvasGroup gamePanel;
    [SerializeField] private CanvasGroup endPanel;

    [SerializeField] private Image fadePanel;

    [Header("Intro")]
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Main Narrative")]
    [SerializeField] private TextMeshProUGUI[] narrativeTexts;
    [SerializeField] private string[] narrativeLines;

    [Header("Main Buttons")]
    [SerializeField] private Button startYesButton;
    [SerializeField] private Button startNoButton;
    [SerializeField] private CanvasGroup startButtonGroup;

    [Header("End Buttons")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        var color = fadePanel.color;
        color.a = 1f;
        fadePanel.color = color;
    }

    private void Start()
    {
        SetupInitialState();
        BindButtons();

        if (GameController.Instance != null)
            GameController.Instance.OnGameEnded += ShowEndPanel;

        StartCoroutine(BootFlow());
    }

    private void SetupInitialState()
    {
        introPanel.gameObject.SetActive(true);
        mainPanel.gameObject.SetActive(false);
        endPanel.gameObject.SetActive(false);

        startButtonGroup.alpha = 0;
    }

    private void BindButtons()
    {
        startYesButton.onClick.AddListener(StartGameFlow);
        startNoButton.onClick.AddListener(QuitGame);

        retryButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private IEnumerator BootFlow()
    {
        yield return IntroSequence();
        yield return MainSequence();
    }

    // INTRO = title only
    private IEnumerator IntroSequence()
    {
        // boot fade in
        yield return fadePanel.DOFade(0f, 1.5f).WaitForCompletion();

        string title = titleText.text;
        titleText.text = "";
        titleText.alpha = 1;

        yield return new WaitForSeconds(2f);

        yield return titleText.DOFade(0f, 1.5f).WaitForCompletion();

        introPanel.gameObject.SetActive(false);

        // fade to black before switching panel
        yield return fadePanel.DOFade(1f, 1.5f).WaitForCompletion();

        introPanel.gameObject.SetActive(false);
    }

    // MAIN = 3 narrative lines
    private IEnumerator MainSequence()
    {
        mainPanel.gameObject.SetActive(true);

        // reveal main from black
        yield return fadePanel.DOFade(0f, 1.5f).WaitForCompletion();

        for (int i = 0; i < narrativeTexts.Length; i++)
        {
            var text = narrativeTexts[i];
            string line = narrativeLines[i];

            Vector3 originalPos = text.rectTransform.localPosition;
            text.rectTransform.localPosition = originalPos + Vector3.up * 50f;

            Sequence seq = DOTween.Sequence();

            seq.Append(text.DOFade(1f, 0.7f));

            seq.Join(
                text.rectTransform.DOLocalMoveY(originalPos.y, 0.7f)
            );

            text.PlayTypewriterWithMark(line, 30f);
            yield return new WaitUntil(() => text.maxVisibleCharacters >= line.Length);

            yield return seq.WaitForCompletion();
            yield return new WaitForSeconds(0.5f);
        }

        yield return startButtonGroup.DOFade(1f, 1.5f).WaitForCompletion();

        startButtonGroup.interactable = true;
        startButtonGroup.blocksRaycasts = true;
    }

    private void StartGameFlow()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(fadePanel.DOFade(1f, 1.5f));

        seq.AppendCallback(() =>
        {
            mainPanel.gameObject.SetActive(false);

            gamePanel.gameObject.SetActive(true);
            gamePanel.alpha = 1;

            GameController.Instance.BeginGame();
        });

        seq.Append(fadePanel.DOFade(0f, 1.5f));
    }

    private void ShowEndPanel()
    {
        endPanel.gameObject.SetActive(true);
        endPanel.alpha = 0;

        endPanel.DOFade(1f, 1.5f);
    }

    private void RestartGame()
    {
        endPanel.gameObject.SetActive(false);

        GameController.Instance.OnNewGame();
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (GameController.Instance != null)
            GameController.Instance.OnGameEnded -= ShowEndPanel;
    }
}
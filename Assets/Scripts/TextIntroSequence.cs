using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextIntroSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text introText;
    [SerializeField] private Image blackPanel; // optional, can be null

    [Header("Intro Settings")]
    [TextArea(3, 10)]
    [SerializeField]
    private string paragraph =
        "They told you the mask was something you wore, but it was always something you became...";

    [SerializeField] private float fadeInTime = 1.2f;
    [SerializeField] private float holdTime = 5.5f;
    [SerializeField] private float fadeOutTime = 1.2f;

    [Header("Skip / Next")]
    [SerializeField] private string nextSceneName = "MainMenu";
    [SerializeField] private bool allowSkip = true;

    private Coroutine sequence;

    void Awake()
    {
        if (introText == null) introText = GetComponentInChildren<TMP_Text>();
        SetAlpha(introText, 0f);

        if (blackPanel != null)
            SetAlpha(blackPanel, 1f); // start fully black background
    }

    void Start()
    {
        sequence = StartCoroutine(Play());
    }

    void Update()
    {
        if (!allowSkip) return;

        // Space / Enter / mouse click / any key
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            Skip();
        }
    }

    IEnumerator Play()
    {
        introText.text = paragraph;

        // Fade text in (black background stays)
        yield return FadeText(0f, 1f, fadeInTime);

        // Hold
        yield return new WaitForSeconds(holdTime);

        // Fade text out
        yield return FadeText(1f, 0f, fadeOutTime);

        // Load next scene
        LoadNext();
    }

    void Skip()
    {
        if (sequence != null) StopCoroutine(sequence);
        LoadNext();
    }

    void LoadNext()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogWarning("Next Scene Name is empty. Set it in the inspector.");
    }

    IEnumerator FadeText(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            SetAlpha(introText, to);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            SetAlpha(introText, a);
            yield return null;
        }
        SetAlpha(introText, to);
    }

    static void SetAlpha(TMP_Text txt, float a)
    {
        if (txt == null) return;
        Color c = txt.color;
        c.a = a;
        txt.color = c;
    }

    static void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = a;
        img.color = c;
    }
}

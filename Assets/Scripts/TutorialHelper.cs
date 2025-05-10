using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialHelper : MonoBehaviour
{
    private PlayerInputAction playerInput;
    private InputAction move;
    private InputAction punch;
    public List<string> tutorialTexts;
    public TextMeshProUGUI textComponent;
    private int currentTextIndex = 0;
    private int currTutor = 0;
    private bool isTextChanging = false;

    void Awake()
    {
        playerInput = new PlayerInputAction();
        move = playerInput.Player.Move;
        punch = playerInput.Player.Punch;
    }

    void Start()
    {
        updateText();
    }

    private void OnEnable()
    {
        move.Enable();
        punch.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        punch.Disable();
    }

    private void LateUpdate()
    {
        if (isTextChanging) return;
        if (move.WasPerformedThisFrame() && currTutor == 0)
        {
            // Jika tombol gerak ditekan, lanjut ke teks berikutnya
            nextTutor();
        } else if(punch.WasPerformedThisFrame() && currTutor == 1)
        {
            //Jika tombol punch ditekan, lanjut ke teks berikutnya
            nextTutor();
        }
    }

    void updateText()
    {
        textComponent.text = tutorialTexts[currentTextIndex];
    }

    public void onPlayerEnter()
    {
        if(GameObject.FindGameObjectsWithTag("Creeps").Length == 0)
        {
            // Jika tidak ada musuh, lanjut ke teks berikutnya
            nextTutor();
            StartCoroutine(toStage1(1.5f));
        }
    }

    public void nextTutor()
    {
        isTextChanging = true;
        StartCoroutine(nextText(0.5f));
    }

    IEnumerator nextText(float duration)
    {
        yield return new WaitForSeconds(duration);

        currentTextIndex = (currentTextIndex + 1) % tutorialTexts.Count;
        currTutor++;
        updateText();
        isTextChanging = false;
    }

    IEnumerator toStage1(float duration)
    {
        yield return new WaitForSeconds(duration);

        GameManager.Instance.AddScore(100);
        GameManager.Instance.LoadStage(1);
    }
}

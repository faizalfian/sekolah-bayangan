using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialHelper : MonoBehaviour
{
    private PlayerInputAction playerInput;
    private InputAction move;
    private InputAction punch;
    private InputAction dash;
    private InputAction push;
    public List<string> tutorialTexts;
    public TextMeshProUGUI textComponent;
    private int currentTextIndex = 0;
    private int currTutor = 0;
    private bool isTextChanging = false;
    private bool tutorEnd = false;

    void Awake()
    {
        playerInput = new PlayerInputAction();
        move = playerInput.Player.Move;
        punch = playerInput.Player.Punch;
        dash = playerInput.Player.Dash;
        push = playerInput.Player.Push;
    }

    void Start()
    {
        updateText();
        
    }

    private void OnEnable()
    {
        move.Enable();
        punch.Enable();
        dash.Enable();
        push.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        punch.Disable();
        dash.Disable();
        push.Disable();
    }

    private void LateUpdate()
    {
        if (isTextChanging) return;
        if (move.WasPerformedThisFrame() && currTutor == 0)
        {
            // Jika tombol gerak ditekan, lanjut ke teks berikutnya
            nextTutor();
        } else if (punch.WasPerformedThisFrame() && currTutor == 1)
        {
            //Jika tombol punch ditekan, lanjut ke teks berikutnya
            nextTutor();
        } else if(dash.WasPerformedThisFrame() && currTutor == 2)
        {
            //Jika tombol dash ditekan, lanjut ke teks berikutnya
            nextTutor();
        }
        else if (push.WasPerformedThisFrame() && currTutor == 3)
        {
            //Jika tombol push ditekan, lanjut ke teks berikutnya
            nextTutor();
        } else if (currTutor == 4)
        {
            StartCoroutine(nextTextWIthDelay(2f));
        }
    }

    void updateText()
    {
        textComponent.text = tutorialTexts[currentTextIndex];
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

    public void OnEnemyDead(int _)
    {
        Invoke(nameof(CheckAfterDelay), 1.6f);
    }

    void CheckAfterDelay()
    {
        if (tutorEnd) return;
        if (GameObject.FindGameObjectsWithTag("Creeps").Length == 0)
        {
            nextTutor();
            Invoke(nameof(toStage1), 1.5f);
            tutorEnd = true;
        }
    }

    void toStage1()
    {
        GameManager.Instance.AddScore(100);
        GameManager.Instance.LoadStage(1);
    }

    IEnumerator nextTextWIthDelay(float duration)
    {
        isTextChanging = true;
        yield return new WaitForSeconds(duration);
        currentTextIndex = (currentTextIndex + 1) % tutorialTexts.Count;
        currTutor++;
        updateText();
        isTextChanging = false;
    }
}

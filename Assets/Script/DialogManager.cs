using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public GameObject dialogPanel;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogText;
    public CharacterImageAnimator characterImageAnimator;
    private TypingEffect typingEffect;

    // [NEW] UI untuk pilihan
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;

    // [NEW] Data keputusan pemain
    public List<string> playerDecisions = new List<string>();

    [System.Serializable]
    public class DialogLine
    {
        public string characterName;
        public string dialog;
        public Sprite characterSprite;
        public DialogChoice[] choices; // [NEW]
    }

    public DialogLine[] dialogLines;
    private int currentLine = 0;
    private bool waitingForChoice = false;

    void Start()
    {
        dialogPanel.SetActive(false);
        typingEffect = dialogText.GetComponent<TypingEffect>();
        choicePanel.SetActive(false); // [NEW]
    }

    public void StartDialog()
    {
        if (dialogLines.Length > 0)
        {
            dialogPanel.SetActive(true);
            currentLine = 0;
            ShowNextLine();
        }
        else
        {
            Debug.LogWarning("Tidak ada data dialog!");
        }
    }

    public void ShowNextLine()
    {
        if (currentLine >= dialogLines.Length)
        {
            EndDialog();
            return;
        }

        DialogLine line = dialogLines[currentLine];
        characterNameText.text = line.characterName;
        typingEffect.StartTyping(line.dialog);
        characterImageAnimator.FadeIn(line.characterSprite);

        if (line.choices != null && line.choices.Length > 0)
        {
            waitingForChoice = true;
            ShowChoices(line.choices);
        }
        else
        {
            currentLine++; // lanjut ke dialog berikutnya kalau tidak ada pilihan
        }
    }

    void EndDialog()
    {
        dialogPanel.SetActive(false);
    }

    void Update()
    {
        if (dialogPanel.activeSelf && !waitingForChoice && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }

    void ShowChoices(DialogChoice[] choices)
    {
        choicePanel.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = choices[i].choiceText;
                int index = i; // penting untuk closure di lambda
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choices[index]));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnChoiceSelected(DialogChoice choice)
    {
        playerDecisions.Add(choice.consequenceTag);
        currentLine = choice.nextLineIndex;
        choicePanel.SetActive(false);
        waitingForChoice = false;
        ShowNextLine();
    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject dialogPanel;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogText;
    public CharacterImageAnimator characterImageAnimator; // opsional
    private TypingEffect typingEffect;

    [Header("Choice UI")]
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;

    [Header("Character Sprites")]
    public Image leftCharacterImage;   // MC (Bisma)
    public Image rightCharacterImage;  // NPC / Musuh

    [System.Serializable]
    public class DialogLine
    {
        public string characterName;
        public string dialog;
        public Sprite characterSprite;
        public DialogChoice[] choices;
    }

    public DialogLine[] dialogLines;
    private int currentLine = 0;
    private bool waitingForChoice = false;
    private string pendingChoiceText = null;
    private int nextLineAfterChoice = -1;

    public List<string> playerDecisions = new();

    void Start()
    {
        dialogPanel.SetActive(false);
        typingEffect = dialogText.GetComponent<TypingEffect>();
        choicePanel.SetActive(false);
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

    void Update()
    {
        if (dialogPanel.activeSelf && !waitingForChoice && Input.GetKeyDown(KeyCode.Space))
        {
            if (pendingChoiceText != null)
            {
                currentLine = nextLineAfterChoice;
                pendingChoiceText = null;
                nextLineAfterChoice = -1;
            }

            ShowNextLine();
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
        UpdateCharacterImages(line.characterName, line.characterSprite);

        if (line.choices != null && line.choices.Length > 0)
        {
            waitingForChoice = true;
            ShowChoices(line.choices);
        }
        else
        {
            currentLine++;
        }
    }

    void EndDialog()
    {
        dialogPanel.SetActive(false);
        leftCharacterImage.color = new Color(1, 1, 1, 0);
        rightCharacterImage.color = new Color(1, 1, 1, 0);
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
                int index = i;
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
        pendingChoiceText = choice.choiceText;
        nextLineAfterChoice = choice.nextLineIndex;
        choicePanel.SetActive(false);
        waitingForChoice = false;

        characterNameText.text = "Bisma";
        typingEffect.StartTyping(pendingChoiceText);
        UpdateCharacterImages("Bisma", leftCharacterImage.sprite); // tetap pakai sprite sebelumnya
    }

    void UpdateCharacterImages(string speakerName, Sprite sprite)
    {
        leftCharacterImage.color = new Color(1, 1, 1, 0);
        rightCharacterImage.color = new Color(1, 1, 1, 0);

        if (speakerName == "Bisma")
        {
            leftCharacterImage.sprite = sprite;
            leftCharacterImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            rightCharacterImage.sprite = sprite;
            rightCharacterImage.color = new Color(1, 1, 1, 1);
        }
    }
}

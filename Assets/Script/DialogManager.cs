using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DialogManager : MonoBehaviour
{
    public System.Action OnDialogEnd; // 🔹 Tambahan: Event setelah dialog selesai
    private int selectedChoiceIndex = 0;

    [Header("UI Components")]
    public GameObject dialogPanel;
    public GameObject bgPanel;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogText;
    public CharacterImageAnimator characterImageAnimator; // opsional
    private TypingEffect typingEffect;

    [Header("Choice UI")]
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;

    [Header("Character Sprites")]
    public Image leftCharacterImage;
    public Image rightCharacterImage;

    [System.Serializable]
    public class DialogLine
    {
        public string characterName;
        public string dialog;
        public Sprite characterSprite;
        public DialogChoice[] choices;
        public int nextLineIndex = -1; // ⬅️ Manual kontrol
    }

    public DialogLine[] dialogLines;
    private int currentLine = 0;
    private bool waitingForChoice = false;
    private string pendingChoiceText = null;
    private int nextLineAfterChoice = -1;
    private bool jumpToManualLine = false;

    public List<string> playerDecisions = new();

    void Start()
    {
        bgPanel.SetActive(false);
        dialogPanel.SetActive(false);
        choicePanel.SetActive(false);
        typingEffect = dialogText.GetComponent<TypingEffect>();
        //gameObject.SetActive(false);
    }

    public void StartDialog()
    {
        if (dialogLines.Length > 0)
        {
            dialogPanel.SetActive(true);
            bgPanel.SetActive(true);
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
        if (dialogPanel.activeSelf)
        {
            if (waitingForChoice)
            {
                // Navigasi pilihan dengan panah
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    UpdateChoiceSelection(-1);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    UpdateChoiceSelection(1);
                }
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    choiceButtons[selectedChoiceIndex].onClick.Invoke();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if (pendingChoiceText != null)
                {
                    currentLine = nextLineAfterChoice;
                    Debug.Log(currentLine + "pendingChoice");
                    pendingChoiceText = null;
                    nextLineAfterChoice = -1;
                    jumpToManualLine = true;
                }
                ShowNextLine();
            }
        }
    }

    public void ShowNextLine()
    {
        Debug.Log(currentLine);
        if (currentLine < 0 || currentLine >= dialogLines.Length)
        {
            Debug.Log(currentLine + "end");
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
            return; // Keluar setelah menampilkan choices
        }

        // Tentukan line berikutnya
        if (line.nextLineIndex == -1)
        {
            // Jika nextLineIndex = -1, akhiri dialog setelah line ini selesai
            currentLine = dialogLines.Length; // Akan memicu EndDialog() di pemanggilan berikutnya
        }
        else if (line.nextLineIndex >= 0)
        {
            // Jika ada nextLineIndex yang valid, lompat ke line tersebut
            currentLine = line.nextLineIndex;
        }
        else
        {
            // Default: lanjut ke line berikutnya secara sequential
            currentLine++;
        }


    }

    void EndDialog()
    {
        dialogPanel.SetActive(false);
        bgPanel.SetActive(false);
        leftCharacterImage.color = new Color(1, 1, 1, 0);
        rightCharacterImage.color = new Color(1, 1, 1, 0);
        OnDialogEnd?.Invoke(); // 🔹 Panggil event setelah dialog selesai
    }

    void ShowChoices(DialogChoice[] choices)
    {
        choicePanel.SetActive(true);
        selectedChoiceIndex = 0;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].onClick.RemoveAllListeners();
                int index = i;
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choices[index]));
                choiceTexts[i].text = (i == selectedChoiceIndex ? "> " : "") + choices[i].choiceText;
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

        characterNameText.text = "Bima";
        typingEffect.StartTyping(pendingChoiceText);
        UpdateCharacterImages("Bima", leftCharacterImage.sprite);

        // Setelah menampilkan choice text, langsung lanjut ke next line
        if (nextLineAfterChoice != -1)
        {
            currentLine = nextLineAfterChoice;
            pendingChoiceText = null;
            nextLineAfterChoice = -1;
            ShowNextLine(); // Panggil manual untuk line berikutnya
        }
        else
        {
            // Jika tidak ada next line, akhiri dialog
            EndDialog();
        }
    }

    void UpdateCharacterImages(string speakerName, Sprite sprite)
    {
        leftCharacterImage.color = new Color(1, 1, 1, 0);
        rightCharacterImage.color = new Color(1, 1, 1, 0);

        if (speakerName == "Bima")
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

    void UpdateChoiceSelection(int direction)
    {
        choiceTexts[selectedChoiceIndex].text = dialogLines[currentLine].choices[selectedChoiceIndex].choiceText;

        int choicesCount = dialogLines[currentLine].choices.Length;
        selectedChoiceIndex = (selectedChoiceIndex + direction + choicesCount) % choicesCount;

        choiceTexts[selectedChoiceIndex].text = "> " + dialogLines[currentLine].choices[selectedChoiceIndex].choiceText;

        EventSystem.current.SetSelectedGameObject(choiceButtons[selectedChoiceIndex].gameObject);
    }
}

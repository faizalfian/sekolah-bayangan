using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogManager : MonoBehaviour
{

    private int selectedChoiceIndex = 0;

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
                    // Pilih opsi yang sedang disorot
                    choiceButtons[selectedChoiceIndex].onClick.Invoke();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if (pendingChoiceText != null)
                {
                    currentLine = nextLineAfterChoice;
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
        if (currentLine < 0 || currentLine >= dialogLines.Length)
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
            // Jika ada pilihan, tampilkan pilihan
            waitingForChoice = true;
            ShowChoices(line.choices);
        }
        else
        {
            if (jumpToManualLine)
            {
                // Jika ada pilihan yang telah dipilih, melanjutkan ke line yang ditentukan
                jumpToManualLine = false; // reset flag
                return; // Tidak perlu melanjutkan ke baris berikutnya sekarang
            }

            // Jika baris dialog memiliki nextLineIndex, lanjutkan ke baris tersebut
            if (line.nextLineIndex != -1)
            {
                currentLine = line.nextLineIndex;
            }
            else
            {
                EndDialog();
            }
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
    selectedChoiceIndex = 0;

    for (int i = 0; i < choiceButtons.Length; i++)
    {
        if (i < choices.Length)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].onClick.RemoveAllListeners();
            int index = i;
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choices[index]));

            // Tambahkan ">" hanya pada pilihan pertama
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
        jumpToManualLine = true;
        choicePanel.SetActive(false);
        waitingForChoice = false;

        characterNameText.text = "Bisma";
        typingEffect.StartTyping(pendingChoiceText);
        UpdateCharacterImages("Bisma", leftCharacterImage.sprite);
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

    void UpdateChoiceSelection(int direction)
    {
     // Hilangkan simbol ">" dari yang sebelumnya
        choiceTexts[selectedChoiceIndex].text = dialogLines[currentLine].choices[selectedChoiceIndex].choiceText;

        int choicesCount = dialogLines[currentLine].choices.Length;
        selectedChoiceIndex = (selectedChoiceIndex + direction + choicesCount) % choicesCount;

        // Tambahkan simbol ">" ke pilihan yang baru
        choiceTexts[selectedChoiceIndex].text = "> " + dialogLines[currentLine].choices[selectedChoiceIndex].choiceText;

        // Fokuskan tombol baru secara visual agar tersorot
        EventSystem.current.SetSelectedGameObject(choiceButtons[selectedChoiceIndex].gameObject);
    }
}
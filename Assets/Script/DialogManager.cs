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
    public CharacterImageAnimator characterImageAnimator;
    private TypingEffect typingEffect;

    [Header("Choice UI")]
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;

    [System.Serializable]
    public class DialogLine
    {
        public string characterName;
        public string dialog;
        public Sprite characterSprite;
        public DialogChoice[] choices; // null if no choice
    }

    public DialogLine[] dialogLines;

    private int currentLine = 0;
    private bool waitingForChoice = false;
    private string pendingChoiceText = null;     // menyimpan teks pilihan
    private int nextLineAfterChoice = -1;        // menyimpan index setelah pilihan
    public List<string> playerDecisions = new(); // menyimpan keputusan pemain

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
                // Setelah menampilkan pilihan pemain, lanjut ke respons NPC
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
        characterImageAnimator.FadeIn(line.characterSprite);

        // Jika ada pilihan
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
                int index = i; // untuk closure di lambda
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
        // Simpan hasil keputusan pemain
        playerDecisions.Add(choice.consequenceTag);

        // Simpan teks yang akan ditampilkan, dan indeks baris berikutnya
        pendingChoiceText = choice.choiceText;
        nextLineAfterChoice = choice.nextLineIndex;

        choicePanel.SetActive(false);
        waitingForChoice = false;

        // Tampilkan teks pilihan sebagai dialog dari pemain
        characterNameText.text = "Bisma"; // Ganti sesuai nama karakter utama
        typingEffect.StartTyping(pendingChoiceText);
    }
}

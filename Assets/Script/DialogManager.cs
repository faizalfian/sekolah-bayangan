using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    // [Header("UI Components")]
    public GameObject dialogPanel;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogText;
    public Image characterImage;
    private TypingEffect typingEffect;

    // [Header("Dialog Data")]
    [System.Serializable]
    public class DialogLine
    {
        public string characterName;
        public string dialog;
        public Sprite characterSprite;
    }

    public DialogLine[] dialogLines;
    private int currentLine = 0;

    void Start()
    {
        dialogPanel.SetActive(false);
        typingEffect = dialogText.GetComponent<TypingEffect>();
    }

    public void StartDialog()
    {
        if (dialogLines.Length > 0)
        {
            dialogPanel.SetActive(true);
            currentLine = 0;
            dialogText.text = ""; // Kosongkan teks saat mulai dialog
            ShowNextLine();
        }
        else
        {
            Debug.LogWarning("Tidak ada data dialog!");
        }
    }

    public void ShowNextLine()
    {
        if (currentLine < dialogLines.Length)
        {
            characterNameText.text = dialogLines[currentLine].characterName;
            characterImage.sprite = dialogLines[currentLine].characterSprite;
            typingEffect.StartTyping(dialogLines[currentLine].dialog); // Gunakan typing effect
            currentLine++;
        }
        else
        {
            dialogPanel.SetActive(false); // Tutup panel jika dialog selesai
        }
    }

    void Update()
    {
        if (dialogPanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }
}

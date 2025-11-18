using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class DialogueUI : MonoBehaviour
{
    public GameObject optionPanel;
    public Button option1, option2, option3;
    public TMP_Text option1Text, option2Text, option3Text;

    public void ShowOptions(string text1, Action act1, string text2, Action act2, string text3, Action act3)
    {
        optionPanel.SetActive(true);

        option1Text.text = text1;
        option2Text.text = text2;
        option3Text.text = text3;

        option1.onClick.RemoveAllListeners();
        option2.onClick.RemoveAllListeners();
        option3.onClick.RemoveAllListeners();

        option1.onClick.AddListener(() => act1());
        option2.onClick.AddListener(() => act2());
        option3.onClick.AddListener(() => act3());
    }

    public void HideOptions()
    {
        optionPanel.SetActive(false);
    }
}

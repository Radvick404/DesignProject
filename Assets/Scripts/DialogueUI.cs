using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject optionPanel;
    public Button option1;
    public Button option2;
    public Button option3;
    public TMP_Text option1Text;
    public TMP_Text option2Text;
    public TMP_Text option3Text;

    public GameObject replyPanel;
    public TMP_Text replyText;

    public void ShowOptions(
        string o1, System.Action a1,
        string o2, System.Action a2,
        string o3, System.Action a3)
    {
        replyPanel.SetActive(false);
        optionPanel.SetActive(true);

        option1Text.text = o1;
        option2Text.text = o2;
        option3Text.text = o3;

        option1.onClick.RemoveAllListeners();
        option2.onClick.RemoveAllListeners();
        option3.onClick.RemoveAllListeners();

        option1.onClick.AddListener(() => a1());
        option2.onClick.AddListener(() => a2());
        option3.onClick.AddListener(() => a3());
    }

    public void HideOptions()
    {
        optionPanel.SetActive(false);
    }

    public void ShowReply(string reply)
    {
        optionPanel.SetActive(false);
        replyPanel.SetActive(true);
        replyText.text = reply;
    }

    public void HideReply()
    {
        replyPanel.SetActive(false);
    }
}

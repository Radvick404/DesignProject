using UnityEngine;

[System.Serializable]
public struct DialogueOption
{
    public string text;
    public int trustChange;
    public string replyText;
}

[System.Serializable]
public class DialogueSet
{
    public DialogueOption option1;
    public DialogueOption option2;
    public DialogueOption option3;
}

[System.Serializable]
public class DialogueTier
{
    public int minTrust;                  // Example: 0
    public int maxTrust;                  // Example: 30
    public DialogueSet swingDialogue;
    public DialogueSet slideDialogue;
    public DialogueSet seesawDialogue;
}

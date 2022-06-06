using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string dialogueTag;

    public string name;

    [TextArea(2, 9)]
    public string[] sentences;

}

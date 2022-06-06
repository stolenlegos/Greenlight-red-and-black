using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Dialogue dialogue;
    private bool dialogueEnded;
    public bool freezeOnDialogueStart;

    private void Start() {
        dialogueEnded = false;
    }

    public void TriggerDialogue(){
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
    private void OnMouseEnter(){
        if(this.gameObject.tag == "box_Big" && !dialogueEnded){
            TriggerDialogue();
            dialogueEnded = true;
        }
        else { }
    }
    private void OnTriggerEnter2D(Collider2D collision){
        if(this.gameObject.tag == "Examiner" && !dialogueEnded){
            TriggerDialogue();
            dialogueEnded = true;
        }
        else { }
        if(this.gameObject.tag == "GameStart" && !dialogueEnded)
        {
            TriggerDialogue();
            dialogueEnded = true;
        }
    }
}

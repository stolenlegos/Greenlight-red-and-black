using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    //Dialogue framework
    public Text nameText;
    public Text dialogueText;
    public Image textEnder;
    public static DialogueManager instance;
    private Coroutine typeWriterCoroutine;
    private bool typing = false;
    private float starTime = 0f;
    private float endTime = 1f;

    private Dialogue currentDialogue;
    //Referenced Managers and other Classes
    public Animator animator;

    //Sentence and bark framework
    private Queue<string> sentences;
    private string currentSentence;
    private string barkString;
    private Scene scene;

    //Referenced Actions
    public static event Action careOn;

    //Cursoe activation bool
    public bool careOff = true;

    //prevent repeats

    //conversation and bark bools

    //puzzle bools

    //Rest Point bools



    // Start is called before the first frame update

    private void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else{
            Destroy(gameObject);
        }
    }

    private void OnEnable(){

    }

    void Start(){
        sentences = new Queue<string>();
        scene = SceneManager.GetActiveScene();
}
    private void Update(){
        if(scene.name != "MainGame")
        {
            Destroy(gameObject);
        }
        if (sentences.Count > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!typing)
                {
                    DisplayNextSentence(currentDialogue);
                }
                else
                {
                    if (typeWriterCoroutine != null)
                    {
                        StopCoroutine(typeWriterCoroutine);
                    }
                    typing = false;
                    dialogueText.text = currentSentence;
                }
            }
        }
        else if (animator != null)
        {
            if (sentences.Count == 0 && animator.GetBool("IsOpen") == true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!typing)
                    {
                        DisplayNextSentence(currentDialogue);
                    }
                    else
                    {
                        if (typeWriterCoroutine != null)
                        {
                            StopCoroutine(typeWriterCoroutine);
                        }
                        typing = false;
                        dialogueText.text = currentSentence;
                    }
                }
            }
        }
        if (typing){
            textEnder.enabled = false;
        }
        if (!typing){
            if (starTime <= 0){
                textEnder.enabled = true;
                starTime += Time.deltaTime;
            }
            else if (starTime >= endTime){
                textEnder.enabled = false;
                starTime -= Time.deltaTime;
            }
        }
        if (Input.GetKeyDown(KeyCode.R)){
            
        }
    }

    public void StartDialogue (Dialogue dialogue){
        if (animator != null)
        {
            animator.SetBool("IsOpen", true);
        }
        nameText.text = dialogue.name;
        currentDialogue = dialogue;
        sentences.Clear();

            foreach (string sentence in dialogue.sentences){
                sentences.Enqueue(sentence);
            }

        DisplayNextSentence(currentDialogue);
    }

    public void DisplayNextSentence(Dialogue currentDialogue){
        if (sentences.Count == 0){
            EndDialogue(currentDialogue);
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
        if (typeWriterCoroutine != null){
            StopCoroutine(typeWriterCoroutine);
        }
        typeWriterCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence){
        currentSentence = sentence;
        typing = true;
        if (typing){
            
        }
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray()){
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
        typing = false;
    }

    void EndDialogue(Dialogue currentDialogue){
        if (animator != null)
        {
            animator.SetBool("IsOpen", false);
        }
    }

}

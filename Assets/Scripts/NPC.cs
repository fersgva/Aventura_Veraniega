using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC : Interactuable
{
    [SerializeField] string[] sentences;
    Animator anim;
    GameObject bubbleGO;
    TextMeshProUGUI bubbleText;
    int sentenceIndex = 0;
    int lastTalkingAnim = 0;
    GameObject player;
    Vector3 globalCanvasPos;
    Coroutine currentSpeech;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bubbleGO = thisCanvas.transform.GetChild(0).gameObject;
        bubbleText = bubbleGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        anim = GetComponent<Animator>();
        globalCanvasPos = thisCanvas.transform.TransformPoint(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Interact()
    {
        base.Interact();
        bubbleGO.SetActive(true);
        StartCoroutine(TurnToPlayer());
    }
    public override void EnableIcon()
    {
        base.EnableIcon();
        bubbleGO.SetActive(false);
    }
    IEnumerator Speech()
    {
        //-----------Random anim ------------//
        int randomTalk = 0;
        yield return new WaitWhile(() => (randomTalk = Random.Range(1, 4)) == lastTalkingAnim) ;
        lastTalkingAnim = randomTalk;
        anim.SetTrigger("talk" + randomTalk);
        //--------------------------------------//


        char[] thisSentenceCaracs = sentences[sentenceIndex].ToCharArray();
        foreach (char carac in thisSentenceCaracs)
        {
            bubbleText.text += carac;
            yield return new WaitForSeconds(0.05f);
        }
        interactIcon.SetActive(true);
    }
    IEnumerator TurnToPlayer()
    {
        thisCanvas.transform.SetParent(null);
        thisCanvas.transform.position = globalCanvasPos;
        Vector3 dirToLook = (player.transform.position - transform.position).normalized;
        Quaternion rotDestino = Quaternion.LookRotation(dirToLook, transform.up);
        while (transform.rotation != rotDestino)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotDestino, 360 * Time.deltaTime);
            yield return null;
        }
        currentSpeech = StartCoroutine(Speech());
        //Doy la posibilidad de que me pueda "cortar" el Player.
        StartCoroutine(PlayerInteractions.plInterScr.ContinueConversation(this));
    }

    public void NextSentence()
    {
        if (currentSpeech != null) //Si hay un speech en proceso...
        {
            //Lo corto...
            StopCoroutine(currentSpeech);
            currentSpeech = null;
            bubbleText.text = sentences[sentenceIndex];

        }
        else
        {
            Debug.Log("hola");
            sentenceIndex++;
            if (sentenceIndex < sentences.Length)
            {
                interactIcon.SetActive(false);
                bubbleText.text = "";
                currentSpeech = StartCoroutine(Speech());
            }
            else
            {
                thisCanvas.transform.SetParent(transform);
                PlayerInputsAnims.plInputScr.enabled = true;
                PlayerInteractions.plInterScr.interacting = false;
                sentenceIndex = 0; //prepararlo para la próxima.
                bubbleText.text = "";
            }
        }

        
    }
}
    




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
        //------------Random anim--------------//
        int randomTalk = 0;
        yield return new WaitUntil(() => (randomTalk = Random.Range(1, 4)) != lastTalkingAnim);
        lastTalkingAnim = randomTalk;
        anim.SetTrigger("talk" + randomTalk);
        //-----------------------------------//

        char[] thisSentenceCaracs = sentences[sentenceIndex].ToCharArray();
        foreach (char carac in thisSentenceCaracs)
        {
            bubbleText.text += carac;
            yield return new WaitForSeconds(0.05f);
        }
        interactIcon.SetActive(true);
        StartCoroutine(PlayerInteractions.plInterScr.ContinueConversation(this));
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
        StartCoroutine(Speech());
    }

    public void NextSentence()
    {
        sentenceIndex++;
        if(sentenceIndex < sentences.Length)
        {
            interactIcon.SetActive(false);
            bubbleText.text = "";
            StartCoroutine(Speech());
        }
        else
        {
            thisCanvas.transform.SetParent(transform);
            PlayerInputsAnims.plInputScr.enabled = true;
            PlayerInteractions.plInterScr.interacting = false;
            sentenceIndex = 0; //prepararlo para la pr?xima.
            bubbleText.text = "";
        }
    }
}
    




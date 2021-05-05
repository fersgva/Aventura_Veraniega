using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC : Interactuable
{
    [SerializeField] string[] sentences;
    GameObject bubbleGO;
    TextMeshProUGUI bubbleText;
    int sentenceIndex = 0;
    GameObject player;
    RectTransform canvasTransform;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        bubbleGO = thisCanvas.transform.GetChild(0).gameObject;
        bubbleText = bubbleGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        canvasTransform = thisCanvas.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Talk()
    {
        thisCanvas.SetActive(true);
        bubbleGO.SetActive(true);
        interactIcon.SetActive(false);
        StartCoroutine(Speech());
        StartCoroutine(TurnToPlayer());
    }
    public override void EnableIcon()
    {
        base.EnableIcon();
        bubbleGO.SetActive(false);
    }
    IEnumerator Speech()
    {
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
        Vector3 dirToLook = (player.transform.position - transform.position).normalized;
        Quaternion rotDestino = Quaternion.LookRotation(dirToLook, transform.up);
        while (transform.rotation != rotDestino)
        {
            Debug.Log(canvasTransform.TransformPoint(canvasTransform.rect.center));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotDestino, 360 * Time.deltaTime);
            yield return null;
        }
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
            thisCanvas.SetActive(false);
            PlayerInputsAnims.plInputScr.enabled = true;
            PlayerInteractions.plInterScr.interacting = false;
            sentenceIndex = 0; //prepararlo para la próxima.
            bubbleText.text = "";
        }
    }
}
    




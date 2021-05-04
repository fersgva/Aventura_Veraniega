using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] GameObject thisCanvas;
    GameObject bubbleText;
    GameObject interactIcon;
    private void Awake()
    {
        bubbleText = thisCanvas.transform.GetChild(0).gameObject;
        interactIcon = thisCanvas.transform.GetChild(1).gameObject;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Talk()
    {
        thisCanvas.SetActive(true);
        bubbleText.SetActive(true);
        interactIcon.SetActive(false);
    }
    public void EnableIcon()
    {
        thisCanvas.SetActive(true);
        bubbleText.SetActive(false);
        interactIcon.SetActive(true);
    }
    public void DisableIcon()
    {
        thisCanvas.SetActive(false);
    }

}

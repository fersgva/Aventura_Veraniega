using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactuable : MonoBehaviour
{
    [SerializeField] protected GameObject thisCanvas;
    protected GameObject interactIcon;
    void Awake()
    {
        interactIcon = thisCanvas.transform.GetChild(1).gameObject;
    }
    public virtual void EnableIcon()
    {
        thisCanvas.SetActive(true);
        interactIcon.SetActive(true);
    }
    public void DisableIcon()
    {
        thisCanvas.SetActive(false);
    }
    public virtual void Interact()
    {
        thisCanvas.SetActive(true);
        interactIcon.SetActive(false);
    }
}

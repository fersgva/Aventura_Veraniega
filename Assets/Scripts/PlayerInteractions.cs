using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    PlayerControls plControls;
    bool interactButton;
    Transform interactionPoint;
    [SerializeField] LayerMask isInteractuable;
    Collider[] collsInFront;
    Collider lastCollInFront;
    NPC lastInteractScript;
    bool interacting;

    public static PlayerInteractions plInterScr;
    private void Awake()
    {
        if (plInterScr == null)
            plInterScr = this;
        else if (plInterScr != this)
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerInputsAnims.plInputScr.plControls.Gameplay.Interact.performed += ctx => interactButton = true;
        interactionPoint = transform.GetChild(2);
    }

    // Update is called once per frame
    void Update()
    {
        if(!interacting)
        {
            collsInFront = Physics.OverlapSphere(interactionPoint.position, 0.7f, isInteractuable);
            if (collsInFront.Length > 0)
            {
                lastCollInFront = collsInFront[0];
                lastInteractScript = lastCollInFront.transform.GetComponent<NPC>();
                lastInteractScript.EnableIcon();
                if (interactButton)
                    HandleInteraction();
            }
            else
            {
                if (lastCollInFront != null) //Hubo un anterior coll. con el que interactuar..
                {
                    lastInteractScript.DisableIcon();
                    lastCollInFront = null;
                }
            }
        }
        
        
    }

    void HandleInteraction()
    {
        Debug.Log("hola");
        if (lastCollInFront.CompareTag("NPC"))
        {
            interacting = true;
            PlayerInputsAnims.plInputScr.enabled = false; //Deshabilito script de movimiento.
            lastInteractScript.GetComponent<NPC>().Talk();
        }
        interactButton = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(interactionPoint.position, 0.7f);
    }
}

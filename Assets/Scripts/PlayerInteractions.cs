using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    PlayerControls plControls;
    Transform interactionPoint;
    [SerializeField] LayerMask isInteractuable;
    Collider[] collsInFront;
    Collider lastCollInFront;
    Interactuable lastInteractScript;
    [HideInInspector] public bool interacting;
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
                lastInteractScript = lastCollInFront.transform.GetComponent<Interactuable>();
                lastInteractScript.EnableIcon();
                if (Gamepad.current.buttonNorth.wasPressedThisFrame)
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
        if (lastCollInFront.CompareTag("NPC"))
        {
            interacting = true;
            PlayerInputsAnims.plInputScr.HandleAnimsForInteraction();
            PlayerInputsAnims.plInputScr.enabled = false; //Deshabilito script de movimiento.
            StartCoroutine(LookAtInteraction());
        }
    }
    IEnumerator LookAtInteraction()
    {

        Vector3 dirToLook = (lastCollInFront.transform.position - transform.position).normalized;
        Quaternion rotDestino = Quaternion.LookRotation(dirToLook, transform.up);
        while (transform.rotation != rotDestino)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotDestino, 360 * Time.deltaTime);
            yield return null;
        }
        lastInteractScript.Interact();
    }
    public IEnumerator ContinueConversation(NPC npcTalking)
    {
        yield return new WaitUntil(() => Gamepad.current.buttonNorth.wasPressedThisFrame);
        npcTalking.NextSentence();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(interactionPoint.position, 0.7f);
    }
}

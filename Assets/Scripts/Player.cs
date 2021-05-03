using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    PlayerControls plControls;
    Vector2 controllerDir;
    Vector3 direction;
    Animator anim;
    float speedFactor = 2f;
    CharacterController controller;
    AnimatorStateInfo currentState;
    Transform interactionPoint;
    bool running;
    bool attack;
    bool interact;
    [SerializeField] LayerMask isInteractuable;
    [SerializeField] GameObject pruebaCubo;
    private void Awake()
    {
        plControls = new PlayerControls();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        interactionPoint = transform.GetChild(2);
        //Evento a la escucha de movernos.
        plControls.Gameplay.Move.performed += ctx => controllerDir = ctx.ReadValue<Vector2>();
        plControls.Gameplay.Move.canceled += ctx => controllerDir = Vector2.zero;

        plControls.Gameplay.Run.performed += ctx => running = true;
        plControls.Gameplay.Sword.performed += ctx => attack = true;
        plControls.Gameplay.Interact.performed += ctx => interact = true;
    }
    private void OnEnable()
    {
        plControls.Gameplay.Enable();
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(pruebaCubo.transform.position);
        Debug.Log(pos);
        currentState = anim.GetCurrentAnimatorStateInfo(0);
        direction = new Vector3(controllerDir.x, 0, controllerDir.y);
        anim.SetFloat("velocity", direction.magnitude * speedFactor);

        if (running)
            HandleRunning();
        if (attack)
            HandleSword();
        if (interact)
            HandleInteraction();

        if (direction.magnitude > 0.125f && !currentState.IsName("BasicAttack") && !currentState.IsName("GuardarEspada1")) //Zona muerta --> También establecido en el animator!!!
        {
            //timeForRest = 0;
            Quaternion rotDestino = Quaternion.LookRotation(direction, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotDestino, 7 * Time.deltaTime);
            controller.Move(direction * direction.magnitude * speedFactor * Time.deltaTime);
        }
        
    }
    void HandleSword()
    {
        if (currentState.IsName("BreathingIdleWSword") && direction.magnitude < 0.125f)
            anim.SetTrigger("guardar");

        else
            anim.SetTrigger("attack");

        attack = false;
    }
    void HandleRunning()
    {
        float minVelocityForRun = 0.9f;
        float growingFactor = 5;
        float maxVelocity = 5f;

        if (direction.magnitude > minVelocityForRun)
        {
            speedFactor += growingFactor * Time.deltaTime;
            if (speedFactor > maxVelocity)
            {
                speedFactor = maxVelocity;
            }
        }
        else
        {
            speedFactor -= growingFactor * Time.deltaTime;
            //Si llegamos al límite inferior del bleend tree o nos quedamos parados...
            if (speedFactor <= 2 || direction.magnitude < 0.5f)
            {
                running = false;
                speedFactor = 2;
            }
            //if(speedFactor <= 2)
            //    running = false;
        }
    }
    void HandleInteraction()
    {
        Debug.Log("inter");
        Collider[] colls = Physics.OverlapSphere(interactionPoint.position, 0.7f, isInteractuable);
        if(colls.Length > 0)
        {
           if(colls[0].CompareTag("NPC"))
           {
                colls[0].transform.GetChild(0).gameObject.SetActive(true);
           }
        }
        interact = false;
    }
    private void OnDisable()
    {
        plControls.Gameplay.Disable();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(interactionPoint.position, 0.7f);
    }
}

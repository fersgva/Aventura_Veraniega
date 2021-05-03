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
    bool running;
    bool attack;

    private void Awake()
    {
        plControls = new PlayerControls();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        //Evento a la escucha de movernos.
        plControls.Gameplay.Move.performed += ctx => controllerDir = ctx.ReadValue<Vector2>();
        plControls.Gameplay.Move.canceled += ctx => controllerDir = Vector2.zero;

        plControls.Gameplay.DrawSword.performed += ctx => attack = true;
        plControls.Gameplay.Run.performed += ctx => running = true;
    }
    private void OnEnable()
    {
        plControls.Gameplay.Enable();
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    void HandleSword()
    {
        if(currentState.IsName("BreathingIdleWSword") && direction.magnitude < 0.125f)
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
            if(speedFactor > maxVelocity)
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
    // Update is called once per frame
    void Update()
    {
        currentState = anim.GetCurrentAnimatorStateInfo(0);
        direction = new Vector3(controllerDir.x, 0, controllerDir.y);
        anim.SetFloat("velocity", direction.magnitude * speedFactor);
        Debug.Log(direction.magnitude * speedFactor);
        if (running)
            HandleRunning();
        if (attack)
            HandleSword();

        if (direction.magnitude > 0.125f && !currentState.IsName("BasicAttack")) //Zona muerta --> También establecido en el animator!!!
        {
            //timeForRest = 0;
            Quaternion rotDestino = Quaternion.LookRotation(direction, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotDestino, 7 * Time.deltaTime);
            controller.Move(direction * direction.magnitude * speedFactor * Time.deltaTime);
        }
        
    }
    private void OnDisable()
    {
        plControls.Gameplay.Disable();
    }
}

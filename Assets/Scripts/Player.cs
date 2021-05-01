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
    bool running;
    bool attack;
    [SerializeField] GameObject realPlayerSword;
    [SerializeField] GameObject fakeSword;
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
        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        if(currentState.IsName("BreathingIdle"))
            anim.SetTrigger("drawSword");
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
            Debug.Log(speedFactor);
            speedFactor += growingFactor * Time.deltaTime;
            if(speedFactor > maxVelocity)
            {
                speedFactor = maxVelocity;
            }
        }
        else
        {
            speedFactor -= growingFactor * Time.deltaTime;
            if(speedFactor <= 2)
                running = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        direction = new Vector3(controllerDir.x, 0, controllerDir.y);
        anim.SetFloat("velocity", direction.magnitude * speedFactor);
        if(running)
            HandleRunning();
        if (attack)
            HandleSword();
        //if(anim.GetFloat("velocity") < 0.7f)
        //{
        //    CancelRunning();
        //}
        if (direction.magnitude > 0.15f) //Zona muerta --> También establecido en el animator!!!
        {
            //Debug.Log(direction.normalized);
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

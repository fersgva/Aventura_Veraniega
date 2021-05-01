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
    float speedFactor = 2f; //Por si queremos correr más rápido (boost o algo por el estilo)
    CharacterController controller;
    private void Awake()
    {
        plControls = new PlayerControls();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        //Evento a la escucha de movernos.
        plControls.Gameplay.Move.performed += ctx => controllerDir = ctx.ReadValue<Vector2>();
        plControls.Gameplay.Move.canceled += ctx => controllerDir = Vector2.zero;
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
        direction = new Vector3(controllerDir.x, 0, controllerDir.y);
        anim.SetFloat("velocity", direction.magnitude);
        if(direction.magnitude > 0.15f) //Zona muerta --> También establecido en el animator!!!
        {
            //Debug.Log(direction.normalized);
            Quaternion rotDestino = Quaternion.LookRotation(direction, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotDestino, 7 * Time.deltaTime);
            Debug.Log(direction.magnitude * speedFactor);
            controller.Move(direction * direction.magnitude * speedFactor * Time.deltaTime);
        }
        
    }
    private void OnDisable()
    {
        plControls.Gameplay.Disable();
    }
}

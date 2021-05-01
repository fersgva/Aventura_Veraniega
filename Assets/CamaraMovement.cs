using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraMovement : MonoBehaviour
{
    [SerializeField] GameObject player;
    Vector3 initDist;
    // Start is called before the first frame update
    void Start()
    {
        initDist = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.transform.position + initDist;
    }
}

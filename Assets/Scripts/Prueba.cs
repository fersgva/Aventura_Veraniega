using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prueba : MonoBehaviour
{
    [SerializeField] GameObject player;
    float distToPlayer;
    float frequency;
    float amplitude = 10;
    float enemySpeed = 1;
    Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
        distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        frequency = (2 * Mathf.PI) / (distToPlayer * 2);
    }

    // Update is called once per frame
    void Update()
    {
        //El seno no viene determinado por Time.time porque su periodo va a depender de su movimiento en Z.
        //VER: https://wikimedia.org/api/rest_v1/media/math/render/svg/5ac074fa69f509d6566f34c60c098834f3f24495
        float sin = initPos.x + amplitude * Mathf.Sin(frequency * transform.position.z);
        transform.position = new Vector3(sin, 0, transform.position.z);        
        transform.Translate(new Vector3(0, 0, 1) * enemySpeed * Time.deltaTime, Space.Self);
    }
}

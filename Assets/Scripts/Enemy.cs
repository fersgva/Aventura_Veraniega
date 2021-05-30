using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    Vector3 initPos;
    Vector3 currentDest;
    float navRadius = 10;
    float minDistanceToNewPoint = 3;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        initPos = transform.position;
        StartCoroutine(NavigateToNewDest());
    }

    // Update is called once per frame
    void Update()
    {
    }
    IEnumerator NavigateToNewDest()
    {
        Vector3 newDest = initPos + Random.insideUnitSphere * navRadius;
        //Para que el nuevo punto al que ir no est� tan cerca del que estamos ahora.
        while(Vector3.Distance(transform.position, newDest) < minDistanceToNewPoint)
        {
            newDest = initPos + Random.insideUnitSphere * navRadius;
            yield return null;
        }
        currentDest = newDest;
        agent.SetDestination(currentDest);
        yield return new WaitUntil(() => agent.remainingDistance == 0 && !agent.pathPending);
        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        anim.SetBool("looking", true);
        yield return new WaitForSeconds(4);
        anim.SetBool("looking", false);
        StartCoroutine(NavigateToNewDest());
    }
}

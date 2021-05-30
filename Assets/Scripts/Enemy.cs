using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mine.Utilities;

public class Enemy : MonoBehaviour
{
    [SerializeField] LayerMask isDetectable;
    [SerializeField] LayerMask isObstacle;
    [SerializeField] GameObject head;
    FieldOfView enemView;
    NavMeshAgent agent;
    Vector3 initPos;
    Vector3 currentDest;
    float navRadius = 10;
    float minDistanceToNewPoint = 3;
    Animator anim;
    enum State { patrol, combat};
    State enemState;
    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemView = head.GetComponent<FieldOfView>();
        anim = GetComponent<Animator>();
        initPos = transform.position;
        enemState = State.patrol;
        StartCoroutine(NavigateToNewDest());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(enemState == State.patrol)
        {
            Collider[] colls = Physics.OverlapSphere(head.transform.position, enemView.viewRadius, isDetectable);
            if (colls.Length > 0) //Player detectado dentro de nuestro radio.
            {
                target = colls[0].gameObject;
                Vector3 dirToTarget = target.transform.position - head.transform.position;
                if (Vector3.Angle(head.transform.forward, dirToTarget.normalized) <= enemView.viewAngle / 2) //Player en nuestro cono de visión.
                {
                    //Y si NO hay obstáculo entre donde estoy y el player....
                    if (!Physics.Raycast(head.transform.position, dirToTarget, dirToTarget.magnitude, isObstacle))
                    {
                        enemState = State.combat;
                        StopAllCoroutines();
                        anim.SetBool("looking", false);
                        agent.stoppingDistance = 2;
                    }
                }
            }
        }
        else if(enemState == State.combat)
        {
            agent.SetDestination(target.transform.position);
        }
    }
    IEnumerator NavigateToNewDest()
    {
        Vector3 newDest = initPos + Random.insideUnitSphere * navRadius;
        //Para que el nuevo punto al que ir no esté tan cerca del que estamos ahora.
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

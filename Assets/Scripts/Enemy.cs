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
    enum State { patrol, chase, combat};
    State enemState;
    GameObject target;
    Vector3 dirToTarget;
    bool playerOnSight;
    float combatTimer;
    float timeForNextAttack;
    float meleeDistance = 1.4f;
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
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if(enemState == State.chase)
        {
            agent.speed = 4f;
            agent.SetDestination(target.transform.position);
            if(agent.remainingDistance <= meleeDistance) //Nos ha alcanzado
            {
                agent.isStopped = true;
                anim.SetBool("combatIdle", true);
                timeForNextAttack = Random.Range(0f, 4f); //Tiempo random para el primer ataque a lanzar.
                enemState = State.combat;
            }
        }
        else if(enemState == State.combat)
        {
            dirToTarget = target.transform.position - transform.position;
            transform.forward = dirToTarget.normalized;
            agent.SetDestination(target.transform.position);
            combatTimer += Time.deltaTime;
            if (agent.remainingDistance <= meleeDistance) //Si la distancia sigue siendo melee
            {
                agent.isStopped = true;
                if(combatTimer >= timeForNextAttack)
                {
                    anim.SetTrigger("attack");
                    timeForNextAttack = Random.Range(0f, 4f); //Déspués de cada ataque, calculamos el tiempo para el siguinete.
                    combatTimer = 0;
                }
            }
            else if (agent.remainingDistance  > 1.5f && !info.IsName("EnemyAttack"))
            {
                agent.isStopped = false;
                anim.SetBool("combatIdle", false);
                enemState = State.chase;
            }
        }
    }
    private void FixedUpdate()
    {
        if(enemState == State.patrol)
        {
            if(!playerOnSight)
            {
                Collider[] colls = Physics.OverlapSphere(head.transform.position, enemView.viewRadius, isDetectable);
                if (colls.Length > 0) //Player detectado dentro de nuestro radio.
                {
                    target = colls[0].gameObject;
                    dirToTarget = target.transform.position - head.transform.position;
                    if (Vector3.Angle(head.transform.forward, dirToTarget.normalized) <= enemView.viewAngle / 2) //Player en nuestro cono de visión.
                    {
                        //Y si NO hay obstáculo entre donde estoy y el player....
                        if (!Physics.Raycast(head.transform.position, dirToTarget, dirToTarget.magnitude, isObstacle))
                        {
                            StopAllCoroutines();
                            anim.SetBool("looking", false); //por si estaba mirando.
                            anim.SetTrigger("alert");
                            agent.isStopped = true;
                            playerOnSight = true;
                        }
                    }
                }
            }
            else //Para hacer el seguimiento visual mientras dura la animación de Alert.
            {
                dirToTarget = target.transform.position - head.transform.position;
                transform.forward = dirToTarget.normalized;
            }
            
        }
    }

    void OnAlertFinish()
    {
        agent.isStopped = false;
        agent.stoppingDistance = meleeDistance;
        enemState = State.chase;
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

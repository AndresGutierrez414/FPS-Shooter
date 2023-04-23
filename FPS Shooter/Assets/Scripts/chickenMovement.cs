using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class chickenMovement : MonoBehaviour
{
    // variables //

    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDist;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] float animTransSpeed;

    float stoppingDistanceOrig;
    bool destinationChosen;
    Vector3 startingPos;
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        stoppingDistanceOrig = agent.stoppingDistance;
        startingPos = transform.position;

        animator = GetComponent<Animator>();

        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            speed = Mathf.Lerp(speed, agent.velocity.normalized.magnitude, Time.deltaTime * animTransSpeed);
            animator.SetFloat("Speed", speed);

            // if moving, update look direction //
            if (agent.velocity.magnitude > 0.1f)
            {
                Quaternion rotation = Quaternion.LookRotation(agent.velocity.normalized);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
            }

            StartCoroutine(roam());
        }

    }

    IEnumerator roam()
    {
        if (!destinationChosen && agent.remainingDistance < 0.05f)
        {
            animator.SetBool("Run", false);

            destinationChosen = true;

            agent.stoppingDistance = 0;

            yield return new WaitForSeconds(roamPauseTime);
            destinationChosen = false;

            // generate roandom pos to go to //
            Vector3 randPos = Random.insideUnitSphere * roamDist;
            // keep unit in roam area of start pos //
            randPos += startingPos;

            // check pos then move to pos //
            NavMeshHit hit;
            NavMesh.SamplePosition(randPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);
        }
        else
        {
            animator.SetBool("Run", true);
        }
    }
}

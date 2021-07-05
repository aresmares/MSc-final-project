using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NoAgentControl : MonoBehaviour
{


    public GameObject endPosition;
    private NavMeshAgent navAgent;
    private AutoParkAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = this.GetComponent<NavMeshAgent>();
        agent = this.GetComponent<AutoParkAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        endPosition = GameObject.FindGameObjectWithTag("Finish");
        navAgent.SetDestination(new Vector3(-2.4f,0, endPosition.transform.position.z+2 ));

        //     agent.SetModel();
    }
}

using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using Unity.Barracuda;


[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(SimulationManager))]

public class FullAgentControl : Agent
{
    private Rigidbody _rigitBody;
    private CarController _controller;
    private SimulationManager _simulationManager;
    private float[] _lastActions;
    private GameObject _nearestLot;
    private Rewards _rewards;

    public GameObject manager;
    
    public override void Initialize()
    {
        _rigitBody = GetComponent<Rigidbody>();
        _controller = GetComponent<CarController>();
        _rewards = GetComponent<Rewards>();

        _simulationManager = manager.GetComponent<SimulationManager>();
        // _simulationManager.InitializeSimulation();
    }

    public override void OnEpisodeBegin()
    {
        _simulationManager.ResetSimulation();
        _simulationManager.InitializeSimulation();
        _nearestLot = null;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        _lastActions = vectorAction;
        _controller.CurrentSteeringAngle = vectorAction[0];
        _controller.CurrentAcceleration = vectorAction[1];
        _controller.CurrentBrakeTorque = vectorAction[2];

        // // Apply Fuzzy rewards per action taken
        // AddReward(_rewards.FuzzyDistanceAlignmentReward(gameObject,_nearestLot));
        // AddReward(_rewards.FuzzyDistanceAccelerationReward(gameObject,_nearestLot));

    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        actionsOut[2] = Input.GetAxis("Jump");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("barrier") || other.gameObject.CompareTag("car") ||
            other.gameObject.CompareTag("tree"))
        {
            // Collision Reward
            AddReward(_rewards.CollisionReward());
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (_lastActions != null && _simulationManager.InitComplete)
        {
            _nearestLot = GameObject.Find("EndPosition");
            Vector3 dirToTarget = (_nearestLot.transform.position - transform.position).normalized;
            sensor.AddObservation(transform.position.normalized);
            sensor.AddObservation(this.transform.InverseTransformPoint(_nearestLot.transform.position));
            sensor.AddObservation(this.transform.InverseTransformVector(_rigitBody.velocity.normalized));
            sensor.AddObservation(this.transform.InverseTransformDirection(dirToTarget));
            sensor.AddObservation(transform.forward);
            sensor.AddObservation(transform.right);
        }
        else
        {
            sensor.AddObservation(new float[18]);
        }
    }

    public IEnumerator JackpotReward(float bonus)
    {
        // Bonus on hitting goal
        if(bonus > 0.2f)
            Debug.LogWarning("Jackpot hit! " + bonus);
        AddReward(0.2f + bonus);
        yield return new WaitForEndOfFrame();
        EndEpisode();
    }

}

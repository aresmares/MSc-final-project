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

public class FullAgent : Agent
{
    private Rigidbody _rigitBody;
    private CarController _controller;
    private float[] _lastActions;
    private ParkingLot _nearestLot;

    // public GameObject manager;
    
    public override void Initialize()
    {
        _rigitBody = GetComponent<Rigidbody>();
        _controller = GetComponent<CarController>();
        Debug.Log("nowing");

        // _simulationManager.InitializeSimulation();
    }

    public override void OnEpisodeBegin()
    {
        _nearestLot = null;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Debug.Log(vectorAction);
        _lastActions = vectorAction;
        _controller.CurrentSteeringAngle = vectorAction[0];
        _controller.CurrentAcceleration = vectorAction[1];
        _controller.CurrentBrakeTorque = vectorAction[2];
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
            AddReward(-0.01f);
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
                    // Debug.Log("observing");

        // if (_lastActions != null && _simulationManager.InitComplete)
        if (_lastActions != null)
        {   
            Debug.Log("observing");
            // if(_nearestLot == null)
            //     _nearestLot = _simulationManager.GetRandomEmptyParkingSlot();
            Vector3 dirToTarget = (_nearestLot.transform.position - transform.position).normalized;
            sensor.AddObservation(transform.position.normalized);
            sensor.AddObservation(
                this.transform.InverseTransformPoint(_nearestLot.transform.position));
            sensor.AddObservation(
                this.transform.InverseTransformVector(_rigitBody.velocity.normalized));
            sensor.AddObservation(
                this.transform.InverseTransformDirection(dirToTarget));
            sensor.AddObservation(transform.forward);
            sensor.AddObservation(transform.right);
            // sensor.AddObservation(StepCount / MaxStep);
            float velocityAlignment = Vector3.Dot(dirToTarget, _rigitBody.velocity);
            AddReward(0.001f * velocityAlignment);
        }
        else
        {
            sensor.AddObservation(new float[18]);
        }
    }

    public IEnumerator JackpotReward(float bonus)
    {
        if(bonus > 0.2f)
            Debug.LogWarning("Jackpot hit! " + bonus);
        AddReward(0.2f + bonus);
        yield return new WaitForEndOfFrame();
        EndEpisode();
    }

    public void setModel(NNModel model)
    {
        SetModel("Inference",model);
    }
}

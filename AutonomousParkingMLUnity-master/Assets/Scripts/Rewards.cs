using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 
using UnityEngine;

public class Rewards : MonoBehaviour
{
    public float atGoal = 1.0f;
    public float atGoalScaleFactor = 2.0f;
    public float alignmentBonus =  0.001f;
    public float velocityAlignment =  0.001f;
    public float collisions = -0.1f;

    public float VelocityAlignmentReward(Vector3 target, Vector3 position)
    {
        return velocityAlignment * Vector3.Dot(target,position);
    }

    public float CollisionReward()
    {
        return collisions;
    }
    public float EmptyLot()
    {
        return atGoal;
    }
    public float EmptyLotGoal()
    {
        return atGoal * atGoalScaleFactor;
    }


    // FUZZY METHODS


    public AnimationCurve fuzzyDistance; 
    public AnimationCurve fuzzyAcceleration; 
    public AnimationCurve fuzzyAlignment; 
    public AnimationCurve fuzzyVelocity;

    public float fuzzyScaleFactor;

    // RULES:
    //      Rule 1 : IF distance to goal is large AND acceleration is static THEN reward is large
    //      Rule 2 : IF distance to goal is small AND acceleration is large THEN reward is small
    public float FuzzyDistanceAccelerationReward(GameObject agent, GameObject goalPosition)
    {  
        float distance = Vector3.Distance(transform.position, goalPosition.transform.position);
        float acceleration = GetComponent<CarController>().CurrentAcceleration;
        // Debug.Log(acceleration  + ",A");

        float distance_value = fuzzyDistance.Evaluate(distance);
        float acceleration_value = fuzzyAcceleration.Evaluate(acceleration);

        float reward = distance_value * acceleration_value;

        // Debug.Log(reward   +".accel");
        return reward * fuzzyScaleFactor;
    }

    // RULES:
    //      Rule 1 : IF distance to goal is small, decreasing AND agent is aligned to goal THEN reward is large, increasing
    //                                distance < 2               0 -> 0.5 < alignemnt > 0.5 -> 1
    //      Rule 2 : IF distance to goal is medium AND agent is aligned to goal THEN reward is small, constant
    //                                2 < distance < 8
    //      Rule 3 : IF distance to goal is large, increasing AND agent is aligned to goal THEN reward is small, decreasing
    //                                distance > 8                      
    public float FuzzyDistanceAlignmentReward(GameObject agent, GameObject goalPosition)
    {  
        float distance = Vector3.Distance(transform.position, goalPosition.transform.position);
        float alignment = agent.transform.eulerAngles.y / 360;
        // Debug.Log(alignment  + ",A");

        float distance_value = fuzzyDistance.Evaluate(distance);
        float alignment_value = fuzzyAlignment.Evaluate(alignment);

        float reward = alignment_value * distance_value;

        // Debug.Log(reward   +".align");
        return reward * fuzzyScaleFactor;

    }


    public float FuzzyVelocityReward(GameObject agent, GameObject goalPosition)
    {  
        float distance = Vector3.Distance(transform.position, goalPosition.transform.position);
        Vector3 velocity = agent.GetComponent<Rigidbody>().velocity;

        float vector = Mathf.Sqrt(velocity[0]*velocity[0] + velocity[2]*velocity[2]);
        // Debug.Log(vector  + ",A");

        float reward = 0;
        if (distance < 5) 
        {
            // / = fuzzyDistance.Evaluate(distance);
            reward= fuzzyVelocity.Evaluate(vector);
            
            // reward = distance_value * velocity_value;
        }
        // Debug.Log(reward  + ",A");

        return reward * fuzzyScaleFactor;

    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingLot : MonoBehaviour
{
    public bool IsOccupied { get; set; }
    public bool IsGoal { get; set; }

    public Vector3 Orientation => transform.forward;
    private Collider fullEndCollider;

    private void Awake()
    {
        fullEndCollider = GetComponent<Collider>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("agent"))
        {
            if (fullEndCollider.bounds.Intersects(other.bounds))
            {
                if (!IsOccupied)
                {
                    bool goal = false;
                    // float bonusfactor = 0.8f;
                    if (IsGoal)
                    {
                        // bonusfactor = 2 * 0.8f;
                        goal = true;
                    }

                    StartCoroutine( other.gameObject.transform.parent.GetComponent<AutoParkAgent>().JackpotReward(goal));
                }
            }
        }
    }

}

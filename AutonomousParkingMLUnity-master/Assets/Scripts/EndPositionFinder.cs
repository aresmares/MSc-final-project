using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPositionFinder : MonoBehaviour
{
    private GameObject[] parkingLots;


    // Update is called once per frame
    void Update()
    {
        parkingLots = GameObject.FindGameObjectsWithTag("parkingslot");
        if (parkingLots != null) 
        {
            foreach (GameObject lot in parkingLots)
            {
                ParkingLot slot = lot.GetComponent<ParkingLot>();

               if (slot.IsGoal)
               {
                    // transform.position = new Vector3(-2.4f,0, slot.transform.position.z+2 );
                    transform.position = slot.transform.position;

               }            
            }
        }


    }
}
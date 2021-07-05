using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NoAgentSimulationManager : MonoBehaviour
{
   GameObject endPosition;
   [SerializeField] private List<ParkingLot> parkingLots;
   [SerializeField] private List<GameObject> carPrefabs;

   private List<GameObject> parkedCars;
   
   private  float spawnXMin = -20f;
   private  float spawnXMax = 1.1f;
   private  float spawnZMin = -5f;
   private  float spawnZMax = 10f;
   private bool _initComplete = false;

   public bool InitComplete => _initComplete;

   private void Start()
   {
      parkedCars = new List<GameObject>();
      InitializeSimulation();

   }

   public void InitializeSimulation()
   {
      _initComplete = false;
      Debug.Log("start");
      StartCoroutine(OccupyParkingSlotsWithRandomCars());
       
   }

   public IEnumerator OccupyParkingSlotsWithRandomCars()
   {
      foreach (ParkingLot parkingLot in parkingLots)
      {
         parkingLot.IsOccupied = false;
         parkingLot.IsGoal = false;
      }
      yield return new WaitForSeconds(1);

      int total = Random.Range(15, 19);
      for (int i = 0; i < total; i++)
      {
         ParkingLot lot = parkingLots.Where(r => r.IsOccupied == false).OrderBy(r => Guid.NewGuid()).FirstOrDefault();
         if (lot != null)
         {
            GameObject carInstance = Instantiate(carPrefabs[Random.Range(0, 3)]);
            carInstance.transform.position = new Vector3(lot.transform.position.x, 1, lot.transform.position.z);
            parkedCars.Add(carInstance);
            lot.IsOccupied = true;
            if(parkedCars.Count >= total)
               break;
         }
      }

      ParkingLot randomSpot = GetRandomEmptyParkingSlot();
      randomSpot.IsGoal = true;
      Debug.Log(randomSpot.name);

      _initComplete = true;
      
   }

   public ParkingLot GetRandomEmptyParkingSlot()
   {
     return parkingLots.Where(r => r.IsOccupied == false).OrderBy(r => Guid.NewGuid())
         .FirstOrDefault();
   }


}

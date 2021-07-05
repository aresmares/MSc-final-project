using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimulationManager : MonoBehaviour
{
   GameObject endPosition;
   [SerializeField] private List<ParkingLot> parkingLots;
   [SerializeField] private List<GameObject> carPrefabs;
   [SerializeField] private AutoParkAgent agent;

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
       
      // yield new WaitForSeconds(2);

      // EndPositionFinder targetPosition = new EndPositionFinder();
      // Init end position


   }

   public void RepositionAgentRandom()
   {
      if (agent != null)
      {
         agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
         agent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
         agent.GetComponent<CarController>().CurrentSteeringAngle = 0f;
         agent.GetComponent<CarController>().CurrentAcceleration = 0f;
         agent.GetComponent<CarController>().CurrentBrakeTorque = 0f;
         agent.transform.rotation = Quaternion.Euler(0,180,0);
         agent.transform.position = transform.parent.position + new Vector3(Random.Range(spawnXMin,spawnXMax),-0.61f,Random.Range(spawnZMin,spawnZMax));
      }
   }

   public void ResetSimulation()
   {
      foreach (GameObject parkedCar in parkedCars)
      {
         Destroy(parkedCar);
      }

      foreach (ParkingLot parkingLot in parkingLots)
      {
         parkingLot.IsOccupied = false;
         parkingLot.IsGoal = false;

      }
      parkedCars.Clear();
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

      PositionAtSafePlace(randomSpot.gameObject);

      // For move to goal 
      NoAgentControl moveTo = new NoAgentControl();
      _initComplete = true;
      
   }

   public ParkingLot GetRandomEmptyParkingSlot()
   {
     return parkingLots.Where(r => r.IsOccupied == false).OrderBy(r => Guid.NewGuid())
         .FirstOrDefault();
   }

   public void PositionAtSafePlace(GameObject nearestLotGameObject)
   {      

      if (agent != null)
      {
         agent.GetComponent<Rigidbody>().velocity = Vector3.zero;
         agent.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
         agent.GetComponent<CarController>().CurrentSteeringAngle = 0f;
         agent.GetComponent<CarController>().CurrentAcceleration = 0f;
         agent.GetComponent<CarController>().CurrentBrakeTorque = 0f;


         Vector3 temp = new Vector3(agent.transform.parent.transform.position.x-2.5f , 1, nearestLotGameObject.transform.position.z+2);
         // Vector3 temp = new Vector3(-2.4f , 1, 17);

         Vector3 newPosition = temp +
                               nearestLotGameObject.transform.right +
                               nearestLotGameObject.transform.forward ;

         agent.transform.rotation = Quaternion.Euler(0,180,0);

         agent.transform.position = newPosition ;
 
      }
   }
}

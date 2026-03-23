using System;

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

using UnityEngine.AI;

public class Avatar_Controller : MonoBehaviour

{

  [SerializeField] private NavMeshAgent navMeshAgent;

  private Transform avatarTransform;

    public static event Action OnAvatarReached;



  private Vector3 targetPosition;
  private Vector3 targetCameraPosition;
  private Quaternion targetRotation;
  [SerializeField] private DataRecorder dataRecorder;
  [SerializeField] private Animator animator;


  private Vector3 targetPos;

 

  private Vector3 initalPosition;

  private Quaternion initalRotation;

  private string curentObjectID;

  private List<string> visitedData = new List<string>();



  //public

  // public ArtworkDirectory artworkDirectory;




  void Start()

  {

    avatarTransform = transform;
    targetPosition = avatarTransform.position;
    targetRotation = avatarTransform.rotation;

    initalPosition = transform.position;
    initalRotation = transform.rotation;

  }



  private void OnEnable()

  {

    GeminiController.OnTourIDsReceived += HandleTourIDs;

    // CameraController.OnCameraReached += HandleCameraReached;

  }



  private void OnDisable()

  {

    GeminiController.OnTourIDsReceived -= HandleTourIDs;

    // CameraController.OnCameraReached -= HandleCameraReached;

  }



  // // Update is called once per frame

  void Update()

  {



  }



  private void HandleTourIDs(string[] tourIDs)

  {

    // Debug.Log("Avatar Start Navigation");

    StartCoroutine(NavigationTour(tourIDs));



  }


  private IEnumerator NavigationTour(string[] tourIDs)

  {

    if (tourIDs.Length == 0 )
    {

      yield break;

    }

    // for (int i = 0; i < tourIDs.Length; i++)

    // {
    //Update target position based on tour ID

      //Find Closest Position to Avatar
      // tourIDs = FindClosesTourID(tourIDs);
      UpdateTargetAvatar(tourIDs[0]);

      //Get data for current object
      curentObjectID = tourIDs[0];
      visitedData.Add(curentObjectID);
      animator.SetBool("isWalking", true);
    navMeshAgent.SetDestination(targetPosition);
    // StartCoroutine(
    //       RotateToTarget(targetRotation));

    // Wait until the avatar reaches the painting
    while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
    {
      animator.SetBool("isWalking", true);
        
          yield return null;
      }

                  do
            {
                targetRotation = Quaternion.LookRotation(
                    new Vector3(targetCameraPosition.x - targetPosition.x, 0, targetCameraPosition.z - targetPosition.z)
                );
                avatarTransform.rotation = Quaternion.RotateTowards(avatarTransform.rotation, targetRotation, 200f * Time.deltaTime);
                yield return null;
            } while (Quaternion.Angle(avatarTransform.rotation, targetRotation) > 0.1f);
      animator.SetBool("isWalking", false);
      
      // animator.SetBool("isWalking", false);
            OnAvatarReached?.Invoke();
      
      

      yield return new WaitForSeconds(30f); // Wait for 3 seconds at the exhibit
  
  }





  private void UpdateTargetAvatar(string tourID)
  {
        string cameraID = tourID.Replace("exhibit", "camera");


    targetPosition = dataRecorder.GetPosition(tourID);
    targetCameraPosition = dataRecorder.GetPosition(cameraID);
    targetRotation = dataRecorder.GetOrientation(tourID);
    Debug.Log("Avatar moving to " + tourID + " at position " + targetPosition.ToString());

  }

  private string[] FindClosesTourID(string[] tourIDs)
  {
    Vector3 avatarPos = avatarTransform.position;
    float closestDistance = Mathf.Infinity;
    string closestTourID = tourIDs[0];

    foreach (string tourID in tourIDs)
    {
      Vector3 tourPosition = dataRecorder.GetPosition(tourID);
      float distance = Vector3.Distance(avatarPos, tourPosition);
      if (distance < closestDistance)
      {
        closestDistance = distance;
        closestTourID = tourID;
      }
    }

    // Reorder tourIDs so that closestTourID is first
    if (closestTourID != tourIDs[0])
    {
      List<string> reorderedTourIDs = new List<string>();
      reorderedTourIDs.Add(closestTourID);
      foreach (string tourID in tourIDs)
      {
        if (tourID != closestTourID)
        {
          reorderedTourIDs.Add(tourID);
        }
      }
      tourIDs = reorderedTourIDs.ToArray();
    }

    print("Order"+ tourIDs);

    return tourIDs;
  }

  private IEnumerator RotateToTarget(Quaternion targetRotation)
{
    float rotationSpeed = 5f; // Adjust this for faster/slower rotation
    
    while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f)
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            Time.deltaTime * rotationSpeed
        );
        yield return null;
    }
    
    // Snap to exact rotation at the end
    transform.rotation = targetRotation;
}

  public string GetCurrentObject()

  {

    return curentObjectID;

  }

  public List<string> GetVisitedData()

  {

    return visitedData;

  }

      public string getCurrentObjectID()
  {
    return curentObjectID;
  }

  public List<string> getVisitedData()
  {
    return visitedData;
  }

 

}
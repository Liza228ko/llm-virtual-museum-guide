using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Runtime.CompilerServices;
using Uralstech.UGemini.Models.Content;
using Unity.VisualScripting;
using Newtonsoft.Json;
using System.Collections.Generic;
using Oculus.Voice.Dictation;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.AI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GeminiController : MonoBehaviour
{
  // This is the "broadcast" that our AvatarController will listen for
  private const string BackendUrl = "http://localhost:8000/ask";

  private Vector3 targetPosition;
  private object targetRotation;

  //publics
  public static event Action<string[]> OnTourIDsReceived;

  public UIManager uiManager;

  public AppDictationExperience voiceToText;

  public OnResponseEvent OnResponse;

  // Event triggered when the camera reaches its destination
  public event Action OnCameraReached;

  public GameObject avatar;
  public Avatar_Controller avatarController;

  public List<string> visitedObjects = new List<string>();
  public string[] tourIDs;
  public string[] tours;
  public string avatarPosition;

  public NavMeshAgent cameraNavMeshAgent;
  [SerializeField] private DataRecorder dataRecorder;


  // Simple serializable position class
  [Serializable]
  public class OnResponseEvent : UnityEvent<string> { }
  public class SimplePosition
  {
    public float x;
    public float y;
    public float z;

    public SimplePosition(Vector3 vector)
    {
      x = vector.x;
      y = vector.y;
      z = vector.z;
    }
  }


  [Serializable]
  public class NavigationData
  {
    public string introduction;
    public string[] tour;
    public string[] tour_ids;
  }


  
  public void OnTextInput(string query)
  {
    if (uiManager != null)
    {
      uiManager.SetUserQuery(query);
    }
    StartCoroutine(SendRequest(query));
  }

  private IEnumerator SendRequest(string query)
  {
    if (uiManager) uiManager.SetFeedback("Sending request...");

    WWWForm form = new WWWForm();
    form.AddField("query", query ?? "");

    // Get current object ID with null safety
    string currentObjectID = avatarController?.getCurrentObjectID();
    form.AddField("currentObject", currentObjectID ?? "");

    // Get avatar position - FIXED: Use SimplePosition wrapper
    SimplePosition pos = new SimplePosition(avatar.transform.position);
    avatarPosition = JsonConvert.SerializeObject(pos); //your current position 
    form.AddField("currentAvatarPosition", avatarPosition);

    // Get visited data with null safety
    var visitedData = avatarController?.getVisitedData();
    string visitedJson = visitedData != null ? JsonConvert.SerializeObject(visitedData) : "[]";
    form.AddField("visitedObjects", visitedJson);

    using (UnityWebRequest webRequest = UnityWebRequest.Post(BackendUrl, form))
    {
      yield return webRequest.SendWebRequest();

      if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
          webRequest.result == UnityWebRequest.Result.ProtocolError)
      {
        if (uiManager) uiManager.SetFeedback("Error: " + webRequest.error);
      }
      else
      {
        string jsonResponse = webRequest.downloadHandler.text;
        Debug.Log("Received: " + jsonResponse);

        ResponseStructure response = JsonConvert.DeserializeObject<ResponseStructure>(jsonResponse);
        ProcessResponse(response);
      }
    }
  }

  private void ProcessResponse(ResponseStructure response)
  {
    string content = response.Content;
    List<string> tasks = response.Tasks;

    if (string.IsNullOrEmpty(content) || tasks == null || tasks.Count == 0)
    {
      Debug.LogWarning("Empty content or tasks in response");
      return;
    }


    if (tasks[0].Contains("navigation"))
    {
      ProcessGeminiResponse(response.Content); // This will now update the UI
    }
    else if (tasks[0].Contains("information"))
    {
      try
      {
        var infoData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
        if (infoData.ContainsKey("response"))
        {
          uiManager.SetFeedback(infoData["response"]);
          OnResponse.Invoke(infoData["response"]);

        }
        else
        {
          uiManager.SetFeedback(response.Content);
        }
      }
      catch
      {
        uiManager.SetFeedback(response.Content);
      }
    }
    else if (tasks[0].Contains("preference"))
    {
      try
      {
        var prefData = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
        if (prefData.ContainsKey("response"))
        {
          uiManager.SetFeedback(prefData["response"]);
          OnResponse.Invoke(prefData["response"]);

        }
        else
        {
          uiManager.SetFeedback(response.Content);
        }
      }
      catch
      {
        uiManager.SetFeedback(response.Content);
      }
    }
    else
    {
      uiManager.SetFeedback(response.Content);
    }
  }


  public void ProcessGeminiResponse(string response)
  {
    if (string.IsNullOrEmpty(response))
    {
      return;
    }

    // Use JsonConvert for consistency and robustness
    NavigationData navigationContent = JsonConvert.DeserializeObject<NavigationData>(response);

    Debug.Log(navigationContent.introduction);

    if (uiManager != null)
    {
      uiManager.SetFeedback(navigationContent.introduction);
      OnResponse.Invoke(navigationContent.introduction);

    }
    
    // ----------------------------------------------------
    tourIDs = navigationContent.tour_ids;
    tours = navigationContent.tour;
    OnTourIDsReceived?.Invoke(tourIDs);
    StartCoroutine(NavigationTour(tourIDs));

  }



  public void ReorderTourIDs(string[] tourIDs)
  {
    //based on avatarPosition
    //reorder positions of exhibits to visit in tourIDs
    //for this get exhibits postion 

  }

  private void UpdateTargetCamera(string tourID)
  {
    string cameraID = tourID.Replace("exhibit", "camera");


    targetPosition = dataRecorder.GetPosition(cameraID);
    targetRotation = dataRecorder.GetOrientation(cameraID);
    Debug.Log("Avatar moving to " + cameraID + " at position " + targetPosition.ToString());

  }

  private IEnumerator NavigationTour(string[] tourIDs)
  {




    // update camera target positions and orientations
    if(tourIDs.Length==0)
    {
      yield break;
    }
    UpdateTargetCamera(tourIDs[0]);
    cameraNavMeshAgent.SetDestination(targetPosition);

    visitedObjects.Add(tourIDs[0]);



    // Wait until the camera reaches the painting
    while (cameraNavMeshAgent.pathPending || cameraNavMeshAgent.remainingDistance > cameraNavMeshAgent.stoppingDistance)
    {
      yield return null;
    }


    yield return new WaitForSeconds(1f); // Wait for 3 seconds at the exhibit
    OnCameraReached?.Invoke();

    WaitForSeconds wait = new WaitForSeconds(1f);
    yield return wait;


  }

  void Start()
  {
    if (voiceToText != null)
    {
      voiceToText.DictationEvents.OnFullTranscription.AddListener(OnVoiceInput);

    }
    else
    {
      Debug.LogError("VoiceToText is missing! Please assign it in the Inspector.");
    }
    StartCoroutine(SendRequest("Introduce the virtual museum to me. Ask me to intrioduce myself to give me a more tailored tour."));

  }
  private void OnVoiceInput(string query)
  {
    // 1. Show what you said in the UI immediately
    if (uiManager != null)
    {
      uiManager.SetUserQuery(query);
    }

    // 2. Then send the request to Gemini
    StartCoroutine(SendRequest(query));
  }

  // Update is called once per frame
  void Update()
  {
    // Check if Keyboard is connected to avoid errors
    if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
    {
      voiceToText.Activate();
    }
  }

  [Serializable]
  public class ResponseStructure
  {
    [JsonProperty("content")]
    public string Content { get; set; }  // Now always a JSON string

    [JsonProperty("tasks")]
    public List<string> Tasks { get; set; }
  }
}
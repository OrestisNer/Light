using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

public class CustomVisionAnalyser: MonoBehaviour
{

    /// <summary>
    /// Unique instance of this class
    /// </summary>
    public static CustomVisionAnalyser Instance;


    /// <summary>
    /// Insert your Prediction Key here
    /// </summary>
    private string predictionKey = "f3b8398c7ce641db9c6c0514bbfc0e9d";


    /// <summary>
    /// Insert your Classification Project Id here
    /// </summary>
    private string classficationProjectId = "5c863786-eb9f-4692-8b55-35bfd06b46bd";

    /// <summary>
    /// Insert your Classification iteration
    /// </summary>
    private string publishName = "default";


    /// <summary>
    /// Insert your Classification Project Id here
    /// </summary>
    private string classificationProjectId = "5c863786-eb9f-4692-8b55-35bfd06b46bd";

    /// <summary>
    /// Object Detection Proeject ID
    /// </summary>
    private string objectDetectionProjectId = "4911c521-a26e-47e9-9fea-b353c6f30c66";

    /// <summary>
    /// Insert your prediction endpoint here
    /// </summary>
    private string predictionEndpoint = "https://light.cognitiveservices.azure.com/customvision/v3.0/Prediction/";


    /// <summary>
    /// Classification endpoint
    /// </summary>
    private string classificationEndpoint;

    /// <summary>
    /// Object Detection endpoint
    /// </summary>
    private string objectDetectionEndpoint;

    /// <summary>
    /// Byte array of the image to submit for analysis
    /// </summary>
    [HideInInspector] public byte[] imageBytes;


    /// <summary>
    /// Initialises this class
    /// </summary>
    private void Awake()
    {
        // Allows this instance to behave like a singleton
        Instance = this;
        classificationEndpoint = predictionEndpoint + classificationProjectId + "/classify/iterations/" + publishName + "/image";
        objectDetectionEndpoint = predictionEndpoint + objectDetectionProjectId + "/detect/iterations/" + publishName + "/image";
    }


    /// <summary>
    /// Call the Computer Vision Service to submit the image.
    /// </summary>
    public IEnumerator AnalyseLastImageCaptured(string imagePath, string type = "classify")
    {
        WWWForm webForm = new WWWForm();

        
        string endpoint = "";
    
        if (type == "detect")
        {
            endpoint = objectDetectionEndpoint;
        }else
        {
            endpoint = classificationEndpoint;
        }

        Debug.LogFormat("Endpoint: " + endpoint);

        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(endpoint, webForm))
        {
            // Gets a byte array out of the saved image
            imageBytes = GetImageAsByteArray(imagePath);
            unityWebRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            unityWebRequest.SetRequestHeader("Prediction-Key", predictionKey);

            // The upload handler will help uploading the byte array with the request
            unityWebRequest.uploadHandler = new UploadHandlerRaw(imageBytes);
            unityWebRequest.uploadHandler.contentType = "application/octet-stream";

            // The download handler will help receiving the analysis from Azure
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

            // Send the request
            yield return unityWebRequest.SendWebRequest();

            string jsonResponse = unityWebRequest.downloadHandler.text;

            // The response will be in JSON format, therefore it needs to be deserialized    

            // The following lines refers to a class that you will build in later Chapters
            // Wait until then to uncomment these lines
            Debug.Log("JSON RESPONSE: " + jsonResponse);

            /*
            JObject jsonObj = JObject.Parse(jsonResponse);
            AnalysisObject analysisObject = new AnalysisObject();
            analysisObject.predictions = new List<Prediction>();
            foreach (JObject item in jsonObj["predictions"].Children())
            {
                Prediction prediction = new Prediction();
                prediction.probability = Convert.ToDouble(item.GetValue("probability").ToString());
                prediction.tagName = item.GetValue("tagName").ToString();
                analysisObject.predictions.Add(prediction);
            }
        
            SceneOrganiser.Instance.SetTagsToLastLabel(analysisObject);

            */

            // Create a texture. Texture size does not matter, since
            // LoadImage will replace with the incoming image size.
            //Texture2D tex = new Texture2D(1, 1);
            //tex.LoadImage(imageBytes);
            //SceneOrganiser.Instance.quadRenderer.material.SetTexture("_MainTex", tex);

            // The response will be in JSON format, therefore it needs to be deserialized
            //AnalysisRootObject analysisRootObject = new AnalysisRootObject();
            //analysisRootObject = JsonConvert.DeserializeObject<AnalysisRootObject>(jsonResponse);

            //SceneOrganiser.Instance.FinaliseLabel(analysisRootObject);
        }
    }

    /// <summary>
    /// Returns the contents of the specified image file as a byte array.
    /// </summary>
    static byte[] GetImageAsByteArray(string imageFilePath)
    {
        FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);

        BinaryReader binaryReader = new BinaryReader(fileStream);

        return binaryReader.ReadBytes((int)fileStream.Length);
    }

}

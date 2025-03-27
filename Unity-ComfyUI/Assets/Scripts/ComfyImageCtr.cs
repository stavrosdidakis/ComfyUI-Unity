using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Linq;                // Added this line
using Newtonsoft.Json.Linq;       // Ensure you have Newtonsoft.Json installed

[System.Serializable]
public class ImageData {
    public string filename;
    public string subfolder;
    public string type;
}

[System.Serializable]
public class OutputData {
    public ImageData[] images;
}

[System.Serializable]
public class PromptData {
    public OutputData outputs;
}

public class ComfyImageCtr : MonoBehaviour {

    public Image outputImage; // UI Image to display the downloaded image

    public void RequestFileName(string id) {
        StartCoroutine(RequestFileNameRoutine(id));
    }

    IEnumerator RequestFileNameRoutine(string promptID)
    {
        string url = "http://127.0.0.1:8188/history/" + promptID;
        int retries = 3;
        for (int attempt = 1; attempt <= retries; attempt++)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string jsonResponse = webRequest.downloadHandler.text;
                    Debug.Log("History response: " + jsonResponse);

                    string filename = ExtractFilename(jsonResponse);
                    Debug.Log("Extracted filename: " + filename);

                    if (string.IsNullOrEmpty(filename))
                    {
                        Debug.LogWarning("Filename is null or empty.");
                        yield break;
                    }

                    string imageURL = $"http://127.0.0.1:8188/view?filename={Uri.EscapeDataString(filename)}";
                    Debug.Log("Constructed image URL: " + imageURL);

                    StartCoroutine(DownloadImage(imageURL));
                    yield break;
                }
                else
                {
                    Debug.LogWarning($"Attempt {attempt}/{retries} failed: {webRequest.error}");
                    if (attempt < retries)
                    {
                        yield return new WaitForSeconds(2f); // Wait before retrying
                    }
                }
            }
        }
        Debug.LogWarning("Failed to fetch history after multiple attempts.");
    }

    string ExtractFilename(string jsonString) {
        try {
            JObject root = JObject.Parse(jsonString);
            // Get the first property of the root object (the unique prompt ID)
            JProperty promptProperty = root.Properties().FirstOrDefault();
            if (promptProperty != null) {
                JObject promptData = promptProperty.Value as JObject;
                if (promptData != null && promptData["outputs"] != null) {
                    JObject outputs = promptData["outputs"] as JObject;
                    if (outputs != null) {
                        // First try to get the output node with key "140"
                        JObject outputNode = outputs["140"] as JObject;
                        if (outputNode != null) {
                            JArray images = outputNode["images"] as JArray;
                            if (images != null && images.Count > 0) {
                                JObject firstImage = images[0] as JObject;
                                string filename = firstImage["filename"]?.ToString();
                                if (!string.IsNullOrEmpty(filename))
                                    return filename;
                            }
                        }
                        // If key "140" doesn't exist, iterate over all outputs
                        foreach (var outputProperty in outputs.Properties()) {
                            JObject output = outputProperty.Value as JObject;
                            if (output != null) {
                                JArray images = output["images"] as JArray;
                                if (images != null && images.Count > 0) {
                                    JObject firstImage = images[0] as JObject;
                                    string filename = firstImage["filename"]?.ToString();
                                    if (!string.IsNullOrEmpty(filename))
                                        return filename;
                                }
                            }
                        }
                    }
                }
            }
            Debug.LogError("Filename not found in JSON response.");
            return "";
        } catch (Exception ex) {
            Debug.LogError("Error parsing JSON: " + ex.Message);
            return "";
        }
    }

    IEnumerator DownloadImage(string imageUrl) {
        yield return new WaitForSeconds(0.5f);
            
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success){
                //updateUI.saveDataJSON();

                //Image processing
                Texture2D textureComfyUI = DownloadHandlerTexture.GetContent(webRequest);
                //timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
                //fileName = timeStamp + ".png";
                //SaveTexture(textureComfyUI, fileName);     //Save for JSON and GPT analysis

            } else {
                Debug.LogError("Image download failed: " + webRequest.error);
                Debug.LogError("Response Code: " + webRequest.responseCode);
            }
        }
    }
}

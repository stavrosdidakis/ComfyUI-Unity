using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;

public class ImageDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;              // Reference to the TMP_Dropdown component
    public string directoryPath = "C:/Your/Directory/Path";  // Path to your image folder
    public RawImage rawImage;                   // Reference to the RawImage UI element

    // Fixed target dimensions for the RawImage (and for cropping/scaling)
    public int targetSize = 180;

    void Start()
    {
        // Ensure required references are assigned
        if (rawImage == null)
        {
            Debug.LogError("RawImage reference not set!");
            return;
        }
        if (dropdown == null)
        {
            Debug.LogError("Dropdown reference not set!");
            return;
        }

        // Optionally enforce fixed size via script (if not already set in the Editor)
        rawImage.rectTransform.sizeDelta = new Vector2(targetSize, targetSize);

        PopulateDropdown();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Populate the dropdown with image file names from the directory
    void PopulateDropdown()
    {
        dropdown.ClearOptions();

        if (Directory.Exists(directoryPath))
        {
            string[] imageFiles = Directory.GetFiles(directoryPath, "*.*")
                .Where(file => file.EndsWith(".jpg") || file.EndsWith(".png") || file.EndsWith(".jpeg"))
                .ToArray();

            var fileNames = new System.Collections.Generic.List<string>();
            foreach (var file in imageFiles)
            {
                fileNames.Add(Path.GetFileName(file));
            }

            dropdown.AddOptions(fileNames);
        }
        else
        {
            dropdown.AddOptions(new System.Collections.Generic.List<string> { "Directory not found!" });
        }
    }

    // Handle dropdown selection to load and display the image
    void OnDropdownValueChanged(int index)
    {
        string selectedFileName = dropdown.options[index].text;

        if (Directory.Exists(directoryPath))
        {
            string filePath = Path.Combine(directoryPath, selectedFileName);

            if (File.Exists(filePath))
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                Texture2D texture = new Texture2D(2, 2);  // Temporary texture for loading

                if (texture.LoadImage(fileData))
                {
                    // Process the loaded image: crop it to a centered square and scale to 180x180
                    Texture2D processedTexture = CropAndScaleTexture(texture, targetSize, targetSize);

                    // Assign the processed texture to the RawImage
                    rawImage.texture = processedTexture;
                    // Ensure the RawImage size remains fixed
                    rawImage.rectTransform.sizeDelta = new Vector2(targetSize, targetSize);
                }
                else
                {
                    Debug.LogError("Failed to load image: " + filePath);
                }
            }
            else
            {
                Debug.LogError("File does not exist: " + filePath);
            }
        }
        else
        {
            Debug.LogError("Directory not found: " + directoryPath);
        }
    }

    // Crop the source texture to a centered square and scale it to the target dimensions
    private Texture2D CropAndScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        // Determine the size of the square crop (use the smaller dimension)
        int cropSize = Mathf.Min(source.width, source.height);
        int cropX = (source.width - cropSize) / 2;
        int cropY = (source.height - cropSize) / 2;

        // Crop the image
        Texture2D cropped = new Texture2D(cropSize, cropSize);
        Color[] croppedPixels = source.GetPixels(cropX, cropY, cropSize, cropSize);
        cropped.SetPixels(croppedPixels);
        cropped.Apply();

        // Create a new texture for the scaled (final) image
        Texture2D scaled = new Texture2D(targetWidth, targetHeight, cropped.format, false);

        // Scale the cropped texture using bilinear sampling
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                // Calculate normalized coordinates (0 to 1)
                float u = x / (float)targetWidth;
                float v = y / (float)targetHeight;
                Color newColor = cropped.GetPixelBilinear(u, v);
                scaled.SetPixel(x, y, newColor);
            }
        }
        scaled.Apply();
        return scaled;
    }
}

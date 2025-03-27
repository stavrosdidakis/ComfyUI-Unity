using UnityEngine;
using UnityEngine.UI;  // For standard UI
using TMPro;           // For TextMeshPro (if you're using TMP)
using System.IO;      // For file directory and file manipulation

public class DirectoryDropdown : MonoBehaviour
{
    // Public fields to assign via Inspector
    public TMP_Dropdown dropdown;  // Reference to the TMP_Dropdown component (or Dropdown if using standard UI)
    public string directoryPath = "C:/Your/Directory/Path";  // Directory path where files are located

    void Start()
    {
        // Get the files and populate the dropdown when the scene starts
        PopulateDropdown();
    }

    // Method to populate the dropdown with file names from the directory
    void PopulateDropdown()
    {
        // Clear any existing options in the dropdown
        dropdown.ClearOptions();

        // Check if the directory exists
        if (Directory.Exists(directoryPath))
        {
            // Get all files from the directory
            string[] files = Directory.GetFiles(directoryPath);

            // List to hold the file names
            var fileNames = new System.Collections.Generic.List<string>();

            // Loop through all files and add only file names (without the path)
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                fileNames.Add(fileName);
            }

            // Add the file names as options to the dropdown
            dropdown.AddOptions(fileNames);
        }
        else
        {
            // If the directory does not exist, show a warning in the dropdown
            dropdown.AddOptions(new System.Collections.Generic.List<string> { "Directory not found!" });
        }
    }
}

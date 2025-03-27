using UnityEngine;
using TMPro;  // Import the TextMeshPro namespace

public class NumberInput : MonoBehaviour
{
    public TMP_InputField inputField;  // Reference to the TMP_InputField
    public enum InputType { Integer, Float }; // Enum to choose between Integer or Float
    public InputType inputType = InputType.Integer; // Default to Integer
    public int minIntValue = 0; // Minimum value for integer input
    public int maxIntValue = 2; // Maximum value for integer input
    public float minFloatValue = 0f; // Minimum value for float input
    public float maxFloatValue = 2f; // Maximum value for float input

    void Start()
    {
        // Add listener to the input field to check for changes
        inputField.onValueChanged.AddListener(OnInputChanged);
    }

    // Called whenever the input field value changes
    void OnInputChanged(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            if (inputType == InputType.Integer)
            {
                // Check if the input is a valid integer and within the allowed range
                if (int.TryParse(input, out int result))
                {
                    if (result < minIntValue || result > maxIntValue)
                    {
                        inputField.text = minIntValue.ToString();  // Reset to the minimum integer value
                    }
                }
                else
                {
                    // Reset to the minimum integer value if it's not valid
                    inputField.text = minIntValue.ToString();
                }
            }
            else if (inputType == InputType.Float)
            {
                // Check if the input is a valid float and within the allowed range
                if (float.TryParse(input, out float result))
                {
                    if (result < minFloatValue || result > maxFloatValue)
                    {
                        inputField.text = minFloatValue.ToString();  // Reset to the minimum float value
                    }
                }
                else
                {
                    // Reset to the minimum float value if it's not valid
                    inputField.text = minFloatValue.ToString();
                }
            }
        }
        else
        {
            // Reset to the minimum value if input is empty
            if (inputType == InputType.Integer)
            {
                inputField.text = minIntValue.ToString();
            }
            else if (inputType == InputType.Float)
            {
                inputField.text = minFloatValue.ToString();
            }
        }
    }
}

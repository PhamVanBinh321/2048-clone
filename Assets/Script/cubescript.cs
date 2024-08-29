using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class cubescript : MonoBehaviour
{
    public int value; // Giá trị của khối
    public Text text; // Text component để hiển thị giá trị
    public int dahopnhat = 0;
    public Image blockImage; // Image component for coloring the block

    // Define a color mapping based on values
    private static readonly Dictionary<int, Color> valueToColor = new Dictionary<int, Color>
    {
        { 2, new Color(0.8f, 0.8f, 0.8f) }, // Light gray
        { 4, new Color(0.9f, 0.9f, 0.6f) }, // Light yellow
        { 8, new Color(0.9f, 0.7f, 0.5f) }, // Light orange
        { 16, new Color(0.9f, 0.5f, 0.3f) }, // Orange
        { 32, new Color(0.9f, 0.3f, 0.3f) }, // Red
        // Add more mappings as needed
        { 64, new Color(0.6f, 0.3f, 0.1f) }, // Darker red
        { 128, new Color(0.3f, 0.5f, 0.3f) }, // Green
        { 256, new Color(0.2f, 0.4f, 0.8f) }, // Blue
        { 512, new Color(0.1f, 0.2f, 0.6f) }, // Dark blue
        { 1024, new Color(0.5f, 0.2f, 0.8f) }, // Purple
        { 2048, new Color(0.9f, 0.8f, 0.2f) }  // Yellow
    };

    void Start()
    {
        // Khởi tạo hoặc cập nhật Text với giá trị ban đầu
        UpdateText();
    }

    public void SetValue(int newValue)
    {
        value = newValue;
        UpdateText();
        UpdateColor();
    }

    private void UpdateText()
    {
        if (text != null)
        {
            text.text = value.ToString();
        }
        else
        {
            Debug.Log("Text component is not assigned.");
        }
    }

    private void UpdateColor()
    {
        if (blockImage != null)
        {
            if (valueToColor.TryGetValue(value, out Color color))
            {
                blockImage.color = color;
            }
            else
            {
                blockImage.color = Color.white; // Default color if value not in dictionary
            }
        }
        else
        {
            Debug.Log("Image component is not assigned.");
        }
    }
}

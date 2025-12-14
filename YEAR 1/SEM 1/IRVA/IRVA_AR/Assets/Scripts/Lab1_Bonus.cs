using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
public class Lab1_Bonus : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public ARPointCloudManager pointCloudManager;
    public Camera arCamera; 
    public TextMeshProUGUI text; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (planeManager == null || pointCloudManager == null || arCamera == null || text == null)
        {
            return;
        }

        int planeCount = planeManager.trackables.count;

        int totalPointCount = 0;
        foreach (ARPointCloud cloud in pointCloudManager.trackables)
        {
            if (cloud.positions.HasValue)
            {
                totalPointCount += cloud.positions.Value.Length;
            }
        }

        Vector3 camPosition = arCamera.transform.position;

        Vector3 camRotation = arCamera.transform.eulerAngles;

        text.text =
            $"Nr Plane: {planeCount}\n" +
            $"Puncte caracteristice: {totalPointCount}\n" +
            $"Pozitie Camera : {camPosition.ToString("F2")}\n" +
            $"Orientare Camera : {camRotation.ToString("F1")}";

    }
}

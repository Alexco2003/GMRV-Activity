using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
public class Lab4_Bonus : MonoBehaviour
{

    [SerializeField]
    private Button nextButton;

    private ARFaceManager faceManager;

    private int currentIndex = 0;

    [SerializeField]
    public Material[] faceMaterials;

    void Awake()
    {
        faceManager = GetComponent<ARFaceManager>();
        nextButton.onClick.AddListener(SwapFaces);

        faceManager.facePrefab.GetComponent<MeshRenderer>().material = faceMaterials[currentIndex];
    }

    void SwapFaces()
    {
        currentIndex = (currentIndex + 1) % faceMaterials.Length;
        faceManager.facePrefab.GetComponent<MeshRenderer>().material = faceMaterials[currentIndex];

        foreach (ARFace face in faceManager.trackables)
        {
            face.GetComponent<MeshRenderer>().material = faceMaterials[currentIndex];
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

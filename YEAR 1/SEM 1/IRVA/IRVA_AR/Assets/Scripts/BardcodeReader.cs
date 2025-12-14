using TMPro;
using UnityEngine;
using Vuforia;

public class BarcodeReader : MonoBehaviour
{
    BarcodeBehaviour mBarcodeBehaviour;

    public TextMeshProUGUI text;

    void Start()
    {
        mBarcodeBehaviour = GetComponent<BarcodeBehaviour>();
    }

    void Update()
    {
        if (mBarcodeBehaviour != null && mBarcodeBehaviour.InstanceData != null)
        {
            Debug.Log(mBarcodeBehaviour.InstanceData.Text);
            text.text = mBarcodeBehaviour.InstanceData.Text;
        }
    }
}
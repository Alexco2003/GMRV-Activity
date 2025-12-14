using UnityEngine;
using UnityEngine.UI;
using static L4_VR_MetaXR_Basics.Scripts.Utils;
using TMPro;


namespace L4_VR_MetaXR_Basics.Scripts
{
    public class Bonus : MonoBehaviour
    {
        [SerializeField]
        private GunFireController gunFireController;

        [SerializeField]
        private TextMeshPro textBonus;

        [SerializeField]
        private Color semiAutomaticColor = Color.yellow;

        [SerializeField]
        private Color automaticColor = Color.green;

        private void Start()
        {
            if (gunFireController != null)
            {
                gunFireController.OnFireModeChanged += UpdateLabel;
                UpdateLabel(gunFireController.CurrentFireMode);
            }
        }

        private void OnDestroy()
        {
            if (gunFireController != null)
            {
                gunFireController.OnFireModeChanged -= UpdateLabel;
            }
        }

        public void OnPoke()
        {
            if (gunFireController == null) return;
            gunFireController.ToggleFireMode();
        }

        private void UpdateLabel(GunFireMode mode)
        {
            var text = mode == GunFireMode.Automatic ? "AUTO" : "SEMI";
            var color = mode == GunFireMode.Automatic ? automaticColor : semiAutomaticColor;


            if (textBonus != null)
            {
                textBonus.text = text;
                textBonus.color = color;
            }
        }
    }
}
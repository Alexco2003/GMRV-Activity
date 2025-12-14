
using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using static L4_VR_MetaXR_Basics.Scripts.Utils;

namespace L4_VR_MetaXR_Basics.Scripts
{
    /// <summary>
    /// Script used to control gun firing based on trigger presses.
    /// </summary>
    public class GunFireController : MonoBehaviour
    {
        [SerializeField]
        [BoxGroup("Gun components")]
        private GunFingerCurlController gunFingerCurlController;

        [SerializeField]
        [BoxGroup("Settings")]
        private GunFireMode gunFireMode = GunFireMode.SemiAutomatic;

        [SerializeField]
        [BoxGroup("Settings")]
        [Min(0.001f)]
        [InfoBox("Shots per second used in Automatic mode.")]
        private float automaticFireRate = 10f;

        // In semi-automatic mode, this flag ensures the gun can only fire once per trigger press.
        private bool _canFire = true;

        private Coroutine _autoFireCoroutine;

        public event Action OnGunFired;
        public event Action<GunFireMode> OnFireModeChanged;

        public GunFireMode CurrentFireMode => gunFireMode;

        private void OnEnable()
        {
            gunFingerCurlController.OnTriggerPressed += HandleTriggerPressed;
            gunFingerCurlController.OnTriggerReleased += HandleTriggerReleased;
        }

        private void OnDisable()
        {
            gunFingerCurlController.OnTriggerPressed -= HandleTriggerPressed;
            gunFingerCurlController.OnTriggerReleased -= HandleTriggerReleased;
        }

        private void HandleTriggerPressed()
        {
            if (gunFireMode == GunFireMode.SemiAutomatic)
            {
                if (_canFire)
                {
                    FireGun();
                    _canFire = false;
                }
            }
            else if (gunFireMode == GunFireMode.Automatic)
            {
                StartAutomaticFire();
            }
        }

        private void HandleTriggerReleased()
        {
            if (gunFireMode == GunFireMode.SemiAutomatic)
            {
                _canFire = true;
            }
            else if (gunFireMode == GunFireMode.Automatic)
            {
                StopAutomaticFire();
            }
        }

        private void FireGun()
        {
            Debug.Log("[GunFireController] Gun fired!");
            OnGunFired?.Invoke();
        }

        private void StartAutomaticFire()
        {
            if (_autoFireCoroutine == null)
            {
                _autoFireCoroutine = StartCoroutine(AutomaticFireRoutine());
            }
        }

        private void StopAutomaticFire()
        {
            if (_autoFireCoroutine != null)
            {
                StopCoroutine(_autoFireCoroutine);
                _autoFireCoroutine = null;
            }
        }

        private IEnumerator AutomaticFireRoutine()
        {
            var interval = 1f / Mathf.Max(automaticFireRate, 0.0001f);
            while (true)
            {
                FireGun();
                yield return new WaitForSeconds(interval);
            }
        }

        public void SetFireMode(GunFireMode mode)
        {
            if (gunFireMode == mode) return;

            if (gunFireMode == GunFireMode.Automatic)
            {
                StopAutomaticFire();
            }

            gunFireMode = mode;
            OnFireModeChanged?.Invoke(mode);
        }

        public void ToggleFireMode()
        {
            var newMode = gunFireMode == GunFireMode.SemiAutomatic ? GunFireMode.Automatic : GunFireMode.SemiAutomatic;
            SetFireMode(newMode);
        }
    }
}
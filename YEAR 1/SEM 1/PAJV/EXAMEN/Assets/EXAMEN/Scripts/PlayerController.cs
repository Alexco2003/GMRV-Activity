using UnityEngine;
using DarkRift;
using DarkRift.Client.Unity;
using System.Collections;

namespace PAJV
{
    public class PlayerController : MonoBehaviour
    {
        private UnityClient client;

        [Header("Movement")]
        private float moveSpeed = 5f;
        private float rotSpeed = 150f;

        [Header("Combat")]
        private float dashSpeed = 20f;
        private float dashDuration = 0.25f;
        private bool isDashing = false;
        private bool canDash = true;

        [Header("Network Info")]
        public ushort NetworkId;
        public bool IsLocal = false;

        private Rigidbody rb;
        private Vector3 lastSentPos;
        private float lastSentRot;

        public bool InputDisabled = false;

        public void Initialize(UnityClient c, ushort netId, bool isLocal)
        {
            client = c;
            NetworkId = netId;
            IsLocal = isLocal;

            rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

     
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            rb.mass = 50f;
            rb.linearDamping = 5f;
        }

        void Update()
        {
            if (!IsLocal || InputDisabled) return;

            if (!isDashing)
            {
                float move = 0;
                if (Input.GetKey(KeyCode.W)) move = 1;
                if (Input.GetKey(KeyCode.S)) move = -1;

                float rotate = 0;
                if (Input.GetKey(KeyCode.E)) rotate = 1;
                if (Input.GetKey(KeyCode.Q)) rotate = -1;

 
                transform.Rotate(Vector3.up * rotate * rotSpeed * Time.deltaTime);

                Vector3 forwardVel = transform.forward * move * moveSpeed;

                rb.linearVelocity = new Vector3(forwardVel.x, rb.linearVelocity.y, forwardVel.z);
            }


            if (Input.GetKeyDown(KeyCode.Space) && canDash && !isDashing)
            {
                StartCoroutine(PerformDash());
            }


            if (Vector3.Distance(transform.position, lastSentPos) > 0.05f ||
                Mathf.Abs(transform.eulerAngles.y - lastSentRot) > 1f)
            {
                SendMovement();
                lastSentPos = transform.position;
                lastSentRot = transform.eulerAngles.y;
            }
        }

        IEnumerator PerformDash()
        {
            isDashing = true;
            canDash = false;

            rb.linearVelocity = transform.forward * dashSpeed;

            float timer = 0;
            while (timer < dashDuration && isDashing)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            StopDash();

            yield return new WaitForSeconds(1.0f);
            canDash = true;
        }

        void StopDash()
        {
            if (!isDashing) return;

            isDashing = false;

            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!IsLocal) return;

            if (isDashing)
            {
           
                StopDash();

                if (collision.gameObject.CompareTag("Player"))
                {
                    PlayerController target = collision.gameObject.GetComponent<PlayerController>();
                    if (target != null)
                    {
                        Debug.Log($"LOVITURA! ID-ul celui lovit: {target.NetworkId}");
                        SendHit(target.NetworkId);
                    }
                }
            }
        }

        void SendHit(ushort targetId)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(targetId);
                using (Message msg = Message.Create(5, writer))
                {
                    client.SendMessage(msg, SendMode.Reliable);
                }
            }
        }

        void SendMovement()
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.Write(transform.position.x);
                writer.Write(transform.position.y);
                writer.Write(transform.position.z);
                writer.Write(transform.eulerAngles.y);

                using (Message msg = Message.Create(1, writer))
                {
                    client.SendMessage(msg, SendMode.Unreliable);
                }
            }
        }
    }
}
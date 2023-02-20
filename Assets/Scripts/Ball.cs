using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace StormDreams
{
    public class Ball : NetworkBehaviour
    {
        [field: SyncVar]
        public bool CanInteract { get; private set; } = true;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void ApplyForce(Vector3 force)
        {
            _rigidbody.AddForce(force, ForceMode.VelocityChange);
        }

        public void OpenState()
        {
            CanInteract = true;
        }

        public void LockState()
        {
            CanInteract = false;
        }

        public void ResetState()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}

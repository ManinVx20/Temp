using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    [CreateAssetMenu]
    public class DashSkillSO : SkillSO
    {
        [SerializeField]
        private float _dashSpeed;

        public override void Activate(GameObject parent, Vector3 direction, float delta)
        {
            Rigidbody rigidbody = parent.GetComponent<Rigidbody>();

            rigidbody.velocity = Vector3.zero;

            Vector3 newVelocity = direction * _dashSpeed;

            rigidbody.AddForce(newVelocity, ForceMode.VelocityChange);
        }
    }
}

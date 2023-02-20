using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    [CreateAssetMenu]
    public class FlashSkillSO : SkillSO
    {
        [SerializeField]
        private float _flashDistance;
        [SerializeField]
        private float _wallOffset;
        [SerializeField]
        private Vector2 _fieldUpLeftCorner;
        [SerializeField]
        private Vector2 _fieldDownRightCorner;

        public override void Activate(GameObject parent, Vector3 direction, float delta)
        {
            // Rigidbody rigidbody = parent.GetComponent<Rigidbody>();

            Vector3 newPosition = parent.transform.position + direction * _flashDistance;

            if (newPosition.x < _fieldUpLeftCorner.x + _wallOffset)
            {
                newPosition.x = _fieldUpLeftCorner.x + _wallOffset;
            }
            else if (newPosition.x > _fieldDownRightCorner.x - _wallOffset)
            {
                newPosition.x = _fieldDownRightCorner.x - _wallOffset;
            }

            if (newPosition.z > _fieldUpLeftCorner.y - _wallOffset)
            {
                newPosition.z = _fieldUpLeftCorner.y - _wallOffset;
            }
            else if (newPosition.z < _fieldDownRightCorner.y + _wallOffset)
            {
                newPosition.z = _fieldDownRightCorner.y + _wallOffset;
            }

            parent.transform.position = newPosition;
        }
    }
}

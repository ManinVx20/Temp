using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    public class SkillHolder : MonoBehaviour
    {
        public enum SkillState
        {
            Ready,
            Active,
            Cooldown
        }

        [SerializeField]
        private SkillSO _skillSO;

        private float _cooldownTimer;
        private float _activeTimer;
        private SkillState _state = SkillState.Ready;

        public bool IsActive() => _activeTimer > 0.0f;

        public void UpdateSkillState(bool active, GameObject parent, Vector3 direction, float delta, bool replaying)
        {
            switch (_state)
            {
                case SkillState.Ready:
                    if (active)
                    {
                        _skillSO.Activate(parent, direction, delta);
                        _state = SkillState.Active;
                        _activeTimer = _skillSO.ActiveTime;
                    }
                    break;
                case SkillState.Active:
                    if (_activeTimer > 0.0f)
                    {
                        if (!replaying)
                        {
                            _activeTimer -= delta;
                        }
                    }
                    else
                    {
                        _state = SkillState.Cooldown;
                        _cooldownTimer = _skillSO.CooldownTime;
                    }
                    break;
                case SkillState.Cooldown:
                    if (_cooldownTimer > 0.0f)
                    {
                        if (!replaying)
                        {
                            _cooldownTimer -= delta;
                        }
                    }
                    else
                    {
                        _state = SkillState.Ready;
                    }
                    break;
            }
        }
    }
}

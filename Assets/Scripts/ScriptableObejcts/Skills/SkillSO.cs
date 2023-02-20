using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    public abstract class SkillSO : ScriptableObject
    {
        public Sprite Sprite;
        public string Name;
        public float CooldownTime;
        public float ActiveTime;

        public abstract void Activate(GameObject parent, Vector3 direction, float delta);
    }
}

using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;

namespace StormDreams
{
    public class Goal : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 2)]
        private int _team;

        private void OnTriggerEnter(Collider other)
        {
            if (InstanceFinder.IsServer)
            {
                if (other.gameObject.TryGetComponent<Ball>(out Ball ball))
                {
                    if (ball.CanInteract)
                    {
                        int teamGoal = _team == 1 ? 2 : 1;

                        MatchManager.Instance.HandleGoal(teamGoal);
                    }
                }
            }
        }
    }
}

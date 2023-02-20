using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace StormDreams
{
    public class MatchManager : NetworkBehaviour
    {
        public static MatchManager Instance { get; private set; }

        [SerializeField]
        private GameObject _ballPrefab;
        [SerializeField]
        private float _matchTime;

        [SyncObject]
        private readonly SyncTimer _matchTimer = new SyncTimer();
        [SyncVar(OnChange = nameof(OnTeamOneGoalChange))]
        private int _teamOneGoal;
        [SyncVar(OnChange = nameof(OnTeamTwoGoalChange))]
        private int _teamTwoGoal;

        private Ball _currentBall;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            if (IsServer)
            {
                Instance = this;

                _matchTimer.OnChange += OnMatchTimerChange;
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            Invoke(nameof(StartMatch), 5.0f);
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();

            if (IsServer)
            {
                _matchTimer.OnChange -= OnMatchTimerChange;
            }
        }

        private void Update()
        {
            if (InstanceFinder.IsServer && Input.GetKeyDown(KeyCode.P))
            {
                StartMatch();
            }

            _matchTimer.Update(Time.deltaTime);
        }

        public void StartMatch()
        {
            GameObject ballGameObject = Instantiate(_ballPrefab);
            InstanceFinder.ServerManager.Spawn(ballGameObject);

            _currentBall = ballGameObject.GetComponent<Ball>();
            _currentBall.transform.position = Vector3.up;

            _matchTimer.StartTimer(_matchTime, true);
        }

        public void HandleGoal(int team)
        {
            _currentBall.LockState();

            if (team == 1)
            {
                _teamOneGoal += 1;
            }
            else
            {
                _teamTwoGoal += 1;
            }

            Debug.Log($"{_teamOneGoal} - {_teamTwoGoal}");

            Invoke(nameof(ResetMatch), 2.0f);
        }

        private void ResetMatch()
        {
            _currentBall.ResetState();

            _currentBall.transform.position = Vector3.up;

            _currentBall.OpenState();
        }

        private void OnMatchTimerChange(SyncTimerOperation op, float prev, float next, bool asServer)
        {
            if (!asServer)
            {
                if (op == SyncTimerOperation.Finished)
                {
                    Debug.Log($"Finish match: {_teamOneGoal} - {_teamTwoGoal}");
                }
            }
        }

        private void OnTeamOneGoalChange(int prev, int next, bool asServer)
        {
            if (!asServer)
            {

            }
        }

        private void OnTeamTwoGoalChange(int prev, int next, bool asServer)
        {
            if (!asServer)
            {
                
            }
        }
    }
}

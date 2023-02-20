using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using UnityEngine;

namespace StormDreams
{
    public class PlayerMovement : NetworkBehaviour
    {
        public struct MoveData : IReplicateData
        {
            public float Horizontal;
            public float Vertical;
            public bool Jump;
            public bool SkillOne;
            public bool SkillTwo;

            public MoveData(float horizontal, float vertical, bool jump, bool skillOne, bool skillTwo)
            {
                Horizontal = horizontal;
                Vertical = vertical;
                Jump = jump;
                SkillOne = skillOne;
                SkillTwo = skillTwo;
                _tick = 0;
            }

            private uint _tick;
            public void Dispose() { }
            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }

        public struct ReconcileData : IReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;

            public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
            {
                Position = position;
                Rotation = rotation;
                Velocity = velocity;
                AngularVelocity = angularVelocity;
                _tick = 0;
            }

            private uint _tick;
            public void Dispose() { }
            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }

        [SerializeField]
        private float _moveSpeed;
        [SerializeField]
        private float _turnSpeed;

        [SerializeField]
        private bool _isGrounded;
        [SerializeField]
        private float _groundCheckRadius;
        [SerializeField]
        private float _groundCheckMaxDistance;
        [SerializeField]
        private LayerMask _groundLayerMask;
        [SerializeField]
        private float _jumpForce;
        [SerializeField]
        private float _pushForce;

        [SerializeField]
        private SkillHolder _skillOneHolder;
        [SerializeField]
        private SkillHolder _skillTwoHolder;

        private Rigidbody _rigidbody;
        private PlayerInput _playerInput;
        private PlayerAnimator _playerAnimator;

        private bool _jumpQueued;
        private bool _skillOneQueued;
        private bool _skillTwoQueued;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();
            _playerAnimator = GetComponentInChildren<PlayerAnimator>();

            InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
            InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;
        }

        private void OnDestroy()
        {
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
                InstanceFinder.TimeManager.OnPostTick -= TimeManager_OnPostTick;
            }
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            _jumpQueued |= _playerInput.JumpInput;
            _skillOneQueued |= _playerInput.SkillOneInput;
            _skillTwoQueued |= _playerInput.SkillTwoInput;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (IsServer)
            {
                if (other.gameObject.TryGetComponent<Ball>(out Ball ball))
                {
                    if (ball.CanInteract)
                    {
                        Vector3 direction = (ball.transform.position - other.GetContact(0).point).normalized;
                        Vector3 force = direction * _pushForce;

                        ball.ApplyForce(force);
                    }
                }
            }
        }

        private void TimeManager_OnTick()
        {
            if (IsOwner)
            {
                BuildActions(out MoveData moveData);
                Move(moveData, false);
            }

            if (IsServer)
            {
                Move(default, true);
            }
        }

        private void TimeManager_OnPostTick()
        {
            if (IsOwner)
            {
                Reconcile(default, false);
            }

            if (IsServer)
            {
                ReconcileData reconcileData = new ReconcileData(
                    transform.position, transform.rotation, _rigidbody.velocity, _rigidbody.angularVelocity);
                Reconcile(reconcileData, true);
            }
        }

        private void BuildActions(out MoveData moveData)
        {
            moveData = default;

            moveData = new MoveData(
                _playerInput.MovementInput.x, _playerInput.MovementInput.y, _jumpQueued, _skillOneQueued, _skillTwoQueued);

            _jumpQueued = false;
            _skillOneQueued = false;
            _skillTwoQueued = false;
        }

        private bool CheckGrounded()
        {
            Vector3 origin = transform.position + Vector3.up;

            if (Physics.SphereCast(
                origin, _groundCheckRadius, Vector3.down, out RaycastHit hit, _groundCheckMaxDistance, _groundLayerMask))
            {
                return true;
            }

            return false;
        }

        [Replicate]
        private void Move(MoveData moveData, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
        {
            float delta = (float)InstanceFinder.TimeManager.TickDelta;

            _isGrounded = CheckGrounded();

            _playerAnimator.SetGrounded(_isGrounded);

            Vector3 direction = new Vector3(moveData.Horizontal, 0.0f, moveData.Vertical).normalized;

            _playerAnimator.SetMoving(direction.sqrMagnitude > 0.0f);

            _skillOneHolder?.UpdateSkillState(moveData.SkillOne, gameObject, direction, delta, replaying);
            _skillTwoHolder?.UpdateSkillState(moveData.SkillTwo, gameObject, direction, delta, replaying);

            if (_skillOneHolder.IsActive() || _skillTwoHolder.IsActive())
            {
                return;
            }

            if (direction.sqrMagnitude > 0.0f)
            {
                Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, _turnSpeed * delta);
            }

            if (moveData.Jump && _isGrounded)
            {
                if (!replaying)
                {
                    _playerAnimator.SetJump();
                }

                _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
            }

            Vector3 extraGravityForce = Vector3.up * Physics.gravity.y * _moveSpeed;
            _rigidbody.AddForce(extraGravityForce, ForceMode.Acceleration);

            Vector3 newVelocity = direction * _moveSpeed;
            newVelocity.y = _rigidbody.velocity.y;

            _rigidbody.velocity = newVelocity;
        }

        [Reconcile]
        private void Reconcile(ReconcileData reconcileData, bool asServer, Channel channel = Channel.Unreliable)
        {
            transform.position = reconcileData.Position;
            transform.rotation = reconcileData.Rotation;
            _rigidbody.velocity = reconcileData.Velocity;
            _rigidbody.angularVelocity = reconcileData.AngularVelocity;
        }
    }
}

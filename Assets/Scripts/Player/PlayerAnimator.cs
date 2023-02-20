using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;

namespace StormDreams
{
    public class PlayerAnimator : NetworkBehaviour
    {
        private Animator _animator;

        private int _isGroundedHash;
        private int _isMovingHash;
        private int _jumpHash;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _isGroundedHash = Animator.StringToHash("IsGrounded");
            _isMovingHash = Animator.StringToHash("IsMoving");
            _jumpHash = Animator.StringToHash("Jump");
        }

        public void SetGrounded(bool value)
        {
            _animator.SetBool(_isGroundedHash, value);
        }

        public void SetMoving(bool value)
        {
            _animator.SetBool(_isMovingHash, value);
        }

        public void SetJump()
        {
            _animator.SetTrigger(_jumpHash);
        }
    }
}

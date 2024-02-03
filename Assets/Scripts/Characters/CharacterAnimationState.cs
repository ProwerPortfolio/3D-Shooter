// Created and owned by Sankoh_Tew. Hi, dataminers! ;)

#region Usings

using UnityEngine;

#endregion

namespace Shooter3D
{
    /// <summary>
    /// ��������� ���������� ���������.
    /// </summary>
    public class CharacterAnimationState : MonoBehaviour
    {
        #region Parameters

        private const float inputControlLerpRate = 10f;

        /// <summary>
        /// ������ �� CharacterController.
        /// </summary>
        [SerializeField] private CharacterController characterController;

        /// <summary>
        /// ������ �� Animator.
        /// </summary>
        [SerializeField] private Animator animator;

        /// <summary>
        /// ������ �� CharacterMove.
        /// </summary>
        [SerializeField] private CharacterMove characterMove;

        private Vector3 inputControl;

        #endregion

        #region API



        #region Unity API

        private void LateUpdate()
        {
            Vector3 movementSpeed = transform.InverseTransformDirection(characterController.velocity);

            inputControl = Vector3.MoveTowards(inputControl, characterMove.targetDirectionControl, Time.deltaTime * inputControlLerpRate);

            animator.SetFloat("Normalize Movement X", inputControl.x);
            animator.SetFloat("Normalize Movement Z", inputControl.z);

            animator.SetBool("Is Sprint", characterMove.IsSprint);
            animator.SetBool("Is Crouch", characterMove.IsCrouch);
            animator.SetBool("Is Aiming", characterMove.IsAiming);
            animator.SetBool("Is Ground", characterController.isGrounded);

            if (!characterController.isGrounded)
            {
                animator.SetFloat("Jump", movementSpeed.y);
            }

            Vector3 groundSpeed = characterController.velocity;
            groundSpeed.y = 0;
            animator.SetFloat("Ground Speed", groundSpeed.magnitude);

            animator.SetFloat("Distance To Ground", characterMove.DistanceToGround);
        }

        #endregion

        #region Public API



        #endregion

        #endregion
    }
}
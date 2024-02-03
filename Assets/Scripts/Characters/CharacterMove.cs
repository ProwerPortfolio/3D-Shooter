// Created and owned by Sankoh_Tew. Hi, dataminers! ;)

#region Usings

using Unity.VisualScripting;
using UnityEngine;

#endregion

namespace Shooter3D
{
    /// <summary>
    /// ����������� ���������.
    /// </summary>
    public class CharacterMove : MonoBehaviour
    {
        #region Parameters

        /// <summary>
        /// ������ �� CharacterController.
        /// </summary>
        [SerializeField] private CharacterController controller;

        /// <summary>
        /// �������� ��������� �� ����� ���� � ���������.
        /// </summary>
        [Header("Movement")]
        [SerializeField] private float rifleRunSpeed;

        /// <summary>
        /// �������� ��������� �� ����� ������� � ���������.
        /// </summary>
        [SerializeField] private float rifleSprintSpeed;

        /// <summary>
        /// �������� ��������� �� ����� ������ � ������������ � ���������.
        /// </summary>
        [SerializeField] private float aimingWalkSpeed;

        /// <summary>
        /// �������� ��������� �� ����� ���� � ������������ � ���������.
        /// </summary>
        [SerializeField] private float aimingRunSpeed;

        [SerializeField] private float accelerationRate;

        /// <summary>
        /// �������� ��������� � �������.
        /// </summary>
        [SerializeField] private float CrouchSpeed;

        /// <summary>
        /// �������� ������ ���������.
        /// </summary>
        [SerializeField] private float jumpSpeed;

        /// <summary>
        /// ������ ���������, ������������ � �������.
        /// </summary>
        [Header("State")]
        [SerializeField] private float crouchHeight;

        /// <summary>
        /// ������� �� ������ ��������?
        /// </summary>
        private bool isAiming;
        /// <summary>
        /// ������� �� ������ ��������?
        /// </summary>
        private bool isJump;
        /// <summary>
        /// � ������� �� ������ ��������?
        /// </summary>
        private bool isCrouch;
        /// <summary>
        /// ��������� �� ������ ��������?
        /// </summary>
        private bool isSprint;
        /// <summary>
        /// ��������� �� �����������.
        /// </summary>
        private float distanceToGround;

        [HideInInspector] public Vector3 targetDirectionControl;

        private float BaseCharacterHeight;
        private float BaseCharacterHeightOffset;

        private Vector3 directionControl;
        private Vector3 movementDirection;

        #endregion

        #region API

        /// <summary>
        /// ������������ ���������.
        /// </summary>
        private void Move()
        {
            directionControl = Vector3.MoveTowards(directionControl, targetDirectionControl, Time.deltaTime * accelerationRate);

            if (controller.isGrounded)
            {
                movementDirection = directionControl * GetCurrentSpeedByState();

                if (isJump)
                {
                    movementDirection.y = jumpSpeed;
                    isJump = false;
                }

                movementDirection = transform.TransformDirection(movementDirection);
            }

            movementDirection += Physics.gravity * Time.deltaTime;

            controller.Move(movementDirection * Time.deltaTime);
        }

        /// <summary>
        /// ��������� �������� ���������� �� �����������.
        /// </summary>
        private void UpdateDistanceToGround()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1000))
            {
                distanceToGround = Vector3.Distance(transform.position, hit.point);
            }
        }

        #region Unity API

        private void Start()
        {
            BaseCharacterHeight = controller.height;
            BaseCharacterHeightOffset = controller.center.y;
        }

        private void Update()
        {
            Move();
            UpdateDistanceToGround();
        }

        #endregion

        #region Public API

        public float JumpSpeed => jumpSpeed;

        public float CrouchHeight => crouchHeight;

        public bool IsCrouch => isCrouch;

        public bool IsSprint => isSprint;

        public bool IsAiming => isAiming;

        public float DistanceToGround => distanceToGround;

        /// <summary>
        /// ������ ���������.
        /// </summary>
        public void Jump()
        {
            if (!controller.isGrounded) return;

            isJump = true;
        }

        /// <summary>
        /// ���������� ���������.
        /// </summary>
        public void Crouch()
        {
            if (!controller.isGrounded) return;
            if (isSprint) return;

            isCrouch = true;
            controller.height = crouchHeight;
            controller.center = new Vector3(0, controller.center.y / 2, 0);
        }

        /// <summary>
        /// ����� ��������� �� ��������� �������.
        /// </summary>
        public void UnCrouch()
        {
            isCrouch = false;

            controller.height = BaseCharacterHeight;
            controller.center = new Vector3(0, BaseCharacterHeightOffset, 0);
        }

        /// <summary>
        /// ������ ���������.
        /// </summary>
        public void Sprint()
        {
            if (!controller.isGrounded) return;

            if (isCrouch) return;

            isSprint = true;
        }

        /// <summary>
        /// ����������� ������� ���������.
        /// </summary>
        public void UnSprint()
        {
            isSprint = false;
        }

        /// <summary>
        /// ������������ ���������.
        /// </summary>
        public void Aiming()
        {
            isAiming = true;
        }

        /// <summary>
        /// ����� ��������� �� ������������.
        /// </summary>
        public void UnAiming()
        {
            isAiming = false;
        }

        /// <summary>
        /// �������� ������� �������� � ����������� �� �������� ���������.
        /// </summary>
        /// <returns>������� ��������.</returns>
        public float GetCurrentSpeedByState()
        {
            if (isCrouch)
            {
                return CrouchSpeed;
            }

            if (isAiming)
            {
                if (isSprint)
                    return aimingRunSpeed;
                else
                    return aimingWalkSpeed;
            }

            if (!isAiming)
            {
                if (isSprint)
                    return rifleSprintSpeed;
                else
                    return rifleRunSpeed;
            }

            return rifleRunSpeed;
        }

        #endregion

        #endregion
    }
}
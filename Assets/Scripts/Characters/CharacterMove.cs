// Created and owned by Sankoh_Tew. Hi, dataminers! ;)

#region Usings

using Unity.VisualScripting;
using UnityEngine;

#endregion

namespace Shooter3D
{
    /// <summary>
    /// Перемещение персонажа.
    /// </summary>
    public class CharacterMove : MonoBehaviour
    {
        #region Parameters

        /// <summary>
        /// Ссылка на CharacterController.
        /// </summary>
        [SerializeField] private CharacterController controller;

        /// <summary>
        /// Скорость персонажа во время бега с винтовкой.
        /// </summary>
        [Header("Movement")]
        [SerializeField] private float rifleRunSpeed;

        /// <summary>
        /// Скорость персонажа во время спринта с винтовкой.
        /// </summary>
        [SerializeField] private float rifleSprintSpeed;

        /// <summary>
        /// Скорость персонажа во время ходьбы в прицеливании с винтовкой.
        /// </summary>
        [SerializeField] private float aimingWalkSpeed;

        /// <summary>
        /// Скорость персонажа во время бега в прицеливании с винтовкой.
        /// </summary>
        [SerializeField] private float aimingRunSpeed;

        [SerializeField] private float accelerationRate;

        /// <summary>
        /// Скорость персонажа в присяди.
        /// </summary>
        [SerializeField] private float CrouchSpeed;

        /// <summary>
        /// Скорость прыжка персонажа.
        /// </summary>
        [SerializeField] private float jumpSpeed;

        /// <summary>
        /// Высота персонажа, находящегося в присяди.
        /// </summary>
        [Header("State")]
        [SerializeField] private float crouchHeight;

        /// <summary>
        /// Целится ли сейчас персонаж?
        /// </summary>
        private bool isAiming;
        /// <summary>
        /// Прыгает ли сейчас персонаж?
        /// </summary>
        private bool isJump;
        /// <summary>
        /// В присяди ли сейчас персонаж?
        /// </summary>
        private bool isCrouch;
        /// <summary>
        /// Спринтует ли сейчас персонаж?
        /// </summary>
        private bool isSprint;
        /// <summary>
        /// Дистанция до приземления.
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
        /// Передвижение персонажа.
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
        /// Обновляет значение расстояния до приземления.
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
        /// Прыжок персонажа.
        /// </summary>
        public void Jump()
        {
            if (!controller.isGrounded) return;

            isJump = true;
        }

        /// <summary>
        /// Приседание персонажа.
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
        /// Выход персонажа из состояния присяди.
        /// </summary>
        public void UnCrouch()
        {
            isCrouch = false;

            controller.height = BaseCharacterHeight;
            controller.center = new Vector3(0, BaseCharacterHeightOffset, 0);
        }

        /// <summary>
        /// Спринт персонажа.
        /// </summary>
        public void Sprint()
        {
            if (!controller.isGrounded) return;

            if (isCrouch) return;

            isSprint = true;
        }

        /// <summary>
        /// Прекращение спринта персонажа.
        /// </summary>
        public void UnSprint()
        {
            isSprint = false;
        }

        /// <summary>
        /// Прицеливание персонажа.
        /// </summary>
        public void Aiming()
        {
            isAiming = true;
        }

        /// <summary>
        /// Выход персонажа из прицеливания.
        /// </summary>
        public void UnAiming()
        {
            isAiming = false;
        }

        /// <summary>
        /// Получает текущую скорость в зависимости от текущего состояния.
        /// </summary>
        /// <returns>Текущая скорость.</returns>
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
// Created and owned by Sankoh_Tew. Hi, dataminers! ;)

#region Usings

using UnityEngine;

#endregion

namespace Shooter3D
{
    /// <summary>
    /// Контроллер управления персонажем.
    /// </summary>
    public class CharacterMovementController : MonoBehaviour
    {
        #region Parameters

        /// <summary>
        /// Ссылка на CharacterController.
        /// </summary>
        [SerializeField] private CharacterMove targetMovement;

        /// <summary>
        /// Ссылка на камеру.
        /// </summary>
        [SerializeField] private ThirdPersonCamera thdCamera;

        [SerializeField] private Vector3 aimingOffset;
        
        #endregion

        #region API



        #region Unity API

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            targetMovement.targetDirectionControl = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            thdCamera.rotationControl = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            if (targetMovement.targetDirectionControl != Vector3.zero || targetMovement.IsAiming)
            {
                thdCamera.isRotateTarget = true;
            }
            else
            {
                thdCamera.isRotateTarget = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                targetMovement.Aiming();
                thdCamera.SetTargetOffset(aimingOffset);
            }
                
            if (Input.GetMouseButtonUp(1))
            {
                targetMovement.UnAiming();
                thdCamera.SetDefaultOffset();
            }

            if (Input.GetAxis("Jump") == 1)
                targetMovement.Jump();

            if (Input.GetKeyDown(KeyCode.LeftControl))
                targetMovement.Crouch();

            if (Input.GetKeyUp(KeyCode.LeftControl))
                targetMovement.UnCrouch();

            if (Input.GetKeyDown(KeyCode.LeftShift))
                targetMovement.Sprint();

            if (Input.GetKeyUp(KeyCode.LeftShift))
                targetMovement.UnSprint();
        }

        #endregion

        #region Public API



        #endregion

        #endregion
    }
}
// Created and owned by Sankoh_Tew. Hi, dataminers! ;)

#region Usings

using UnityEngine;

#endregion

namespace Shooter3D
{
    /// <summary>
    /// ������ �� �������� ����.
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        #region Parameters

        /// <summary>
        /// ������ �� Transform �������, �� ������� ������ ������.
        /// </summary>
        [SerializeField] private Transform target;

        /// <summary>
        /// �������� ������.
        /// </summary>
        [SerializeField] private Vector3 offset;

        /// <summary>
        /// ��������� ������.
        /// </summary>
        [SerializeField] private float distance;

        /// <summary>
        /// ����������� ��������� ������ �� ����.
        /// </summary>
        [SerializeField] private float minDistance;

        /// <summary>
        /// �������� ������������ ������.
        /// </summary>
        [SerializeField] private float distanceLerpRate;

        /// <summary>
        /// �������� ������������ ��������� ����.
        /// </summary>
        [SerializeField] private float rotateTargetLerpRate;

        /// <summary>
        /// ���������������� ����.
        /// </summary>
        [SerializeField] private float sensetive;

        /// <summary>
        /// ������������ ����� �������� ������ �� Y.
        /// </summary>
        [Header("Rotation Limits")]
        [SerializeField] private float maxLimitY;

        /// <summary>
        /// ����������� ����� �������� ������ �� Y.
        /// </summary>
        [SerializeField] private float minLimitY;

        /// <summary>
        /// �������� ����������� � ��������� ������.
        /// </summary>
        [SerializeField] private float changeOffsetRate;

        /// <summary>
        /// ������� �� ������� ������ �� �������?
        /// </summary>
        [HideInInspector] public bool isRotateTarget;

        /// <summary>
        /// ���������� ��������.
        /// </summary>
        [HideInInspector] public Vector2 rotationControl;

        /// <summary>
        /// ��������� ������������ �������� ���� �� ��� X?
        /// </summary>
        private float deltaRotationX;
        /// <summary>
        /// ��������� ������������ �������� ���� �� ��� Y?
        /// </summary>
        private float deltaRotationY;

        private Vector3 defaultOffset;
        private Vector3 targetOffset;

        /// <summary>
        /// ������� ���������.
        /// </summary>
        private float currentDistance;

        #endregion

        #region API

        /// <summary>
        /// ������������ ���� �������� ������.
        /// </summary>
        /// <param name="angle">����.</param>
        /// <param name="min">����������� ��������.</param>
        /// <param name="max">������������ ��������.</param>
        /// <returns></returns>
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            if (angle > 360)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }

        /// <summary>
        /// ��������� ��������� ��������.
        /// </summary>
        /// <param name="position">�������.</param>
        /// <returns></returns>
        private Vector3 AddLocalOffset(Vector3 position)
        {
            Vector3 result = position;
            result += new Vector3(0, offset.y, 0);
            result += transform.right * offset.x;
            result += transform.forward * offset.z;

            return result;
        }

        #region Unity API

        private void Start()
        {
            defaultOffset = offset;
            targetOffset = offset;
        }
        
        private void Update()
        {
            #region Calculate rotation and translation

            deltaRotationX += rotationControl.x * sensetive;
            deltaRotationY += rotationControl.y * sensetive;

            deltaRotationY = ClampAngle(deltaRotationY, minLimitY, maxLimitY);

            offset = Vector3.MoveTowards(offset, targetOffset, Time.deltaTime * changeOffsetRate);

            Quaternion finalRotation = Quaternion.Euler(-deltaRotationY, deltaRotationX, 0);
            Vector3 finalPosition = target.position - (finalRotation * Vector3.forward * distance);
            finalPosition = AddLocalOffset(finalPosition);

            #endregion

            #region Calculate current distance

            float targetDistance = distance;

            RaycastHit hit;

            if (Physics.Linecast(target.position + new Vector3(0, offset.y, 0), finalPosition, out hit))
            {
                float distanceToHit = Vector3.Distance(target.position + new Vector3(0, offset.y, 0), hit.point);

                if (hit.transform != target)
                {
                    if (distanceToHit < distance)
                        targetDistance = distanceToHit;
                }
            }

            currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, Time.deltaTime * distanceLerpRate);
            currentDistance = Mathf.Clamp(currentDistance, minDistance, distance);

            #endregion

            #region Correct camera position

            finalPosition = target.position - (finalRotation * Vector3.forward * currentDistance);

            #endregion

            #region Apply transform

            transform.rotation = finalRotation;
            transform.position = finalPosition;

            transform.position = AddLocalOffset(transform.position);

            #endregion

            #region Rotation target

            if (isRotateTarget)
            {
                // �������� �������!

                //Quaternion targetRotation = Quaternion.Euler(transform.rotation.x, transform.eulerAngles.y, transform.eulerAngles.z);

                //target.rotation = Quaternion.RotateTowards(target.rotation, targetRotation, Time.deltaTime * rotateTargetLerpRate);

                target.rotation = Quaternion.Euler(transform.rotation.x, transform.eulerAngles.y, transform.eulerAngles.z);
            }

            #endregion
        }

        #endregion

        #region Public API

        /// <summary>
        /// �������� �������� ������.
        /// </summary>
        /// <param name="offset">����� �������� ������.</param>
        public void SetTargetOffset(Vector3 offset)
        {
            targetOffset = offset;
        }

        public void SetDefaultOffset()
        {
            targetOffset = defaultOffset;
        }

        #endregion

        #endregion
    }
}
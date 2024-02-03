// Created and owned by Sankoh_Tew. Hi, dataminers! ;)

#region Usings

using UnityEngine;

#endregion

namespace Shooter3D
{
    /// <summary>
    /// Камера от третьего лица.
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        #region Parameters

        /// <summary>
        /// Ссылка на Transform объекта, за которым следит камера.
        /// </summary>
        [SerializeField] private Transform target;

        /// <summary>
        /// Смещение камеры.
        /// </summary>
        [SerializeField] private Vector3 offset;

        /// <summary>
        /// Дистанция камеры.
        /// </summary>
        [SerializeField] private float distance;

        /// <summary>
        /// Минимальная дистанция камеры от цели.
        /// </summary>
        [SerializeField] private float minDistance;

        /// <summary>
        /// Скорость интерполяции камеры.
        /// </summary>
        [SerializeField] private float distanceLerpRate;

        /// <summary>
        /// Скорость интерполяции разворота цели.
        /// </summary>
        [SerializeField] private float rotateTargetLerpRate;

        /// <summary>
        /// Чувствительность мыши.
        /// </summary>
        [SerializeField] private float sensetive;

        /// <summary>
        /// Максимальный лимит вращения камеры по Y.
        /// </summary>
        [Header("Rotation Limits")]
        [SerializeField] private float maxLimitY;

        /// <summary>
        /// Минимальный лимит вращения камеры по Y.
        /// </summary>
        [SerializeField] private float minLimitY;

        /// <summary>
        /// Скорость приближения и отдаления камеры.
        /// </summary>
        [SerializeField] private float changeOffsetRate;

        /// <summary>
        /// Вращать ли целевой объект за камерой?
        /// </summary>
        [HideInInspector] public bool isRotateTarget;

        /// <summary>
        /// Контроллер вращения.
        /// </summary>
        [HideInInspector] public Vector2 rotationControl;

        /// <summary>
        /// Насколько пользователь повернул мышь по оси X?
        /// </summary>
        private float deltaRotationX;
        /// <summary>
        /// Насколько пользователь повернул мышь по оси Y?
        /// </summary>
        private float deltaRotationY;

        private Vector3 defaultOffset;
        private Vector3 targetOffset;

        /// <summary>
        /// Текущая дистанция.
        /// </summary>
        private float currentDistance;

        #endregion

        #region API

        /// <summary>
        /// Ограничивает угол поворота камеры.
        /// </summary>
        /// <param name="angle">Угол.</param>
        /// <param name="min">Минимальное значение.</param>
        /// <param name="max">Максимальное значение.</param>
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
        /// Добавляет локальное смещение.
        /// </summary>
        /// <param name="position">Позиция.</param>
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
                // Персонаж трясётся!

                //Quaternion targetRotation = Quaternion.Euler(transform.rotation.x, transform.eulerAngles.y, transform.eulerAngles.z);

                //target.rotation = Quaternion.RotateTowards(target.rotation, targetRotation, Time.deltaTime * rotateTargetLerpRate);

                target.rotation = Quaternion.Euler(transform.rotation.x, transform.eulerAngles.y, transform.eulerAngles.z);
            }

            #endregion
        }

        #endregion

        #region Public API

        /// <summary>
        /// Изменяет смещение камеры.
        /// </summary>
        /// <param name="offset">Новое смещение камеры.</param>
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
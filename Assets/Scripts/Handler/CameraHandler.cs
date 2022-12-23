using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZQ
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        private Transform myTransform;
        private Vector3 cameraTransformPosition;
        private LayerMask ignoreLayers;

        public static CameraHandler instance;

        private float lookSpeedX = 0.1f;
        private float lookSpeedY = 0.03f;

        private float followSpeed = 2f;
        private float pivotSpeed = 0.03f;

        private float targetPosition;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        private float minimumPivot = -35;
        private float maximumPivot = 55;

        private float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;

        private Vector3 currentVelocity = Vector3.zero;
        private void Awake()
        {
            instance = this;
            myTransform = this.gameObject.transform;
            defaultPosition = cameraTransform.localPosition.z;
            //关闭8、9、10的层
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }
        public void FollowTarget(float delta)
        {
            //造成拖拉移动缓动的效果
            Vector3 targetPosition =
            //Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref currentVelocity, delta / followSpeed);
            myTransform.position = targetPosition;

            HandleCameraCollisions(delta);
        }
        /// <summary>
        /// 限定摄像机旋转范围
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="mouseXInput"></param>
        /// <param name="mouseYInput"></param>
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            lookAngle += (mouseXInput * lookSpeedX) / delta;
            pivotAngle += (mouseYInput * lookSpeedY) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            //Vector3 rotation = Vector3.zero;
            //rotation.y = lookAngle;
            //Quaternion targetRotation = Quaternion.Euler(rotation);
            //myTransform.rotation = targetRotation;
            myTransform.rotation = Quaternion.Euler(0,lookAngle,0);

            //rotation = Vector3.zero;
            //rotation.x = pivotAngle;
            //targetRotation = Quaternion.Euler(rotation);
            //cameraPivotTransform.localRotation = targetRotation;
            cameraPivotTransform.localRotation = Quaternion.Euler(pivotAngle,0,0);
        }

        private void HandleCameraCollisions(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast
            (cameraPivotTransform.position,cameraSphereRadius,direction,
             out hit,Mathf.Abs(targetPosition),ignoreLayers))
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(dis - cameraCollisionOffset);
            }
            if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = minimumCollisionOffset; 
            }
            cameraTransformPosition.z = 
            Vector3.SmoothDamp(new Vector3(0,0, cameraTransform.localPosition.z), new Vector3(0, 0, targetPosition),
            ref currentVelocity,delta / 0.2f).z;

            cameraTransform.localPosition = cameraTransformPosition;

        }

    }
}

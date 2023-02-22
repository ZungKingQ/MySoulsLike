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
        public LayerMask ignoreLayers;
        public InputHandler inputHandler;

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
        public float maximunLockOnDistrance = 30;
        public Transform nearestLockOnTarget;
        public Transform currentLockOnTarget;
        public Transform leftLockOnTarget;
        public Transform rightLockOnTarget;

        List<CharacterManager> availableTargets = new List<CharacterManager>();

        private Vector3 currentVelocity = Vector3.zero;
        private void Awake()
        {
            instance = this;
            myTransform = this.gameObject.transform;
            defaultPosition = cameraTransform.localPosition.z;
            //关闭8、9、10的层
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
            targetTransform = FindObjectOfType<PlayerManager>().transform;
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
            // 非锁定状态下，正常控制摄像机移动
            if (!inputHandler.lockOn_Flag && currentLockOnTarget == null)
            {
                lookAngle += (mouseXInput * lookSpeedX) / delta;
                pivotAngle += (mouseYInput * lookSpeedY) / delta;
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

                //Vector3 rotation = Vector3.zero;
                //rotation.y = lookAngle;
                //Quaternion targetRotation = Quaternion.Euler(rotation);
                //myTransform.rotation = targetRotation;
                myTransform.rotation = Quaternion.Euler(0, lookAngle, 0);

                //rotation = Vector3.zero;
                //rotation.x = pivotAngle;
                //targetRotation = Quaternion.Euler(rotation);
                //cameraPivotTransform.localRotation = targetRotation;
                cameraPivotTransform.localRotation = Quaternion.Euler(pivotAngle, 0, 0);
            }
            // 锁定状态下
            else
            {
                float velocity = 0;
                // 摄像机看向当前目标的方向
                Vector3 dir = currentLockOnTarget.position - transform.position;
                dir.Normalize();
                dir.y = 0;
                // 设置摄像机转向
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;

                dir = currentLockOnTarget.position - cameraPivotTransform.position;
                dir.Normalize();
                // 设置摄像机支点欧拉角 //？
                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                cameraPivotTransform.localEulerAngles = eulerAngle;
            }
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
        /// <summary>
        /// 获取一定视野内的目标信息
        /// </summary>
        public void HandleLockOn()
        {
            // 先假定一个最短距离
            float shortestDistance = Mathf.Infinity;
            float shortestDistance_LeftTarget = Mathf.Infinity;
            float shortestDistance_rightTarget = Mathf.Infinity;
            // 获取摄像机球形范围内的所有碰撞体
            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 30);

            for(int i = 0; i < colliders.Length; i++)
            {
                // 获取碰撞体的CharacterManger脚本
                CharacterManager characterManager = colliders[i].GetComponent<CharacterManager>();

                if(characterManager != null)
                {
                    // 看向目标的矢量
                    Vector3 lockTargetPosition = characterManager.transform.position - targetTransform.position;
                    // 角色到目标的距离
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, characterManager.transform.position);
                    // 角度
                    float viewableAngle = Vector3.Angle(lockTargetPosition, cameraTransform.forward);
                    // 碰撞体非角色且abs(viewableAngle)<50且距离在可锁定距离范围之内
                    if (characterManager.transform.root != targetTransform.transform.transform.root && viewableAngle > -50 && viewableAngle < 50 && distanceFromTarget <= maximunLockOnDistrance)
                    {
                        // 添加到有意义的列表中
                        availableTargets.Add(characterManager);
                    }
                }
            }
            for(int i = 0; i < availableTargets.Count; i++)
            {
                float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[i].transform.position);
                // 得到距离最近的目标
                if(distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[i].transform;
                }
                // 锁定状态中
                if(inputHandler.lockOn_Flag)
                {
                    // 以当前锁定目标为原点，得到可获取信息目标的位置
                    Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[i].transform.position);
                    var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[i].transform.position.x;
                    var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[i].transform.position.x;
                    // 分别设置为左侧
                    if(relativeEnemyPosition.x > 0 && distanceFromLeftTarget < shortestDistance_LeftTarget)
                    {
                        shortestDistance_LeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = availableTargets[i].lockOnTransform;
                    }
                    if (relativeEnemyPosition.x < 0 && distanceFromRightTarget < shortestDistance_rightTarget)
                    {
                        shortestDistance_rightTarget = distanceFromRightTarget;
                        rightLockOnTarget = availableTargets[i].lockOnTransform;
                    }
                }
            }
        }
        /// <summary>
        /// 清空所有锁定的目标，并清空列表
        /// </summary>
        public void ClearLockOnTargets()
        {
            availableTargets.Clear();
            currentLockOnTarget = null;
            nearestLockOnTarget = null;
        }
    }
}

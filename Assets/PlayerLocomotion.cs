using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace SG
{
    public class PlayerLocomotion : MonoBehaviour
    {

        Transform cameraObject;//储存主相机的位置
        InputHandler inputHandler; //输入处理组件
        Vector3 moveDirection;//移动向量

        [HideInInspector]
        public Transform myTransform;//记录位置信息
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;
        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();
        }

        #region Movement
        Vector3 normalVector;
        Vector3 TargetPosition;

        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOrride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;

            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir=myTransform.forward;
            }

            float rs = rotationSpeed;
            Quaternion tr= Quaternion.LookRotation(targetDir);
            Quaternion targetRotation= Quaternion.Slerp(myTransform.rotation, tr, rs*delta);

            myTransform.rotation = targetRotation;
        }

        public void Update()
        {
            float delta = Time.deltaTime;

            inputHandler.TickInput(delta);

            moveDirection = cameraObject.forward*inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y= 0;

            float speed = movementSpeed;
            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);
            

            if(animatorHandler.canRotate) 
            {
                HandleRotation(delta);
            }

        }
        #endregion

    }
}


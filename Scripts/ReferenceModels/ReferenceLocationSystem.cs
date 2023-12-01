/*
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Vuforia;

namespace CoordinateSystem
{
    public class ReferenceLocationSystem : MonoBehaviour
    {
        [SerializeField] private ModelTargetBehaviour mtarget;
        [SerializeField] private GameObject vfCamera;
        [SerializeField] private GameObject user;
        [SerializeField] private GameObject target;
        [SerializeField] private InferenceLocation _inLocation;
        [SerializeField] private GameObject ObjModel;

        public float distance = -1;
        public Vector4 direction;

        [SerializeField] private bool autoStopTrack = true;
        [SerializeReference] private bool _objTracked = false;

        private void ActiveEquipmentModelInLocation()
        {
            this.ObjModel.transform.position = user.transform.position + this._inLocation.RelativePosition;
            this.ObjModel.SetActive(true);
        }

        private void TrackObject(ObserverBehaviour o, Vuforia.TargetStatus t)
        {
            //bool isTrackStatus = t.Status.Equals(Vuforia.Status.TRACKED);
            //if (!this._objTracked? !isTrackStatus : isTrackStatus? this.autoStopTrack : true)
            //    return;
            Debug.Log("Object tracked " + t.Status);

            Vector3 userPos = vfCamera.transform.position;
            Vector3 targetPos = target.transform.position;

            Quaternion direction = Quaternion.LookRotation(targetPos - userPos);
            float distance = Vector3.Distance(userPos, targetPos);

            Quaternion targetRotation = Quaternion.LookRotation(
                target.transform.InverseTransformDirection(targetPos - userPos), target.transform.up);

            this._inLocation = new InferenceLocation(targetRotation, direction, distance);
            this.ActiveEquipmentModelInLocation();

            // ### debug ###
            this.distance = this._inLocation.Distance;
            this.direction = new Vector4(this._inLocation.Direction.w, this._inLocation.Direction.x,
                this._inLocation.Direction.y, this._inLocation.Direction.z);
            this._objTracked = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            this.mtarget.OnTargetStatusChanged += this.TrackObject;
        }

        private void Awake()
        {
            if (vfCamera == null || target == null)
                throw new NullReferenceException();
        }

        private void ResetInLocation()
        {
            this._inLocation = new InferenceLocation(Quaternion.identity, Quaternion.identity, -1);
        }

        public void OnEnable()
        {
            this._objTracked = false;
            this.ResetInLocation();
        }

        /*
        private void Update()
        {
            Vector3 userPos = user.transform.position;
            Vector3 targetPos = target.transform.position;

            Quaternion direction = Quaternion.LookRotation(targetPos - userPos, user.transform.up);
            float distance = Vector3.Distance(userPos, targetPos);

            //rotação relativa
            Quaternion targetRotation = Quaternion.LookRotation(
                target.transform.InverseTransformDirection(targetPos - userPos), target.transform.up);

            this._inLocation = new InferenceLocation(targetRotation, direction, distance);

            // ### debug ###
            this.direction = new Vector4(this._inLocation.Direction.w, this._inLocation.Direction.x,
                this._inLocation.Direction.y, this._inLocation.Direction.z);
        }
        *

#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            //Gizmos.color = Color.green;
            //Gizmos.DrawSphere(this._inLocation.RelativePosition, 0.04f);
            //Gizmos.color = Color.magenta;
            //Gizmos.DrawLine(user.transform.position, user.transform.position + this._inLocation.RelativePosition);

            //Gizmos.color = Color.red;
            //Vector3 rotation = Vector3.zero + (this._inLocation.TargetRotation * Vector3.forward * 1);
            //Gizmos.DrawLine(Vector3.zero, rotation);
            //Gizmos.DrawLine(Vector3.zero, Vector3.up * (rotation.y % 1));
            //Gizmos.DrawLine(Vector3.zero, Vector3.right * (rotation.z % 1));

            if (this.ObjModel.activeSelf)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(user.transform.position, this.ObjModel.transform.position);

                //Gizmos.color = Color.green;
                //Gizmos.DrawWireCube(this.ObjModel.transform.position, 
                //    this.ObjModel.gameObject.GetComponent<BoxCollider>().size);
            }
        }

        public void OnGIU()
        {
            EditorGUILayout.TextArea(".distance: " + this._inLocation.Distance.ToString());
        }
#endif

    }
}
*/
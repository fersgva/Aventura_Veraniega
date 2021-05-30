using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mine.Utilities
{
    public class Utilities : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public static Vector3 CalculateDirFromAngle(float angleInDegrees, bool isGlobal, Vector3 localRotation)
        {
            if (!isGlobal)
            {
                angleInDegrees += localRotation.y;
            }
            return (new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad))).normalized;
        }

    }
}


using Mine.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor
{
    //De esta manera controlo desde escena como se ve el fieldOFVIEW
    private void OnSceneGUI()
    {
        //El gameObject en el que clickamos y por lo tanto se ve el fov: target.
        FieldOfView thisObject = (FieldOfView) target;
        Handles.color = Color.white;
        //Dibujamos un círculo de 360º
        Handles.DrawWireArc(thisObject.transform.position, Vector3.up, Vector3.forward, 360, thisObject.viewRadius);
        Vector3 dirA = Utilities.CalculateDirFromAngle(-thisObject.viewAngle / 2, false, thisObject.transform.eulerAngles);
        Vector3 dirB = Utilities.CalculateDirFromAngle(thisObject.viewAngle / 2, false, thisObject.transform.eulerAngles);
        //Se dibuja una línea desde el punto central al punto de la nueva dirección (normalizada) multiplicada por magnitud (radio de vista)
        Handles.DrawLine(thisObject.transform.position, thisObject.transform.position + dirA * thisObject.viewRadius);
        Handles.DrawLine(thisObject.transform.position, thisObject.transform.position + dirB * thisObject.viewRadius);
    }
}

using UnityEngine;

namespace Tools.Utils
{
    [AddComponentMenu("Tools/Utils/Sensor")]
    public class Sensor : MonoBehaviour
    {
        public bool IsTargetInViewAngle(Transform viewPoint, Transform target, float viewAngle)
        {
            Vector3 dir = target.position - viewPoint.position;
            float cosAngle = Vector3.Dot(dir.normalized, viewPoint.forward);
            float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;
            bool inViewAngle = angle < viewAngle;
            return inViewAngle;
        }
    }
}

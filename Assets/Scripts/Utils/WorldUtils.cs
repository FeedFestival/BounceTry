using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{

    public static class WorldUtils
    {
        public static GameObject CreateUnitTarget(string unitName)
        {
            var go = CreateFromPrefab("Prefabs/UnitTarget", Vector3.zero);
            go.name = unitName + "_UnitTarget";

            return go;
        }

        public static GameObject CreateFromPrefab(string resourcePath, Vector3 originPosition)
        {
            var targetPrefab = Resources.Load(resourcePath) as GameObject;
            return GameObject.Instantiate(targetPrefab, originPosition, Quaternion.identity) as GameObject;
        }

        public static Vector3 GetDirection(Vector3 vectorFrom, Vector3 vectorTo)
        {
            return vectorTo - vectorFrom;
        }

        public static Quaternion SmoothLook(Quaternion rotation, Vector3 newDirection, float speed)
        {
            if (newDirection == Vector3.zero)
                return rotation;
            return Quaternion.Lerp(rotation, Quaternion.LookRotation(newDirection), Time.deltaTime * speed);
        }

        public static Vector3 GetPointHitAtMousePosition(Collider collider = null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (collider)
            {
                if (collider.Raycast(ray, out hit, 100))
                    return new Vector3(Mathf.Round(hit.point.x * 100f) / 100f, Mathf.Round(hit.point.y * 100f) / 100f, Mathf.Round(hit.point.z * 100f) / 100f);
            }
            else
            {
                if (Physics.Raycast(ray, out hit, 100))
                    return new Vector3(Mathf.Round(hit.point.x * 100f) / 100f, Mathf.Round(hit.point.y * 100f) / 100f, Mathf.Round(hit.point.z * 100f) / 100f);
            }
            return Vector3.zero;
        }

        public static Vector3 GetMidPointOffset(Vector3 pos1, Vector3 pos2, float? offset = null, bool returnMidPoint = false)
        {
            //get the direction between the two transforms -->
            Vector3 dir = (pos2 - pos1).normalized;

            //get a direction that crosses our [dir] direction
            //NOTE! : this can be any of a buhgillion directions that cross our [dir] in 3D space
            //To alter which direction we're crossing in, assign another directional value to the 2nd parameter
            Vector3 perpDir = Vector3.Cross(dir, Vector3.right);

            //get our midway point
            Vector3 midPoint = (pos1 + pos2) / 2f;

            if (returnMidPoint)
                return midPoint;

            //get the offset point
            //This is the point you're looking for.
            Vector3 offsetPoint = midPoint + (perpDir * offset.Value);

            return offsetPoint;
        }
    }
}

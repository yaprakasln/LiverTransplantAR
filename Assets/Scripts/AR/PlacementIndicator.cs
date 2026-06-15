using UnityEngine;

namespace LiverTransplantAR.AR
{
    public class PlacementIndicator : MonoBehaviour
    {
        void Start()
        {
            // Create a simple circular visual if no mesh is assigned
            if (GetComponent<MeshFilter>() == null)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                GetComponent<MeshFilter>().mesh = go.GetComponent<MeshFilter>().mesh;
                DestroyImmediate(go);
                
                transform.rotation = Quaternion.Euler(90, 0, 0);
                transform.localScale = Vector3.one * 0.3f;
                
                var mat = new Material(Shader.Find("Unlit/Color"));
                mat.color = new Color(0, 1, 1, 0.5f);
                GetComponent<MeshRenderer>().material = mat;
            }
        }
    }
}

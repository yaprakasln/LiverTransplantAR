using UnityEngine;

namespace LiverTransplantAR.Visuals
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(SkinnedMeshRenderer))]
    public class LiverMeshGenerator : MonoBehaviour
    {
        public int Resolution = 32;
        private Mesh _mesh;
        private SkinnedMeshRenderer _skinnedRenderer;

        void Awake()
        {
            GenerateLiverMesh();
        }

        public void GenerateLiverMesh()
        {
            _mesh = new Mesh();
            _mesh.name = "ProceduralLiver";

            int vertexCount = Resolution * Resolution * 2; // Two lobes
            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            int[] triangles = new int[(Resolution - 1) * (Resolution - 1) * 6 * 2];

            // Generate Right Lobe (Larger)
            GenerateLobe(0, new Vector3(0.5f, 0, 0), new Vector3(1.5f, 1f, 0.8f), vertices, normals, uvs, triangles, 0);
            
            // Generate Left Lobe (Smaller)
            GenerateLobe(Resolution * Resolution, new Vector3(-0.5f, 0, 0), new Vector3(1f, 0.8f, 0.6f), vertices, normals, uvs, triangles, (Resolution - 1) * (Resolution - 1) * 6);

            _mesh.vertices = vertices;
            _mesh.normals = normals;
            _mesh.uv = uvs;
            _mesh.triangles = triangles;
            
            // Add BlendShapes for Growth
            AddGrowthBlendShape(_mesh, vertices);

            GetComponent<MeshFilter>().sharedMesh = _mesh;
            _skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
            _skinnedRenderer.sharedMesh = _mesh;
        }

        private void GenerateLobe(int offset, Vector3 center, Vector3 scale, Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles, int triOffset)
        {
            for (int y = 0; y < Resolution; y++)
            {
                for (int x = 0; x < Resolution; x++)
                {
                    int index = offset + y * Resolution + x;
                    float v = (float)y / (Resolution - 1);
                    float u = (float)x / (Resolution - 1);

                    float theta = v * Mathf.PI;
                    float phi = u * 2 * Mathf.PI;

                    Vector3 pos = new Vector3(
                        Mathf.Sin(theta) * Mathf.Cos(phi),
                        Mathf.Cos(theta),
                        Mathf.Sin(theta) * Mathf.Sin(phi)
                    );

                    vertices[index] = center + Vector3.Scale(pos, scale * 0.5f);
                    normals[index] = pos.normalized;
                    uvs[index] = new Vector2(u, v);
                }
            }

            int t = triOffset;
            for (int y = 0; y < Resolution - 1; y++)
            {
                for (int x = 0; x < Resolution - 1; x++)
                {
                    int i = offset + y * Resolution + x;
                    triangles[t++] = i;
                    triangles[t++] = i + Resolution + 1;
                    triangles[t++] = i + Resolution;

                    triangles[t++] = i;
                    triangles[t++] = i + 1;
                    triangles[t++] = i + Resolution + 1;
                }
            }
        }

        private void AddGrowthBlendShape(Mesh mesh, Vector3[] baseVertices)
        {
            Vector3[] deltaVertices = new Vector3[baseVertices.Length];
            
            // The "Small" version (30% size) is our base, so the BlendShape will represent "Growing to 100%"
            // Actually, we'll do the opposite: Base is full size, BlendShape 100% is small.
            // But let's follow the user's logic: Starts at 0.3 or 0.4.
            
            for (int i = 0; i < baseVertices.Length; i++)
            {
                // Shrink towards center of lobes for the "Small" shape
                // We'll calculate a 'shrunk' version
                Vector3 lobeCenter = baseVertices[i].x > 0 ? new Vector3(0.5f, 0, 0) : new Vector3(-0.5f, 0, 0);
                Vector3 direction = baseVertices[i] - lobeCenter;
                deltaVertices[i] = -direction * 0.7f; // Shrink by 70%
            }

            mesh.AddBlendShapeFrame("ShrinkToInitial", 100f, deltaVertices, null, null);
        }
    }
}

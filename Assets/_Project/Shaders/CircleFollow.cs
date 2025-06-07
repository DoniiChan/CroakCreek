using UnityEngine;

namespace CroakCreek
{
    public class CircleFollow : MonoBehaviour
    {
        public static int PosID = Shader.PropertyToID("_Position");
        public static int SizeID = Shader.PropertyToID("_Size");

        public Material wallMaterial;
        public Camera cam;
        public LayerMask mask;

        void Update()
        {
            var dir = cam.transform.position - transform.position;
            var ray = new Ray(transform.position, dir.normalized);

            if (Physics.Raycast(ray, 3000, mask))
                wallMaterial.SetFloat(SizeID, 1);
            else
                wallMaterial.SetFloat(SizeID, 0);

                var view = cam.WorldToViewportPoint(transform.position);
            wallMaterial.SetVector(PosID, view);

        }
    }
}

using UnityEngine;

public static class RendererExtensions
{
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        if (renderer.gameObject.GetComponent<Collider>() != null)
        {
           
            return GeometryUtility.TestPlanesAABB(planes, renderer.gameObject.GetComponent<Collider>().bounds);
        }else
        {
            return false;
        }
    }
}
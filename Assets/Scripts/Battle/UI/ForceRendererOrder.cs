using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ForceRendererOrder : MonoBehaviour
{
    [SerializeField] string sortingLayerName = "Default";
    [SerializeField] int orderInLayer = -100;

    void Awake()
    {
        var r = GetComponent<Renderer>();

        r.sortingLayerName = sortingLayerName;
        r.sortingOrder = orderInLayer;
    }
}
using UnityEngine;


[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class BezierCurve : MonoBehaviour
{
    public enum CurveType
    {
        Quadratic, 
        Cubic      
    }

    [Header("Type de courbe")]
    [SerializeField] private CurveType curveType = CurveType.Cubic;

    [Header("Points de contrôle")]
    [SerializeField] private Transform p0;
    [SerializeField] private Transform p1;
    [SerializeField] private Transform p2;
    [Tooltip("Utilisé uniquement en mode Cubic")]
    [SerializeField] private Transform p3;

    [Header("Affichage")]
    [Tooltip("Nombre de segments de la courbe. Plus la valeur est haute, plus la courbe est lisse.")]
    [Range(2, 200)]
    [SerializeField] private int resolution = 50;

    [Header("Gizmos")]
    [SerializeField] private bool showControlPolygon = true;
    [SerializeField] private float pointGizmoRadius = 0.15f;

    private LineRenderer lineRenderer;

    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();

        if (!HasRequiredPoints()) return;

        DrawCurve();
    }

    
    private bool HasRequiredPoints()
    {
        if (p0 == null || p1 == null || p2 == null) return false;
        if (curveType == CurveType.Cubic && p3 == null) return false;
        return true;
    }

    
    
    private void DrawCurve()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = resolution + 1; 

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution; 
            lineRenderer.SetPosition(i, Evaluate(t));
        }
    }

    
    public Vector3 Evaluate(float t)
    {
        t = Mathf.Clamp01(t);

        return curveType == CurveType.Quadratic
            ? Quadratic(p0.position, p1.position, p2.position, t)
            : Cubic(p0.position, p1.position, p2.position, p3.position, t);
    }

    
    public static Vector3 Quadratic(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float u = 1f - t;

        return (u * u) * a
             + (2f * u * t) * b
             + (t * t) * c;
    }

    
    public static Vector3 Cubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        float u = 1f - t;

        return (u * u * u) * a
             + (3f * u * u * t) * b
             + (3f * u * t * t) * c
             + (t * t * t) * d;
    }

    
    public static Vector3 CubicDeCasteljau(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        Vector3 cd = Vector3.Lerp(c, d, t);

        Vector3 abbc = Vector3.Lerp(ab, bc, t);
        Vector3 bccd = Vector3.Lerp(bc, cd, t);

        return Vector3.Lerp(abbc, bccd, t);
    }

    private void OnDrawGizmos()
    {
        if (!HasRequiredPoints()) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(p0.position, pointGizmoRadius);
        Gizmos.DrawWireSphere(p2.position, pointGizmoRadius);
        if (curveType == CurveType.Cubic) Gizmos.DrawWireSphere(p3.position, pointGizmoRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(p1.position, pointGizmoRadius);

        if (!showControlPolygon) return;

        Gizmos.color = Color.grey;
        if (curveType == CurveType.Quadratic)
        {
            Gizmos.DrawLine(p0.position, p1.position);
            Gizmos.DrawLine(p1.position, p2.position);
        }
        else
        {
            Gizmos.DrawLine(p0.position, p1.position);
            Gizmos.DrawLine(p1.position, p2.position);
            Gizmos.DrawLine(p2.position, p3.position);
        }
    }
}
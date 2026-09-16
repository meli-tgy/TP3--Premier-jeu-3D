using UnityEngine;


[RequireComponent(typeof(Camera))]
public class BezierCameraFlight : MonoBehaviour
{
    [Header("Trajectoire")]
    [SerializeField] private BezierCurve curve;

    [Header("Vitesse")]
    [Tooltip("Vitesse de déplacement en unités Unity par seconde")]
    [SerializeField] private float speed = 5f;
    [Tooltip("Rejoue la trajectoire en boucle une fois arrivé au bout")]
    [SerializeField] private bool loop = true;

    [Header("Orientation")]
    [Tooltip("La caméra s'oriente selon la direction de déplacement")]
    [SerializeField] private bool lookAlongPath = true;
    [Tooltip("Vitesse de rotation vers la nouvelle direction (plus haut = plus rigide)")]
    [SerializeField] private float lookSmoothing = 5f;

    [Header("Précision de l'arc-length")]
    [Tooltip("Nombre d'échantillons utilisés pour mesurer la longueur réelle de la courbe. Plus haut = plus précis.")]
    [SerializeField] private int arcLengthSamples = 200;

 
    private float[] cumulativeLengths;
    private float totalLength;

    private float distanceTravelled;

    private void Start()
    {
        BuildArcLengthTable();
        distanceTravelled = 0f;

        if (curve != null)
            transform.position = curve.Evaluate(0f);
    }

    private void Update()
    {
        if (curve == null || totalLength <= 0f) return;

        distanceTravelled += speed * Time.deltaTime;

        if (distanceTravelled >= totalLength)
        {
            if (loop)
            {
                distanceTravelled %= totalLength;
            }
            else
            {
                distanceTravelled = totalLength;
                enabled = false; 
            }
        }

        float t = DistanceToT(distanceTravelled);
        Vector3 targetPosition = curve.Evaluate(t);

        if (lookAlongPath)
        {
           
            float lookAheadT = Mathf.Clamp01(t + 0.01f);
            Vector3 lookAtPoint = curve.Evaluate(lookAheadT);
            Vector3 direction = lookAtPoint - targetPosition;

            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSmoothing * Time.deltaTime);
            }
        }

        transform.position = targetPosition;
    }

    private void BuildArcLengthTable()
    {
        if (curve == null)
        {
            totalLength = 0f;
            return;
        }

        cumulativeLengths = new float[arcLengthSamples + 1];
        cumulativeLengths[0] = 0f;

        Vector3 previousPoint = curve.Evaluate(0f);

        for (int i = 1; i <= arcLengthSamples; i++)
        {
            float t = (float)i / arcLengthSamples;
            Vector3 currentPoint = curve.Evaluate(t);

            float segmentLength = Vector3.Distance(previousPoint, currentPoint);
            cumulativeLengths[i] = cumulativeLengths[i - 1] + segmentLength;

            previousPoint = currentPoint;
        }

        totalLength = cumulativeLengths[arcLengthSamples];
    }

    
    private float DistanceToT(float distance)
    {
        distance = Mathf.Clamp(distance, 0f, totalLength);

        
        int index = 0;
        for (int i = 0; i < cumulativeLengths.Length - 1; i++)
        {
            if (cumulativeLengths[i + 1] >= distance)
            {
                index = i;
                break;
            }
        }

        float lengthBefore = cumulativeLengths[index];
        float lengthAfter = cumulativeLengths[index + 1];
        float segmentLength = lengthAfter - lengthBefore;

        
        float lerpFactor = segmentLength > 0.0001f
            ? (distance - lengthBefore) / segmentLength
            : 0f;

        float tBefore = (float)index / arcLengthSamples;
        float tAfter = (float)(index + 1) / arcLengthSamples;

        return Mathf.Lerp(tBefore, tAfter, lerpFactor);
    }
}
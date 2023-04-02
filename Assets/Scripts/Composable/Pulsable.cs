using UnityEngine;

public class Pulsable : MonoBehaviour
{

    private SpriteRenderer sr;

    public float minimum = 0.3f;
    public float maximum = 1f;
    public float cyclesPerSecond = 2.0f;
    private float a;
    private bool increasing = true;
    Color color;

    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        color = sr.color;
        a = maximum;
    }



    void Update()
    {
        if (sr != null)
        {
            float t = Time.deltaTime;
            if (a >= maximum) increasing = false;
            if (a <= minimum) increasing = true;
            a = increasing ? a += t * cyclesPerSecond * 2 : a -= t * cyclesPerSecond;
            color.a = a;
            sr.color = color;
        }
    }

    public void Reset()
    {
        color.a = maximum;
        sr.color = color;
    }
}
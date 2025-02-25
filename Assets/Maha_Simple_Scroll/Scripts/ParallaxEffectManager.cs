using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class ParallaxEffectManager : MonoBehaviour
{
    [Serializable]
    private class ParallaxLayer
    {
        public RectTransform transform;
        public float speed = 0.5f;
        [HideInInspector] public Vector2 initialPosition;
    }

    [SerializeField] private ParallaxLayer[] layers = new ParallaxLayer[6];
    private ScrollRect scrollRect;

    void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();

        // Initialize layers with default values if not set in inspector
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i] == null)
            {
                layers[i] = new ParallaxLayer();
            }

            if (layers[i].transform != null)
            {
                layers[i].initialPosition = layers[i].transform.anchoredPosition;
            }
        }
    }

    void Update()
    {
        if (scrollRect == null)
        {
            return;
        }

        // Calculate viewport center difference
        Vector2 viewPortPosition = scrollRect.viewport.InverseTransformPoint(layers[0].transform.position);
        Vector2 centerDifference = scrollRect.viewport.rect.center - new Vector2(viewPortPosition.x, viewPortPosition.y);

        // Update each layer's position
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i]?.transform == null)
            {
                continue;
            }

            float adjustment = -centerDifference.x * layers[i].speed;
            layers[i].transform.anchoredPosition = new Vector2(
                layers[i].initialPosition.x + adjustment,
                layers[i].initialPosition.y
            );
        }
    }
}

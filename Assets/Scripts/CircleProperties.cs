using UnityEngine;

public class CircleProperties : MonoBehaviour
{
    public bool isConnected = false; // Добавляем переменную для отслеживания соединения
    public float snapRadius = 0.5f; // Радиус привязки
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color ballColor;
    [SerializeField] private ParticleSystem particle;

    public void SetBallColor(Color color, Sprite sprite)
    {
        ballColor = color;
        spriteRenderer.sprite = sprite;
    }

    public Color GetBallColor( )
    {
        return ballColor;
    }

    public void Boom()
    {
        particle.Play();
    }
}

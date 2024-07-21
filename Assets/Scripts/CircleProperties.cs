using UnityEngine;

public class CircleProperties : MonoBehaviour
{
    public bool isConnected = false; // ��������� ���������� ��� ������������ ����������
    public float snapRadius = 0.5f; // ������ ��������
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

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BallGenerator : MonoBehaviour
{
    public GameObject ballPrefab; // ������ ��� ������
    public Vector2 spacing = new Vector2 ( 1.0f , 1.0f ); // ���������� ����� �������� �� ����������� � ���������
    public int rows = 8; // ���������� �����
    public int columns = 6; // ���������� ��������
    public Color[] ballColors; // ������ ������ ��� �������
    public Sprite[] ballSprites;

    public UnityAction<bool> OnBlockScreen;

    Vector3 startPosition;

    IEnumerator GenerateBalls( )
    {
        OnBlockScreen?.Invoke(true);
        // ��������� ������� ��� ��������� �������
        startPosition = transform.position - new Vector3 ( ( columns - 1 ) * spacing.x / 2 , ( rows - 1 ) * spacing.y / 2 , 0 );

        for ( int row = 0; row < rows; row++ )
        {
            for ( int col = 0; col < columns; col++ )
            {
                // ��������� ������� ��� ������� ������ � �����
                Vector3 position = startPosition + new Vector3 ( col * spacing.x , 8f , 0 );

                // ������� ��������� ������ �� �������
                GameObject ball = Instantiate ( ballPrefab , position , Quaternion.identity );

                // ��� �������, ����� ������� ������ ��������� ��������� ����������
                ball.transform.parent = transform;

                // �������� ��������� SpriteRenderer ������
                CircleProperties circleProperties = ball.GetComponent<CircleProperties> ();

                // �������� ��������� ���� �� ������� ������ � ��������� ��� � ������
                if ( circleProperties != null && ballColors.Length > 0 )
                {
                    int randomId = Random.Range(0, ballColors.Length);
                    circleProperties.SetBallColor(ballColors[randomId], ballSprites[randomId]); // ������������� ���� ����
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        OnBlockScreen?.Invoke(false);
    }

    public void ReplaceBall(CircleProperties ball)
    {
        OnBlockScreen?.Invoke(true);

        ball.Boom();

        StartCoroutine(DelayBallActivate(ball));
    }

    IEnumerator DelayBallActivate(CircleProperties ball)
    {
        yield return new WaitForSeconds(0.2f);

        ball.gameObject.SetActive(false);

        ball.transform.Translate(Vector3.up * 8.8f);

        // ������������� ��������� ����, ��� ��� �������������� ���������
        if (ballColors.Length > 0)
        {
            int randomId = Random.Range(0, ballColors.Length);
            ball.SetBallColor(ballColors[randomId], ballSprites[randomId]);
        }

        yield return new WaitForSeconds(0.5f);

        if(ball != null) ball.gameObject.SetActive(true);
        OnBlockScreen?.Invoke(false);
    }

    public void StartGame( )
    {
        // ���������� ��� ������������ ������
        foreach ( Transform child in transform )
        {
            Destroy ( child.gameObject );
        }

        // ����� ���������� ������
        StartCoroutine(GenerateBalls());
    }
}

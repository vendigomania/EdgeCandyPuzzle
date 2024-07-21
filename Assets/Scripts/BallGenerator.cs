using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BallGenerator : MonoBehaviour
{
    public GameObject ballPrefab; // Префаб для шарика
    public Vector2 spacing = new Vector2 ( 1.0f , 1.0f ); // Расстояние между шариками по горизонтали и вертикали
    public int rows = 8; // Количество строк
    public int columns = 6; // Количество столбцов
    public Color[] ballColors; // Массив цветов для шариков
    public Sprite[] ballSprites;

    public UnityAction<bool> OnBlockScreen;

    Vector3 startPosition;

    IEnumerator GenerateBalls( )
    {
        OnBlockScreen?.Invoke(true);
        // Начальная позиция для генерации шариков
        startPosition = transform.position - new Vector3 ( ( columns - 1 ) * spacing.x / 2 , ( rows - 1 ) * spacing.y / 2 , 0 );

        for ( int row = 0; row < rows; row++ )
        {
            for ( int col = 0; col < columns; col++ )
            {
                // Вычисляем позицию для каждого шарика в сетке
                Vector3 position = startPosition + new Vector3 ( col * spacing.x , 8f , 0 );

                // Создаем экземпляр шарика из префаба
                GameObject ball = Instantiate ( ballPrefab , position , Quaternion.identity );

                // Для примера, можно сделать шарики дочерними объектами генератора
                ball.transform.parent = transform;

                // Получаем компонент SpriteRenderer шарика
                CircleProperties circleProperties = ball.GetComponent<CircleProperties> ();

                // Выбираем случайный цвет из массива цветов и применяем его к шарику
                if ( circleProperties != null && ballColors.Length > 0 )
                {
                    int randomId = Random.Range(0, ballColors.Length);
                    circleProperties.SetBallColor(ballColors[randomId], ballSprites[randomId]); // Устанавливаем цвет шара
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

        // Устанавливаем случайный цвет, как при первоначальной генерации
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
        // Уничтожаем все существующие шарики
        foreach ( Transform child in transform )
        {
            Destroy ( child.gameObject );
        }

        // Снова генерируем шарики
        StartCoroutine(GenerateBalls());
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class PointsConnector : MonoBehaviour
{
    public UnityAction<int> OnLineEnd;

    public GameObject linePrefab; // ������ �����
    public LayerMask obstacleLayer; // ���� �����������

    private List<Transform> points = new List<Transform> (); // ������ ���� �����
    private Transform lastPoint = null; // ��������� ��������� �����
    private List<GameObject> lines = new List<GameObject> (); // ������ ���� �����
    private BallGenerator ballGenerator; // ������ �� BallGenerator
    public bool isConnected = false;

    private bool connectionMade = false;
    void Awake( )
    {
        ballGenerator = FindObjectOfType<BallGenerator> (); // ������� BallGenerator � �����
    }
    void Update( )
    {
        if ( Input.GetMouseButtonDown ( 0 )&& CheckPointerOverUIOrObstacleLayer() )
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint ( Input.mousePosition );
            RaycastHit2D hit = Physics2D.Raycast ( mousePosition , Vector2.zero );

            if ( hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer ( "GameLayer" ) )
            {
                // ������ ���� ������������ ����� �� ������ ���� "GameLayer"
                lastPoint = null;
                ClearLines ();
            }
        }

        if ( Input.GetMouseButton ( 0 ) && CheckPointerOverUIOrObstacleLayer () )
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint ( Input.mousePosition );
            mousePosition.z = 0;

            Collider2D hitCollider = Physics2D.OverlapPoint ( mousePosition , obstacleLayer );
            if ( hitCollider != null )
            {
                Transform hitTransform = hitCollider.transform;
                if ( lastPoint == null || ( lastPoint != hitTransform && CanConnect ( lastPoint , hitTransform ) ) )
                {
                    if ( lastPoint != null )
                    {
                        Color lineColor = lastPoint.GetComponent<CircleProperties> ().GetBallColor ();
                        DrawLine ( lastPoint.position , hitTransform.position , lineColor , hitTransform );

                        points.Add ( hitTransform );
                        lastPoint = hitTransform;
                    }
                    else
                    {
                        points.Add ( hitTransform );
                        lastPoint = hitTransform;
                    }
                }
                else if ( points.Contains ( hitTransform ) )
                {
                    int index = points.IndexOf ( hitTransform );
                    for ( int i = points.Count - 1; i > index; i-- )
                    {
                        RemoveLastLineAndPoint ();
                    }
                }
            }
        }

        if ( Input.GetMouseButtonUp ( 0 ) && CheckPointerOverUIOrObstacleLayer () )
        {
            //if ( points.Count > 0 )
            //{
            //    // ��������� connectionLimit ������ ���� ���� ����������
            
            //    _gameHud.ChangeHealth ( connectionLimit );
            //}

            ClearLinesAndPoints ();
            lastPoint = null;
        }
    }



    private bool CanConnect( Transform point1 , Transform point2 )
    {
        float distance = Vector3.Distance ( point1.position , point2.position );
        if ( distance <= point1.GetComponent<CircleProperties> ().snapRadius )
        {
            if ( point1.GetComponent<CircleProperties> ().GetBallColor () != point2.GetComponent<CircleProperties> ().GetBallColor () )
            {
                return false;
            }

            RaycastHit2D [ ] hits = Physics2D.LinecastAll ( point1.position , point2.position , obstacleLayer );
            foreach ( RaycastHit2D hit in hits )
            {
                if ( hit.collider.transform != point1 && hit.collider.transform != point2 )
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }



    private void DrawLine( Vector3 start , Vector3 end , Color color , Transform hitTransform )
    {
        Debug.Log($"Try draw: {start} -> {end}");
        GameObject line = Instantiate ( linePrefab , Vector3.zero , Quaternion.identity );
        line.transform.position = ( start + end ) / 2;

        Vector3 differenceVector = end - start;
        line.transform.right = differenceVector;

        float distance = Vector3.Distance ( start , end );
        line.transform.localScale = new Vector3 ( distance , 0.1f , 0.1f );

        SpriteRenderer lineRenderer = line.GetComponent<SpriteRenderer> ();
        if ( lineRenderer != null )
        {
            lineRenderer.color = color;
        }

        lines.Add ( line );

        // ������������� isConnected � true ��� ����� �����
        lastPoint.GetComponent<CircleProperties> ().isConnected = true;
        hitTransform.GetComponent<CircleProperties> ().isConnected = true;

        connectionMade = true; // ������������� ���� � true, ��� ��� ���������� �������
    }

    private void ClearLines( )
    {
        foreach ( var line in lines )
        {
            Destroy ( line );
        }
        lines.Clear ();
    }
    private void ClearLinesAndPoints( )
    {
        // ���������� ��� �����
        foreach ( var line in lines )
        {
            Destroy ( line );
        }
        lines.Clear ();

        // ���������� ������ �� �����, ������� ���� ���������
        foreach ( var point in points )
        {
            if ( point != null && point.GetComponent<CircleProperties> ().isConnected )
            {
                ballGenerator.ReplaceBall(point.GetComponent<CircleProperties>());
            }
        }

        if ( connectionMade )
        {
            OnLineEnd?.Invoke(points.Count);
        }

        Debug.Log ( "Score: " + points.Count ); // ������� ������� ���������� ����� � �������

        points.Clear ();
        connectionMade = false; // ���������� ���� ����������
    }

    private void RemoveLastLineAndPoint( )
    {
        if ( lines.Count > 0 )
        {
            GameObject lastLine = lines [ lines.Count - 1 ];
            Destroy ( lastLine );
            lines.RemoveAt ( lines.Count - 1 );
        }

        if ( points.Count > 0 )
        {
            points.RemoveAt ( points.Count - 1 );
        }

        // ��������� ��������� �����
        lastPoint = points.Count > 0 ? points [ points.Count - 1 ] : null;
    }
    private bool CheckPointerOverUIOrObstacleLayer( )
    {
        // �������� UI
        PointerEventData eventData = new PointerEventData ( EventSystem.current );
        eventData.position = Input.mousePosition;
        List<RaycastResult> uiResults = new List<RaycastResult> ();
        EventSystem.current.RaycastAll ( eventData , uiResults );
        bool overUI = uiResults.Count > 0;

        // �������� �������� �� ���� obstacleLayer
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint ( Input.mousePosition );
        RaycastHit2D hit = Physics2D.Raycast ( mousePosition , Vector2.zero , Mathf.Infinity , obstacleLayer );
        bool overObstacleLayer = hit.collider != null;

        return overUI || overObstacleLayer;
    }

}

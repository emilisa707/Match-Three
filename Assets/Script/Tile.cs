using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public float xPosition, yPosition;
    public int column, row;
    public bool isMatched = false;

    private Grid grid;
    private GameObject otherTile;

    private Vector3 firstPosition;
    private Vector3 finalPosition;
    private float swipeAngle;
    private Vector3 tempPosition;

    private int previousColumn, previousRow;

    // Start is called before the first frame update
    void Start()
    {
        grid = FindObjectOfType<Grid>();

        xPosition = transform.position.x;
        yPosition = transform.position.y;

        column = Mathf.RoundToInt((xPosition - grid.startPos.x) / grid.offset.x);
        row = Mathf.RoundToInt((yPosition - grid.startPos.y) / grid.offset.y);
    }

    // Update is called once per frame
    void Update()
    {
        xPosition = (column * grid.offset.x) + grid.startPos.x;
        yPosition = (row * grid.offset.y) + grid.startPos.y;
        SwipeTile();
        Checkmatches();
    }

    void SwipeTile()
    {
        if (Mathf.Abs(xPosition - transform.position.x) > .1)
        {
            tempPosition = new Vector2(xPosition, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            tempPosition = new Vector2(xPosition, transform.position.y);
            transform.position = tempPosition;
            grid.tiles[column, row] = this.gameObject;
        }

        if (Mathf.Abs(yPosition - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, yPosition);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, yPosition);
            transform.position = tempPosition;
            grid.tiles[column, row] = this.gameObject;
        }

        StartCoroutine(CheckMove());
    }

    void Checkmatches()
    {
        if(column > 0 && column < grid.gridSizeX - 1)
        {
            GameObject leftTile = grid.tiles[column - 1, row];
            GameObject rightTile = grid.tiles[column + 1, row];
            if(leftTile != null && rightTile != null)
            {
                if(leftTile.CompareTag(gameObject.tag) && rightTile.CompareTag(gameObject.tag))
                {
                    isMatched = true;
                    rightTile.GetComponent<Tile>().isMatched = true;
                    leftTile.GetComponent<Tile>().isMatched = true;
                }
            }
        }

        if(row > 0 && row < grid.gridSizeY -1)
        {
            GameObject upTile = grid.tiles[column, row + 1];
            GameObject downTile = grid.tiles[column, row - 1];
            if (upTile != null && downTile != null)
            {
                if (upTile.CompareTag(gameObject.tag) && downTile.CompareTag(gameObject.tag))
                {
                    isMatched = true;
                    downTile.GetComponent<Tile>().isMatched = true;
                    upTile.GetComponent<Tile>().isMatched = true;
                }
            }
        }
        if (isMatched)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = Color.grey;
        }
    }
    void OnMouseDown()
    {
        firstPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        finalPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (finalPosition != firstPosition)
        {
            CalculateAngel();
        }
    }

    void CalculateAngel()
    {
        swipeAngle = Mathf.Atan2(finalPosition.y - firstPosition.y, finalPosition.x - firstPosition.x) * 180 / Mathf.PI;
        MoveTile();
    }

    void MoveTile()
    {
        if (swipeAngle > -30 && swipeAngle <= 30)
        {
            SwipeRightMove();
            Debug.Log("Right swipe");
        }
        else if (swipeAngle > 60 && swipeAngle <= 120)
        {
            SwipeUpMove();
            Debug.Log("Up swipe");
        }
        else if (swipeAngle > 150 && swipeAngle >= -150)
        {
            SwipeLeftMove();
            Debug.Log("Left swipe");
        }
        else if (swipeAngle < -60 && swipeAngle >= -120)
        {
            SwipeDownMove();
            Debug.Log("Down swipe");
        }
    }

    void SwipeRightMove()
    {
        otherTile = grid.tiles[column + 1, row];
        otherTile.GetComponent<Tile>().column -= 1;
        column += 1;
    }

    void SwipeUpMove()
    {
        otherTile = grid.tiles[column, row + 1];
        otherTile.GetComponent<Tile>().row -= 1;
        row += 1;
    }

    void SwipeLeftMove()
    {
        otherTile = grid.tiles[column - 1, row];
        otherTile.GetComponent<Tile>().column += 1;
        column -= 1;
    }

    void SwipeDownMove()
    {
        otherTile = grid.tiles[column, row - 1 ];
        otherTile.GetComponent<Tile>().row += 1 ;
        row -= 1;
    }

    IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(.5f);
        if(otherTile !=null)
        {
            if(!isMatched &&!otherTile.GetComponent<Tile>().isMatched)
            {
                otherTile.GetComponent<Tile>().row = row;
                otherTile.GetComponent<Tile>().column = row;
                
                row = previousRow;
                column = previousColumn;
            }
            else
            {
                grid.DestroyMatches();
            }
        }
        otherTile = null;
    }
}
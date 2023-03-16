using UnityEngine;

public class GhostTetromino : MonoBehaviour
{
    void Start()
    {
        tag = "CurrentGhostTetromino";
        foreach (Transform mino in transform)
        {
            mino.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .2f);
        }
    }

    void Update()
    {
        MoveDown();
        FolllowActiveTetromino();
    }

    void FolllowActiveTetromino()
    {
        GameObject currentActive = GameObject.FindGameObjectWithTag("CurrentGhostTetromino");
        if (currentActive != null)
        {
            transform.position = currentActive.transform.position;
            transform.rotation = currentActive.transform.rotation;
        }
    }

    void MoveDown()
    {
        while (CheckIsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }
        if (!CheckIsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0);
        }
    }

    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<GameplayScript>().Round(mino.position);

            if (FindObjectOfType<GameplayScript>().IsInsideGrid(pos) == false)
                return false;

            if (FindObjectOfType<GameplayScript>().GetTransformGridPosition(pos) != null && FindObjectOfType<GameplayScript>().GetTransformGridPosition(pos).parent.tag == "currentActiveTetromino")
                return true;

            if (FindObjectOfType<GameplayScript>().GetTransformGridPosition(pos) != null && FindObjectOfType<GameplayScript>().GetTransformGridPosition(pos).parent != transform)
                return false;
        }
        return true;
    }
}
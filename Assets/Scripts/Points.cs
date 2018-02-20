using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Points : MonoBehaviour {

    [SerializeField]
    Text pointsText;

    [SerializeField]
    private float maxScale = 10;

    [SerializeField]
    private float scaleDuration = 2.0f;

    [SerializeField]
    private float moveDuration = 1.0f;

    bool isScaling = false;
    bool isMoving = false;
    float startTime;

    Vector2 originalSize;
    Vector2 newSize;


    Vector3 finalPosition;
    Vector3 startPosition;

    Text scoreText;
    int totalScore;
    string playerName;

    private void Update()
    {
        if (isScaling)
        {
            float p = (Time.time - startTime) / scaleDuration;
            GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(originalSize, newSize, p);

            if(p > 1)
            {
                isScaling = false;
                StartCoroutine(MoveCoroutine());
            }
        }

        if (isMoving)
        {
            float p = (Time.time - startTime) / moveDuration;
            this.transform.position = Vector3.Lerp(startPosition, finalPosition, p);
            if(p > 1)
            {
                isMoving = false;
                scoreText.text = totalScore.ToString();
            }
        }
    }

    IEnumerator MoveCoroutine()
    {
        yield return new WaitForSeconds(.1f);
        startTime = Time.time;

        isMoving = true;
    }


    public void Setup(int _points, Vector3 _finalPosition, Color _textColor, Text _scoreText, int _totalScore, string _playerName)
    {
        pointsText.text = "+" + _points;
        pointsText.color = _textColor;

        originalSize = this.GetComponent<RectTransform>().sizeDelta;
        newSize = maxScale * originalSize;

        finalPosition = _finalPosition;
        startPosition = this.transform.position;

        totalScore = _totalScore;
        scoreText = _scoreText;
        playerName = _playerName;

        isScaling = true;
        startTime = Time.time;
    }
}

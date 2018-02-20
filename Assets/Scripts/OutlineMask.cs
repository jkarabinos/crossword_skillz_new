using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineMask : MonoBehaviour {

    [SerializeField]
    Image borderImage;

    [SerializeField]
    float moveSpeed = 100;

    [SerializeField]
    int startCorner = 1;

    Vector3 startPosition;

    Vector3[] corners;

    bool isMoving = false;

    private float closeEnough = 10;

	// Use this for initialization
	void Start () {
        startPosition = borderImage.transform.position;

        corners = new Vector3[4];

        float x = borderImage.GetComponent<RectTransform>().sizeDelta.x / 2 - GetComponent<RectTransform>().sizeDelta.x / 5;
        float y = borderImage.GetComponent<RectTransform>().sizeDelta.y / 2 - GetComponent<RectTransform>().sizeDelta.y / 5;

        Vector2 corner1 = new Vector2(-x, -y);
        Vector2 corner2 = new Vector2(-x, y);
        Vector2 corner3 = new Vector2(x, y);
        Vector2 corner4 = new Vector2(x, -y);

        corners[0] = corner1;
        corners[1] = corner2;
        corners[2] = corner3;
        corners[3] = corner4;

        this.transform.localPosition = corners[startCorner];
        borderImage.transform.position = startPosition;
        nextIndex = startCorner;

        isMoving = true;
	}
    int nextIndex;



    // Update is called once per frame
    void Update () {


        if (!isMoving)
            return;

        this.transform.localPosition = Vector2.MoveTowards(this.transform.localPosition, corners[nextIndex], Time.deltaTime * moveSpeed);

        if (Mathf.Abs((this.transform.localPosition - corners[nextIndex]).magnitude) < closeEnough)
            nextIndex = (nextIndex + 1) % 4;

        borderImage.transform.position = startPosition;
	}
}

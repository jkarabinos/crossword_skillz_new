using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SearchingGraphics : MonoBehaviour {

    [SerializeField]
    int gridSize = 4;

    [SerializeField]
    float initAlpha = 50.0f / 255.0f;

    [SerializeField]
    float finalAlpha = 200.0f / 255.0f;

    [SerializeField]
    float tileFadeDuration = .25f;

    Image[,] images;

    bool isAnimating = false;

	// Use this for initialization
	void Awake () {
        images = new Image[gridSize, gridSize];
        int r = 0;
        int c = 0;
        foreach(Transform c1 in this.transform)
        {
            c = 0;
            foreach(Transform c2 in c1)
            {
                images[r, c] = c2.GetComponent<Image>();
                c++;
            }
            r++;
        }
	}

    bool fadingIn = false;
    bool fadingOut = false;

    public void FadeIn()
    {
        //GetComponent<CanvasGroup>().alpha = 1;

        //set all the tiles to their original alpha once enabled
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Image image = images[i, j];
                image.color = new Color(image.color.r, image.color.g, image.color.b, initAlpha);
            }
        }
        CreatePath();

        fadeStartTime = Time.time;

        fadingIn = true;
        fadingOut = false;

        isAnimating = true;
    }

    public void FadeOut()
    {
        //GetComponent<CanvasGroup>().alpha = 0;
        fadeStartTime = Time.time;

        fadingOut = true;
        fadingIn = false;

        isAnimating = false;
    }

    private void OnEnable()
    {
       
    }

    List<List<int>> path;
    bool isFirstLoop = true;

    void CreatePath()
    {
        path = new List<List<int>>();

        int r1 = Random.Range(0, gridSize);
        int row = 0;
        int col = 0;

        if (r1 == 0)
        {
            row = 0;
            col = 0;
        } else if (r1 == 1)
        {
            row = 0;
            col = gridSize - 1;

        } else if (r1 == 2)
        {
            row = gridSize - 1;
            col = 0;
        } else if(r1 == 3)
        {
            row = gridSize - 1;
            col = gridSize - 1;
        }

        int r2 = Random.Range(0, 2);
        int xMul = 0;
        int yMul = 0;

        if (row == 0 && r2 == 0)
            yMul = 1;
        if (row == gridSize - 1 && r2 == 0)
            yMul = -1;
        if (col == 0 && r2 == 1)
            xMul = 1;
        if (col == gridSize - 1 && r2 == 1)
            xMul = -1;

        for(int i = 0; i < gridSize; i++)
        {
            path.Add(new List<int>() { i*xMul + col, i*yMul + row });
        }

        xMul = 0;
        yMul = 0;

        if (col == 0 && r2 == 0)
            xMul = 1;
        if (col == gridSize - 1 && r2 == 0)
            xMul = -1;
        if (row == 0 && r2 == 1)
            yMul = 1;
        if (row == gridSize - 1 && r2 == 1)
            yMul = -1;
        List<int> randTileIndices = path[Random.Range(0, path.Count)];

        for(int i = 1; i < gridSize ; i++)
        {
            path.Add(new List<int>() {i*xMul + randTileIndices[0], i*yMul + randTileIndices[1] });
        }


        isFirstLoop = true;
        startTime = Time.time;
    }

    float startTime;

    float fadeStartTime;
    float fadeDuration = .25f;

    void Fade(bool fadeIn)
    {
        float t = (Time.time - fadeStartTime) / fadeDuration;
        if (fadeIn)
            GetComponent<CanvasGroup>().alpha = Mathf.SmoothStep(0, 1, t);
        else
            GetComponent<CanvasGroup>().alpha = Mathf.SmoothStep(1, 0, t);
    }

    // Update is called once per frame
    void Update () {
        if (fadingIn)
            Fade(true);
        if(fadingOut)
            Fade(false);


        if (!isAnimating)
            return;

        float t = (Time.time - startTime) / tileFadeDuration;
        int index = (int)t;

        if (!isFirstLoop)
            index = index - path.Count;

        if (index >= path.Count && isFirstLoop)
        {
            index = 0;
            isFirstLoop = false;
        }

        else if(index >= path.Count && !isFirstLoop)
        {
            //reset the path
            CreatePath();
            return;
        }
            

        Image tile = images[path[index][0], path[index][1]];
        if(isFirstLoop)
            tile.color = new Color(tile.color.r, tile.color.g, tile.color.b, Mathf.SmoothStep(initAlpha, finalAlpha, (t - index) / tileFadeDuration));
        else
            tile.color = new Color(tile.color.r, tile.color.g, tile.color.b, Mathf.SmoothStep(finalAlpha, initAlpha, (t - index - path.Count) / tileFadeDuration));

    }
}

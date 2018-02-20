using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateZIcon : MonoBehaviour {

    [SerializeField]
    Transform sparkParent;

    [SerializeField]
    GameObject sparkPrefab;

    [SerializeField]
    Transform image1;

    [SerializeField]
    Transform image2;

    [SerializeField]
    float timeBetweenSparks = .25f;

    [SerializeField]
    float speed = 2.0f;

    List<GameObject> sparks = new List<GameObject>();
    float lastTime;

    Transform currentImage;
    Transform hiddenImage;

    //Vector2 targetImagePos;

	// Use this for initialization
	void Start () {
        lastTime = Time.time;

        currentImage = image1;
        hiddenImage = image2;


        hiddenImage.localPosition = new Vector2(0, -image1.GetComponent<RectTransform>().rect.size.y );
       
	}
	
	// Update is called once per frame
	void Update () {
        if ((Time.time - lastTime) > timeBetweenSparks)
            CreateSpark();

        List<GameObject> tempSparks = CopyList(sparks);

        foreach(GameObject s in tempSparks)
        {
            MoveSpark(Time.deltaTime, s);
        }

        MoveImages();
	}


    void MoveImages()
    {
        float dt = Time.deltaTime;

        Vector2 targetImagePos = new Vector2(0, image1.GetComponent<RectTransform>().rect.size.y);

        currentImage.localPosition = Vector2.MoveTowards(currentImage.localPosition, targetImagePos, dt * (speed / 6));
        hiddenImage.localPosition = Vector2.MoveTowards(hiddenImage.localPosition, targetImagePos, dt * (speed / 6));

        if (((Vector2)currentImage.localPosition - targetImagePos).magnitude < 3)
        {

            currentImage.localPosition = new Vector2(0, hiddenImage.localPosition.y - image1.GetComponent<RectTransform>().rect.size.y);

            Transform temp = hiddenImage;

            hiddenImage = currentImage;

            currentImage = temp;
        }


    }

    List<GameObject> CopyList(List<GameObject> list)
    {
        List<GameObject> newList = new List<GameObject>();

        foreach (GameObject go in list)
            newList.Add(go);

        return newList;
    }

    void MoveSpark(float deltaTime, GameObject s)
    {
        Vector2 target = new Vector2(s.transform.localPosition.x, sparkParent.GetComponent<RectTransform>().rect.size.y / 2);

        //Debug.Log("the target pos is " + target);

        s.transform.localPosition = Vector2.MoveTowards(s.transform.localPosition, target, deltaTime * speed);

        if (((Vector2)s.transform.localPosition - target).magnitude < 3)
        {
            sparks.Remove(s);
            Destroy(s);
        }
    }

    void CreateSpark()
    {
        lastTime = Time.time;

        GameObject spark = Instantiate(sparkPrefab, sparkParent, false);

        float w = sparkParent.GetComponent<RectTransform>().rect.size.x / 2;

        spark.transform.localPosition = new Vector2(Random.Range(-w, w), -sparkParent.GetComponent<RectTransform>().rect.size.y / 2);

        sparks.Add(spark);
    }
}

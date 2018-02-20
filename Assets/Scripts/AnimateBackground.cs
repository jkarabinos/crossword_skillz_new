using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateBackground : MonoBehaviour {

    [SerializeField]
    Image borderImage;

    [SerializeField]
    Image borderImage2;

    [SerializeField]
    bool shouldAnimate;

    [SerializeField]
    Sprite[] gifImages;

    [SerializeField]
    float imageDuration = .1f;

    [SerializeField]
    float waitDuration = .5f;

    float startTime;
    int lastIndex = 0;
    int count = 0;

	// Use this for initialization
	void Start () {

        startTime = Time.time;
	}
	
    public void SetAnimation(bool _shouldAnimate)
    {
        shouldAnimate = _shouldAnimate;

        if (!shouldAnimate)
        {
            borderImage.color = new Color(1, 1, 1, 0);
            borderImage2.color = new Color(1, 1, 1, 0);
        }
    }


    public void SetInitialBorderImages(Sprite[] sprites)
    {
        borderImage.sprite = sprites[1];
        borderImage2.sprite = sprites[2];

        gifImages = new Sprite[sprites.Length - 1];

        for(int i = 0; i < sprites.Length; i++)
        {
            if (i != 0)
                gifImages[i - 1] = sprites[i]; 
        }
    }

   

	// Update is called once per frame
	void Update () {

		if(shouldAnimate)
        {

            float p = (Time.time - startTime) / (imageDuration + waitDuration);
            int index = (int)p;
            int index2 = GetNextIndex(index);

            if(index != lastIndex)
            {
                //change the image that currently has alpha value close to zero

                if (count % 2 == 0)
                    borderImage.sprite = gifImages[index2];
                else
                    borderImage2.sprite = gifImages[index2];

                count++;
                lastIndex = index;
            }

            float t = (p - index) * ((waitDuration + imageDuration) / imageDuration);

            

            int a1 = 0;
            int a2 = 1;

            if(count % 2 == 0)
            {
                a2 = 0;
                a1 = 1;
            }

            borderImage.color = new Color(1, 1, 1, Mathf.SmoothStep(a1, a2, t));
            borderImage2.color = new Color(1, 1, 1, Mathf.SmoothStep(a2, a1, t));



        }
	}


    int GetNextIndex(int i)
    {
        i++;

        i = i % gifImages.Length;

        return i;
    }
    
}

using UnityEngine;
using UnityEngine.UI;

public class ComboText : TextToImage
{
    public override int ChangeTextToImage(int someInt)
    {
        if (!isInitialized)
        {
            textImage = transform.GetComponentsInChildren<Image>();
            imageRectTransform = new RectTransform[textImage.Length];
            for (int i = 0; i < imageRectTransform.Length; ++i)
            {
                imageRectTransform[i] = textImage[i].GetComponent<RectTransform>();
            }
            rectTransform = transform.GetComponent<RectTransform>();
            isInitialized = true;
            initialSize = rectTransform.sizeDelta;
            textImage[0].sprite = data.x;
        }

        foreach (var image in textImage)
        {
            image.gameObject.SetActive(false);
        }

        for (int i = 0; i < textImage.Length; ++i)
        {
            textImage[i].gameObject.SetActive(false);
            if (targetWidth == default && targetHeight == default)
            {
                imageRectTransform[i].sizeDelta = new Vector2(initialSize.x / textImage.Length, initialSize.y);
            }
            else if (targetHeight == default)
            {
                imageRectTransform[i].sizeDelta = new Vector2(targetWidth, initialSize.y);
            }
            else if (targetWidth == default)
            {
                imageRectTransform[i].sizeDelta = new Vector2(initialSize.x / textImage.Length, targetHeight);
            }
            else
            {
                imageRectTransform[i].sizeDelta = new Vector2(targetWidth, targetHeight);
            }
        }
        int imageIndex = 1;
        string someIntToString = someInt.ToString();
        rectTransform.sizeDelta = imageRectTransform[0].sizeDelta * (someIntToString.Length + 1);
        textImage[0].gameObject.SetActive(true);
        for (int i = 0; i < someIntToString.Length; ++i)
        {
            textImage[imageIndex].gameObject.SetActive(true);
            //아스키코드 변환된 int값이 나와서 가장 최초의 값인 0의 아스키코드 값을 빼줌
            textImage[imageIndex].sprite = data.numberSprites[someIntToString[i] - '0'];
            imageIndex++;
        }
        return imageIndex;
    }
}

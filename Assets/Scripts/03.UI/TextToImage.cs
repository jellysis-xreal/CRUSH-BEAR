using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class TextToImage : MonoBehaviour
{
    [SerializeField] protected SpriteData data;
    protected Image[] textImage;
    [SerializeField] protected float targetWidth, targetHeight;
    protected RectTransform[] imageRectTransform;
    protected RectTransform rectTransform;
    protected Vector2 initialSize;
    protected bool isInitialized = false;

    public virtual int ChangeTextToImage(int someInt)
    {
        if(!isInitialized)
        {
            textImage = transform.GetComponentsInChildren<Image>();
            imageRectTransform = new RectTransform[textImage.Length];
            for(int i = 0; i < imageRectTransform.Length; ++i)
            {
                imageRectTransform[i] = textImage[i].GetComponent<RectTransform>();
            }
            rectTransform = transform.GetComponent<RectTransform>();
            isInitialized = true;
            initialSize = rectTransform.sizeDelta;
        }

        foreach (var image in textImage) 
        { 
            image.gameObject.SetActive(false);
        }

        for(int i = 0; i < textImage.Length; ++i)
        {
            textImage[i].gameObject.SetActive(false);
            if(targetWidth == default && targetHeight == default)
            {
                imageRectTransform[i].sizeDelta = new Vector2(initialSize.x / textImage.Length, initialSize.y);
            }
            else if(targetHeight == default)
            {
                imageRectTransform[i].sizeDelta = new Vector2(targetWidth, initialSize.y);
            }
            else if(targetWidth == default)
            {
                imageRectTransform[i].sizeDelta = new Vector2(initialSize.x / textImage.Length, targetHeight);
            }
            else
            {
                imageRectTransform[i].sizeDelta = new Vector2(targetWidth, targetHeight);
            }
        }
        int imageIndex = 0;
        string someIntToString = someInt.ToString();
        rectTransform.sizeDelta = imageRectTransform[0].sizeDelta * someIntToString.Length;
        for(int i = 0; i < someIntToString.Length; ++i)
        {
            textImage[imageIndex].gameObject.SetActive(true);          
            //아스키코드 변환된 int값이 나와서 가장 최초의 값인 0의 아스키코드 값을 빼줌
            textImage[imageIndex].sprite = data.numberSprites[someIntToString[i] - '0'];
            imageIndex++;
            if (someIntToString.Length - (i + 1) != 0 && (someIntToString.Length - (i + 1)) % 3 == 0)
            {
                textImage[imageIndex].sprite = data.comma;
                textImage[imageIndex].gameObject.SetActive(true);
                imageRectTransform[imageIndex].sizeDelta = new Vector2(imageRectTransform[i].sizeDelta.x * 0.4f, imageRectTransform[i].sizeDelta.y);
                imageIndex++;
            }
        }
        return imageIndex;
    }

    public int ChangeTextToImage(float someFloat)
    {
        int intergerPart = (int)someFloat;
        int index = ChangeTextToImage(intergerPart);
        int decimalPoint = (int)((someFloat - intergerPart) * 10);
        textImage[++index].gameObject.SetActive(true);
        textImage[index].sprite = data.dot;
        textImage[++index].gameObject.SetActive(true);
        textImage[index].sprite = data.numberSprites[decimalPoint];
        return index;
    }
}

[CreateAssetMenu(fileName = "SpriteData", menuName = "ScriptableObjects/SpriteData", order = 1)]
public class SpriteData : ScriptableObject
{
    public Sprite[] numberSprites;
    public Sprite comma;
    public Sprite dot;
    public Sprite x;
}

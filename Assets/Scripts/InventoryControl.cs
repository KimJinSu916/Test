using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventoryControl : MonoBehaviour
{
    public CharacterControl Player;
    public Sprite basicIcon;
    public Image[] itmeIcon;
    public Text[] itmetext;
    public int[] itmenum;

    public Image selectedImage;
    public RectTransform duplicationSelectedImageRect;
    public RectTransform locationToMove;
    public bool isItemMove = false;

    void Awake()
    {
        itmeIcon = new Image[transform.childCount];
        itmetext = new Text[transform.childCount];

        for (int itmeIconArray = 0; itmeIconArray < itmeIcon.Length; itmeIconArray++)
        {//$"{a}"
            itmeIcon[itmeIconArray] = transform.Find($"Icons_{itmeIconArray}").transform.GetChild(0).GetComponent<Image>();
            itmetext[itmeIconArray] = transform.Find($"Icons_{itmeIconArray}").transform.GetChild(1).GetComponent<Text>();
            itmeIcon[itmeIconArray].sprite = basicIcon;
        }
        GameObject newOBJ = new GameObject();
        newOBJ.AddComponent<RectTransform>().parent = transform.parent;
        newOBJ.AddComponent<Image>().raycastTarget = false;
        duplicationSelectedImageRect = newOBJ.GetComponent<RectTransform>();
        duplicationSelectedImageRect.gameObject.SetActive(false);

        if (transform.parent.gameObject.activeSelf)
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
    public void AddItemToInventory(Sprite itemSprite)
    {
        for (int itmeIconArray = 0; itmeIconArray < itmeIcon.Length; itmeIconArray++)
        {
            if(itmeIcon[itmeIconArray].GetComponent<Image>().sprite == itemSprite)
            {
                itmetext[itmeIconArray].text = string.Format("x{0}", ++itmenum[itmeIconArray]);
                break;
            }
            else if (itmeIcon[itmeIconArray].GetComponent<Image>().sprite == basicIcon)
            {
                itmeIcon[itmeIconArray].GetComponent<Image>().sprite = itemSprite;
                itmetext[itmeIconArray].text = string.Format("x{0}", ++itmenum[itmeIconArray]);
                break;
            }
        }
    }
    public void UI_OnPointerEnter(RectTransform currentRect)
    {
        locationToMove = currentRect;
    }
    public void UI_OnPointerExit()
    {
        locationToMove = null;
    }
    public void UI_OnPointerDown(Image currentImage)
    {
        locationToMove = null;
        selectedImage = currentImage;
    }
    public void UI_OnPointerUp(Image currentImage)
    {
        if (isItemMove)
        {
            isItemMove = false;
            if (locationToMove)
            {
                for(int i=0;i<itmeIcon.Length; i++)
                {
                   
                    if(currentImage.transform.parent.name == $"Icons_{i}")
                    {
                        int n,d;
                        string c = locationToMove.transform.parent.name.Substring(6, 1), cs;

                        int.TryParse(c, out n);
                        Debug.Log(i+1 + " -> " + (n+1));
                        Sprite spriteSwap = locationToMove.GetComponent<Image>().sprite;
                        locationToMove.GetComponent<Image>().sprite = selectedImage.sprite;
                        selectedImage.sprite = spriteSwap;

                        int temp_n = itmenum[n];
                        itmenum[n] = itmenum[i]; //갯수 옮김
                        itmenum[i] = temp_n;

                        string temp_s = itmetext[n].text;
                        itmetext[n].text = itmetext[i].text;//텍스트 옮김
                        itmetext[i].text = temp_s;
                        break;
                    }
                }
            }
            duplicationSelectedImageRect.GetComponent<Image>().sprite = null;
            duplicationSelectedImageRect.gameObject.SetActive(isItemMove);
            selectedImage.enabled = true;
        }
        else
        {
            for(int i = 0; i< itmeIcon.Length; i++)
            {
                if (currentImage.transform.parent.name == $"Icons_{i}" && itmenum[i] > 0)
                {
                    itmenum[i]--;
                    Debug.Log($"아이템 선택:{selectedImage.sprite.name} 남은 갯수:{itmenum[i]}");
                    if (currentImage.sprite.name == "apple")
                    {
                        Player.PlayerHP += 10;
                    }
                    else if (currentImage.sprite.name == "hp")
                    {
                        Player.PlayerHP += 50;
                    }
                    itmetext[i].text = string.Format("x{0}", itmenum[i]);
                    if (itmenum[i] == 0 )
                    {
                        currentImage.sprite = basicIcon;
                        itmetext[i].text = "";
                    }
                    break;
                }
            }
        }
    }
    public void UI_OnDrag(BaseEventData eventData)
    {
        if (selectedImage.sprite == basicIcon)
        {
            return;
        }
        PointerEventData pointerEventData = eventData as PointerEventData;

        if (!isItemMove)
        {
            selectedImage.enabled = false;

            isItemMove = true;
            duplicationSelectedImageRect.GetComponent<Image>().sprite = selectedImage.sprite;
            duplicationSelectedImageRect.position = pointerEventData.position;
            duplicationSelectedImageRect.gameObject.SetActive(isItemMove);
        }
        duplicationSelectedImageRect.localPosition += new Vector3(pointerEventData.delta.x, pointerEventData.delta.y, 0);
    }
}
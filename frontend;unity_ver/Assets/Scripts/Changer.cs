using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Changer : MonoBehaviour
{
    public SpriteRenderer bodyParts;
    public List<Sprite> option = new List<Sprite>();
    private int currentOption = 0;
    public void NextOption(){
        currentOption++;
        if(currentOption >= option.Count){
            currentOption = 0;
        }
        bodyParts.sprite = option[currentOption];
    }
    public void PreviousOption(){
        currentOption--;
        if(currentOption <=0){
            currentOption = option.Count -1;
        }
        bodyParts.sprite = option[currentOption];
    }
}

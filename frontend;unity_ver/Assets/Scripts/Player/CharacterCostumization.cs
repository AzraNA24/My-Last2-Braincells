using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCustomization", menuName = "Customization/CharacterCustomization")]
public class CharacterCustomization : ScriptableObject
{
    [System.Serializable]
    public class CustomizationOption
    {
        public int optionId;
        public RuntimeAnimatorController animatorController;
        public Sprite previewSprite;
    }

    [System.Serializable]
    public class CustomizationType
    {
        public int typeId;
        public string typeName;
        public CustomizationOption[] options;
    }

    public CustomizationType[] customizationTypes;
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraveUICard : MonoBehaviour
{
    [SerializeField] Image character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;

    public void Setup(CardData data)
    {
        character.sprite = data.sprite;
        nameTMP.text = data.name;
        attackTMP.text = data.attack.ToString();
        healthTMP.text = data.health.ToString();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraveUICard : MonoBehaviour
{
    [SerializeField] Image character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text healthTMP;

    public void Setup(Item item)
    {
        character.sprite = item.sprite;
        nameTMP.text = item.name;
        attackTMP.text = item.attack.ToString();
        healthTMP.text = item.health.ToString();
    }
}

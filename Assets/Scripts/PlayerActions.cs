using UnityEngine;
using TMPro;

public class PlayerActions : MonoBehaviour
{
    public void OnAttackButton()
    {
        // español: Este método se llama cuando se presiona el botón de ataque.
        // english: This method is called when the attack button is pressed.
        Debug.Log("Atacar");
    }

    public void OnJumpButton()
    {
        // español: Este método se llama cuando se presiona el botón de salto.
        // english: This method is called when the jump button is pressed.
        Debug.Log("Saltar");
    }

    public void OnDodgeButton()
    {
        // español: Este método se llama cuando se presiona el botón de esquivar.
        // english: This method is called when the dodge button is pressed.
        Debug.Log("Esquivar");
        
    }
}

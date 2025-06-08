using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInstance : MonoBehaviour
{
    private GameObject playerCharacter;

    public void SetCharacter(GameObject character)
    {
        playerCharacter = character;
        playerCharacter.GetComponent<CarInput>().SetPlayerInputRef(GetComponent<PlayerInput>());
    }
}

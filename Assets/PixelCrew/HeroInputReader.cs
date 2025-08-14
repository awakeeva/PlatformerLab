using UnityEngine;
using UnityEngine.InputSystem;

public class HeroInputReader : MonoBehaviour
{
    [SerializeField] private Hero _hero;

    private void OnHorizontalMovement(InputValue context)
    {
        var direction = context.Get<float>();
        _hero.SetDirection(direction);
    }
    
    private void OnSaySomething(InputValue context)
    {
        _hero.SaySomething();
    }
}

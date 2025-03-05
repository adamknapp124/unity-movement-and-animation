using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementControl : MonoBehaviour
{
  PlayerInput playerInput;
  CharacterController characterController;
  Animator animator;

  Vector2 currentMovementInput;
  Vector3 currentMovement;
  bool isMovementPressed;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Awake()
  {
    playerInput = new PlayerInput();
    characterController = GetComponent<CharacterController>();
    animator = GetComponent<Animator>();

    playerInput.Land.Move.started += context => { onMovementInput(context); };
    playerInput.Land.Move.canceled += context => { onMovementInput(context); };
    playerInput.Land.Move.performed += context => { onMovementInput(context); };
  }

  void onMovementInput(InputAction.CallbackContext context)
  {
    currentMovementInput = context.ReadValue<Vector2>();
    currentMovement.x = currentMovementInput.x;
    currentMovement.z = currentMovementInput.y;
    isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
  }

  void OnEnable()
  {
    playerInput.Land.Enable();
  }

  void OnDisable()
  {
    playerInput.Land.Disable();
  }

  void Update()
  {
    if (isMovementPressed)
    {
      animator.SetBool("isWalking", true);
      characterController.Move(currentMovement * Time.deltaTime);
    }
    else
    {
      animator.SetBool("isWalking", false);
    }
  }
}

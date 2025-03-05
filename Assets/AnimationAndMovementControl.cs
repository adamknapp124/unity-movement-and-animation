using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class AnimationAndMovementControl : MonoBehaviour
{
  PlayerInput playerInput;
  CharacterController characterController;
  Animator animator;

  int isWalkingHash = Animator.StringToHash("isWalking");
  int isRunningHash = Animator.StringToHash("isRunning");

  Vector2 currentMovementInput;
  Vector3 currentMovement;
  Vector3 currentRunMovement;
  bool isMovementPressed;
  bool isRunPressed;
  float rotationFactorPerFrame = 15.0f;
  float runMultiplier = 3.0f;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Awake()
  {
    playerInput = new PlayerInput();
    characterController = GetComponent<CharacterController>();
    animator = GetComponent<Animator>();

    playerInput.Land.Move.started += context => { onMovementInput(context); };
    playerInput.Land.Move.canceled += context => { onMovementInput(context); };
    playerInput.Land.Move.performed += context => { onMovementInput(context); };

    playerInput.Land.Run.started += onRun;
    playerInput.Land.Run.canceled += onRun;
  }

  void onMovementInput(InputAction.CallbackContext context)
  {
    currentMovementInput = context.ReadValue<Vector2>();
    currentMovement.x = currentMovementInput.x;
    currentMovement.z = currentMovementInput.y;
    currentRunMovement.x = currentMovementInput.x * runMultiplier;
    currentRunMovement.z = currentMovementInput.y * runMultiplier;
    isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
  }

  void onRun(InputAction.CallbackContext context)
  {
    isRunPressed = context.ReadValueAsButton();
  }

  void handleAnimation()
  {
    bool isWalking = animator.GetBool(isWalkingHash);
    bool isRunning = animator.GetBool(isRunningHash);

    if (isMovementPressed && !isWalking) { animator.SetBool(isWalkingHash, true); }
    else if (!isMovementPressed && isWalking) { animator.SetBool(isWalkingHash, false); }

    if ((isMovementPressed && isRunPressed) && !isRunning) { animator.SetBool(isRunningHash, true); }
    else if ((!isMovementPressed || !isRunPressed) && isRunning) { animator.SetBool(isRunningHash, false); }
  }

  void handleRotation()
  {
    Vector3 positionToLookAt;

    positionToLookAt.x = currentMovement.x;
    positionToLookAt.y = 0.0f;
    positionToLookAt.z = currentMovement.z;

    Quaternion currentRotation = transform.rotation;

    if (isMovementPressed)
    {
      Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
      transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
    }
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
    handleRotation();
    handleAnimation();
    if (isRunPressed) { characterController.Move(currentRunMovement * Time.deltaTime); }
    else { characterController.Move(currentMovement * Time.deltaTime); }

  }
}

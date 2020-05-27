using Assets.Scripts;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerSpy : MonoBehaviour
{
    #region Class variables
    [SerializeField]
    private float jumpHeight = 5f;
    [SerializeField]
    private Rigidbody playerBody;
    [SerializeField]
    private Collider playerCollider;
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private float movementSpeedMultiplier = 1f;
    [SerializeField]
    private float positionChangeSpeedMultiplier = 10f;
    [SerializeField]
    private float runMovementSpeed = 10f;
    [SerializeField]
    private float walkMovementSpeed = 5f;
    [SerializeField]
    private float crouchMovementSpeed = 4f;
    [SerializeField]
    private float crawlMovementSpeed = 2.5f;
    [SerializeField]
    private float maxInteractionDistance = 3f;

    private Vector3 inputVector;
    private Vector3 bodyScale;

    private float xAxisMovement;
    private float zAxisMovement;
    private float distanceToTheGround;
    private float positionChangeSpeed;

    private bool isJumping;
    private bool isCrouching;
    private bool isCrawling;
    private bool isRunning;
    private bool isOnGround;
    private bool isInteracting;

    private IInteractableComponent lastInteractableComponent;
    #endregion

    #region Unity methods
    // Start is called before the first frame update
    void Start()
    {
        // Initialize player vody and collider if not specified in inspector
        if (!playerBody)
        {
            playerBody = GetComponent<Rigidbody>();
        }
        if (!playerCollider)
        {
            playerCollider = GetComponent<Collider>();
        }
        if (!playerCamera)
        {
            playerCamera = Camera.main;
        }
        //playerBody.interpolation = RigidbodyInterpolation.Extrapolate;
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        HandleInteractionsUsingRayCast();
    }

    void FixedUpdate()
    {
        HandlePlayerMovement();
    }
    #endregion

    #region Custom methods
    void Initialize()
    {
        xAxisMovement = Input.GetAxis("Horizontal");
        zAxisMovement = Input.GetAxis("Vertical");
        isJumping = Input.GetButtonDown("Jump");
        isCrouching = Input.GetButton("Crouch");
        isCrawling = Input.GetButton("Crawl");
        isRunning = Input.GetButton("Run");
        isInteracting = Input.GetButton("Interact");

        positionChangeSpeed = positionChangeSpeedMultiplier * Time.deltaTime;

        distanceToTheGround = playerCollider.bounds.extents.y;
        isOnGround = Physics.Raycast(transform.position, Vector3.down, distanceToTheGround + 0.1f);
    }

    void HandlePlayerMovement()
    {
        HandleJump();
        HandlePosture();
        ChangeVelocity();
    }

    void HandleJump()
    {
        if (CheckIfEligableForJump())
        {
            playerBody.AddForce(Vector3.up * jumpHeight , ForceMode.VelocityChange);
        }
    }

    bool CheckIfEligableForJump()
    {
        return isJumping && isOnGround && !isCrawling && !isCrouching;
    }

    void HandlePosture()
    {
        HandleCrouchingPosture();
        HandleLyingPosture();
        HandleStandingPosture();
    }

    void ChangeVelocity()
    {
        inputVector = transform.right * (xAxisMovement * movementSpeedMultiplier) +
                        transform.up * playerBody.velocity.y +
                        transform.forward * (zAxisMovement * movementSpeedMultiplier);
        playerBody.velocity = inputVector;
    }

    void HandleCrouchingPosture()
    {
        if (isCrouching)
        {
            movementSpeedMultiplier = crouchMovementSpeed;

            bodyScale = new Vector3(1f, 0.6f, 1f); //TODO take care of that, Keep creating new ones or keep those as object params?, prob will change upon model change
            playerBody.transform.localScale = Vector3.Lerp(transform.localScale, bodyScale, positionChangeSpeed);
        }
    }
    
    void HandleLyingPosture()
    {
        if (isCrawling)
        {
            movementSpeedMultiplier = crawlMovementSpeed;

            //TODO crawling is floating above floor level - investigate why
            bodyScale = new Vector3(1f, 0.4f, 1f);  //TODO take care of that, Keep creating new ones or keep those as object params?, prob will change upon model change
            playerBody.transform.localScale = Vector3.Lerp(transform.localScale, bodyScale, positionChangeSpeed / 2);
            bodyScale = new Vector3(1f, 0.4f, 2.5f); //TODO take care of that, Keep creating new ones or keep those as object params?, prob will change upon model change
            playerBody.transform.localScale = Vector3.Lerp(transform.localScale, bodyScale, positionChangeSpeed / 2);

        }
    }

    void HandleStandingPosture()
    {
        if (!isCrawling && !isCrouching)
        {
            if (isRunning)
            {
                movementSpeedMultiplier = runMovementSpeed;
            }
            if (!isRunning)
            {
                movementSpeedMultiplier = walkMovementSpeed;
            }

            bodyScale = new Vector3(1f, 1f, 1f); //TODO take care of that, Keep creating new ones or keep those as object params?, prob will change upon model change
            playerBody.transform.localScale = Vector3.Lerp(transform.localScale, bodyScale, positionChangeSpeed * 2f);
        }
    }

    void HandleInteractionsUsingRayCast()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Debug.DrawRay(ray.origin, ray.direction * maxInteractionDistance, Color.magenta);

        //Cast ray and return hit object
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            //Check distance condition
            if (CheckIfInteractableObjectIsInRange(raycastHit))
            {
                IInteractableComponent interactableComponent = raycastHit.transform.gameObject.GetComponent<IInteractableComponent>();

                //Higlight hit object and undo highlight if player stoped looking a object
                if (interactableComponent != null && lastInteractableComponent != interactableComponent)
                {
                    interactableComponent.HandleHover(true);
                    RemoveInteractableObjectHoverEffect();
                    lastInteractableComponent = interactableComponent;
                }

                //Handle click if interactable component was found
                if (interactableComponent != null && isInteracting)
                {
                    interactableComponent.HandleClick();
                }

            }
            else
            {
                //Undo hover effect if player moved too far from object
                RemoveInteractableObjectHoverEffect();
            }
        }
        else 
        {
            //Undo hover effect if player is not looking at any object (ray hit nothing)
            RemoveInteractableObjectHoverEffect();
        }
    }

    void RemoveInteractableObjectHoverEffect()
    {
        if (lastInteractableComponent != null)
        {
            lastInteractableComponent.HandleHover(false);
            lastInteractableComponent = null;
        }
    }

    //TODO Think of moving this to some knd of helpers class
    bool CheckIfInteractableObjectIsInRange(RaycastHit raycastHit)
    {
        return raycastHit.distance <= maxInteractionDistance;
    }
    #endregion
}   

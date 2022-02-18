using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

#pragma warning disable 618, 649
namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] sand_sounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip[] concrete_sounds;  
        [SerializeField] private AudioClip[] wood_sounds;  
        [SerializeField] private AudioClip[] carpet_sounds;  
        [SerializeField] private AudioClip[] stair_sounds;  
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        [SerializeField] private GameObject player_text;
        [SerializeField] private GameObject sit_icon; 
        [SerializeField] private GameObject space_icon; 
        [SerializeField] private GameObject storm;
        [SerializeField] private GameObject reverbzone1;
        [SerializeField] private GameObject reverbzone2;
        
        private AudioClip[] m_FootstepSounds;

        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        private GameObject conversing_npc;
        private Renderer npc_renderer;

        private bool in_conversation;
        private bool last_conversation_active;
        private bool offscreen_chat;

        private Vector3 lastMoveDir = Vector3.zero;
        private bool sitting;
        private Vector3 lastPos = Vector3.zero;
        private bool sit_used;
        private bool space_used;
        private bool sittable_highlighted = false;
        private bool reverb_triggered;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
            in_conversation = false;
            EventManager.StartListening("ConversationStarted", StartConversation);
            EventManager.StartListening("ConversationEnded", EndConversation);
            EventManager.StartListening("LastConversationEnded", EndConversation);
            EventManager.StartListening("DespawnHouse", DespawnedHouse);
            offscreen_chat = false;
            sitting = false;
            sit_icon.GetComponent<CanvasRenderer>().SetAlpha(0);
            space_icon.GetComponent<CanvasRenderer>().SetAlpha(0);
            sit_used = false;
            space_used = false;
            reverb_triggered = false;
        }


        // Update is called once per frame
        private void Update()
        {
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            // if (!m_Jump)
            // {
            //     m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            // }
            RaycastHit floor_hit;
            if (Physics.Raycast(transform.position, Vector3.down, out floor_hit, 1)) {
                string tag = floor_hit.collider.tag;
                m_StepInterval = 5;
                if (tag == "terrain") {
                    m_LandSound = sand_sounds[0];
                    m_FootstepSounds = sand_sounds;
                } else if (tag == "concrete") {
                    m_LandSound = concrete_sounds[0];
                    m_FootstepSounds = concrete_sounds;
                } else if (tag == "carpet") {
                    m_LandSound = carpet_sounds[0];
                    m_FootstepSounds = carpet_sounds;
                } else if (tag == "stairs") {
                    m_LandSound = stair_sounds[0];
                    m_FootstepSounds = stair_sounds;
                    m_StepInterval = 3;
                } else if (tag == "wood") {
                    m_LandSound = wood_sounds[0];
                    m_FootstepSounds = wood_sounds;
                }
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            if (sitting && Input.GetKeyDown(KeyCode.Space)) {
                if (!space_used) {
                    space_used = true;
                    StartCoroutine(LerpImageAlpha(0, .1f, space_icon)); 
                }
                StartCoroutine(LerpToMonitorPos(lastPos, 0.3f));
            }

            if (!sit_used || Input.GetKeyDown(KeyCode.E)) {
                Ray rayOrigin1 = m_Camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                RaycastHit hit1;
                if (Physics.Raycast(rayOrigin1, out hit1, 5f))
                {
                    if (hit1.collider.gameObject.tag == "sittable") {
                        // Debug.Log("HERE1");
                        if (Vector3.Distance(m_CharacterController.transform.position, hit1.point) < 1.6f) {
                            if (!sitting) {
                                if (!sit_used) {
                                    sittable_highlighted = true;
                                    StartCoroutine(LerpImageAlpha(.85f, .1f, sit_icon));
                                }
                                if (Input.GetKeyDown(KeyCode.E)) {
                                    lastPos = m_CharacterController.transform.position;
                                    sit_used = true;
                                    if (!space_used) {
                                        StartCoroutine(LerpImageAlpha(.85f, .1f, space_icon)); 
                                    }
                                    StartCoroutine(LerpToMonitorPos(hit1.point, 0.3f));
                                    StartCoroutine(LerpImageAlpha(0, .1f, sit_icon)); 
                                }
                            }
                        }
                    
                    } else {
                        // Debug.Log("HERE2");
                        
                        if (sittable_highlighted & !sit_used) {
                            sittable_highlighted = false;
                            StartCoroutine(LerpImageAlpha(0, .1f, sit_icon)); 
                        }
                    }
                }
            }
            
            

            if (Input.GetMouseButtonDown(0)) {
                // Debug.Log("yes?");
                    Ray rayOrigin = m_Camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                    RaycastHit hit;
                    if (Physics.Raycast(rayOrigin, out hit, 5))
                    {
                        // Debug.Log(hit.collider.gameObject.name + ": " + hit.point);
                        if (!in_conversation && hit.collider.CompareTag("NPC"))
                        {
                            Debug.Log("hit NPC");

                            EventManager.TriggerEvent("RequestingConversation", 
                                new Dictionary<string, object> {
                                    { "player", gameObject },
                                    { "npc", hit.collider.gameObject }
                                });

                            // hit.collider.gameObject.GetComponent<NPCAIController>().StartConversation(gameObject);
                            
                        }
                    }
                    
                
            }

            if (in_conversation) {
                CheckNPCDistance();
            }
        }

        private void LateUpdate() {
            storm.transform.position = gameObject.transform.position;
        }

        private void StartConversation(Dictionary<string, object> message) {
            conversing_npc = (GameObject) message["object"];
            npc_renderer = conversing_npc.GetComponentInChildren<Renderer>();
            in_conversation = true;
            Conversation.player_text = player_text.GetComponent<TMPro.TextMeshProUGUI>();
        }

        private void EndConversation(Dictionary<string, object> message) {
            in_conversation = false;
            conversing_npc = null;
            npc_renderer = null;
        }

        private void CheckNPCDistance() {
            Vector3 player_npc_vector = conversing_npc.transform.position - gameObject.transform.position;
            float player_npc_angle = Vector3.SignedAngle(player_npc_vector, gameObject.transform.forward, Vector3.up);
            //  !conversing_npc.GetComponent<NPCAIController>().npc_canvas.GetComponent<Renderer>().isVisible

            if (Vector3.Distance(gameObject.transform.position, conversing_npc.transform.position) > 3.0f || (player_npc_angle > 52.0f || player_npc_angle < -52.0f)) {
                if (!offscreen_chat) {
                    offscreen_chat = true;
                    EventManager.TriggerEvent("TextOffscreen", 
                        new Dictionary<string, object> {
                            {"offscreen", true}
                        });
                }

            } else {
                if (offscreen_chat) {
                    offscreen_chat = false;
                    EventManager.TriggerEvent("TextOffscreen", 
                        new Dictionary<string, object> {
                            {"offscreen", false}
                        });
                }
            }
        }

        private void DespawnedHouse(Dictionary<string, object> message) {
            sitting = false;
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            
            if (!sitting) {
                m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

                ProgressStepCycle(speed);
            }
            
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }

            if (hit.gameObject.tag == "NPC") {
                return;
            }

            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

        IEnumerator LerpToMonitorPos (Vector3 target_pos, float duration) {
            
            float time = 0;
            Vector3 startPosition = m_CharacterController.transform.position;
            
            while (time < duration) {
                m_CharacterController.transform.position = Vector3.Lerp(startPosition, target_pos, time/duration);
                
                time += Time.deltaTime;
                yield return null;
            }

            m_CharacterController.transform.position = target_pos;
            sitting = !sitting;
        }

        IEnumerator LerpImageAlpha (float target_alpha, float duration, GameObject image) {
            
            float time = 0;
            float start_alpha = image.GetComponent<CanvasRenderer>().GetAlpha();

            while (time < duration) {

                //color = Color32.Lerp(color, newColor, time/duration);
                image.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(start_alpha, target_alpha, time/duration));
                
                
                time += Time.deltaTime;
                yield return null;
            }

            image.GetComponent<CanvasRenderer>().SetAlpha(target_alpha);
        }

        private void OnTriggerExit(Collider other) {
            if (other.name == "reverbtrigger") {
                reverbzone2.SetActive(false);
                reverbzone1.SetActive(true);
            } else if (other.name == "endreverbtrigger") {
                reverbzone2.SetActive(true);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.name == "EndStormTrigger") {
                EventManager.TriggerEvent("EndingStorm", null);
                other.gameObject.SetActive(false);
                reverbzone1.SetActive(true);
            } else if (other.name == "OvermanRoomTrigger") {
                EventManager.TriggerEvent("EnteredOvermanRoom", null);
                other.gameObject.SetActive(false);
            } else if (other.name == "reverbtrigger") {
                reverbzone2.SetActive(true);
                reverbzone1.SetActive(false);
            } else if (other.name == "endreverbtrigger") {
                reverbzone2.SetActive(false);
            }
        }
    }
}

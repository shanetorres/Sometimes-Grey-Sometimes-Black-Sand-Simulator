using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;
using TMPro;

public class NPCAIController : MonoBehaviour
{
    [Header("Navmesh Settings")]
    [SerializeField] private NavMeshAgent m_NavMeshAgent;
    [SerializeField] private GameObject m_Destination;
    [SerializeField] private float m_rotationSpeed = 3f;

    [Header("Animation Settings")]
    [SerializeField] private Animator m_Animator;
    [SerializeField] private CurrentState m_CurrentState;

    public GameObject npc_canvas;
    [SerializeField] private GameObject npc_gui;

    private bool in_conversation;
    private GameObject character_controller;
    private GameObject rotate_target;
    [SerializeField] private GameObject npc_text;
    private TMP_Text npct;

    private Camera mainCamera;

    private bool has_conversation;
    private bool npc_can_talk;

    [SerializeField] private bool is_last_npc;
    [SerializeField] private bool is_overman;
    [SerializeField] private GameObject house_destination;
    [SerializeField] private GameObject brush;
    [SerializeField] AudioSource brush_audio;
    [SerializeField] AudioSource npc_audio_source;
    [SerializeField] AudioClip[] clips;
    [SerializeField] AudioClip talking_sound;
    [SerializeField] private GameObject[] objecst_to_look_at;
    [SerializeField] string gender;
    Animator brush_animator;
    private bool doing_action;
    private float last_npc_destination_distance;
    private bool ignore_rotate_target;
    private float initial_volume;
    public float destination_distance;

    //overman settings
    bool cleaning_stain;
    int current_stain_index;
    bool started_brushing;

    bool reached_player;
    float time;
    float anim_wait_time;
    float anim_going_time;
    bool animating;
    string animation_string = "";
    float animation_length;
    bool listening;
    Vector3 inside_house_pos;



    // tried to be elegant with a dictionary of functions but it wouldnt fucking work
    // so just using if statements instead whatever
    // private Dictionary<string, System.Action> last_npc_actions;




    // Start is called before the first frame update
    void Start()
    {
        in_conversation = false;
        doing_action = false;
        character_controller = GameObject.FindGameObjectWithTag("Player");
        ignore_rotate_target = false;
        reached_player = false;
        listening = false;
        // last_npc_actions["gotofirstdoor"] = GoToFirstDoor;

        // npc_text = npc_canvas.transform.GetChild(0).gameObject;
        // npc_canvas = gameObject.transform.Find("Canvas").gameObject;
        // // npc_text = npc_canvas.transform.GetChild(1).gameObject;
        // npc_text = npc_canvas.transform.Find("Text").gameObject;
        npct = npc_text.GetComponent<TMPro.TextMeshProUGUI>();
        npct.ForceMeshUpdate();
        npct.text = "";

        mainCamera = Camera.main;

        int convo_determinator = Random.Range(0,2);
        convo_determinator = 1;
        if (convo_determinator == 1) {
            has_conversation = true;
        }
        
        initial_volume = npc_audio_source.volume;

        EventManager.StartListening("RequestingConversation", StartConversation);
        EventManager.StartListening("ConversationEnded", EndConversation);
        EventManager.StartListening("AllConversationsHad", AllConversationsHadListener);
        EventManager.StartListening("NpcSpeaking", NpcSpeakingListener);
        EventManager.StartListening("LastConversationEnded", EndLastConversation);
        EventManager.StartListening("GUILerp", GUIListener);
        EventManager.StartListening("PlayTalkingSound", PlayTalkingSound);
        time = Time.time;
        anim_wait_time = 10.0f;
        animating = false;

        
        if (!is_last_npc) {
            inside_house_pos = GetRandomPointInsideCollider(house_destination.GetComponent<BoxCollider>());
            m_NavMeshAgent.SetDestination(inside_house_pos);
            rotate_target = m_Destination;
        } else if (is_last_npc) {
            Debug.Log("in if: " + gameObject.name);
            last_npc_destination_distance = 1.5f;
            m_Destination = character_controller;
            m_NavMeshAgent.destination = m_Destination.transform.position;
            // npct.text = "Hi!";
            npc_canvas.SetActive(false);
            
            EventManager.StartListening("NpcAction", NpcActionListener);

            
            rotate_target = character_controller;
            Debug.Log("FUCKINT ROTATE TARGET GOD FUCKING DMAMIT FUCKING LISTEN TO ME YOU FUCKING DOG SHIT PIECE OF HSIT");
            Debug.Log(rotate_target);
            Debug.Log("m_Destination"+m_Destination.name);
        } 

        if (is_overman) {
            brush_animator = brush.GetComponent<Animator>();
            EventManager.StartListening("StainCleaned", DoneBrushing);
            EventManager.StartListening("NPCSpoke", NPCSpokeListener);
            EventManager.StartListening("NpcAction", NpcActionListener);
        }
        // else if (is_overman) {
        //     rotate_target = m_Destination;
        // }
        cleaning_stain = false;
        
        npc_gui.GetComponent<CanvasRenderer>().SetAlpha(0);

        npc_can_talk = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckRemainingDistance();
        SetNPCState();
        // convo_canvas.transform.LookAt(convo_canvas.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);

        if (in_conversation && !is_last_npc && !is_overman) {
            if (Vector3.Distance(rotate_target.transform.position, gameObject.transform.position) > 5.0f) {
                // EndConversation(null);
                EventManager.TriggerEvent("ConversationEnded", null);
            }
        }

        if (npc_canvas.activeInHierarchy) {
            npc_canvas.transform.LookAt(npc_canvas.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }

        if (is_last_npc) {
            m_NavMeshAgent.destination = m_Destination.gameObject.transform.position;
            
            // Debug.Log("rotateto"+rotate_target);
        }

        if (is_overman & !listening) {
            // Debug.Log("stain count" + StainManager.stains.Count);
            StainCheck();
            if (!cleaning_stain && (Time.time - time > anim_wait_time) && !animating) {
                int animation = Random.Range(0,2);
                animating = true;
                anim_going_time = Time.time;
                switch(animation) {
                    case 0:
                        m_Animator.SetBool("scratching", true);
                        animation_string = "scratching";
                        animation_length = 3.5f;
                        break;
                    case 1:
                        m_Animator.SetBool("looking", true);
                        animation_string = "looking";
                        animation_length = 3.5f;
                        break;
                }
            } else if (animating) {
                
                if ((Time.time-anim_going_time) > animation_length) {
                    Debug.Log("MADE IT");
                    animating = false;
                    m_Animator.SetBool("looking", false);
                    m_Animator.SetBool("scratching", false);
                    m_Animator.SetBool(animation_string, false);
                    anim_wait_time = Random.Range(2, 20);
                    time = Time.time;
                }
            }
        }
    }



    private void SetNPCState()
    {
        
        float velocity = m_NavMeshAgent.velocity.magnitude;
        // MANAGER IS WALKING.
        if (velocity >= .2)
        {
            npc_can_talk = false;
            m_CurrentState = CurrentState.Walking;
            m_Animator.SetBool("walking", true);
        } else if (npc_can_talk) {
            Debug.Log("NPC_CANTALK:" + npc_can_talk);
            m_Animator.SetBool("walking", false);
            StartCoroutine(PlayTalkingAnimation());
        }
        else {
            m_CurrentState = CurrentState.Idle;
            m_Animator.SetBool("walking", false);
            npc_can_talk = false;
        }
        if (velocity < 1.5 && !ignore_rotate_target)
        // if (velocity < 1.5 || is_last_npc)
        {
            
            RotateTowardsDestination(rotate_target);
        }
    }

    private void CheckRemainingDistance() {
        if (!is_last_npc && !is_overman && Vector3.Distance(gameObject.transform.position, inside_house_pos) < 2.5f ) {
            EventManager.TriggerEvent("npcDestroyed", null);
            Debug.Log("Triggering event");

            EventManager.StopListening("RequestingConversation", StartConversation);
            EventManager.StopListening("ConversationEnded", EndConversation);
            EventManager.StopListening("AllConversationsHad", AllConversationsHadListener);
            EventManager.StopListening("NpcSpeaking", NpcSpeakingListener);
            EventManager.StopListening("LastConversationEnded", EndLastConversation);
            EventManager.StopListening("GUILerp", GUIListener);
            Destroy(gameObject);
        }

        if (is_last_npc && Vector3.Distance(gameObject.transform.position, m_Destination.transform.position) < last_npc_destination_distance) {
            
            m_NavMeshAgent.isStopped = true;
            npc_canvas.SetActive(true);
            // convo_canvas.SetActive(false);
            doing_action = false;
            EventManager.TriggerEvent("ActionCompleted", null);

            if (!reached_player) {
                reached_player = true;
                StartConversation(new Dictionary<string, object> {
                                    { "player", character_controller },
                                    { "npc", gameObject }
                                });
            }
            // rotate_target = character_controller;

        } 

        if (is_overman && cleaning_stain && !started_brushing && Vector3.Distance(gameObject.transform.position, m_Destination.transform.position) < 1.5f) {
            m_NavMeshAgent.isStopped = true;
            EventManager.TriggerEvent("BrushingStain", new Dictionary<string, object> {
                                    { "stain", current_stain_index },
                                });
            m_Animator.SetBool("brushing", true);
            brush_animator.SetBool("brushing", true);
            brush_audio.Play();
            started_brushing = true;
        }
    }

    private void Step() {
        if (Vector3.Distance(character_controller.transform.position, gameObject.transform.position) < 5) {

        npc_audio_source.clip = GetRandomClip();
        npc_audio_source.Play();
        }

    }

    private AudioClip GetRandomClip() {
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }

    private void RotateTowardsDestination(GameObject rotateTo) {
        if (rotateTo != null) {
            Vector3 dlf_direction = (rotateTo.transform.position - m_NavMeshAgent.transform.position);
            Quaternion dlf_lookRotation = Quaternion.LookRotation(new Vector3(dlf_direction.x, 0, dlf_direction.z));
            m_NavMeshAgent.transform.rotation = Quaternion.Slerp(m_NavMeshAgent.transform.rotation, dlf_lookRotation, Time.deltaTime * m_rotationSpeed);
        }
    }

    public void StartConversation(Dictionary<string, object> message) {
        if (has_conversation) {
            if (GameObject.ReferenceEquals((GameObject) message["npc"], gameObject)) {
                Debug.Log("STARTING CONVERSATION");
                // convo_canvas.SetActive(false);
                // npc_audio_source.volume = .3f;
                // if has conversation
                if(!cleaning_stain) {
                    rotate_target = (GameObject) message["player"];
                }
                
                in_conversation = true;

                
                Conversation.npc_text = npc_text.GetComponent<TMPro.TextMeshProUGUI>();

                if (is_last_npc) {
                    EventManager.TriggerEvent("ConversationStarted", 
                                new Dictionary<string, object> {
                                    { "npc", "last_npc" },
                                    { "object", gameObject}
                                });
                } else if (is_overman) {
                    EventManager.TriggerEvent("ConversationStarted", 
                                new Dictionary<string, object> {
                                    { "npc", "overman" },
                                    { "object", gameObject}
                                });
                } else {
                    m_NavMeshAgent.isStopped = true;
                    EventManager.TriggerEvent("ConversationStarted", 
                                new Dictionary<string, object> {
                                    { "npc", "regular" },
                                    { "object", gameObject},
                                    { "gender", gender}
                                });
                }

                
            }
        }
    }

    public void EndConversation(Dictionary<string, object> message) {
        Debug.Log("in end conversation");
        if (in_conversation) {
            // npc_audio_source.volume = initial_volume;
            Debug.Log("in end conversation event");
            m_NavMeshAgent.isStopped = false;
            
            in_conversation = false;
            has_conversation = false;
            m_Animator.SetBool("talking", false);
            npc_can_talk = false;

            rotate_target = m_Destination;
        }

    }
    
    public void EndLastConversation(Dictionary<string, object> message) {
        Debug.Log("In end last npc");
        StartCoroutine(LeaveIntoBuilding());
    }

    IEnumerator LeaveIntoBuilding() {
        Debug.Log("In coroutine");
        yield return new WaitForSeconds(8.0f);

        Debug.Log("after seconds");
        last_npc_destination_distance = 0.0f;
        EndConversation(null);
    }

    void AllConversationsHadListener(Dictionary<string, object> message) {
        has_conversation = false;
        // convo_canvas.SetActive(false);
    }

    void NpcSpeakingListener(Dictionary<string, object> message) {
        npc_can_talk = true;
        
    }

    void NpcActionListener(Dictionary<string, object> message) {
        Debug.Log("NpcActionListener");
        Debug.Log((string) message["function"]);
        // last_npc_actions[(string) message["function"]]();
        if ((string) message["function"] == "gotofirstdoor" && is_last_npc) {
            GoToFirstDoor();
        } else if ((string) message["function"] == "pointstraight" && is_last_npc) {
            m_Animator.SetBool("pointStraight", true);
        } else if ((string) message["function"] == "pointright" && is_last_npc) {
            m_Animator.SetBool("pointRight", true);
        } else if ((string) message["function"] == "stoppointing" && is_last_npc) {
            Debug.Log("IN HERE I AM IN HERE");
            m_Animator.SetBool("pointStraight", false);
            m_Animator.SetBool("pointRight", false);
        } else if ((string) message["function"] == "rotateplayer" && is_last_npc) {
            ChangeRotateTarget("rotateplayer");
        } else if ((string) message["function"] == "stopstains" && is_overman) {
            listening = true;
            m_Animator.SetBool("scratching", false);
            m_Animator.SetBool("looking", false);
            
            EventManager.TriggerEvent("ActionCompleted", null);
        } else if ((string) message["function"] == "startstains" && is_overman) {
            listening = false;
            EventManager.TriggerEvent("ActionCompleted", null);
        } else if ((string) message["function"] == "stopnpcspawning") {
            Debug.Log("CALLED ACTION SHIT");
            EventManager.TriggerEvent("stopnpcspawning", null);
            EventManager.TriggerEvent("ActionCompleted", null);
        }
    }

    IEnumerator PlayTalkingAnimation() {
        npc_can_talk = false;
        yield return new WaitForSeconds(.4f);
        m_Animator.SetBool("talking", true);
        
        
        yield return new WaitForSeconds(2.7f);
        m_Animator.SetBool("talking", false);
        // m_Animator.SetBool("talking", false);
        // npc_can_talk = false;
        Debug.Log("done");
    }



    void GoToFirstDoor() {
        Debug.Log("SUPPOSED TO GO TO FIRST DOOR");
        doing_action = true;
        m_Destination = house_destination;
        // Vector3 pos = GetRandomPointInsideCollider(m_Destination.GetComponent<BoxCollider>());
        Vector3 pos = m_Destination.GetComponent<BoxCollider>().ClosestPoint(gameObject.transform.position);
        m_NavMeshAgent.destination = pos;
        rotate_target = m_Destination;
        m_NavMeshAgent.isStopped = false;
        last_npc_destination_distance = 20.0f;
        Debug.Log(Vector3.Distance(gameObject.transform.position, m_Destination.transform.position));
    }

    void ChangeRotateTarget(string target) {
        if (target == "rotatedestination") {
            rotate_target = m_Destination;
            
        } if (target == "rotateseconddoor") {
            ignore_rotate_target = true;
            Quaternion rot = m_NavMeshAgent.transform.rotation;
            Debug.Log("original: " + rot.eulerAngles);
            Quaternion adjusted_rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z + 30);
            Debug.Log("adjusted: " + adjusted_rotation.eulerAngles);
            Debug.Log(ignore_rotate_target);
            m_NavMeshAgent.transform.rotation = Quaternion.Slerp(m_NavMeshAgent.transform.rotation, adjusted_rotation, Time.deltaTime * m_rotationSpeed);
            EventManager.TriggerEvent("ActionCompleted", null);

        } if (target == "rotateplayer") {
            rotate_target = character_controller;
            EventManager.TriggerEvent("ActionCompleted", null);
        }
    }


    Vector3 GetRandomPointInsideCollider( BoxCollider boxCollider ) {
        Vector3 extents = boxCollider.size / 2f;
        Vector3 point = new Vector3(
            Random.Range( -extents.x, extents.x ),
            Random.Range( -extents.y, extents.y ),
            Random.Range( -extents.z, extents.z )
        );

        return boxCollider.transform.TransformPoint( point );
    }

    void StainCheck() {
        if (StainManager.stains.Count > 0 && !cleaning_stain) {
            int stain_index = 0;
            if (StainManager.stains.Count > 1) {
                stain_index = GetClosestStainIndex();
            }

            // todo should loop through stains and select the closest one
            m_NavMeshAgent.destination = StainManager.stains[stain_index].transform.position;
            m_Destination = StainManager.stains[stain_index];
            rotate_target = m_Destination;
            cleaning_stain = true;
            animating = false;
            current_stain_index = stain_index;
        }
    }


    int GetClosestStainIndex() {
        int tMin = 0;
        float minDist = StainManager.stains.Count - 1;
        for (int i = 0; i < StainManager.stains.Count; i++) {
            float dist = Vector3.Distance(gameObject.transform.position, StainManager.stains[i].transform.position);
            if (dist < minDist)
            {
                tMin = i;
                minDist = dist;
            }
        }

        return tMin;
    }

    void DoneBrushing(Dictionary<string, object> message) {
        time = Time.time;
        anim_wait_time = Random.Range(4, 12);
        cleaning_stain = false;
        started_brushing = false;
        m_Animator.SetBool("brushing", false);
        brush_animator.SetBool("brushing", false);
        brush_audio.Stop();
        m_NavMeshAgent.isStopped = false;
        m_Destination = gameObject;
        m_NavMeshAgent.destination = gameObject.transform.position; // todo reset this to player
        int player_target = Random.Range(0,6);
         // todo uncomments
        if (player_target == 1) {
            rotate_target = character_controller;
        } else if (player_target == 2 || player_target == 3) {
            int target_index = Random.Range(0, objecst_to_look_at.Length);
            rotate_target = objecst_to_look_at[target_index];
        }
    }

    void GUIListener(Dictionary<string, object> message) {
        if (in_conversation) {
            Debug.Log(message["alpha"]);
            StartCoroutine(LerpAlpha((float) message["alpha"], 0.1f));
        }
        
    }

    void PlayTalkingSound(Dictionary<string, object> message) {
        if (in_conversation) {
            npc_audio_source.PlayOneShot(talking_sound);
        }
    }

    void NPCSpokeListener(Dictionary<string, object> message) {
        if (is_overman && !cleaning_stain) {
            StartCoroutine(LookAtPlayer());
        }
    }

    IEnumerator LookAtPlayer() {
        yield return new WaitForSeconds(Random.Range(0.2f, 0.6f));
        if (!cleaning_stain) {
            rotate_target = character_controller;
        }
    }

    IEnumerator LerpAlpha (float target_alpha, float duration) {
        float time = 0;
        float start_alpha = npc_gui.GetComponent<CanvasRenderer>().GetAlpha();

        while (time < duration) {

            //color = Color32.Lerp(color, newColor, time/duration);
            npc_gui.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(start_alpha, target_alpha, time/duration));
            
            
            time += Time.deltaTime;
            yield return null;
        }

        npc_gui.GetComponent<CanvasRenderer>().SetAlpha(target_alpha);
    }



    public enum CurrentState
    {
        Idle,
        Walking,
        Talking
    }
}

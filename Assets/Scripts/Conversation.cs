using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public struct dialogue {
    public string sentence;
    public string speaker;
    public string action;
    public float wait_time;

    public dialogue(string sentence, string speaker, string action, float wait_time = 0) {
        this.sentence = sentence;
        this.speaker = speaker;
        this.action = action;
        this.wait_time = wait_time;
    }
}

public class Conversation : MonoBehaviour
{
    public static TMP_Text player_text;
    public static TMP_Text npc_text; 

    private bool active_conversation;
    private bool doing_action;

    private bool revealing_text;

    [SerializeField] private GameObject offscreen_image;
    [SerializeField] private GameObject offscreen_text_object;
    [SerializeField] private GameObject player_speak_icon;
    [SerializeField] private GameObject player_gui;
    private TMP_Text offscreen_text;


    // private static List<dialogue[][]> conversations = new List<dialogue[][]>(new dialogue[][] {new dialogue[] {new dialogue("Hi.", "player", ""), new dialogue("Hi", "npc", ""), new dialogue("what's up","player", ""), 
    //                             new dialogue("Nothing much", "npc", ""), new dialogue("what about you", "npc", ""), new dialogue("shut the fuck up motherfucker","player", "")
    // }});

    private static List<dialogue[]> conversations = new List<dialogue[]> {
        new dialogue[] {new dialogue("Hi", "player", ""), new dialogue("I'm so sorry, I can't really talk right now.", "npc", "")},
        new dialogue[] {new dialogue("So sorry, in a rush", "npc", "")},
        new dialogue[] {
            new dialogue("Hi", "player", ""), 
            new dialogue("Hi.", "npc", ""),
            new dialogue("I've got a question for you.", "player", ""), 
            new dialogue("Sure, what's that?", "npc", ""),
            new dialogue("How do I get into that house over there?", "player", ""), 
            new dialogue("Oh, well you just go through one of the doors.", "npc", ""),
        },
        new dialogue[] {new dialogue("Sorry, no time.", "npc", "")},
        new dialogue[] {
            new dialogue("Hey", "player", ""), 
            new dialogue("Hi!", "npc", ""),
            new dialogue("I can't seem to get into that house over there, do you know how to get in?", "player", ""), 
            new dialogue("Oh it's easy! All you have to do is walk thorugh one of the doors!", "npc", "")
        },
        new dialogue[] {
            new dialogue("Hi", "player", ""), 
            new dialogue("Hey", "npc", ""),
            new dialogue("Is there a way to get into that house over there other than going through a door?", "player", ""), 
            new dialogue("No, that's really the only way. But the good news is anyone can go through them.", "npc", "")
        },
        new dialogue[] {new dialogue("Sorry, have to keep moving", "npc", "")},
        new dialogue[] {
            new dialogue("Hi", "player", ""), 
            new dialogue("Hey!", "npc", ""),
            new dialogue("Are you going into that house over there?", "player", ""), 
            new dialogue("Yep!", "npc", ""),
            new dialogue("Why?", "player", ""),
            new dialogue("Well it's the only place I want to go really. I honestly don't know anywhere else someone go to. And actually be fulfilled that is.", "npc", ""),
        },
        new dialogue[] {new dialogue("Can't talk sorry.", "npc", "")},
        new dialogue[] {new dialogue("Gotta keep moving.", "npc", "")},
        new dialogue[] {new dialogue("A little busy, so sorry.", "npc", "")},
        new dialogue[] {
            new dialogue("Hey", "player", ""), 
            new dialogue("Hey!", "npc", ""),
            new dialogue("You're going into that house over there right?", "player", ""), 
            new dialogue("Yep that's right!", "npc", ""),
            new dialogue("Can I ask you something?", "player", ""),
            new dialogue("Sure!", "npc", ""),
            new dialogue("What if someone can't make it into that house?", "player", ""),
            new dialogue("Well, anyone can get into the house. The doors open for every person.", "npc", ""),
            new dialogue("What if someone isn't in the house?", "player", ""),
            new dialogue("Well, then they're in the wrong place. But the good news is that's ok, because they can get into the house whenever they want. And then they'll be alright.", "npc", ""),
        },
        new dialogue[] {new dialogue("No time to talk, apologies.", "npc", "")},
        new dialogue[] {
            new dialogue("Hi", "player", ""), 
            new dialogue("Hey", "npc", ""),
            new dialogue("You're headed to that house?", "player", ""), 
            new dialogue("Indeed I am.", "npc", ""),
            new dialogue("Can I ask you a question?", "player", ""),
            new dialogue("Sure thing.", "npc", ""),
            new dialogue("If someone never made it into that house, what do you think would happen?", "player", ""),
            new dialogue("Well it wouldn't be good.", "npc", ""),
            new dialogue("Wouldn't be good at all.", "npc", "", 3.0f),
            new dialogue("I don't even want to think about that.", "npc", "", 3.5f),
            new dialogue("But I'd tell that person that they don't have to worry, the house is an escape from that.", "npc", "", 2.5f),
            new dialogue("They just have to go in.", "npc", "", 4.0f),
            new dialogue("What if they can't get in?", "player", ""),
            new dialogue("They can!", "npc", ""),
        },
        new dialogue[] {
            new dialogue("Hello", "player", ""), 
            new dialogue("Hi", "npc", ""),
            new dialogue("How are you?", "player", ""), 
            new dialogue("I'm good! How are you?", "npc", ""),
            new dialogue("I'm a little anxious because I feel like I'm supposed to go in that house over there but I can't get in", "player", ""),
            new dialogue("I think you're supposed to as well, I think everyone is supposed to.", "npc", ""),
            new dialogue("There's nowhere else to be. You could look around for miles and you'd never find a house like this.", "npc", "", 3.0f),
            new dialogue("But I might be actually able to get into other houses", "player", ""),
            new dialogue("You might, but is that really where you're supposed to be? Where anyone is supposed to be?", "npc", ""),
            new dialogue("I guess I don't know.", "player", "")
        },
        new dialogue[] {new dialogue("Gotta move.", "npc", "")},
        new dialogue[] {
            new dialogue("Hi", "player", ""), 
            new dialogue("Hi!", "npc", ""),
            new dialogue("You're going into that house?", "player", ""), 
            new dialogue("Yes!", "npc", ""),
            new dialogue("Why? What about it makes you want to go into it?", "player", ""),
            new dialogue("Well it really truly is a beautiful house, I mean even just looking at it i feel things that i've never felt before.", "npc", ""),
            new dialogue("Things I didn't know were possible to feel.", "npc", "", 3.0f),
            new dialogue("It's beautiful?", "player", ""),
            new dialogue("Yeah can't you see? The structure is so elaborate and ornate, it's unlike anything I've ever seen. Truly breathtaking. I could honestly stare at it forever.", "npc", ""),
            new dialogue("What about it is elaborate and ornate?", "player", ""),
            new dialogue("Well everything, from the craftsmanship of the facade, to it's staggering height, it's attention to detail in even the seemingly most insignificant places.", "npc", ""),
            new dialogue("Every time I look at it there's something new and beautiful that I find.", "npc", "", 4.0f),
            new dialogue("Hm.", "player", ""),
            new dialogue("Yeah, take a second to really look at it, study it. It will change you.", "npc", ""),
            new dialogue("I've been looking at it though, I've been looking at it really hard.", "player", ""),
        },
        new dialogue[] {
            new dialogue("Hello.", "player", ""), 
            new dialogue("Hi!", "npc", ""),
            new dialogue("Are you going into that house over there?", "player", ""), 
            new dialogue("Yes.", "npc", ""),
            new dialogue("Can I ask why you want to get into the house?", "player", ""),
            new dialogue("Well it's the only place where I feel truly good. The only place where anyone can feel truly good I believe", "npc", ""),
            new dialogue("What does that mean if I can't get in then?", "player", ""),
            new dialogue("You can't get in?", "npc", ""),
            new dialogue("Yeah I don't see any doors.", "player", ""),
            new dialogue("Strange, well I can assure you there are doors there.", "npc", ""),
            new dialogue("Can I ask you a question?", "player", ""),
            new dialogue("Sure ask me anything.", "npc", ""),
            new dialogue("What does the house look like to you?", "player", ""),
            new dialogue("What does the house look like?", "npc", ""),
            new dialogue("Yeah like imagine I can't see the house, describe to me what you see", "player", ""),
            new dialogue("Well, it's a really breathtaking structure. It's height is staggering, It has this beautiful color palette unlike anything I've ever seen. It just radiates warmth and comfort whenever I look at it. The shape of the house is magnificent, truly creative in its design. And behind it there's this big, big yard. With the greenest grass I've ever seen.", "npc", ""),
            new dialogue("Am I crazy?", "player", ""),
            new dialogue("I don't think you're crazy.", "npc", ""),
        },
        new dialogue[] {new dialogue("Sorry, can't talk.", "npc", "")},
        new dialogue[] {new dialogue("Gotta keep moving.", "npc", "")},
        new dialogue[] {new dialogue("I'm so sorry, I'm busy right now.", "npc", "")},
        new dialogue[] {new dialogue("...", "npc", "")},
        new dialogue[] {
            new dialogue("Hi", "player", ""), 
            new dialogue("Hi!", "npc", ""),
            new dialogue("Are you ok? Your eyes look a little burnt out", "npc", "", 1.5f), 
            new dialogue("Burnt out?", "player", ""), 
            new dialogue("Yeah.", "npc", ""),
            new dialogue("What do you mean?", "player", ""), 
            new dialogue("Well normally the people I see headed to this house, I can sense a kind of light behind their eyes.", "npc", ""),
            new dialogue("I can't sense that with you.", "npc", "", 3.5f),
            new dialogue("Is that a problem? How do I fix that?", "player", ""),
            new dialogue("You’re in the right place, go on into the house and it will help.", "npc", ""), 
            new dialogue("But I can't get in.", "player", ""),
            new dialogue("You can't get in?", "npc", ""), 
            new dialogue("Yeah, there are no doors.", "player", ""),
            new dialogue("Hmm no doors? There's plenty of doors.", "npc", ""),
            new dialogue("You may not be looking hard enough. Those who look will find", "npc", "", 3.0f),
            new dialogue("Ok I'll keep looking.", "player", ""),
        },
        new dialogue[] {
            new dialogue("Hi.", "player", ""),
            new dialogue("Does that house over there have any doors?", "player", ""), 
            new dialogue("Yeah, I can see several of them.", "npc", ""),
            new dialogue("Oh my god this is so frustrating.", "player", ""), 
            new dialogue("Why what's wrong?", "npc", ""),
            new dialogue("Well everyone can see the doors and can go in but I don't see any doors", "player", ""),
            new dialogue("You can't see any of the doors? There's a lot of them. And you can just walk right through them", "npc", ""), 
            new dialogue("Yeah I mean I'm trying really hard to see them but they're just not there", "player", ""),
            new dialogue("Hmmmm", "npc", ""), 
            new dialogue("Someone once told me it's not about striving or effort but just trusting.", "npc", "", 1.5f), 
            new dialogue("Maybe you could try just trusting that they are there?", "npc", "", 3.5f), 
            new dialogue("Yeah i'll try that, thank you", "player", "")
        },
        new dialogue[] {
            new dialogue("Hi how are you.", "player", ""),
            new dialogue("I'm good.", "npc", ""), 
            new dialogue("How are you doing?", "npc", "", 2.0f),
            new dialogue("I'm not doing that great, I'm a little stressed.", "player", ""), 
            new dialogue("I'm sorry to hear that, why are you stressed?", "npc", ""),
            new dialogue("Well I'm trying to get into that house over there, it seems like I'm supposed to or at least that's what I've heard.", "player", ""),
            new dialogue("But there's no doors. No matter how hard I look I can't find a door to go in.", "player", ""),
            new dialogue("Hmm, well I can assure you the doors are there. I can see them myself.", "npc", ""),
            new dialogue("Is something wrong with me I mean why can’t I see the doors?", "player", ""),
            new dialogue("Maybe something is holding you back from seeing them?", "npc", ""),
            new dialogue("Try letting go of whatever is holding you back.", "npc", "", 2.8f),
            new dialogue("Maybe. I can try that.", "player", ""),
        },
        new dialogue[] {
            new dialogue("Hey", "player", ""),
            new dialogue("Hi", "npc", ""),
            new dialogue("Can I ask you something?", "player", ""), 
            new dialogue("What is that house over there actually?", "player", ""), 
            new dialogue("That house? That's the one and only house that everyone is supposed to be in", "npc", ""),
            new dialogue("But what if someone can't get in.", "player", ""),
            new dialogue("Everyone can get in, and everyone should go in.", "npc", ""),
            new dialogue("I can't get in.", "player", ""),
            new dialogue("Yes you can, everyone can get in.", "npc", ""),
            new dialogue("I can't there's no doors. I've been trying.", "player", ""),
            new dialogue("You see the house right?", "npc", ""),
            new dialogue("Yes.", "player", ""),
            new dialogue("And you've seen people going into the house?", "npc", ""),
            new dialogue("Yes.", "player", ""),
            new dialogue("Doesn't that mean that the house has doors?", "npc", ""),
            new dialogue("Well I'm not sure honestly, I guess I'm kind of starting to wonder if the house is what it's been said to be.", "player", ""),
            new dialogue("That seems a little arrogant to me, why do you think you can just determine what the house is or isn't when it's been made evidently clear in front of you what it actually is?", "npc", ""),
            new dialogue("It doesn't feel all that evident to me is what I'm saying", "player", ""),
            new dialogue("What makes you think you have the answers?", "npc", ""),
            new dialogue("I don't really think I have any answers at all and that seems to be why I can't get in.", "player", ""),
            new dialogue("That doesn't make any sense.", "npc", ""),
            new dialogue("Ok.", "player", ""),
            new dialogue("Listen you can ponder this all you want, and I'm really not trying to be rude or mean or anything, but if you don't go into that house, things are really not going to go well for you.", "npc", ""),
            new dialogue("And you're going to wish you would have just went in, but at that point it won't matter.", "npc", "", 4.5f)
        },
    };

    private static dialogue[] last_conversation = new dialogue[] {
        new dialogue ("Hi!", "npc", ""),
        new dialogue ("Hey", "player", ""), 
        new dialogue ("what are you doing standing out here alone?", "npc", "", 0.5f),                                    //pointtodoor animation
        new dialogue ("Are you ok?", "npc", "", 1.0f),
        new dialogue ("Yeah I'm fine I think", "player", ""), 
        new dialogue ("Do you know how to get into this house?", "player", ""), 
        new dialogue ("Well I just go through one of the doors they’re all unlocked", "npc", "", 0.7f),
        new dialogue ("But there aren’t any doors!", "player", ""),
        new dialogue ("No doors? What do you mean I can see all of them right in front of me", "npc", "", 0.6f),
        new dialogue ("All I see is a wall", "player", ""),
        new dialogue ("Here come with me I’ll show you the doors", "npc", "", .75f), 
        new dialogue ("", "npc", "gotofirstdoor", 1.5f), 
        new dialogue ("So you can't see this door at all?", "npc", "pointstraight"),                                          // fuck it just play an animation that will look better anyways
        new dialogue ("Nope", "player", "") /*action should be idle to stop pointing*/, 
        new dialogue ("What about this door? That door?", "npc", "pointright", 0.3f),  
        new dialogue ("Nope I can't see any of it", "player", "stoppointing"), 
        new dialogue ("Are you sure you want to see them? Maybe that's why they're not there", "npc", "rotateplayer", 0.7f),
        new dialogue ("But how would that make sense doors don’t just appear out of thin air they are always there", "player", ""),
        new dialogue ("I’m sorry I don’t have all the answers.", "npc", "", .6f),
        new dialogue ("It’s ok, I’m just frustrated I guess. Everyone’s talking about these doors and I just can’t see them", "player", ""),
        new dialogue ("I don’t really want to be walking around in this grey sand anymore I mean I think I’m alright for now but I don’t know if I’ll always be", "player", ""),
        new dialogue ("I’m really sorry, do you want me to stay with you until you see one? I can stay with you until you see one", "npc", "", .6f),
        new dialogue ("No it’s ok you can go inside", "player", ""),
        new dialogue ("Honestly I don’t mind waiting with you at all I’m sure you’ll see one eventually", "npc", ""),
        new dialogue ("No it’s ok", "player", ""),
        new dialogue ("I’m serious I’ll wait", "npc", "", .8f),
        new dialogue ("It’s ok", "player", ""),
    };

    private static dialogue[] overman_conversation = new dialogue[] {
        new dialogue ("Hello?", "player", ""), 
        new dialogue ("Hi", "npc", ""), 
        new dialogue ("Oh, hi", "player", ""), 
        new dialogue ("", "npc", ""),
        new dialogue ("What is this place?", "player", ""), 
        new dialogue ("What is this place?", "npc", ""),  
        new dialogue ("Yeah", "player", ""), 
        new dialogue ("You don't recognize it?", "npc", ""),  
        new dialogue ("Um, I don't think so", "player", ""), 
        new dialogue ("Hmm. Well it's just, a place I guess", "npc", ""),  
        new dialogue ("", "npc", "", 3.0f),   
        new dialogue ("Why are there so many different chairs in here?", "player", ""), 
        new dialogue ("Oh, well sometimes I need to sit down.", "npc", ""), 
        new dialogue ("You can sit down too if you want.", "npc", "", 2.0f),  
        new dialogue ("But you only need one chair for that.", "player", ""), 
        new dialogue ("Huh?", "npc", ""), 
        new dialogue ("You only need one chair to sit down.", "player", ""), 
        new dialogue ("Oh, well I mean yeah I guess. I'm not sure why I guess they're just kind of here.", "npc", ""), 
        new dialogue ("I like all these chairs though.", "npc", "", 3.0f),
        new dialogue ("", "npc", "", 3.0f),
        new dialogue ("I was just out in a storm I think.", "player", ""), 
        new dialogue ("Everything was black and then I saw this place and came in.", "player", ""), 
        new dialogue ("I could hear the storm, what was it like?", "npc", ""),
        new dialogue ("Well it was loud, and really dark.", "player", ""), 
        new dialogue ("I couldn't see anything and the wind was strong", "player", ""), 
        new dialogue ("Wow, well that sounds horrible. Good thing it seems to be over.", "npc", ""),
        new dialogue ("I'm glad you came in, this place is a great shelter", "npc", "", 2.5f),
        new dialogue ("I was trying to get into a house over there that lots of others were going into but it had no doors.", "player", ""), 
        new dialogue ("I tried pretty hard", "player", ""),
        new dialogue ("That's alright.", "npc", ""),
        new dialogue ("Does anybody else ever come into this house?", "player", ""), 
        new dialogue ("Um maybe, I'm not sure but probably not.", "npc", ""),
        new dialogue ("It's only really ever been you that comes in every once in a while", "npc", "", 2.5f),
        new dialogue ("How often are you in this house?", "player", ""), 
        new dialogue ("Well, I'm always in here.", "npc", ""),
        new dialogue ("You don't ever leave?", "player", ""), 
        new dialogue ("Why would I?", "npc", ""),
        new dialogue ("I've got everything I need in here", "npc", "", 2.5f),
        new dialogue ("Plus if I left it'd get to dirty, I've gotta keep it clean just in case you come by", "npc", "", 2.7f),
        new dialogue ("Just like now.", "npc", "", 4.0f),
        new dialogue ("What are you cleaning?", "player", ""), 
        new dialogue ("Just these blotches. I don't like when they show up so I try to take care of them", "npc", ""),
        new dialogue ("Sometimes a lot of them come and it takes a while but I'm usually able to clean them", "npc", "", 4.0f),
        new dialogue ("", "npc", "", 3.0f),
        new dialogue ("I can't really tell who you are.", "player", ""), 
        new dialogue ("That's ok.", "npc", ""),
        new dialogue ("", "npc", "", 3.0f),
        new dialogue ("Can I ask you more questions?", "player", ""), 
        new dialogue ("Sure. I like questions. I can't guarantee that I'll have answers though", "npc", ""),
        new dialogue ("Can you tell me if walking around in the sand is ok?", "player", ""), 
        new dialogue ("Or if being in this house is ok?", "player", ""), 
        new dialogue ("Like, would it be ok for a while?", "player", ""), 
        new dialogue ("A long while?", "player", ""), 
        new dialogue ("Why are you thinking about these kinds of things?", "npc", ""),
        new dialogue ("Well because I have to.", "player", ""), 
        new dialogue ("I don't think you have to. In fact I think it's ok if you don't", "npc", ""),
        new dialogue ("But what if I fall through the sand one day? What if it collapses underneath me and I have no way of getting back?", "player", ""), 
        new dialogue ("Is that happening right now?", "npc", ""),
        new dialogue ("Well, no i guess not.", "player", ""), 
        new dialogue ("Have you ever seen something like that happen?", "npc", ""),
        new dialogue ("No", "player", ""), 
        new dialogue ("So you don't know for sure if something like that can even occur?", "npc", "stopstains"),
        new dialogue ("I don't", "player", ""), 
        new dialogue ("Then I don't think you have to think about that.", "npc", ""),
        new dialogue ("", "npc", "", 1.0f),
        new dialogue ("But its not that easy.", "player", ""), 
        new dialogue ("I know it's not.", "npc", ""),
        new dialogue ("I'm just worried, I feel like I have to be in there", "player", ""), 
        new dialogue ("Why do you feel that way?", "npc", ""),
        new dialogue ("I don't know I guess it just seems like I can't actually like myself and others if I'm not in that house.", "player", ""), 
        new dialogue ("How can I ever be justified if I'm not in that house? How can I ever be alright if I don't ever find a way in?", "player", ""),
        new dialogue ("I try not to think about it but I always do. I always think about the house. I look for other houses but I feel wrong, I've been hearing about that house since the beginning and I feel wrong that I'm not in it.", "player", ""),  
        new dialogue ("I just feel wrong.", "player", ""),
        new dialogue ("Why do you feel wrong?", "npc", ""),
        new dialogue ("I don't know I've just heard the house was supposed to be the purpose of all this walking, that's what I've always heard.", "player", ""),
        new dialogue ("Hearing that it's the right way to go, everything else is fleeting. No matter how hard I try subconciously I'm always thinking like that", "player", ""), 
        new dialogue ("But I can't get in.", "player", ""),
        new dialogue ("The truth is though I don't even want to be in that house.", "player", ""),
        new dialogue ("I don't think the idea of it even works for me. I think I like this house more than I would ever like it in there.", "player", ""),
        new dialogue ("But still I need something to hold onto. Out in the sand I'm too all over the place I slip up too much I'm too volatile.", "player", ""),
        new dialogue ("The weather is too volatile. I'm not secure enough and people out there keep telling me I just need to go into that house and things will be better", "player", ""),
        new dialogue ("But I can't get in. Am I wrong? Is that why I can't get in, because I'm wrong?", "player", ""),
        new dialogue ("I don't think you're wrong. And just like it's probably nice for others to be in that house I think it's nice when you're in this house.", "npc", ""),
        new dialogue ("But i feel wrong.", "player", ""),
        new dialogue ("I feel guilty. I feel like a bad person. Like I did something like I messed up and I'm going to pay for that and that's why I can't get in over there.", "player", ""),
        new dialogue ("What went wrong with me that I’m now the way I am? Was it my fault?", "player", ""),
        new dialogue ("I don't think anything went wrong. I like who you are.", "npc", ""),
        new dialogue ("", "npc", "", 3.0f),
        new dialogue ("I tried I tried really hard you know I really tried to get into that house", "player", ""),
        new dialogue ("I know you did. I'm sorry.", "npc", ""),
        new dialogue ("I just want to know if I'm going to be ok.", "player", ""),
        new dialogue ("I know you do.", "npc", ""),
        new dialogue ("So then what do I do? Where do I go?", "player", ""),
        new dialogue ("I think it's fine if you keep walking.", "npc", ""),
        new dialogue ("But what if I can never get into that house?", "player", ""),
        new dialogue ("What if I can never get into any other house?", "player", ""),
        new dialogue ("You can always get into this one. I'll always be in here.", "npc", ""),
        new dialogue ("", "npc", "startstains", 1.0f),
        new dialogue ("Yeah.", "player", "despawnhouse")
    };

    // private static dialogue[][] conversations = new dialogue[][] {new dialogue[] {new dialogue("Hi.", "player", ""), new dialogue("Hi", "npc", ""), new dialogue("what's up","player", ""), 
    //                             new dialogue("Nothing much", "npc", ""), new dialogue("what about you", "npc", ""), new dialogue("shut the fuck up motherfucker","player", "")
    // }};

    dialogue[] current_conversation;
    private int conversation_index;
    private bool waitaclick; // this is hacky as shit but whatever
    private int conversation_number;
    private bool all_conversations_completed;
    private bool in_last_convo;
    private bool in_overman_convo;
    private bool last_conversation_over;
    private bool revealing_player_text;
    private int player_characters_revealed;
    private bool final_reveal = false;
    private bool offscreen = false;
    
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("ConversationStarted", InitializeConversation);
        EventManager.StartListening("ConversationEnded", EndConversation);
        EventManager.StartListening("ActionCompleted", ActionCompletedListener);
        EventManager.StartListening("TextOffscreen", TextOffscreenListener);

        offscreen_text = offscreen_text_object.GetComponent<TMPro.TextMeshProUGUI>();
        offscreen_text.text = "";
        Color32 color = offscreen_text.faceColor;
        offscreen_text.faceColor = new Color32(color.r, color.g, color.b, 0);
        offscreen_image.GetComponent<CanvasRenderer>().SetAlpha(0);
        player_speak_icon.GetComponent<CanvasRenderer>().SetAlpha(0);
        player_gui.GetComponent<CanvasRenderer>().SetAlpha(0);

        conversation_number = 0;
        active_conversation = false;
        conversation_index = 0;
        waitaclick = true;
        doing_action = false;
        all_conversations_completed = false;
        in_last_convo = false;
        in_overman_convo = false;
        revealing_text = false;
        revealing_player_text = false;
        last_conversation_over = false;
        player_characters_revealed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("active_conversation: " + active_conversation);
        // Debug.Log("conversation_index: " + conversation_index);
        // Debug.Log("doing_action: " + doing_action);
        // Debug.Log("revealing_text: " + revealing_text);
        Debug.Log(doing_action);
        if (active_conversation && conversation_index > 0 && !doing_action && !revealing_text) {
            if (Input.GetMouseButtonDown(0)) {
                // if (!waitaclick) {
                    
                    UpdateConversation();
                // } else {
                //     waitaclick = false;
                // }
                
            }
        }
    }

    private void InitializeConversation(Dictionary<string, object> message) {
        active_conversation = true;
        Debug.Log("InitializeConversation");
        npc_text.text = "";
        offscreen_text.text = "";
        player_text.text = "";

        if ((string) message["npc"] == "last_npc" && !last_conversation_over) {
            current_conversation = last_conversation;
            in_last_convo = true;
        } else if ((string) message["npc"] == "overman") {
            current_conversation = overman_conversation;
            in_overman_convo = true;
        } else if ((string) message["npc"] == "regular") {
            current_conversation = conversations[conversation_number];
        }
        

        if (current_conversation[0].speaker == "player") {
            StartCoroutine(RevealInitialText(current_conversation[conversation_index], player_text));
        } else if (current_conversation[0].speaker == "npc") {
            StartCoroutine(RevealInitialText(current_conversation[conversation_index], npc_text));
        }

    }

    private void UpdateConversation() {
        if (conversation_index < current_conversation.Length) {
            // if (all_conversations_completed) {
            //     EventManager.TriggerEvent("InLastConversation", new Dictionary<string, object> {
            //                         { "active", true }
            //                     });
            // }
            
            if (current_conversation[conversation_index].speaker == "player") {
                StartCoroutine(LerpOffscreenAlpha(0, 0.1f, player_speak_icon));
                // player_text.text = current_conversation[conversation_index].sentence; 
                StartCoroutine(RevealPlayerText(current_conversation[conversation_index]));

            } else if (current_conversation[conversation_index].speaker == "npc") {  
                // npc_text.text = current_conversation[conversation_index].sentence; 
                StartCoroutine(RevealNPCText(current_conversation[conversation_index]));

                if (current_conversation[conversation_index].action.Length == 0 && Random.Range(0,7) == 3) {
                    EventManager.TriggerEvent("NpcSpeaking", null);
                }    
            }

           
        } else {
            
            if (!in_last_convo) {
                EventManager.TriggerEvent("ConversationEnded", null);
            } else {
                Debug.Log("IN HERE CALLING CONVERSATION ENDED");
                LastConversationEnded();
            }
            
        }

    }

    IEnumerator RevealInitialText(dialogue text, TMP_Text text_object) {
        revealing_text = true;
        string full_text = text.sentence;

        int characters_revealed = 0;

        if (text.speaker == "npc") {
            if (!offscreen) {
                EventManager.TriggerEvent("GUILerp", new Dictionary<string, object> {
                    {"alpha", .92f}
                });
            } else {
                StartCoroutine(LerpOffscreenAlpha(.92f, .1f, offscreen_image));
            }
        } else {
            StartCoroutine(LerpOffscreenAlpha(.92f, .1f, player_gui));
        }

        while (characters_revealed < full_text.Length) {
            while (full_text[characters_revealed] == ' ') {
                ++characters_revealed;
            }

            if (text.speaker == "npc") {
                EventManager.TriggerEvent("PlayTalkingSound", null);
            }

            ++characters_revealed;

            text_object.text = full_text.Substring(0, characters_revealed);

            yield return new WaitForSeconds(0.04f);
        }

        yield return new WaitForSeconds(1.0f);

        revealing_text = false;
        conversation_index++;
        

        // if (text.speaker == "npc") {
        //     EventManager.TriggerEvent("GUILerp", new Dictionary<string, object> {
        //         {"alpha", 0f}
        //     });
        //     StartCoroutine(LerpOffscreenAlpha(0, .1f, offscreen_image));
        // } else {
        //     StartCoroutine(LerpOffscreenAlpha(0f, .1f, player_gui));
        // }
        
        
        EventManager.TriggerEvent("GUILerp", new Dictionary<string, object> {
            {"alpha", 0f}
        });
        StartCoroutine(LerpOffscreenAlpha(0, .1f, offscreen_image));
        StartCoroutine(LerpOffscreenAlpha(0f, .1f, player_gui));
        npc_text.text = "";
        offscreen_text.text = "";
        player_text.text = "";

        if (conversation_index < current_conversation.Length && current_conversation[conversation_index].speaker == "player") {
            StartCoroutine(LerpOffscreenAlpha(1, 0.1f, player_speak_icon));
        } else {
            UpdateConversation();
        }

    }

    IEnumerator RevealNPCText(dialogue text) {


        revealing_text = true;
        string full_text = text.sentence;


        int characters_revealed = 0;

        if (text.wait_time > 0) {
            yield return new WaitForSeconds(text.wait_time);
        }

        player_text.text = "";
        npc_text.text = "";
        offscreen_text.text = "";

        if (text.sentence == "") {
            if (!offscreen) {
                EventManager.TriggerEvent("GUILerp", new Dictionary<string, object> {
                        {"alpha", 0f}
                });
            } else {
                StartCoroutine(LerpOffscreenAlpha(0f, .1f, offscreen_image));
            }

            
            if (text.action.Length > 0 && text.action != "gotofirstdoor") {
                doing_action = true;
                EventManager.TriggerEvent("NpcAction", new Dictionary<string, object> {
                                { "function", text.action }
                            });
                
            }

            if (!in_overman_convo) {
                yield return new WaitForSeconds(1.0f);
            } else if (in_overman_convo) {
                yield return new WaitForSeconds(Random.Range(2.0f, 7.0f));
            }
                
        } else {
            if (!offscreen) {
                EventManager.TriggerEvent("GUILerp", new Dictionary<string, object> {
                    {"alpha", .92f}
                });
            } else {
                StartCoroutine(LerpOffscreenAlpha(.92f, .1f, offscreen_image));
            }
            

            if (text.action.Length > 0 && text.action != "gotofirstdoor") {
                doing_action = true;
                EventManager.TriggerEvent("NpcAction", new Dictionary<string, object> {
                                { "function", text.action }
                            });
            }

            
            bool chip = true;
            while (characters_revealed < full_text.Length) {
                while (full_text[characters_revealed] == ' ') {
                    ++characters_revealed;
                }

                EventManager.TriggerEvent("NPCSpoke", null);

                ++characters_revealed;

                npc_text.text = full_text.Substring(0, characters_revealed);
                offscreen_text.text = full_text.Substring(0, characters_revealed);
                if (chip) {
                    EventManager.TriggerEvent("PlayTalkingSound", null);

                }
                
                chip = !chip;
                yield return new WaitForSeconds(0.055f);
            }
        }

        revealing_text = false;

        if (text.action == "gotofirstdoor") {
            Debug.Log("GO TO FIRST DOOR");
            EventManager.TriggerEvent("NpcAction", new Dictionary<string, object> {
                            { "function", text.action }
                        });
            doing_action = true;
            EventManager.TriggerEvent("GUILerp", new Dictionary<string, object> {
                {"alpha", 0f}
            });
            StartCoroutine(LerpOffscreenAlpha(0, .1f, offscreen_image));
        }


        conversation_index++;
        if ((conversation_index >= current_conversation.Length || current_conversation[conversation_index].speaker == "npc") && !doing_action) {
            if (conversation_index >= current_conversation.Length) {
                yield return new WaitForSeconds(3.0f);
            }
            UpdateConversation();
        } else {
            if (current_conversation[conversation_index].speaker == "player") {
                StartCoroutine(LerpOffscreenAlpha(1, 0.1f, player_speak_icon));
            }
            yield return new WaitForSeconds(5.0f);
            if (!revealing_text) {
                EventManager.TriggerEvent("GUILerp", new Dictionary<string, object> {
                    {"alpha", 0f}
                });
                StartCoroutine(LerpOffscreenAlpha(0, .1f, offscreen_image));
                npc_text.text = "";
                offscreen_text.text = "";
            }

        }

    }

    IEnumerator RevealPlayerText(dialogue text) {
        string full_text = text.sentence;

        player_text.text = full_text.Substring(0, player_characters_revealed);

        if (current_conversation[conversation_index].action.Length > 0) {
            Debug.Log("action event");
            EventManager.TriggerEvent("NpcAction", new Dictionary<string, object> {
                            { "function", current_conversation[conversation_index].action }
                        });
            doing_action = true;
        }

        StartCoroutine(LerpOffscreenAlpha(.92f, .1f, player_gui));

        while (Input.GetMouseButton(0)) {
            if (player_characters_revealed < full_text.Length) {
                while (full_text[player_characters_revealed] == ' ') {
                    ++player_characters_revealed;
                }

                EventManager.TriggerEvent("NPCSpoke", null);

                ++player_characters_revealed;

                player_text.text = full_text.Substring(0, player_characters_revealed);
                
            }
            yield return new WaitForSeconds(0.06f);
        }

        

        if (player_characters_revealed == full_text.Length) {
            
            
            // todo lerp alpha down to 0?
            conversation_index++;
            player_characters_revealed = 0;
            revealing_player_text = false;

            yield return new WaitForSeconds(Random.Range(0.2f, 0.6f));
            StartCoroutine(LerpOffscreenAlpha(0, .1f, player_gui));
            player_text.text = ""; // IF SOMETHINGS BROKEN ITS BECAUSE OF THIS
            if (conversation_index < current_conversation.Length) {
                if (current_conversation[conversation_index].speaker == "player") {
                    StartCoroutine(LerpOffscreenAlpha(1, 0.1f, player_speak_icon));
                } else {
                    StartCoroutine(LerpOffscreenAlpha(0, 0.1f, player_speak_icon));
                    UpdateConversation();
                    
                }
            } else if (conversation_index == current_conversation.Length) {
                UpdateConversation();
                StartCoroutine(LerpOffscreenAlpha(0, 0.1f, player_speak_icon));
            } else {
                StartCoroutine(LerpOffscreenAlpha(0, 0.1f, player_speak_icon));
            }

            

            // if (!revealing_player_text) {
            //     yield return new WaitForSeconds(Random.Range(1.0f, 1.5f));
            //     StartCoroutine(LerpOffscreenAlpha(0, .1f, player_gui));
            // }
            
        } else {
            StartCoroutine(LerpOffscreenAlpha(1, 0.1f, player_speak_icon));
        }

    }

    void ActionCompletedListener(Dictionary<string, object> message) {
        doing_action = false;
        if (in_last_convo && conversation_index > 1 && current_conversation[conversation_index-1].action == "gotofirstdoor") {
            StartCoroutine(LerpOffscreenAlpha(1, 0.1f, player_speak_icon));
        }
    }

    void TextOffscreenListener(Dictionary<string, object> message) {
        offscreen = (bool) message["offscreen"];

        if (offscreen) {
            EventManager.TriggerEvent("GUILerp", new Dictionary<string, object> {
                {"alpha", 0f}
            });
            if (offscreen_text.text.Length > 0) {
                StartCoroutine(LerpOffscreenAlpha(.92f, 0.1f, offscreen_image));
            }
            
            StartCoroutine(LerpText(255, 0.1f, offscreen_text));
            StartCoroutine(LerpText(0, 0.1f, npc_text));
        } else {
            if (npc_text.text.Length > 0) {
                EventManager.TriggerEvent("GUILerp", new Dictionary<string, object> {
                    {"alpha", .92f}
                });
            }

            StartCoroutine(LerpOffscreenAlpha(0, 0.1f, offscreen_image));
            StartCoroutine(LerpText(0, 0.1f, offscreen_text));
            StartCoroutine(LerpText(255, 0.1f, npc_text));
        }
    }

    IEnumerator LerpOffscreenAlpha (float target_alpha, float duration, GameObject image) {
        
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

    IEnumerator LerpText (byte target_alpha, float duration, TMP_Text text) {
        
        float time = 0;
        // float start_alpha = text.GetComponent<Renderer>().material.color.a;
        Color32 color = text.faceColor;
        Color32 newColor = new Color32(color.r, color.g, color.b, target_alpha);
        while (time < duration) {

            //color = Color32.Lerp(color, newColor, time/duration);
            text.faceColor = Color.Lerp(color, newColor, time/duration);
            
            
            time += Time.deltaTime;
            yield return null;
        }

        text.faceColor = newColor;
    }

    private void EndConversation(Dictionary<string, object> message) {
        Debug.Log("End Conversation");
        // delete convo from dialogue
        active_conversation = false;
        conversation_index = 0;
        offscreen = false;
        EventManager.TriggerEvent("GUILerp", new Dictionary<string, object> {
            {"alpha", 0f}
        });
        StopAllCoroutines();
        StartCoroutine(LerpOffscreenAlpha(0, 0.1f, offscreen_image));
        StartCoroutine(LerpOffscreenAlpha(0, .1f, player_gui));
        StartCoroutine(LerpOffscreenAlpha(0, 0.1f, player_speak_icon));
        StartCoroutine(LerpText(0, 0.1f, offscreen_text));
        npc_text.text = "";
        offscreen_text.text = "";
        player_text.text = "";
        revealing_player_text = false;
        revealing_text = false;
        waitaclick = true;
        Debug.Log("REMOVING CONVERSATION");
        Debug.Log(conversations.Count);
        Debug.Log(conversation_number);
        if (conversations.Count > 0) {
            conversations.RemoveAt(conversation_number);
        }
        

        if (!all_conversations_completed && conversations.Count == 0) {
            EventManager.TriggerEvent("AllConversationsHad", null);
            all_conversations_completed = true;
        }
    }

    private void LastConversationEnded() {
        active_conversation = false;
        conversation_index = 0;
        EventManager.TriggerEvent("GUILerp", new Dictionary<string, object> {
            {"alpha", 0f}
        });
        StartCoroutine(LerpOffscreenAlpha(0, .1f, player_gui));
        npc_text.text = "";
        offscreen_text.text = "";
        player_text.text = "";
        waitaclick = true;
        last_conversation_over = true;
        Debug.Log("IN HERE CALLING CONVERSATION ENDED2");

        EventManager.TriggerEvent("StartStorm", null);
        EventManager.TriggerEvent("LastConversationEnded", null);
    }
}

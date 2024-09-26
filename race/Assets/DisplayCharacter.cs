using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCharacter : MonoBehaviour
{
    #region Character Pieces
    public Transform HeadParent;
    public Transform ChestParent;
    public Transform RightArmParent;
    public Transform LeftArmParent;
    public Transform HipsParent;
    public Transform RightLegParent;
    public Transform LeftLegParent;

    GameObject Head;
    GameObject Chest;
    GameObject RightArm;
    GameObject LeftArm;
    GameObject Hips;
    GameObject RightLeg;
    GameObject LeftLeg;
    #endregion

    CustomisationManager cM;
    AIManager aiM;


    public bool Debugging;
    public bool Build;
    public bool BuildPlayerCharacter;
    public bool BuildAIcharacter;
    public int AIid;
    bool Built;

    public int HeadSelected;
    public int BodySelected;
    public int LegsSelected;
    public int AnimationStyleSelected;

    private void Start()
    {
        GetStartStats();
    }

    public void GetStartStats()
    {
        cM = FindObjectOfType<CustomisationManager>();
        aiM = FindObjectOfType<AIManager>();

        if (BuildPlayerCharacter) GetPlayerCharacter();
        if (BuildAIcharacter) GetAICharacter();

        if (Build) BuildCharacter();
    }

    public void GetPlayerCharacter()
    {
        HeadSelected = cM.HeadSelected;
        BodySelected = cM.BodySelected;
        LegsSelected = cM.LegsSelected;
        AnimationStyleSelected = cM.AnimationStyleSelected;
    }

    public void GetAICharacter()
    {
        HeadSelected = aiM.AICharacters[AIid].CharacterHead;
        BodySelected = aiM.AICharacters[AIid].CharacterBody;
        LegsSelected = aiM.AICharacters[AIid].CharacterLegs;
        AnimationStyleSelected = aiM.AICharacters[AIid].CharacterStyle;
    }

    public void RefreshCharacter()
    {
        ClearCharacter();
        GetStartStats();
        BuildCharacter();
    }

    public void ClearCharacter()
    {
        Destroy(Head);
        Destroy(Chest);
        Destroy(RightArm);
        Destroy(LeftArm);
        Destroy(Hips);
        Destroy(RightLeg);
        Destroy(LeftLeg);
    }

    Customisation animStyle;
    public void BuildCharacter()
    {
        cM = FindObjectOfType<CustomisationManager>();
        aiM = FindObjectOfType<AIManager>();
        Build = false;

        Head = Instantiate(cM.Heads[HeadSelected].InWorldModel, HeadParent);
        Head.transform.localScale = Vector3.one;
        Head.transform.localPosition = Vector3.zero;
        Head.transform.localEulerAngles = Vector3.zero;
        Head.transform.name = "HEAD";

        GameObject Body = Instantiate(cM.Bodies[BodySelected].InWorldModel);
        Chest = Body.transform.Find("Chest").gameObject;
        RightArm = Body.transform.Find("R Arm").gameObject;
        LeftArm = Body.transform.Find("L Arm").gameObject;
        Chest.transform.SetParent(ChestParent);
        RightArm.transform.SetParent(RightArmParent);
        LeftArm.transform.SetParent(LeftArmParent);

        Chest.transform.localScale = Vector3.one;
        Chest.transform.localPosition = Vector3.zero;
        Chest.transform.name = "CHARACTER_CHEST";
        Chest.transform.localEulerAngles = Vector3.zero;

        RightArm.transform.localScale = Vector3.one;
        RightArm.transform.localPosition = Vector3.zero;
        RightArm.transform.name = "CHARACTER_R_ARM";
        RightArm.transform.localEulerAngles = Vector3.zero;

        LeftArm.transform.localScale = Vector3.one;
        LeftArm.transform.localPosition = Vector3.zero;
        LeftArm.transform.name = "CHARACTER_L_ARM";
        LeftArm.transform.localEulerAngles = Vector3.zero;

        Destroy(Body);

        GameObject Legs = Instantiate(cM.Legs[LegsSelected].InWorldModel);

        Hips = Legs.transform.Find("Hips").gameObject;
        RightLeg = Legs.transform.Find("R Leg").gameObject;
        LeftLeg = Legs.transform.Find("L Leg").gameObject;

        Hips.transform.SetParent(HipsParent);
        RightLeg.transform.SetParent(RightLegParent);
        LeftLeg.transform.SetParent(LeftLegParent);

        Hips.transform.localScale = Vector3.one;
        Hips.transform.localPosition = Vector3.zero;
        Hips.transform.localEulerAngles = Vector3.zero;
        Hips.transform.name = "PLAYER_HIPS";

        RightLeg.transform.localScale = Vector3.one;
        RightLeg.transform.localPosition = Vector3.zero;
        RightLeg.transform.localEulerAngles = Vector3.zero;
        RightLeg.transform.name = "CHARACTER_LEG";

        LeftLeg.transform.localScale = Vector3.one;
        LeftLeg.transform.localPosition = Vector3.zero;
        LeftLeg.transform.localEulerAngles = Vector3.zero;
        LeftLeg.transform.name = "CHARACTER_LEG";

        Destroy(Legs);

        animStyle = cM.AnimationStyles[AnimationStyleSelected];

        upperBodyAnimator.Rebind();
        lowerBodyAnimator.Rebind();
        upperBodyAnimator.Update(0.1f);
        lowerBodyAnimator.Update(0.1f);
        Built = true;
        PlayAnimation(AnimationPlaying);
    }


    #region Animation

    public Animator upperBodyAnimator;
    public Animator lowerBodyAnimator;

    string LastUpperBodyAnimationPlayed = "";
    string LastLowerBodyAnimationPlayed = "";

    public void PlayAnimation(int Animation)
    {
        upperBodyAnimator.Rebind();
        lowerBodyAnimator.Rebind();
        upperBodyAnimator.Update(0.1f);
        lowerBodyAnimator.Update(0.1f); 

        int animationPrefix = animStyle.AnimationID;
        int animationID = animationPrefix + Animation;

        upperBodyAnimator.SetInteger("AnimationID", animationID);
        //lowerBodyAnimator.SetTrigger(LowerBodyAnimationName);

        Debug.Log("Updated Animation ID to " + animationID);
    }

    public int AnimationPlaying = 0;

    #endregion
}

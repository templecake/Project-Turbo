using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCharacterScript : MonoBehaviour
{
    CustomisationManager cM;

    void Start()
    {
       cM = FindObjectOfType<CustomisationManager>();
    }


    public bool Refresh = false;
    public bool Player;
    private void Update()
    {
        if (Refresh)
        {
            Refresh = false;
            RefreshCharacter();
        }
    }
    public List<GameObject> spawnedDisplayObjs = new List<GameObject>();

    public int HeadChosen;
    public int BodyChosen;
    public int LegsChosen;
    public int AnimationStyleChosen;

    Customisation head;
    Customisation body;
    Customisation legs;
    Customisation animStyle;

    #region Model Creation;

    public GameObject Head_Model;
    public GameObject Body_Model;

    public GameObject R_Arm_Top_Model;
    public GameObject R_Arm_Bottom_Model;
    public GameObject R_Hand;

    public GameObject L_Arm_Top_Model;
    public GameObject L_Arm_Bottom_Model;
    public GameObject L_Hand;

    public GameObject Hips_Model;

    public GameObject R_Leg_Top_Model;
    public GameObject R_Leg_Bottom_Model;
    public GameObject R_Foot_Model;

    public GameObject L_Leg_Top_Model;
    public GameObject L_Leg_Bottom_Model;
    public GameObject L_Foot_Model;

    public void RefreshCharacter()
    {
        cM = FindObjectOfType<CustomisationManager>();

        for (int i = 0; i < spawnedDisplayObjs.Count; i++) //CLEAR PREVIOUS CUSTOMISATIONS
        {
            Destroy(spawnedDisplayObjs[i].gameObject);
        }
        spawnedDisplayObjs.Clear();

        if (Player)
        {
            HeadChosen = cM.HeadSelected;
            BodyChosen = cM.BodySelected;
            LegsChosen = cM.LegsSelected;
            AnimationStyleChosen = cM.AnimationStyleSelected;
        }

        head = cM.Heads[HeadChosen];
        body = cM.Bodies[BodyChosen];
        legs = cM.Legs[LegsChosen];
        animStyle = cM.AnimationStyles[AnimationStyleChosen];

        //HEAD SPAWNING
        GameObject _head = Instantiate(head.InWorldModel.transform.Find("head").gameObject);
        _head.transform.SetParent(Head_Model.transform);
        _head.transform.name = "head";
        _head.transform.localScale = Vector3.one;
        _head.transform.localPosition = Vector3.zero;
        _head.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_head);

        GameObject _hat = Instantiate(head.InWorldModel.transform.Find("hat").gameObject);
        _hat.transform.SetParent(Head_Model.transform);
        _hat.transform.name = "hat";
        _hat.transform.localScale = Vector3.one;
        _hat.transform.localPosition = Vector3.zero;
        _hat.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_hat);

        GameObject _face = Instantiate(head.InWorldModel.transform.Find("face").gameObject);
        _face.transform.SetParent(Head_Model.transform);
        _face.transform.name = "hat";
        _face.transform.localScale = Vector3.one;
        _face.transform.localPosition = Vector3.zero;
        _face.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_face);

        //UPPER BODY SPAWNING
        GameObject _body = Instantiate(body.InWorldModel.transform.Find("body").gameObject);
        _body.transform.SetParent(Body_Model.transform);
        _body.transform.name = "body";
        _body.transform.localScale = Vector3.one;
        _body.transform.localPosition = Vector3.zero;
        _body.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_body);

        GameObject _r_arm_top = Instantiate(body.InWorldModel.transform.Find("r_arm_top").gameObject);
        _r_arm_top.transform.SetParent(R_Arm_Top_Model.transform);
        _r_arm_top.transform.name = "r_arm_top";
        _r_arm_top.transform.localScale = Vector3.one;
        _r_arm_top.transform.localPosition = Vector3.zero;
        _r_arm_top.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_r_arm_top);

        GameObject _r_arm_bottom = Instantiate(body.InWorldModel.transform.Find("r_arm_bottom").gameObject);
        _r_arm_bottom.transform.SetParent(R_Arm_Bottom_Model.transform);
        _r_arm_bottom.transform.name = "r_arm_bottom";
        _r_arm_bottom.transform.localScale = Vector3.one;
        _r_arm_bottom.transform.localPosition = Vector3.zero;
        _r_arm_bottom.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_r_arm_bottom);

        GameObject _r_hand = Instantiate(body.InWorldModel.transform.Find("r_hand").gameObject);
        _r_hand.transform.SetParent(R_Hand.transform);
        _r_hand.transform.name = "r_hand";
        _r_hand.transform.localScale = Vector3.one;
        _r_hand.transform.localPosition = Vector3.zero;
        _r_hand.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_r_hand);


        GameObject _l_arm_top = Instantiate(body.InWorldModel.transform.Find("l_arm_top").gameObject);
        _l_arm_top.transform.SetParent(L_Arm_Top_Model.transform);
        _l_arm_top.transform.name = "l_arm_top";
        _l_arm_top.transform.localScale = Vector3.one;
        _l_arm_top.transform.localPosition = Vector3.zero;
        _l_arm_top.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_l_arm_top);

        GameObject _l_arm_bottom = Instantiate(body.InWorldModel.transform.Find("l_arm_bottom").gameObject);
        _l_arm_bottom.transform.SetParent(L_Arm_Bottom_Model.transform);
        _l_arm_bottom.transform.name = "l_arm_bottom";
        _l_arm_bottom.transform.localScale = Vector3.one;
        _l_arm_bottom.transform.localPosition = Vector3.zero;
        _l_arm_bottom.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_l_arm_bottom);

        GameObject _l_hand = Instantiate(body.InWorldModel.transform.Find("l_hand").gameObject);
        _l_hand.transform.SetParent(L_Hand.transform);
        _l_hand.transform.name = "l_hand";
        _l_hand.transform.localScale = Vector3.one;
        _l_hand.transform.localPosition = Vector3.zero;
        _l_hand.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_l_hand);

        //LOWER BODY SPAWNING
        GameObject _hips = Instantiate(legs.InWorldModel.transform.Find("hips").gameObject);
        _hips.transform.SetParent(Hips_Model.transform);
        _hips.transform.name = "hips";
        _hips.transform.localScale = Vector3.one;
        _hips.transform.localPosition = Vector3.zero;
        _hips.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_hips);

        GameObject _r_leg_top = Instantiate(legs.InWorldModel.transform.Find("r_leg_top").gameObject);
        _r_leg_top.transform.SetParent(R_Leg_Top_Model.transform);
        _r_leg_top.transform.name = "r_leg_top";
        _r_leg_top.transform.localScale = Vector3.one;
        _r_leg_top.transform.localPosition = Vector3.zero;
        _r_leg_top.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_r_leg_top);

        GameObject _r_leg_bottom = Instantiate(legs.InWorldModel.transform.Find("r_leg_bottom").gameObject);
        _r_leg_bottom.transform.SetParent(R_Leg_Bottom_Model.transform);
        _r_leg_bottom.transform.name = "r_leg_bottom";
        _r_leg_bottom.transform.localScale = Vector3.one;
        _r_leg_bottom.transform.localPosition = Vector3.zero;
        _r_leg_bottom.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_r_leg_bottom);

        GameObject _r_foot = Instantiate(legs.InWorldModel.transform.Find("r_foot").gameObject);
        _r_foot.transform.SetParent(R_Foot_Model.transform);
        _r_foot.transform.name = "r_foot";
        _r_foot.transform.localScale = Vector3.one;
        _r_foot.transform.localPosition = Vector3.zero;
        _r_foot.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_r_foot);


        GameObject _l_leg_top = Instantiate(legs.InWorldModel.transform.Find("l_leg_top").gameObject);
        _l_leg_top.transform.SetParent(L_Leg_Top_Model.transform);
        _l_leg_top.transform.name = "l_leg_top";
        _l_leg_top.transform.localScale = Vector3.one;
        _l_leg_top.transform.localPosition = Vector3.zero;
        _l_leg_top.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_l_leg_top);

        GameObject _l_leg_bottom = Instantiate(legs.InWorldModel.transform.Find("l_leg_bottom").gameObject);
        _l_leg_bottom.transform.SetParent(L_Leg_Bottom_Model.transform);
        _l_leg_bottom.transform.name = "l_leg_bottom";
        _l_leg_bottom.transform.localScale = Vector3.one;
        _l_leg_bottom.transform.localPosition = Vector3.zero;
        _l_leg_bottom.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_l_leg_bottom);

        GameObject _l_foot = Instantiate(legs.InWorldModel.transform.Find("l_foot").gameObject);
        _l_foot.transform.SetParent(L_Foot_Model.transform);
        _l_foot.transform.name = "l_foot";
        _l_foot.transform.localScale = Vector3.one;
        _l_foot.transform.localPosition = Vector3.zero;
        _l_foot.transform.localRotation = Quaternion.identity;
        spawnedDisplayObjs.Add(_l_foot);
    }

    #endregion

    #region Animation

    public Animator upperBodyAnimator;
    public Animator lowerBodyAnimator;

    public void PlayAnimation(string Animation)
    {
        string animationPrefix = animStyle.AnimationPrefix;
        string UpperBodyAnimationName = (animationPrefix + "_UPPERBODY_" + Animation);
        string LowerBodyAnimationName = (animationPrefix + "_LOWERBODY_" + Animation);

        upperBodyAnimator.SetTrigger(UpperBodyAnimationName);
        lowerBodyAnimator.SetTrigger(LowerBodyAnimationName);
    }

    public void AnimatorSetBool(string BoolName, bool value)
    {
        upperBodyAnimator.SetBool(BoolName, value);
        lowerBodyAnimator.SetBool(BoolName, value);
    }
    #endregion

}

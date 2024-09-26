using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class QuestStorage : ScriptableObject
{
    [Header("Quest Information")]
    public string QuestName;
    public string QuestDescription;
    public Sprite QuestDisplayImage;

    [Header("Quest Requirements")]
    public QuestManager.QuestType QuestType; //place first, place top 3, hit opponent with item, collect cubits
    public int AmountNeeded;

    [Header("Currency Rewards")]
    public int CubitReward;
    public int GolbitReward;
    public int AdbitReward;
    public int MultiplierStepReward;

    [Header("Item Rewards")]
    public int[] CustomisationReward;
    public int[] BlockReward;
    public int[] BlockRewardAmount;
    public int[] CrateReward;
}

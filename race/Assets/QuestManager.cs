using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    #region Quest Information
    public List<QuestStorage> Quests;

    public List<int> QuestIDs;
    public List<int> QuestAmountDone;
    public List<int> SlotRaceNumber;

    

    public enum QuestType
    {
        Place_First, Place_Top_Three, Hit_Players, Collect_Cubits 
    }

    public string QuestStatus(int number)
    {
        QuestStorage thisQuest = Quests[QuestIDs[number]];
        return thisQuest.QuestDescription + " (" + QuestAmountDone[number] + "/" + thisQuest.AmountNeeded + ")";
    }

    public string FinishedQuestStatus(int questID)
    {
        QuestStorage thisQuest = Quests[questID];
        return thisQuest.QuestDescription + " (" + thisQuest.AmountNeeded + "/" + thisQuest.AmountNeeded + ")";
    }

    public Sprite QuestDisplayImage(int number)
    {
        return Quests[QuestIDs[number]].QuestDisplayImage;
    }

    public float QuestCompletion(int number)
    {
        QuestStorage thisQuest = Quests[QuestIDs[number]];
        return ((float)QuestAmountDone[number] / (float)thisQuest.AmountNeeded);
    }
    #endregion

    #region Unity Functions

    public void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            AddNewQuest();
        }
    }

    #endregion

    #region In Race

    [Header("In Race")]
    public List<int> QuestsToShow;
    public List<bool> QuestCompleted;
    public List<int> QuestsProgressBeforeRace;
    int SlotNumOn;

    public void GetPreRaceQuestInformation()
    {
        QuestsToShow.Clear();
        QuestCompleted.Clear();
        QuestsProgressBeforeRace.Clear();

        for (int i = 0; i < QuestIDs.Count; i++)
        {
            QuestsToShow.Add(QuestIDs[i]);
            QuestCompleted.Add(false);
        }

        for (int i = 0; i < QuestAmountDone.Count; i++)
        {
            QuestsProgressBeforeRace.Add(QuestAmountDone[i]);
        }

        for (int i = 0; i < SlotRaceNumber.Count; i++)
        {
            SlotRaceNumber[i] = i;
        }
        SlotNumOn = 5;
    }

    #endregion

    #region Quest Adding and Removing
    public void AddNewQuest()
    {
        int qid = Random.Range(0, Quests.Count);
        QuestIDs.Add(qid);
        QuestAmountDone.Add(0);

        QuestsToShow.Add(qid);
        QuestsProgressBeforeRace.Add(0);
        QuestCompleted.Add(false);
        SlotRaceNumber.Add(SlotNumOn);
        SlotNumOn++;
    } 
    
    public void RemoveQuest(int at)
    {
        QuestIDs.RemoveAt(at);
        QuestAmountDone.RemoveAt(at);
        SlotRaceNumber.RemoveAt(at);
    }

    public void CheckQuests()
    {
        for (int i = 0; i < QuestIDs.Count; i++) 
        {
            if (QuestAmountDone[i] >= Quests[QuestIDs[i]].AmountNeeded)
            {
                QuestCompleted[SlotRaceNumber[i]] = true;
                FinishQuest(i);
            }
        }
    }

    public void FinishQuest(int number)
    {
        //reward player
        BlockStorage bs = FindObjectOfType<BlockStorage>();
        CustomisationManager cm = FindObjectOfType<CustomisationManager>();
        CrateManager cratem = FindObjectOfType<CrateManager>();
        Player player = FindObjectOfType<Player>();

        QuestStorage ThisQuest = Quests[number];


        for (int i = 0; i < ThisQuest.BlockReward.Length; i++)
        {
            if (ThisQuest.BlockReward[i] >= -1) bs.AddBlock(ThisQuest.BlockReward[i], ThisQuest.BlockRewardAmount[i]);
        }

        for (int i = 0; i < ThisQuest.CustomisationReward.Length; i++)
        {
            if (ThisQuest.CustomisationReward[i] > -1) cm.UnlockItem(ThisQuest.CustomisationReward[i]);
        }

        for (int i = 0; i < ThisQuest.CrateReward.Length; i++)
        {
            if (ThisQuest.CrateReward[i] > -1) cratem.AddCrate(ThisQuest.CrateReward[i]);
        }

        player.AddCubits(ThisQuest.CubitReward);
        player.AddAdventureCubits(ThisQuest.AdbitReward);
        player.AddGoldenCubits(ThisQuest.GolbitReward);
        player.MultiplierPoints += ThisQuest.MultiplierStepReward;


        //remove quest
        RemoveQuest(number);

        AddNewQuest();
    }

    #endregion

    #region Quest Completion

    public void PlacedFirst()
    {
        for (int i = 0; i < QuestIDs.Count; i++)
        {
            if (Quests[QuestIDs[i]].QuestType == QuestType.Place_First)
            {
                QuestAmountDone[i]++;
            } 
        }
        CheckQuests();
    }

    public void PlacedTopThree()
    {
        for (int i = 0; i < QuestIDs.Count; i++)
        {
            if (Quests[QuestIDs[i]].QuestType == QuestType.Place_Top_Three)
            {
                QuestAmountDone[i]++;
            }
        }
        CheckQuests();
    }

    public void HitPlayerWithItem()
    {
        for (int i = 0; i < QuestIDs.Count; i++)
        {
            if (Quests[QuestIDs[i]].QuestType == QuestType.Hit_Players)
            {
                QuestAmountDone[i]++;
            }
        }
        CheckQuests();
    }

    public void CollectedCubitsInRace()
    {
        for (int i = 0; i < QuestIDs.Count; i++)
        {
            if (Quests[QuestIDs[i]].QuestType == QuestType.Collect_Cubits)
            {
                QuestAmountDone[i]++;
            }
        }
        CheckQuests();
    }


    #endregion
}

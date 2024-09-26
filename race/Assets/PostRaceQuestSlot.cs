using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PostRaceQuestSlot : MonoBehaviour
{
    QuestManager qm;
    [Header("Quest Information")]
    public int QuestID;
    public int StartProgress;

    public bool QuestFinished;

    public bool Finished;

    public int QuestSlotNumber;

    [Header("UI Items")]
    public Image DisplayImage;
    public Slider ProgressSlider;
    public Text InformationText;
    public GameObject InformationPanel;
    public GameObject RewardsPanel;

    public Transform RewardsParent;
    public GameObject RewardsSlot;
    public GameObject[] StandardRewards;

    public void Start()
    {
        qm = FindObjectOfType<QuestManager>();
        if (QuestFinished)
        {
            StartCoroutine(RunFinished());
        }
        else
        {
            StartCoroutine(RunProgress());
        }
        transform.localScale = Vector3.zero;
    }


    float ProgressMade;

    IEnumerator RunFinished()
    {
        QuestStorage thisQuest = qm.Quests[QuestID];

        InformationText.text = qm.FinishedQuestStatus(QuestID);
        DisplayImage.sprite = qm.Quests[QuestID].QuestDisplayImage;
        float AmountFrom = StartProgress;
        float AmountTo = qm.Quests[QuestID].AmountNeeded;
        float ProgressMade = AmountFrom / AmountTo;
        ProgressSlider.value = ProgressMade;

        float scale = 0;
        while(scale < 1)
        {
            scale += Time.deltaTime * 4;
            transform.localScale = Vector3.one * scale;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.localScale = Vector3.one;

        while (ProgressMade < 1)
        {
            ProgressMade += Time.deltaTime * 2;
            ProgressSlider.value = ProgressMade;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ProgressSlider.value = 1;

        scale = 1;
        while(scale > 0)
        {
            scale -= Time.deltaTime * 10;
            InformationPanel.transform.localScale = Vector3.one * scale;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        InformationPanel.SetActive(false);
        RewardsPanel.SetActive(true);
        RewardsPanel.transform.localScale = Vector3.zero;
        scale = 0;
        while(scale < 1) 
        {
            scale += Time.deltaTime * 10;
            RewardsPanel.transform.localScale = Vector3.one * scale;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        RewardsPanel.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(0.25f);

        float RewardWaitTime = 0.125f;

        if (thisQuest.CubitReward > 0)
        {
            StandardRewards[0].SetActive(true);
            StandardRewards[0].GetComponentInChildren<Text>().text = thisQuest.CubitReward.ToString();
            yield return new WaitForSeconds(RewardWaitTime);
        }

        if (thisQuest.AdbitReward > 0)
        {
            StandardRewards[1].SetActive(true);
            StandardRewards[1].GetComponentInChildren<Text>().text = thisQuest.AdbitReward.ToString();
            yield return new WaitForSeconds(RewardWaitTime);
        }

        if (thisQuest.GolbitReward > 0)
        {
            StandardRewards[2].SetActive(true);
            StandardRewards[2].GetComponentInChildren<Text>().text = thisQuest.GolbitReward.ToString();
            yield return new WaitForSeconds(RewardWaitTime);
        }

        if (thisQuest.MultiplierStepReward > 0)
        {
            StandardRewards[3].SetActive(true);
            StandardRewards[3].GetComponentInChildren<Text>().text = thisQuest.MultiplierStepReward.ToString();
            yield return new WaitForSeconds(RewardWaitTime);
        }

        for (int i = 0; i < thisQuest.BlockReward.Length; i++)
        {
            GameObject slot = Instantiate(RewardsSlot, RewardsParent);
            slot.transform.localScale = Vector3.zero;
            slot.GetComponent<QuestItemRewardSlotRace>().BlockID = thisQuest.BlockReward[i];
            slot.GetComponent<QuestItemRewardSlotRace>().BlockAmount = thisQuest.BlockRewardAmount[i];
            slot.GetComponent<QuestItemRewardSlotRace>().StartUp();
            yield return new WaitForSeconds(RewardWaitTime);
        }

        for (int i = 0; i < thisQuest.CustomisationReward.Length; i++)
        {
            GameObject slot = Instantiate(RewardsSlot, RewardsParent);
            slot.transform.localScale = Vector3.zero;
            slot.GetComponent<QuestItemRewardSlotRace>().CustomisationID = thisQuest.CustomisationReward[i];
            slot.GetComponent<QuestItemRewardSlotRace>().StartUp();
            yield return new WaitForSeconds(RewardWaitTime);
        }

        for (int i = 0; i < thisQuest.CrateReward.Length; i++)
        {
            GameObject slot = Instantiate(RewardsSlot, RewardsParent);
            slot.transform.localScale = Vector3.zero;
            slot.GetComponent<QuestItemRewardSlotRace>().CrateID = thisQuest.CrateReward[i];
            slot.GetComponent<QuestItemRewardSlotRace>().StartUp();
            yield return new WaitForSeconds(RewardWaitTime);
        }

        yield return new WaitForSeconds(.25f);
        scale = 1;
        while(scale > 0)
        {
            scale -= Time.deltaTime * 4;
            transform.localScale = Vector3.one * scale;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        Finished = true;
        Destroy(gameObject);

        yield return false;
    }

    IEnumerator RunProgress()
    {
        InformationText.text = qm.QuestStatus(QuestSlotNumber);
        DisplayImage.sprite = qm.Quests[QuestID].QuestDisplayImage;
        float AmountFrom = StartProgress;
        float AmountTo = qm.Quests[QuestID].AmountNeeded;
        float ProgressMade = (float)AmountFrom / (float)AmountTo;
        ProgressSlider.value = ProgressMade;
        float ProgressTo = (float)qm.QuestAmountDone[QuestSlotNumber] / (float)AmountTo;

        float scale = 0;
        while (scale < 1)
        {
            scale += Time.deltaTime * 4;
            transform.localScale = Vector3.one * scale;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.localScale = Vector3.one;

        

        while (ProgressMade < ProgressTo)
        {
            ProgressMade += Time.deltaTime * 2;
            ProgressSlider.value = ProgressMade;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        ProgressSlider.value = ProgressTo;

        Finished = true;

        yield return false;
    }

    void Update()
    {
        
    }
}

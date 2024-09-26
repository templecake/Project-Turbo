using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : MonoBehaviour
{
    public int QuestSlotID;
    QuestManager qM;
    RealWorldMainMenu rM;

    [SerializeField] Image displayImage;
    [SerializeField] Text displayText;

    public void RefreshSlot()
    {
        qM = FindObjectOfType<QuestManager>();
        rM = FindObjectOfType<RealWorldMainMenu>();

        displayImage.sprite = qM.QuestDisplayImage(QuestSlotID);
        displayText.text = qM.QuestStatus(QuestSlotID);
    }

    public void RemoveQuest()
    {
        rM.QuestCurrentlyDisplaying = 0;
        qM.RemoveQuest(QuestSlotID);
        qM.AddNewQuest();
        rM.RefreshQuestDisplayBoard();
    }

    public void SelectQuest()
    {
        rM.QuestCurrentlyDisplaying = QuestSlotID;
        rM.RefreshQuestDisplayBoard();
    }

}

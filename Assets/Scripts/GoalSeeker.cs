using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoalSeeker : MonoBehaviour
{

    public TextMeshProUGUI Exercise;
    //public Text Sleep;
    //public Text Game;
    public TextMeshProUGUI Discontent;
    public TextMeshProUGUI Speech;

    Goal[] myGoals;
    Action[] myActions;
    Action myChangeOverTime;
    const float TICK_LENGTH = 3f;

    void Start()
    {
        myGoals = new Goal[3];
        myGoals[0] = new Goal("Exercise", 4);
        myGoals[1] = new Goal("Sleep", 3);
        myGoals[2] = new Goal("Game", 3);


        myActions = new Action[3];
        myActions[0] = new Action("working out");
        myActions[0].targetGoals.Add(new Goal("Exercise", -3f));
        myActions[0].targetGoals.Add(new Goal("Sleep", +4f));
        myActions[0].targetGoals.Add(new Goal("Game", +2f));

        myActions[1] = new Action("taking a nap");
        myActions[1].targetGoals.Add(new Goal("Exercise", +1f));
        myActions[1].targetGoals.Add(new Goal("Sleep", -5f));
        myActions[1].targetGoals.Add(new Goal("Game", +2f));

        myActions[2] = new Action("playing a game");
        myActions[2].targetGoals.Add(new Goal("Exercise", +2f));
        myActions[2].targetGoals.Add(new Goal("Sleep", +1f));
        myActions[2].targetGoals.Add(new Goal("Game", -3f));

        myChangeOverTime = new Action("time tick");
        myChangeOverTime.targetGoals.Add(new Goal("Exercise", +4f));
        myChangeOverTime.targetGoals.Add(new Goal("Sleep", +1f));
        myChangeOverTime.targetGoals.Add(new Goal("Game", +2f));

        Debug.Log("Starting clock. A single hour will pass every " + TICK_LENGTH + " seconds.");
        InvokeRepeating("TimeTick", 0f, TICK_LENGTH);

        Debug.Log("Hit W to begin actions");
    }

    void TimeTick()
    {
        //rateSinceLastTime = changeSinceLastTime / timeSinceLast
        //basicRate = 0.95 * basicRate + 0.05 * rateSinceLastTime

        foreach(Goal goal in myGoals)
        {
            goal.Value += myChangeOverTime.GetGoalChange(goal);
            goal.Value = Mathf.Max(goal.Value, 0);
        }
        PrintGoals();
    }

    void PrintGoals()
    {
        string stringGoal = "";
        foreach (Goal goal in myGoals)
        {
            stringGoal += goal.Name + ": " + goal.Value + "; ";
        }
        stringGoal += "Discomfort: " + CurrentDiscomfort();
        Debug.Log(stringGoal);
        Exercise.text = stringGoal;

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Action bestAction = DecideAction(myActions, myGoals);
            Speech.text = "I feel like " + bestAction.Name;
            //GetComponent<TMPro.TextMeshProUGUI>("I feel like " + bestAction.Name).text;
            //TextMeshProUGUI Speech = GetComponent<TextMeshProUGUI>("I feel like " + bestAction.Name);
            Debug.Log("I feel like " + bestAction.Name);

            foreach(Goal goal in myGoals)
            {
                goal.Value += bestAction.GetGoalChange(goal);
                goal.Value = Mathf.Max(goal.Value, 0);
            }
            PrintGoals();
        }
        //TextMeshProUGUI Exercise = GetComponent<TextMeshProUGUI>("I feel like " + bestAction.Name);
        //TextMeshProUGUI Discontent = GetComponent<TextMeshProUGUI>("I feel like " + bestAction.Name);
        //TextMeshProUGUI Game = GetComponent<TextMeshProUGUI>("I feel like " + bestAction.Name);
        //TextMeshProUGUI Sleep = GetComponent<TextMeshProUGUI>("I feel like " + bestAction.Name);

    }
    Action DecideAction(Action[] actions, Goal[] goals)
    {
        Action urgentAction = null;
        float urgentValue = float.PositiveInfinity;

        foreach (Action action in actions)
        {
            float thisValue = Discomfort(action, goals);

            if(thisValue < urgentValue)
            {
                urgentValue = thisValue;
                urgentAction = action;
            }
        }
        return urgentAction;
    }
    float CurrentDiscomfort()
    {
        float total = 0;
        foreach(Goal goal in myGoals)
        {
            total += (goal.Value * goal.Value);
        }
        return total;
    }
    float Discomfort(Action action, Goal[] goals)
    {
        float discomfort = 0;
        foreach(Goal goal in goals)
        {
            float newValue = goal.Value + action.GetGoalChange(goal);
            newValue = Mathf.Max(newValue);

            discomfort += goal.GainDiscomfort(newValue);
        }
        return discomfort;
    }
}

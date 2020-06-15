using UnityEngine;

[CreateAssetMenu(fileName = "NewTasks", menuName = "Task Data")]
public class TaskData : ScriptableObject {
    public DrinkTask[] tasks;
}


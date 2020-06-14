using UnityEngine;

[CreateAssetMenu(fileName = "NewTasks", menuName = "Task Data")]
public class TaskData : ScriptableObject {
    public DrinkTask[] tasks;
}

[System.Serializable]
public class DrinkTask {
    [TextArea]
    public string message;

    public DrinkTask (string newMessage) {
        message = newMessage;
    }
}

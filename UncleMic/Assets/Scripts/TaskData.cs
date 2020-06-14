using UnityEngine;

[CreateAssetMenu(fileName = "NewTasks", menuName = "Task Data")]
public class TaskData : ScriptableObject {
    public Task[] tasks;
}

[System.Serializable]
public class Task {
    public string message;
}

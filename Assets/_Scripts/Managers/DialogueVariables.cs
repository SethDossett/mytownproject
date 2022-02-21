using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;

public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

    Story globalVariablesStory;

    public DialogueVariables(TextAsset loadGlobalsJSON)
    {
        globalVariablesStory = new Story(loadGlobalsJSON.text);

        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach(string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
            Debug.Log($"Initialized global dialogue variable: {name} = {value}");
        }
    }


    public void StartListening(Story story) =>
         story.variablesState.variableChangedEvent += VariablesChanged;

    public void StopListening(Story story) => 
        story.variablesState.variableChangedEvent -= VariablesChanged;

    private void VariablesChanged(string name, Ink.Runtime.Object value)
    {
        Debug.Log("Variable Changed: " + name + " = " + value);
    }
}

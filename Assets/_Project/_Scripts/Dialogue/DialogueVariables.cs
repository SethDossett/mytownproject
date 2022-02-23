using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace MyTownProject.Dialogue
{
    public class DialogueVariables
    {
        public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

        Story globalVariablesStory;

        public DialogueVariables(TextAsset loadGlobalsJSON)
        {
            globalVariablesStory = new Story(loadGlobalsJSON.text);

            variables = new Dictionary<string, Ink.Runtime.Object>();
            foreach (string name in globalVariablesStory.variablesState)
            {
                Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
                variables.Add(name, value);
                Debug.Log($"Initialized global dialogue variable: {name} = {value}");
            }
        }


        public void StartListening(Story story)
        {
            VariablesToStory(story);
            story.variablesState.variableChangedEvent += VariablesChanged;
        }

        public void StopListening(Story story) =>
            story.variablesState.variableChangedEvent -= VariablesChanged;


        private void VariablesChanged(string name, Ink.Runtime.Object value)
        {
            Debug.Log("Variable Changed: " + name + " = " + value);
            if (variables.ContainsKey(name))
            {
                variables.Remove(name);
                variables.Add(name, value);
            }
        }

        private void VariablesToStory(Story story)
        {
            foreach (KeyValuePair<string, Ink.Runtime.Object> variable in variables)
            {
                story.variablesState.SetGlobal(variable.Key, variable.Value);
            }
        }
    }
}
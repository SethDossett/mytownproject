namespace MyTownProject.UI
{
    public enum PromptName{
        Null, Open, Talk, Crouch
    }
    [System.Serializable]
    public class Prompt
    {
        public PromptName name;
        public int priority;
        public string text;

        public Prompt(){
            name = new PromptName();
            priority = new int();
            text = "";
        }
    }

}

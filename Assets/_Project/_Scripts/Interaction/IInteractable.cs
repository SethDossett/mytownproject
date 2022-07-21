namespace MyTownProject.Interaction
{
    public interface IInteractable
    {
        float MaxRange { get; }
        bool CanBeInteractedWith { get; }
        bool CanBeTargeted { get; }
        string Prompt { get; }


        void OnFocus(string _string);
        void OnInteract(PlayerRacasting playerRacasting);
        void OnLoseFocus();
    }
}
public interface IGenericInteractable
{
    public abstract void Interact();
    public abstract void Stop();
    public abstract void Cancel();
    public abstract bool IsReady();
    public abstract bool IsNearMode();
    
}
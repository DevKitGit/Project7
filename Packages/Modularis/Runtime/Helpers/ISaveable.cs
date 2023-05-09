namespace Devkit.Modularis.Helpers
{
    public interface ISaveable<T>
    {
        public abstract void Save(T from);
        
    }
}
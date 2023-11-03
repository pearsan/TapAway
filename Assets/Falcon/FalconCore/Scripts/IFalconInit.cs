namespace Falcon.FalconCore.Scripts
{
    public interface IFalconInit
    {
        void Init();

        int GetPriority();

        bool InitInMainThread();
    }
}
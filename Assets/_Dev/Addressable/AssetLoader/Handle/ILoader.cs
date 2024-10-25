using Cysharp.Threading.Tasks;
using System;

public interface ILoader
{
    public Action OnComplete { get; set; }
    public Action<float> OnProgress { get; set; }
    public abstract void PrepareLoad(bool showPopup);

    public abstract void AddLoadingQueue();
    public abstract UniTask LoadStart();

}

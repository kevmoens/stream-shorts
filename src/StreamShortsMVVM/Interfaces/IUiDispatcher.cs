namespace StreamShorts.MVVM.Interfaces;
public interface IUiDispatcher
{
    Task InvokeAsync(Func<Task> action);
}
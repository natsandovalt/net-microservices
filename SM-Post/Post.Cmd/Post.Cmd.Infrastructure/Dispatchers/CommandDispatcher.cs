using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Dispatchers {
  public class CommandDispatcher : ICommandDispatcher {
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();
    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand {
      if (_handlers.ContainsKey(typeof(T)))
        throw new IndexOutOfRangeException("You cannot register the same command handler twice!");
      
      _handlers.Add(typeof(T), x => handler((T)x));
    }

    public async Task SendAsync(BaseCommand command) {
      if (_handlers.TryGetValue(command.GetType(), out var handler) && handler != null) {
        await handler(command);
      } else {
        throw new ArgumentNullException(nameof(handler), "No command handler was registered");
      }
    }
  }
}
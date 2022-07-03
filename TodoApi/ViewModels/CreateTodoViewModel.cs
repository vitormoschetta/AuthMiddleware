using Flunt.Notifications;
using Flunt.Validations;
using TodoApi.Models;

namespace TodoApi.ViewModels;
public class CreateTodoViewModel : Notifiable<Notification>
{
    public string Title { get; set; }

    public Todo MapTo()
    {
        AddNotifications(new Contract<Notification>()
            .Requires()
            .IsNotNull(Title, "Informe o título da tarefa")
            .IsGreaterThan(Title, 3, "O título deve conter pelo menos 3 caracteres"));

        return new Todo(Title);
    }
}

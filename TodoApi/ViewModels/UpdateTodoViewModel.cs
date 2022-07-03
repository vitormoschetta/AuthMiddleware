using Flunt.Notifications;
using Flunt.Validations;
using TodoApi.Models;

namespace TodoApi.ViewModels;
public class UpdateTodoViewModel : Notifiable<Notification>
{
    public string Title { get; set; }
    public bool Done { get; set; }

    public void MapTo(Todo todo)
    {
        AddNotifications(new Contract<Notification>()
            .Requires()
            .IsNotNull(Title, "Informe o título da tarefa")
            .IsGreaterThan(Title, 3, "O título deve conter pelo menus 3 caracteres"));

        todo.Update(Title, Done);
    }
}
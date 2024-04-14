using l2;
using System.Text.Json;



var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.Map("/ListOfPeople", ListOfPeople);
app.Map("/SearchByEmail", SearchByEmail);
app.Map("/FilterBySex", FilterBySex);
app.Map("/DeletePerson", DeletePerson);
app.Map("/AddPerson", AddPerson);

app.Run();

static void ListOfPeople(IApplicationBuilder app)
{
    app.Run(async context =>
    {

        if (GlobalVariables.people.DataPeople.Count != 0)
        {
            await context.Response.WriteAsJsonAsync(GlobalVariables.people.DataPeople);
        }
        else
        {

            await context.Response.WriteAsJsonAsync(new { Error = "Missing data!" });
        }
    });
}
//Даний делегат записує у змінну email значення параметру запиту, після чого викликається метод SearchByEmail(), якщо у колекції не буде знайдено об'єкта,
//значення поля Email буде дорівнювати значенню email, то він поверне повідомлення, що такого Email немає у колекції,
//інакше поверне об'єкт, об'єкт який відповідає умові у JSON-форматі. 
 static void SearchByEmail(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        string email = context.Request.Query["email"];
        Person person = GlobalVariables.people.SearchByEmail(email);
        if (person != null)
        {

            await context.Response.WriteAsJsonAsync(GlobalVariables.people.SearchByEmail(email));
        }
        else
        {
            await context.Response.WriteAsJsonAsync(new { Error = "Email not found" });
        }
    });
}
//Делегат записує значення параметру запиту у змінну sex. Тепер викликаємо метод FilterBySex(), який поверне список об'єктів, які відповідають заданій умові.
//Якщо у списку немає ні одного елемента, то виводимо повідомлення, що такої статті немає у колекції. Якщо ж є, то тоді виведемо список об'єкттів,.які задовільняють умову
// у форматі JSON-масиву
 static void FilterBySex(IApplicationBuilder app)
{
    app.Run(async context =>
    {

        string sex = context.Request.Query["sex"];
        List<Person> listperson = GlobalVariables.people.FilterBySex(sex);
        if (listperson.Count != 0)
        {
            await context.Response.WriteAsJsonAsync(GlobalVariables.people.FilterBySex(sex));
        }
        else
        {
            await context.Response.WriteAsJsonAsync(new { Error = "Sex not found" });
        }
    });
}
//Цей делегат, також зчитує та записує у змінну email ключ параметру запиту. Видаляє об'єкт за заданим email, за допомогою метода DeletePerson(). Також у змінну
//записуємо значення поля FirstName, щоб при виведені повідомлення про видалення, вказати, кого саме видалено.
 static void DeletePerson(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        string email = context.Request.Query["email"];
        string name = GlobalVariables.people.SearchByEmail(email).FirstName;
        GlobalVariables.people.DeletePerson(email, GlobalVariables.filename);
        await context.Response.WriteAsJsonAsync(new { Status = $"Person {name} is succesful deleted" });
    });
}
//Отже, останній делегат, який ми використовуємо для цієї веб-служби спотку записує у змінну знаення тіла запиту. Потім відкриваємо файловий створенний на основі цієї змінної.
// У іншу змінну зчитуємо та записуємо значення тіла запиту. Після цього десеріалізуємо його, додаєм це значення у колекцію та виводимо відповідне повідомлення 
 static void AddPerson(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        var request = context.Request.Body;
        string body;
        using (StreamReader reader = new StreamReader(request))
        {
            body = await reader.ReadToEndAsync();
        }
        Person form = JsonSerializer.Deserialize<Person>(body);
        string answer = GlobalVariables.people.AddPerson(form, GlobalVariables.filename);
        await context.Response.WriteAsJsonAsync(new { Status = answer });
    });
}
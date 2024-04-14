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
//����� ������� ������ � ����� email �������� ��������� ������, ���� ���� ����������� ����� SearchByEmail(), ���� � �������� �� ���� �������� ��'����,
//�������� ���� Email ���� ���������� �������� email, �� �� ������� �����������, �� ������ Email ���� � ��������,
//������ ������� ��'���, ��'��� ���� ������� ���� � JSON-������. 
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
//������� ������ �������� ��������� ������ � ����� sex. ����� ��������� ����� FilterBySex(), ���� ������� ������ ��'����, �� ���������� ������ ����.
//���� � ������ ���� � ������ ��������, �� �������� �����������, �� ���� ����� ���� � ��������. ���� � �, �� ��� �������� ������ ��'�����,.�� ������������ �����
// � ������ JSON-������
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
//��� �������, ����� ����� �� ������ � ����� email ���� ��������� ������. ������� ��'��� �� ������� email, �� ��������� ������ DeletePerson(). ����� � �����
//�������� �������� ���� FirstName, ��� ��� ������� ����������� ��� ���������, �������, ���� ���� ��������.
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
//����, ������� �������, ���� �� ������������� ��� ���� ���-������ ������ ������ � ����� ������� ��� ������. ���� ��������� �������� ���������� �� ����� ���� �����.
// � ���� ����� ������� �� �������� �������� ��� ������. ϳ��� ����� ����������� ����, ����� �� �������� � �������� �� �������� �������� ����������� 
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
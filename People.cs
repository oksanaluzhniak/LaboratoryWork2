using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace l2
{
    public class People
    {
        public static JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        public List<Person> DataPeople { get; set; }
        //конструктор класу, у якому завантажуються дані з файлу 
        public People(string filename)
        {

            LoadData(filename);
        }
        //метод завантаження даних: спочатку ініціалізовуємо новий екземпляр FileInfo, який виступає обгорткою для шляху до файлу,
        // потім якщо файлу за шляхом filename не існує, то створюємо новий файл.
        //Після цього відриваємо файловий потік для зчитування файлу та  використовуємо директиву using для того, щоб одразу закривати потік;
        //зчитуємо у стрінгову змінну значення файлу, десеріалізуємо вміст файлу і записуємо у властивість DataPeople,
        //при виникнені будь-яких виключень у DataPeople записуємо пусти список класу Person
        public void LoadData(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            if (!fi.Exists)
            {
                FileStream fs = fi.Create();
                fs.Close();
            }
            try
            {
                using (StreamReader fileObj = new StreamReader(filename))
                {
                    string datapeople = fileObj.ReadToEnd();
                    try
                    {
                        this.DataPeople = JsonSerializer.Deserialize<List<Person>>(datapeople)!;
                    }
                    catch (Exception)
                    {
                        this.DataPeople = new List<Person>();
                    }
                }
            }
            catch (Exception)
            {
                this.DataPeople = new List<Person>();
            }
        }
        //метод, для збереженння нових даних у файл. Спочатку серіалізуємо з відступами у змінну text.
        //Після чого відриваємо файловий потік для зчитування файлу, обов'язково використовуємо директиву using для того, щоб одразу закривати потік.
        //Потім записуємо в файл серіалізовані дані
        public void SaveData(List<Person> people, string filename)
        {
            string text = JsonSerializer.Serialize(people, options);
            using (StreamWriter fileObj = new StreamWriter(filename))
            {
                fileObj.Write(text);
            }
        }
        //метод, який шукає задане значення і повертає екзепляр класу, в якому це значенн збігається
        //ініціалізуємо новий об'єкт класу, якому присвоюємо елемент списку, містить елементи з вхідної послідовності, які задовольняють умову.
        public Person SearchByEmail(string email)
        {
            Person persona = this.DataPeople.Where(p => p.Email == email).FirstOrDefault()!;
            return persona;
        }
        //метод який , який поверртає список осіб, які задовільняють задану умову
        public List<Person> FilterBySex(string sex)
        {
            List<Person> persona = new List<Person>(this.DataPeople.Where(p => p.Sex == sex));
            return persona;
        }
        //Метод, для додавання об'єкту класу в файл. Спочатку перевіряємо валідність введенних даних, коли вони не валідні, то виводимо відповідне
        ////повідомлення. Потім якщо у файлі немає особоми з заданим значенням електронної адреси,яка
        //виступає як унікальний ідентифікатор, то тоді додаємо об'єкт класу у колекцію, після чого зберігаємо файл та виводимо відповідне
        //повідомлення. Якщо ж така електронна адреса вже є у колекції, то
        //знаходимо індекс цього обєкту в колекції і на його місце додаємо новий об-єкт, який заданий параметром методу. Зберігаємо нове
        //значення, виводимо відповідне повідомлення. Таким чином ми можемо оновити дані.
        public string AddPerson(Person person, string filename)
        {
            string answer;
            try
            {
                if (ValidateData(person) == true)
                {
                    if (SearchByEmail(person.Email) == null)
                    {
                        this.DataPeople.Add(person);
                        SaveData(this.DataPeople, filename);
                        answer = $"Person {person.FirstName} is succesful added";
                    }
                    else
                    {
                        int i = this.DataPeople.IndexOf(SearchByEmail(person.Email));
                        this.DataPeople[i] = person;
                        SaveData(this.DataPeople, filename);
                        answer = $"Person {person.FirstName} is succesful updated";
                    }
                }
                else
                {
                    answer = $"Check your data!";
                }
            }
            catch (Exception)
            {
                answer = "Сannot be serialized";
            }
            return answer;
        }
        //Метод для видалення об'єкту з колекції за заданим параметром електронної адреми. Спочатку знаходимо індекс цього об'єкту в колекції. Потім видаляємо та зберігаємо файл
        public void DeletePerson(string email, string filename)
        {
            int i = this.DataPeople.IndexOf(SearchByEmail(email));
            this.DataPeople.RemoveAt(i);
            SaveData(this.DataPeople, filename);
        }
        //Метод для перевірки даних, повертає знячення змінної valited. Якщо воно дорівнює false, то дані некоректні
        public bool ValidateData(Person person)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(person);
            bool validated = false;
            if (Validator.TryValidateObject(person, context, results, true))
            {
                validated = true;
            }
            return validated;
        }
    }
}

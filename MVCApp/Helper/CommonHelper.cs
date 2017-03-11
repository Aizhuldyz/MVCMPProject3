using System;
using MVCApp.Models;

namespace MVCApp.Helper
{
    public class CommonHelper
    {
        internal static string GetRowHtml(Person person)
        {
            return $"<tr id={person.Id}><td>{person.Name}</td><td>{person.BirthDate:dd/MM/yyyy}</td>" +
                $"<td>{person.Age}</td>" + $"<td><button name=delete_person delete_id={person.Id} class=\"btn btn-link\">" +
                "Delete</button></td></tr>";
        }


        internal static string GetRowHtml(Badge badge)
        {
            return $"<tr id={badge.Id}><td>{badge.Title}</td><td>{badge.Description}</td>" +
                $"<td><button name=delete_badge delete_id={badge.Id} class=\"btn btn-link\">" +
                "Delete</button></td></tr>";
        }
        internal static int GetAge(DateTime birthDate)
        {
            var now = DateTime.Today;
            int age = now.Year - birthDate.Year;
            if (now < birthDate.AddYears(age)) age--;
            return age;
        }
    }
}
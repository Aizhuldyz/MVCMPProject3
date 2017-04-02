using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using MVCApp.Models;

namespace MVCApp.Helper
{
    public class CommonHelper
    {
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
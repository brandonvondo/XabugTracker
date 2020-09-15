using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XabugTracker.Helpers
{
    public class NameMaker
    {
        Random random = new Random();
        List<string> FirstNames = new List<string>() { "John", "Jacob", "David", "Anya", "Susan", "Joseph", "Jim", "Alex", "PJ", "Abigail", "Chase", "Adriene", "Eddy", "Martel", "Elizabeth", "Patricia", "Scout", "Drew", "Ed", "Ash", "Thomas", "Matt", "Blaine", "Blaze", "Cabal", "Cage", "Luke", "Byrd", "Cynthia", "Chalee", "Gala", "Diamond", "Osborn", "Howie", "Bruce", "Talon", "Tate", "Taylor", "Symphony", "Ris", "Rivers" };
        List<string> LastNames = new List<string>() { "Smith", "Jones", "Brown", "Wilson", "Hill", "Scott", "Cook", "Ross", "Perry", "Powell", "Rivera", "Long", "White", "Garcia", "Gray", "Foster", "Moore", "Campbell", "Walker", "Collins", "Diaz", "Russell", "Griffin", "Walker", "Pray", "Watterson", "Torres", "King", "Scott", "Wright", "Walker", "Young" };

        public string FirstNameGenerate()
        {
            return FirstNames[random.Next(FirstNames.Count)];
        }

        public string LastNameGenerate()
        {
            return LastNames[random.Next(LastNames.Count)];
        }
    }
}
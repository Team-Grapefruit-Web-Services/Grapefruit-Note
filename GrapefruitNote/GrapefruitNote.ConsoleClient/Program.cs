namespace GrapefruitNote.ConsoleClient
{
    using System;
    using System.Linq;

    using GrapefruitNote.Data;

    public class Program
    {
        static void Main()
        {
            var db = new GrapefruitNoteData();
            db.Categories.All().Any();
        }
    }
}

namespace GrapefruitNote.ConsoleClient
{
    using System;
    using System.Linq;

    using GrapefruitNote.Data;

    public class Program
    {
        static void Main()
        {
            using (GrapefruitNoteDbContext db = new GrapefruitNoteDbContext())
            {
                db.Users.Any();
            }
        }
    }
}

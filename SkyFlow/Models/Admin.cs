using System;

namespace SkyFlow.Models
{
    public class Admin : User
    {
        public override void DisplayDashboard()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║         ADMIN DASHBOARD                ║");
            Console.WriteLine("╠════════════════════════════════════════╣");
            Console.WriteLine("║  1. Manage Flights                     ║");
            Console.WriteLine("║  2. View System Overview               ║");
            Console.WriteLine("║  3. Manage Staff                       ║");
            Console.WriteLine("║  4. Logout                             ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.Write("\nSelect option: ");
        }
    }
}
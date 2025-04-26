using System.Text.Json;

namespace SimpleHabitTracker
{
    class Program
    {
        private const string Json = "habits.json";

        enum Status
        {
            Pending,
            Done
        }

        record Habit(int Id, string Name, Status Status, DateTime CreatedOn);


        public static void Main()
        {
            bool programRunning = true;

            // Loading the habits from the json file.
            List<Habit> habits = new List<Habit>();
            string jsonString = File.ReadAllText(Json);
            habits = JsonSerializer.Deserialize<List<Habit>>(jsonString);

            int habitIdTracker = habits[habits.Count - 1].Id; // Should get the last index habit and get the id.

            while (programRunning)
            {
                Console.WriteLine("===HABIT TRACKER===");
                Console.WriteLine("1. Add new habit.");
                Console.WriteLine("2. Mark habit as done.");
                Console.WriteLine("3. View habits.");
                Console.WriteLine("0. Exit.");

                Console.WriteLine("Choose: ");
                int selectedNumber;
                bool result = int.TryParse(Console.ReadLine(), out selectedNumber);

                if (result)
                {
                    switch (selectedNumber)
                    {
                        case 0:
                            Console.WriteLine("Goodbye.");
                            SaveHabits(habits);
                            programRunning = false;
                            break;
                        case 1:

                            Console.WriteLine("Enter habit:");
                            string habitInput = Console.ReadLine();

                            habitIdTracker++;
                            Habit habit = new Habit(habitIdTracker, habitInput, Status.Pending, DateTime.Now);
                            habits.Add(habit);

                            Console.WriteLine("Habit added!");
                            break;
                        case 2:
                            Console.WriteLine("Enter habit Id:");

                            int id;
                            bool parseResult = int.TryParse(Console.ReadLine(), out id);

                            if (parseResult)
                            {
                                if (id > 0)
                                {
                                    
                                    int habitIdx = habits.FindIndex(h => h.Id == id);

                                    if (habitIdx == -1)
                                    {
                                        Console.WriteLine($"Habit id {id} not found!");
                                    }
                                    else
                                    {
                                        habits[habitIdx] = habits[habitIdx] with { Status = Status.Done };
                                        Console.WriteLine("Marked as done.");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("You didn't enter a proper number.");
                            }
                            break;
                        case 3:
                            foreach (Habit h in habits)
                            {
                                if (h.Status == Status.Pending)
                                {
                                    Console.WriteLine($"[] Habit({h.Id}) {h.Name} - Created on: {h.CreatedOn}");
                                }
                                else
                                {
                                    Console.WriteLine($"[X] Habit({h.Id}) {h.Name} - Created on: {h.CreatedOn}");
                                }
                            }
                            break;
                        default:
                            Console.WriteLine("Option with that number doesn't exist!");
                            break;

                    }
                }

            }

            
        }

        static void SaveHabits(List<Habit> habits)
        {

            using (StreamWriter sw = new StreamWriter(Json))
            {
                string json = JsonSerializer.Serialize(habits);
                sw.WriteLine(json);
                sw.Close();   
            }

        }

    }
}
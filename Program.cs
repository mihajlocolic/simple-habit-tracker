﻿using System.Text.Json;


namespace SimpleHabitTracker
{
    class Program
    {
        
        enum Status
        {
            Pending,
            Done
        }
        record Habit(int Id, string Name, Status Status, DateTime CreatedOn);

        private const string Json = "habits.json";
        public static bool programState;
        public static int habitIdTracker;
        private static List<Habit> habits = new List<Habit>();

        public static void Main()
        {
            programState = true;
            ReadJsonData();

            while (programState)
            {
                ShowMenu();
                HandleUserInput();
            }
        }


        static void ReadJsonData()
        {
            if (File.Exists(Json))
            {
                FileInfo file = new FileInfo(Json);
                if (file.Length == 0) {
                    string emptyBody = "[]";
                    File.WriteAllText(Json, emptyBody);
                }

                string jsonString = File.ReadAllText(Json);
                habits = JsonSerializer.Deserialize<List<Habit>>(jsonString);

                if (habits.Count > 0)
                {
                    habitIdTracker = habits.Last().Id; // Store the last habit ID, so the program can know how to auto increment.
                } else
                {
                    habitIdTracker = 0; // If there are no habits to begin with.
                }
                                
            }
            else
            {
                using (FileStream createStream = File.Create(Json))
                {
                    Console.WriteLine("Json file doesn't exist, creating one...\n");
                }
                
            }
        }

        static void SaveHabits(List<Habit> habits)
        {
            if (File.Exists(Json))
            {
                using (StreamWriter sw = new StreamWriter(Json))
                {
                    string json = JsonSerializer.Serialize(habits);
                    sw.WriteLine(json);
                }
            }
            else
            {
                Console.WriteLine("Error: JSON file missing. Cannot save.");
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("===HABIT TRACKER===");
            Console.WriteLine("1. Add new habit.");
            Console.WriteLine("2. Mark habit as done.");
            Console.WriteLine("3. View habits.");
            Console.WriteLine("4. Delete habit.");
            Console.WriteLine("0. Exit.");

            Console.WriteLine("Choose: ");
        }

        static void HandleUserInput()
        {
            int selectedNumber;
            bool result = int.TryParse(Console.ReadLine(), out selectedNumber);

            if (result)
            {
                switch (selectedNumber)
                {
                    case 0:
                        Console.WriteLine("Goodbye.");
                        SaveHabits(habits);
                        programState = false;
                        break;
                    case 1:

                        Console.WriteLine("Enter habit:");
                        string habitInput = Console.ReadLine();

                        habitIdTracker++;
                        Habit habit = new Habit(habitIdTracker, habitInput, Status.Pending, DateTime.Now);
                        habits.Add(habit);

                        Console.WriteLine("Habit added!");
                        SaveHabits(habits);
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
                                    SaveHabits(habits);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("You didn't enter a proper number.");
                        }
                        break;
                    case 3:

                        List<Habit> pendingHabits = new List<Habit>();
                        List<Habit> completedHabits = new List<Habit>();
                        foreach (Habit h in habits)
                        {
                            if (h.Status == Status.Pending)
                            {
                                pendingHabits.Add(h);
                            }
                            else
                            {
                                completedHabits.Add(h);
                            }
                        }

                        Console.WriteLine("\nPENDING HABITS:");
                        foreach (Habit h in pendingHabits)
                        {
                            Console.WriteLine($"Habit({h.Id}) {h.Name} - Created on: {h.CreatedOn}");
                        }

                        Console.WriteLine("\nCOMPLETED HABITS:");
                        foreach (Habit h in completedHabits)
                        {
                            Console.WriteLine($"Habit({h.Id}) {h.Name} - Created on: {h.CreatedOn}");
                        }
                        Console.WriteLine("\n");
                        break;
                    case 4:
                        Console.WriteLine("Enter habit Id:");
                        int habitId;
                        bool parseRslt = int.TryParse(Console.ReadLine(), out habitId);

                        if (parseRslt)
                        {
                            int habitIdx = habits.FindIndex((h) => { return h.Id == habitId; });
                            if (habitIdx != -1)
                            {
                                habits.Remove(habits[habitIdx]);
                                Console.WriteLine("Habit removed.");
                                SaveHabits(habits);
                            }
                            else
                            {
                                Console.WriteLine("Couldn't find that habit.");
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
}
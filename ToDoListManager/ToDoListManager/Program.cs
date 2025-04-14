// Obejectives: Learn about List<T>, foreach loop, switch-case

List<string> tasks = new List<string>();

while (true) {
    Console.WriteLine();
    Console.WriteLine("1. Create 2. Delete 3. Show all tasks");
    string choice = Console.ReadLine();
    switch (choice)
    {
        case "1":
            // Create a task
            Console.WriteLine();
            Console.Write("Provide me a task name: ");
            tasks.Add(Console.ReadLine());
            break;
        case "2":
            // Delete task by index id
            Console.WriteLine();
            Console.Write("Delete the exist task by index id: ");
            int temp = Int32.Parse(Console.ReadLine());
            temp = --temp;
            tasks.RemoveAt(temp);
            break;
        case "3":
            // Show all tasks
            Console.WriteLine();
            Console.WriteLine("Show all tasks: ");
            foreach (var task in tasks)
            {
                Console.WriteLine(task);
            }
            break;
    }
}




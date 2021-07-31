using System;
using System.Collections.Generic;
using System.IO;


namespace PocketMoney
{
    class PocketMoney
    {
        public void Begin()
        {
            Console.WriteLine();
            Console.WriteLine("[0]: start new account");
            Console.WriteLine();
            homePageAccountStatus();
        }

        private void homePageAccountStatus()
        {
            var directories = Directory.EnumerateDirectories("Accounts");
            int i = 0;

            var names = new List<string>();

            foreach(var path in directories)
            {
                var name = path.Split(@"\")[1];
                names.Add(name);
                i++;
                var money = new Money(readMoneyFromFile($"{path}/Money.txt"));
                Console.WriteLine($"[{i}]: {name}");
                Console.WriteLine($"     balance: £{money.pound}.{money.pence}");
                Console.WriteLine();                
            }

            //ask for choice
            //ReadInput
            int choice = int.Parse(Console.ReadLine());

            if (choice == 0)
            {
                startAccount();
            }
            else
            {
                accountChoices(names[choice-1]);
            }

            //if == 0 .. then create new account ... redisplay the above
            //if > 1 .. then AccountDetails(names[choice-1])
        }

        private void accountChoices(string name)
        {
            Console.WriteLine("Would you like to"); 
            Console.WriteLine("[0] add to your account");
            Console.WriteLine("[1] or subtract from your account");

            var choice = Console.ReadLine();
            bool isAdding = true;

            switch (choice)
            {
                case "0" :
                    Console.Write("How much money have you gained: £");
                    break;

                default: 
                    Console.WriteLine("How much money have you spent: £");
                    isAdding = false;
                    break;
            } 
            var input = Console.ReadLine();
            int pence = conversion(input);   
            
            if (isAdding == false)
            {
                pence = pence * -1;
            }

            Console.Write("And what was the reason: ");
            string reason = Console.ReadLine();

            addMoney(pence, reason, readMoneyFromFile($"Accounts/{name}/Money.txt"), name);
        }

        private int conversion(string input)
        {
            var pence = 0;

            try
            {
                pence += int.Parse(input) * 100;
            }
            catch
            {
                var values = input.Split(".");
                pence += int.Parse(values[0]) * 100;
                if (values[1].Length < 2 )// when 0.5 is entered it means 0.50 
                {
                   pence += int.Parse(values[1]) * 10;
                }
                else pence += int.Parse(values[1]);
            }
            return pence;
        }

        private string startAccount()
        {
            Console.WriteLine("Please enter your name");
            var name = Console.ReadLine();

            Console.WriteLine("Please enter you current balance");
            var balance = Console.ReadLine();

            int startBalance = 0;
            try
            {
                int inputMoney = int.Parse(balance);
                startBalance += inputMoney;
            }
            catch
            {
                var values = balance.Split(".");
                startBalance += int.Parse(values[0]) * 100;
                if (values[1].Length < 2 )
                {
                   startBalance += int.Parse(values[1]) * 10;
                }
                else startBalance += int.Parse(values[1]);
            }

            Console.WriteLine("Anything to declear");
            var statment = Console.ReadLine();

            addMoney(0, statment, startBalance, name);
            return name;
        }
        
        private void addMoney(int moneyNumber, string inReason, int startBalance, string name)
        {
            int totalInPence = 0;
            totalInPence = startBalance + moneyNumber;

            var money = new Money(totalInPence);

            Console.WriteLine($"{name} now has £{money.pound}.{money.pence}");
            
            var historyPath = $"Accounts/{name}/History.txt";
            if (!File.Exists(historyPath))
            {
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(historyPath));
                }
                File.Create(historyPath).Dispose();
            }
            using (StreamWriter writetext = new StreamWriter(historyPath, true))
            {   
                writetext.WriteLine(totalInPence + ", " + moneyNumber + ", " + inReason + ", " + DateTime.Now);
            }

            var moneyPath = $"Accounts/{name}/Money.txt";
            if (!File.Exists(moneyPath))
            {
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(moneyPath));
                }
                File.Create(moneyPath).Dispose();
            }
            using (StreamWriter writetext = new StreamWriter(moneyPath))
            {
                writetext.WriteLine(totalInPence);
            }
        }

        private int readMoneyFromFile(string path)
        {
            using(StreamReader readFile = new StreamReader(path)) 
            {
                string[] splitLine;

                string fullLine = readFile.ReadLine();
                splitLine = fullLine.Split(",");

                int preValue = int.Parse(splitLine[0]);

                return preValue;
            }  
        }

        private int convertToPence(int amount)
        {
            int allInPence = amount * 100;
            return allInPence;
        }

        class Money
        {
            public Money(int penceAmount)
            {
                this.pound = penceAmount / 100;
                this.pence = penceAmount % 100;
            }

            public int pound;
            public int pence;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var pocketMoney = new PocketMoney();
            while(true)
            {
                pocketMoney.Begin();
            }
        }
    }
}

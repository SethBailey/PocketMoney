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
                AccountDetails(names[choice-1]);
            }

            //if == 0 .. then create new account ... redisplay the above
            //if > 1 .. then AccountDetails(names[choice-1])
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

        private void homepageActions(int choice, string name)
        {
            switch (choice)
            {
                case 1 : AccountDetails(name); 
                    break;
            }
        }

        private void AccountDetails(string name)
        {
            Console.Write($"How much money has {name} gained: £");
            var input = Console.ReadLine();
            int gainInPence = 0;
            try
            {
                int inputMoney = int.Parse(input);
                gainInPence += inputMoney * 100;
            }
            catch
            {
                var values = input.Split(".");
                gainInPence += int.Parse(values[0]) * 100;
                if (values[1].Length < 2 )
                {
                   gainInPence += int.Parse(values[1]) * 10;
                }
                else gainInPence += int.Parse(values[1]);
            }
            

            Console.Write("And what was the reason: ");
            string reason = Console.ReadLine();

            addMoney(gainInPence, reason, readMoneyFromFile($"Accounts/{name}/Money.txt"), name);
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

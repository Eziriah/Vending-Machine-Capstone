﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Capstone
{
    class VendingMachine
    {
        //create VendingMachine properties for a dictionary of items and a monetary balance
        public Dictionary<string, VendingMachineItem> VendingMachineDictionary { get; set; } = new Dictionary<string, VendingMachineItem> { };

        public decimal MachineBalance { get; private set; } = 0;

        //instantiate log writing method 
        FileLogWriter fileLogWriter = new FileLogWriter();

        string timeStamp = DateTime.Now.ToString();

        //constructor method for an instance of a VendingMachine
        public VendingMachine()
        {
            //file to be read of inventory list 
            string currentDirectory = Environment.CurrentDirectory;
            string whereToRead = Path.Combine(currentDirectory, "vendingmachine.csv");

            //create new file to write to
            FileLogWriter writeLogFile = new FileLogWriter();

            //create new instance of inventory read class
            InventoryRead inventoryRead = new InventoryRead();

            //create new dictionary using invetoryReaed method to populate
            VendingMachineDictionary = inventoryRead.ReadFile(whereToRead);
        }

        //method for displaying the inventory from the dictionary
        public void DisplayAvaliableInvetory()
        {
            foreach (KeyValuePair<string, VendingMachineItem> item in VendingMachineDictionary)
            {
                Console.WriteLine($"{item.Key} - {item.Value.ItemName} - {item.Value.ItemCost:c2} - {item.Value.ItemQuantity} remaining");
            }
        }

        //main menu of a VendingMachine
        public void DisplayMainMenu()
        {

            string mainMenuSelection = "";
            do
            {
                //display main menu options for user to select
                Console.WriteLine("(1) Display Vending Machine Items \n(2) Purchase \n(3) Exit");
                mainMenuSelection = Console.ReadLine();

                if (mainMenuSelection != "1" && mainMenuSelection != "2" && mainMenuSelection != "3")
                {
                    Console.WriteLine("\nYou entered an invalid code, please try again!\n");
                }

                //menu selection to display inventory to user 
                if (mainMenuSelection == "1")
                {
                    DisplayAvaliableInvetory();
                    Console.Write("\n Press any key to return to main menu: ");
                    Console.ReadLine();
                }
                //menu selection to take user to purchasing menu
                if (mainMenuSelection == "2")
                {
                    PurchaseMenu();
                }
                //menu selection to leave program
                if (mainMenuSelection == "3")
                {
                    break;
                }
                //TODO: PRINT SALES REPORT
                //if (mainMenuSelection = 4)

            } while (mainMenuSelection != "3");
        }

        public void PurchaseMenu()
        {
            //set to true to exit to main menu
            bool exitMenu = false;

            while (exitMenu == false)
            {
                Console.WriteLine("\n(1) Feed Money \n(2) Select Product \n(3) Finish Transaction");
                Console.WriteLine($"\nCurrent Money Provided: {MachineBalance:c2}");
                string purchaseMenuSelection = Console.ReadLine();

                if (purchaseMenuSelection != "1" && purchaseMenuSelection != "2" && purchaseMenuSelection != "3")
                {
                    Console.WriteLine("\nYou entered an invalid code, please try again!\n");
                }
                //add money to bank selection
                if (purchaseMenuSelection == "1")
                {
                    FeedMoney();
                }

                //dispense item selection
                if (purchaseMenuSelection == "2")
                {
                    DispenseItem();
                }
               
                //finish transaction selection
                if (purchaseMenuSelection == "3")
                {
                    FinishTransaction();
                    //return to main menu after finished 
                    exitMenu = true;
                }
                
            }
        }

        public void FeedMoney()
        {
            //prompt user for whole dollar amount, then update current machine balance
            Console.Write("\nPlease enter the whole dollar amount you would like to deposit: ");

            //accept money from user 
            decimal depositAmount = decimal.Parse(Console.ReadLine());
            //add deposited amount to machine balance
            MachineBalance += depositAmount;

            //TODO: LOG ENTERED MONEY

            //write to file log for machine 
            string logFeedMoney = $"{timeStamp} FEED MONEY: {depositAmount:c2} {MachineBalance:c2}";
            fileLogWriter.WriteLogMessage(logFeedMoney);
        }
        public void DispenseItem()
        {
            DisplayAvaliableInvetory();
            Console.Write("\nEnter the code of the product you want to purchase:");
            string selectedItemCode = Console.ReadLine();

            //check on item avaliabilty and update inventory 
            if (VendingMachineDictionary.ContainsKey(selectedItemCode))
            {
                //VendingMachineDictionary[selectedItemCode].ItemQuantity; -this needs to be set to property
                VendingMachineItem item = VendingMachineDictionary[selectedItemCode];
                if (item.ItemQuantity >= 1)
                {
                    decimal currentMachineBalance = MachineBalance;
                    //update machine balance after purchasing item
                    MachineBalance -= item.ItemCost;

                    //print slogan of selected item
                    string slogan = "";
                    if (item.ItemCategory == "Chip")
                    {
                        slogan = "Crunch Crunch, Yum!";
                    }
                    else if (item.ItemCategory == "Candy")
                    {
                        slogan = "Munch Munch, Yum!";
                    }
                    else if (item.ItemCategory == "Drink")
                    {
                        slogan = "Glug Glug, Yum!";
                    }
                    else if (item.ItemCategory == "Gum")
                    {
                        slogan = "Chew Chew, Yum!";
                    }

                    //print reciept to user
                    Console.WriteLine($"\nYou selected: {item.ItemName} \nCost of item: {item.ItemCost:c2} \nYour remaining balance: {MachineBalance:c2}  \n{slogan}");

                    //update item inventory by subtracting selected item from group
                    item.ItemQuantity--;

                    //WRITE TO THE LOG
                    string logItemPurchased = $"{timeStamp} {item.ItemName} {item.ItemCode} {currentMachineBalance:c2} {MachineBalance:c2}";
                    fileLogWriter.WriteLogMessage(logItemPurchased);
                }
                else
                {
                    Console.WriteLine("\nThe selected item is SOLD OUT, please select another.");
                }
            }
            else
            {
                Console.WriteLine("\nYou entered an invalid code, please try again!");
            }
        }
        public void FinishTransaction()
        {
            decimal currentMachineBalance = MachineBalance;
            //return customer change in largest coins possible and print change amount to console 
            int coinChange = (int)(MachineBalance * 100);
            int quarter = coinChange / 25;
            coinChange %= 25;
            int dime = coinChange / 10;
            coinChange %= 10;
            int nickel = coinChange / 5;
            coinChange %= 5;

            //write to user change expected in quarters, nickles, dimes
            Console.WriteLine($"\nAmount of change: {MachineBalance:c2} \nquarters: {quarter} \ndimes: {dime} \nnickels: {nickel}\n");

            
            //update machine balance to $0
            MachineBalance = 0;

            //WRITE TO LOG
            string logChangeGiven = $"{timeStamp} GIVE CHANGE: {currentMachineBalance:c2} {MachineBalance:c2}";
            fileLogWriter.WriteLogMessage(logChangeGiven);
        }
    }
}

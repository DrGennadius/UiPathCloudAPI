﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UiPathOrchestrator;

namespace UiPathOrchestratorStartJob
{
    class Program
    {
        static void Main(string[] args)
        {
            UiPathCloudAPI uiPath = new UiPathCloudAPI();
            if (Authentication(uiPath))
            {
                MenuLoop(uiPath);
            }
        }

        static bool Authentication(UiPathCloudAPI uiPathCloudAPI)
        {
            bool result = false;
            bool authenticated = false;

            Console.WriteLine("Authentication...");

            // authenticated = TryAuthorize(uiPathCloudAPI, "", "");
            if (!authenticated)
            {
                string login = ConfigurationManager.AppSettings["login"];
                string password = ConfigurationManager.AppSettings["password"];

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    int tryCount = 3;
                    while (tryCount > 0 && !authenticated)
                    {
                        Console.WriteLine("Attempt #{0}", 4 - tryCount);
                        Console.Write(" Enter login: ");
                        login = Console.ReadLine();
                        Console.Write(" Enter password: ");
                        password = Console.ReadLine();
                        authenticated = TryAuthorize(uiPathCloudAPI, login, password);
                        tryCount--;
                    }
                }
                else
                {
                    authenticated = TryAuthorize(uiPathCloudAPI, login, password);
                }
            }
            if (authenticated)
            {
                try
                {
                    uiPathCloudAPI.GetMainData();
                    result = true;
                }
                catch (WebException)
                {
                    Console.WriteLine(uiPathCloudAPI.LastErrorMessage);
                }
                catch (Exception)
                {

                }
            }

            return result;
        }

        static bool TryAuthorize(UiPathCloudAPI uiPathCloudAPI, string login, string password)
        {
            bool result = false;

            try
            {
                uiPathCloudAPI.Authorize(login, password);
                result = uiPathCloudAPI.Authenticated;
            }
            catch (WebException)
            {
                Console.WriteLine(uiPathCloudAPI.LastErrorMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        static void MenuLoop(UiPathCloudAPI uiPath)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Welcome, {0}!", uiPath.LogicalName);
                Console.WriteLine("Select number:");
                Console.WriteLine("0. Exit.");
                Console.WriteLine("1. Robots.");
                Console.WriteLine("2. Processes.");
                Console.WriteLine("3. Jobs.");
                Console.Write("Enter number: ");
                int number = -1;
                try
                {
                    number = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                }
                switch (number)
                {
                    case -1:
                    case 0:
                        return;
                    case 1:
                        RobotsMenuLoop(uiPath);
                        break;
                    case 2:
                        ProccessesMenuLoop(uiPath);
                        break;
                    case 3:
                        JobsMenuLoop(uiPath);
                        break;
                    default:
                        Console.WriteLine("Incorrected number. Repeat?");
                        string answer = Console.ReadLine().ToLower();
                        if (answer == "n" || answer == "no")
                        {
                            return;
                        }
                        break;
                }
            }
        }

        static void RobotsMenuLoop(UiPathCloudAPI uiPath)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Robots menu.");
                Console.WriteLine("Select number:");
                Console.WriteLine("0. Exit.");
                Console.WriteLine("1. Print list.");
                Console.Write("Enter number: ");
                int number = -1;
                try
                {
                    number = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                }
                switch (number)
                {
                    case -1:
                    case 0:
                        return;
                    case 1:
                        PrintRobots(uiPath.GetRobots());
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Incorrected number. Repeat?");
                        string answer = Console.ReadLine().ToLower();
                        if (answer == "n" || answer == "no")
                        {
                            return;
                        }
                        break;
                }
            }
        }

        static void ProccessesMenuLoop(UiPathCloudAPI uiPath)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Proccesses menu.");
                Console.WriteLine("Select number:");
                Console.WriteLine("0. Exit.");
                Console.WriteLine("1. Print list.");
                Console.Write("Enter number: ");
                int number = -1;
                try
                {
                    number = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                }
                switch (number)
                {
                    case -1:
                    case 0:
                        return;
                    case 1:
                        PrintProcesses(uiPath.GetProcesses());
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Incorrected number. Repeat?");
                        string answer = Console.ReadLine().ToLower();
                        if (answer == "n" || answer == "no")
                        {
                            return;
                        }
                        break;
                }
            }
        }

        static void JobsMenuLoop(UiPathCloudAPI uiPath)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Jobs menu.");
                Console.WriteLine("Select number:");
                Console.WriteLine("0. Exit.");
                Console.WriteLine("1. Print list.");
                Console.WriteLine("2. Start new job.");
                Console.Write("Enter number: ");
                int number = -1;
                try
                {
                    number = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                }
                switch (number)
                {
                    case -1:
                    case 0:
                        return;
                    case 1:
                        PrintJobs(uiPath.GetJobs());
                        Console.ReadKey();
                        break;
                    case 2:
                        Console.Write("Enter robot id: ");
                        try
                        {
                            int robotId = Convert.ToInt32(Console.ReadLine());
                            var newJobs = uiPath.StartJob(robotId);
                            PrintJobs(newJobs, "New Jobs:");
                        }
                        catch (WebException)
                        {
                            Console.WriteLine(uiPath.LastErrorMessage);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Incorrected number. Repeat?");
                        string answer = Console.ReadLine().ToLower();
                        if (answer == "n" || answer == "no")
                        {
                            return;
                        }
                        break;
                }
            }
        }

        static void PrintRobots(List<Robot> robots, string title = "Robots:")
        {
            Console.WriteLine(title);
            int maxLength = robots.Max(x => x.Name.Length);
            string blankName = new string(' ', maxLength);

            foreach (var item in robots)
            {
                Console.WriteLine("\t{0} {1} {2}", item.Id, blankName.Insert(0, item.Name), item.Description.MaxLength(25));
            }
        }

        static void PrintProcesses(List<Process> proccess, string title = "Processes:")
        {
            Console.WriteLine(title);
            foreach (var item in proccess)
            {
                Console.WriteLine("\t{0} {1} {2}", item.Id, item.Name, item.Description.MaxLength(25));
            }
        }

        static void PrintJobs(List<Job> jobs, string title = "Jobs:")
        {
            Console.WriteLine(title);
            foreach (var item in jobs)
            {
                Console.WriteLine("\t{0} {1} {2} {3} {4}", item.Id, item.Key, item.State, item.StartTime, item.EndTime);
            }
        }
    }
}

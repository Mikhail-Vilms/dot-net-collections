using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace DotNetCollections.generic.Tests
{
    [TestClass()]
    public class StackTests
    {
        Stack<string> urlStack;

        [TestInitialize]
        public void PushSeveralRecordsToStack()
        {
            urlStack = new Stack<string>();

            urlStack.Push("https://leetcode.com/10001");
            urlStack.Push("https://leetcode.com/10002");
            urlStack.Push("https://leetcode.com/10003");
            urlStack.Push("https://leetcode.com/10004");
        }

        // [TestMethod()]
        public void Count_ReturnsCorrectNumberOfElements()
        {
            Assert.AreEqual(4, urlStack.Count);
        }


        // [TestMethod()]
        public void Push_AddSeveralRecords_ReturnsCorrectNumberOfElements()
        {
            urlStack.Push("https://leetcode.com/10005");
            urlStack.Push("https://leetcode.com/10006");
            urlStack.Push("https://leetcode.com/10007");
            urlStack.Push("https://leetcode.com/10008");

            Assert.AreEqual(8, urlStack.Count);
        }

        // [TestMethod()]
        public void Clear_ReturnsCorrectNumberOfElements()
        {
            urlStack.Clear();

            Assert.AreEqual(0, urlStack.Count);
        }

        // [TestMethod()]
        public void Pop_RetursCorrectValues()
        {
            Assert.AreEqual("https://leetcode.com/10004", urlStack.Pop());
            Assert.AreEqual("https://leetcode.com/10003", urlStack.Pop());
            Assert.AreEqual("https://leetcode.com/10002", urlStack.Pop());
            Assert.AreEqual("https://leetcode.com/10001", urlStack.Pop());
        }

        // [TestMethod()]
        public void PushRecordsSimultaneously()
        {
            Stack<string> taskBucket = new Stack<string>();

            int numberOfUsers = 10;
            Thread[] users = new Thread[numberOfUsers];
            for (int j = 0; j < numberOfUsers; j++)
            {
                Thread t = new Thread(() =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        string newTask = "Task #" + i;
                        taskBucket.Push(newTask);
                        Console.WriteLine(newTask + " was issued by: " + Thread.CurrentThread.Name);
                    }
                });
                
                t.Name = "User #" + j;
                users[j] = t;
            }

            for (int j = 0; j < numberOfUsers; j++)
            {
                users[j].Start();
            }

            Console.WriteLine("Thread {0} Ending",
                Thread.CurrentThread.Name);

            Thread.Sleep(5000);
        }

        //[TestMethod()]
        public void PushRecordsByGroups()
        {
            Stack<string> taskBucket = new Stack<string>();
            object lockObject = new object();

            int numberOfUsers = 10;
            Thread[] users = new Thread[numberOfUsers];
            for (int j = 0; j < numberOfUsers; j++)
            {
                Thread t = new Thread(() =>
                {
                    lock (lockObject)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            string newTask = "Task #" + i;
                            taskBucket.Push(newTask);
                            Console.WriteLine(newTask + " was issued by: " + Thread.CurrentThread.Name);
                        }
                    }
                });

                t.Name = "User #" + j;
                users[j] = t;
            }

            for (int j = 0; j < numberOfUsers; j++)
            {
                users[j].Start();
            }

            Console.WriteLine("Thread {0} Ending",
                Thread.CurrentThread.Name);

            Thread.Sleep(5000);
        }

        [TestMethod()]
        public void TryToPopFromSeveralThreads_ThrowsException()
        {
            Stack<string> taskBucket = new Stack<string>();
            for (int i = 0; i < 100; i++)
            {
                string taskName = "task #" + i;
                taskBucket.Push(taskName);
            }
            Thread.CurrentThread.Name = "Main";

            int NUMBER_OF_WORKERS = 10;
            Thread[] workers = new Thread[NUMBER_OF_WORKERS];

            for (int j = 0; j < NUMBER_OF_WORKERS; j++)
            {
                Thread t = new Thread(() =>
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if (taskBucket.Count == 0)
                        {
                            Console.WriteLine(Thread.CurrentThread.Name + " HAS NOTHING TO DO;");
                            continue;
                        }

                        Console.WriteLine(Thread.CurrentThread.Name + " is working on: " + taskBucket.Pop());
                    }
                });

                t.Name = "Worker #" + j;
                workers[j] = t;
            }

            for (int j = 0; j < NUMBER_OF_WORKERS; j++)
            {
                workers[j].Start();
            }

            Thread.Sleep(5000);
            Console.WriteLine("Thread {0} Ending",
                Thread.CurrentThread.Name);
        }

        // [TestMethod()]
        public void TryToTestIntVariable()
        {
            int testVar = 0;

            Thread[] workers = new Thread[10];

            for (int j = 0; j < 10; j++)
            {
                Thread t = new Thread(() =>
                {
                    for(int i = 0; i < 10000; i++)
                    {
                        testVar += 1;
                    }
                });

                workers[j] = t;
            }

            for (int j = 0; j < 10; j++)
            {
                workers[j].Start();
            }

            Thread.Sleep(5000);

            Console.WriteLine("testVar: " + testVar); // expected: 10_000       
        }
    }
}
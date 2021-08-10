using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Gelber.ShoppingLine
{
    public class ShoppingMall
    {
        private static Queue<Customer> CustomerQueue = new Queue<Customer>();
        private readonly RegisterTranscation RegisterTotal = null;
        public ShoppingMall() { }
        public ShoppingMall(RegisterTranscation registerTotal)
        {
            RegisterTotal = registerTotal;
        }
        public static Queue<Customer> GetCustomerQueue()
        {
            return CustomerQueue;
        }

        public RegisterTranscation GetRegisterFunctons()
        {
            return RegisterTotal;
        }
        /// <summary>
        /// TotalServedTimes
        /// </summary>
        /// <param name="checkoutRegistrations"></param>
        /// <returns></returns>
        private static int TotalServedTimes(RegisterTranscation checkoutRegistrations)
        {
            int time = 1;
            while (!(CustomerQueue.Count() == 0) || checkoutRegistrations.GetRegisterstatus())
            {
                // If two or more customers arrive at the same time, those with fewer items choose registers
                // before those with more, and if they have the same number of items then type A's choose before
                // type B's. Sort has the CompareTo with IComparable doing that
                var customersAtSameTime = GetCustomersArrivingSameTime(time);
                customersAtSameTime.Sort();
                //------------------------------------------------------------------------------------------------
                //Register to Checkout Counter for the customers that are Customer A and B
                checkoutRegistrations.ServiceCustomer(customersAtSameTime);
                //------------------------------------------------------------------------------------------------
                int index = 0;
                // when customer's time has not yet arrived, it will just Server the existing registrations there
                while (index < checkoutRegistrations.GetRegistrations().Count())
                {
                    Queue<Customer> customer = checkoutRegistrations.GetRegistrations()[index].GetCustomers();

                    // The grocery store always has a single cashier in training. This cashier is always assigned to the
                    // highest numbered register.
                    if (index == checkoutRegistrations.GetRegistrations().Count() - 1)
                    {
                        TraineeServe(customer);
                    }
                    else
                    {
                        ExpertServe(customer);
                    }
                    index++;
                }
                //Time is measured in minutes.
                time++;
            }
            return time;
        }
        /// <summary>
        /// two or more customers arrive at the same time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static List<Customer> GetCustomersArrivingSameTime(int time)
        {
            var customersAtSameTime = new List<Customer>();
            Customer customer = null;
            if (CustomerQueue.Count() > 0)
            {
                customer = CustomerQueue.Peek();
            }
            // If two or more customers arrive at the same time
            while (customer != null && customer.GetCustomerTimeArrived() == time)
            {
                // Add Customer to the customersAtSameTime
                // And Remove from Customer Queue
                customersAtSameTime.Add(CustomerQueue.Dequeue());
                if (CustomerQueue.Count() > 0)
                    customer = CustomerQueue.Peek();
                else
                    customer = null;

            }
            return customersAtSameTime;
        }
        /// <summary>
        /// The register staffed by the
        /// cashier in training takes two minutes for each item.So a customer with n items at a regular
        /// register takes n minutes to check out. However, if the customer ends up at the last register, it
        /// will take 2n minutes to check out.
        /// </summary>
        /// <param name="customer"></param>
        private static void TraineeServe(Queue<Customer> customer)
        {
            Customer localCustomer = null;
            if (customer.Count() > 0)
            {
                localCustomer = customer.Peek();
            }
            if (localCustomer != null)
            {
                if (localCustomer.GetServiceStatus() == false)
                {
                    localCustomer.SetServicesStatus(true);
                }
                else
                {
                    //RemoveItems when checking out
                    if (localCustomer.RemoveShoppingItems() == 0)
                    {
                        //Customers just finishing checking out do not count as being in line (for either kind of Customer).
                        customer.Dequeue();
                    }
                    else
                    {
                        //Set it back to false if the items>0 for taking 2n minutes
                        localCustomer.SetServicesStatus(false);
                    }
                }

            }
        }
        /// <summary>
        /// Regular registers take one minute to process each customer's item.
        /// </summary>
        /// <param name="customer"></param>
        private static void ExpertServe(Queue<Customer> customer)
        {
            Customer localCustomer = null;
            if (customer.Count() > 0)
            {
                localCustomer = customer.Peek();
            }
            //RemoveItems when checking out
            if (localCustomer != null && localCustomer.RemoveShoppingItems() == 0)
            {
                //Customers just finishing checking out do not count as being in line (for either kind of Customer).
                customer.Dequeue();
            }
        }
        public void DisplayResult(string[] args)
        {
            var shippingMall = ReadFromFileStream(args);
            var checkoutRegistrations = shippingMall.GetRegisterFunctons();
            var time = TotalServedTimes(checkoutRegistrations);
            Console.WriteLine($"Finished at: t = {time} minutes");
        }
        private static ShoppingMall ReadFromFileStream(string[] args)
        {

            ShoppingMall shoppingMall = null;
            RegisterTranscation registerFunctions = null;
            StreamReader streamReader = null;
            var line = " ";
            int firstline = 0;
            try
            {
                streamReader = new StreamReader(args[0]);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"File Not Found {e.Message}");
                Environment.Exit(-1);
            }
            try
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (firstline == 0)
                    {
                        int noOfRegisters = Convert.ToInt32(line);
                        registerFunctions = new RegisterTranscation(noOfRegisters);
                    }
                    else
                    {
                        Customer customer = AssignCustomerFromLine(line);
                        CustomerQueue.Enqueue(customer);
                    }
                    firstline++;
                }
                shoppingMall = new ShoppingMall(registerFunctions);
            }
            catch (IOException e)
            {
                Console.WriteLine($"Error reading Input {e.Message}");
                Environment.Exit(-1);
            }
            return shoppingMall;
        }
        /// <summary>
        /// Input is in the form of a single integer (number of registers), followed by a list of pairs. Each pair
        /// specifies the time in minutes(from a fixed offset) when a customer arrives to the set of registers, and
        /// how many items that customer has.Each pair appears white - space separated on a line by itself in the
        /// input file;
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static Customer AssignCustomerFromLine(string line)
        {
            //Each pair appears white - space separated on a line by itself
            var partsOfline = line.Split(' ');
            if (partsOfline.Length != 3)
            {
                Console.WriteLine("Invalid Input");
                Environment.Exit(-1);

            }
            if (partsOfline[0].Equals(ECustomerType.A.ToString()))
            {
                return new Customer(ECustomerType.A, Convert.ToInt32(partsOfline[1]), Convert.ToInt32(partsOfline[2]));
            }
            else if (partsOfline[0].Equals(ECustomerType.B.ToString()))
            {
                return new Customer(ECustomerType.B, Convert.ToInt32(partsOfline[1]), Convert.ToInt32(partsOfline[2]));

            }
            else
            {
                Console.WriteLine("Customer Type is Invalid");

                Environment.Exit(-1);
                return null;

            }
        }
    }
}

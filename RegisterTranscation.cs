using System.Collections.Generic;
using System.Linq;

namespace Gelber.ShoppingLine
{
    public class RegisterTranscation
    {
        private readonly List<Register> Registrations = new List<Register>();
        public RegisterTranscation(int registers)
        {
            // The number of registers is specified by the problem inputs; registers are numbered 1, 2, 3...n for
            // n registers.
            for (int i = 0; i < registers; i++)
            {
                Registrations.Add(new Register(i));
            }
        }
        public List<Register> GetRegistrations()
        {
            return Registrations;
        }
        /// <summary>
        /// Register Shortest Size for Customer A
        /// Customer Type A always chooses the register with the shortest line (fewest number of
        //  customers in line)
        /// </summary>
        /// <returns>Register</returns>
        public Register GetRegisterShortestLine()
        {
            var sortedList = new List<Register>();
            foreach (Register register in Registrations)
            {
                sortedList.Add(register);
            }
            //Shorted by the numbers of customers in one checkout counter
            sortedList.Sort(new CompareRegisterItems());
            return sortedList.First();
        }
        /// <summary>
        /// Register the least items for Customer B
        /// Customer Type B looks at the last customer in each line, and always chooses to be
        //  behind the customer with the fewest number of items left to check out, regardless of
        //  how many other customers are in the line or how many items they have.Customer Type
        //  B will always choose an empty line before a line with any customers in it
        /// </summary>
        /// <returns></returns>
        public Register GetRegisterWithLeastItems()
        {
            var customerRegistrationPair = new Dictionary<Customer, Register>();
            var emptyRegistrations = new List<Register>();
            var customerItems = new List<Customer>();
            foreach (Register register in Registrations)
            {
                // B will always choose an empty line before a line with any customers in it
                if (register.GetCustomers().Count() == 0)
                {
                    emptyRegistrations.Add(register);
                }
                else
                {
                    Customer lastCustomer = null;
                    var customerQueueReg = register.GetCustomers();
                    //Customer Type B looks at the last customer in each line, and always chooses to be
                    lastCustomer = customerQueueReg.Peek();
                    customerRegistrationPair[lastCustomer] = register;
                    customerItems.Add(lastCustomer);
                }
            }
            //B will always choose an empty line before a line with any customers in it
            if (emptyRegistrations.Count() > 0)
            {
                emptyRegistrations.Sort();
                return emptyRegistrations.First();
            }
            else
            {
                //behind the customer with the fewest number of items left to check out,
                customerItems.Sort();
                return customerRegistrationPair[customerItems.First()];
            }
        }
        public void ServiceCustomer(List<Customer> customers)
        {
            foreach (Customer customer in customers)
            {
                //Add to queue for Customer A
                if (customer.GetCustomerType().Equals(ECustomerType.A))
                {
                    Register registerShortestsize = GetRegisterShortestLine();
                    registerShortestsize.GetCustomers().Enqueue(customer);
                }
                //Add to queue for Customer B
                else
                {
                    Register registerGetLeastItems = GetRegisterWithLeastItems();
                    registerGetLeastItems.GetCustomers().Enqueue(customer);
                }
            }
        }
        public bool GetRegisterstatus()
        {
            return Registrations.Any(x => x.GetCustomers().Count != 0);
        }
    }
}

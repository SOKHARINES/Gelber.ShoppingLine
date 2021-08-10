using System;
using System.Collections.Generic;

namespace Gelber.ShoppingLine
{

    public class Register : IComparable<Register>
    {
        public int id;
        private Queue<Customer> CustomerQueue = null;
        public Register(int id)
        {
            this.id = id;
            CustomerQueue = new Queue<Customer>();
        }
        public Queue<Customer> GetCustomers()
        {
            return CustomerQueue;
        }
        public int CompareTo(Register other)
        {
            return this.id.CompareTo(other.id);
        }
    }
}

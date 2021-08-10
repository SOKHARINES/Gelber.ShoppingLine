using System;

namespace Gelber.ShoppingLine
{
    public class Customer : IComparable<Customer>
    {
        private readonly ECustomerType CustomerType;
        private readonly int TimeArrived;
        private int Items;
        private  bool CustomerServiceStatus;
        public int CompareTo(Customer other)
        {
            // If two or more customers arrive at the same time, those with fewer items choose registers
            // before those with more
            var result = Items.CompareTo(other.Items);
            if (result == 0)
            {
                // and if they have the same number of items then type A's choose before
                // type B's.
                result = CustomerType.CompareTo(other.CustomerType);
            }
            return result;
        }
        public Customer(ECustomerType customerType, int timeArrived, int items)
        {
            CustomerType = customerType;
            TimeArrived = timeArrived;
            Items = items;
        }
        public int GetCustomerTimeArrived()
        {
            return TimeArrived;
        }
        public ECustomerType GetCustomerType()
        {
            return CustomerType;
        }
        public int RemoveShoppingItems()
        {
            return --Items;
        }

        public bool GetServiceStatus()
        {
            return CustomerServiceStatus;
        }
        public void SetServicesStatus(bool serviceStatus)
        {
            CustomerServiceStatus = serviceStatus;
        }
    }
}

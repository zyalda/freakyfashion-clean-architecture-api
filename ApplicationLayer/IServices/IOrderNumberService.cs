using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationLayer.IServices
{
    public interface IOrderNumberService
    {
        string Generate(int orderId);
    }
}

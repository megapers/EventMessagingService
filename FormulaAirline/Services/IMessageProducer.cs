using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormulaAirline.Services
{
    public interface IMessageProducer
    {
        public void PublichNewMessage<T>(T message);
        public void Dispose();
    }
}
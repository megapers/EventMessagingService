using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormulaAirline.Services
{
    public interface IMessageProducer
    {
        public Task PublishNewMessageAsync<T>(T message);
    }
}
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Exceptions
{
    public class InvalidCommandCalculationException : Exception
    {
        public InvalidCommandCalculationException(ICommand command)
            : base($"Invalid calculation for command: {command.GetName()}")
        {
        }
        public InvalidCommandCalculationException(ICommand command, string reason)
            : base($"Invalid calculation for command: {command.GetName()}, reason: {reason}")
        {
        }

        public InvalidCommandCalculationException(ICommand command, Exception innerException)
            : base($"Invalid calculation for command: {command.GetName()}", innerException)
        {
        }
    }
}

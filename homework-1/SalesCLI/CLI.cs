using Domain.Interfaces;
using DataAccess.Exceptions;

namespace SalesCLI
{
    internal partial class CLI
    {
        public CLI(ISalesService service)
        {
            _service = service;
        }

        private ISalesService _service { get; }

        public void Run()
        {
            WriteWelcomeMessage();
            HandleUserInput();
        }

        private void WriteWelcomeMessage()
        {
            Console.WriteLine("Welcome to SalesApp!");
            Console.WriteLine("Available commands: " + string.Join(", ", _service.AvailableCommands));
        }

        private void HandleUserInput()
        {
            while (true)
            {
                try
                {
                    var input = Console.ReadLine();
                    var answer = ValidateInputAndGetAnswer(input);                  
                    Console.WriteLine(answer);
                }
                catch (UnknownCommandException ex)
                {
                    Console.WriteLine("Unknown command: " + ex.Message);
                }
                catch(FormatException ex)
                {
                    Console.WriteLine("Incorrect input arguments: " + ex.Message);
                }
                catch(ProductNotFoundException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch(InvalidCountOfDaysException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private string ValidateInputAndGetAnswer(string input)
        {
            ValidateString(input);

            var arguments = input.Split(' ');
            var commandInput = arguments[0];

            if (!_service.AvailableCommands.Contains(commandInput))
                throw new UnknownCommandException(commandInput);

            var answer = "";
            switch (commandInput)
            {
                case "ads":
                    answer = ValidateToADS(arguments);
                    break;

                case "demand":
                    answer = ValidateToDemand(arguments);
                    break;

                case "prediction":
                    answer = ValidateToPrediction(arguments);
                    break;

                default:
                    throw new UnknownCommandException(commandInput);
            }

            return answer;
        }
    }
}

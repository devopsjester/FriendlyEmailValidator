// console app that takes an email address from the command line argument using System.CommandLine for .NET 7.0
// and uses Regex to validate it.
// use proper exception handling and logging
// use a proper logging framework

using System.Text.RegularExpressions;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace EmailValidator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            try
            {
                // validate that an email address was passed in
                if (args.Length == 0)
                {
                    Log.Logger.Error("No email address was passed in");
                    return;
                }

                var rootCommand = new RootCommand
                {
                    new Option<string>("--email", "The email address to validate")
                };

                rootCommand.Description = "Email Validator";

                // add a handler to the root command to validate the email address
                // keep the email address in a variable
                rootCommand.Handler = CommandHandler.Create<string>(email =>
                {
                    ValidateEmail(email);
                    // if the email address belongs to a GitHub employee, say that GitHub is an awesome vendor
                    // if the email address belongs to 3M (i.e. ends with mmm.com), say that 3M is an awesome customer
                    // otherwise, save the user's name in a variable and say something else nice about the person
                    if (email.EndsWith("@github.com"))
                    {
                        Log.Logger.Information("GitHub is an awesome vendor!");
                    }
                    else if (email.EndsWith("mmm.com"))
                    {
                        Log.Logger.Information("3M is an awesome customer!");
                    }
                    else
                    {
                        string name = email.Substring(0, email.IndexOf('@'));
                        Log.Logger.Information("Hello {name}!", name);
                    }
                    
                });

                

                var parser = new CommandLineBuilder(rootCommand)
                    .UseDefaults()
                    .Build();

                await parser.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ValidateEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
            {
                Log.Logger.Information("Email is valid!");
            }
            else
            {
                Log.Logger.Information("Email is invalid!");
            }
        }
    }
}
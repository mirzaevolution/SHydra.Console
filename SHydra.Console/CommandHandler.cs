using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHydra
{
    public class CommandHandler
    {
        public static void ExecuteCommand(string[] args)
        {
            Console.Title = "SteganoHydra v1.0.1";
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            ParseCommand(args).Wait();
        }
        private static async Task ParseCommand(string[] args)
        {
            try
            { 
                if ( 
                        args.Length == 0 || (args.Length == 1 && 
                        args.FirstOrDefault().Equals("-h",StringComparison.InvariantCultureIgnoreCase))
                   )
                {
                    WelcomeScreen();
                    return;
                }
                if(!new int[] { 9, 7, 6 }.Any(f => f == args.Length))
                {
                    ConsoleLogger.ShowErrorMessage("Invalid command. Make sure you input valid command");
                    return;
                }
                SteganoHandler steganoHandler = new SteganoHandler();
                //Get the operation type
                string operation = args.FirstOrDefault(f => (f.Equals("-em", StringComparison.InvariantCultureIgnoreCase)) || (f.Equals("-ex", StringComparison.InvariantCultureIgnoreCase)));
                if(string.IsNullOrEmpty(operation))
                {
                    ConsoleLogger.ShowErrorMessage("Invalid command. You must specify either -em or -ex");
                    return;
                }
                operation = operation.ToLower();
                switch(operation)
                {
                    case "-em":
                        {
                            //Validate whether -i switch exists
                            if(args.Any(f => f.Equals("-i", StringComparison.InvariantCultureIgnoreCase)))
                            {
                                //Get -i value
                                string inputFile = GetNextSwitchValue(args, "-i");

                                //Check input file existence
                                if (!File.Exists(inputFile))
                                {
                                    ConsoleLogger.ShowErrorMessage("Target input file doesn't exist");
                                    return;
                                }

                                //Check input file extension
                                if(!Path.GetExtension(inputFile).Equals(".png",StringComparison.InvariantCultureIgnoreCase))
                                {
                                    ConsoleLogger.ShowErrorMessage("Target input file extension is not PNG format");
                                    return;
                                }

                                //Validate whether -o switch exists
                                if(args.Any(f => f.Equals("-o", StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    //Get -o value
                                    string outputFile = GetNextSwitchValue(args, "-o");

                                    //Check output file extension
                                    if (!Path.GetExtension(outputFile).Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        ConsoleLogger.ShowErrorMessage("Target output file extension must be PNG format");
                                        return;
                                    }

                                    //Validate sub operation switch
                                    if(args.Any(f => f.Equals("-s", StringComparison.InvariantCultureIgnoreCase)))
                                    {
                                        //Get text value 
                                        string text = GetNextSwitchValue(args, "-s");
                                        if (string.IsNullOrEmpty(text.Trim()))
                                        {
                                            ConsoleLogger.ShowErrorMessage("Invalid command. You cannot input empty string");
                                            return;
                                        }

                                        //Get password
                                        string pwd = GetNextSwitchValue(args, "-p");
                                        pwd = pwd.Trim();

                                        //Check password length
                                        if (pwd.Length < 4)
                                        {
                                            ConsoleLogger.ShowErrorMessage("Bad password. Password must be at least 4 chars length");
                                            return;
                                        }

                                        await steganoHandler.EmbedMessage(
                                                fileInputLocation: inputFile,
                                                fileOutputLocation: outputFile,
                                                message: text,
                                                password: pwd
                                            );

                                    }
                                    else if(args.Any(f => f.Equals("-f",StringComparison.InvariantCultureIgnoreCase)))
                                    {
                                        //Get text value 
                                        string fileAttachment = GetNextSwitchValue(args, "-f");
                                        if (!File.Exists(fileAttachment))
                                        {
                                            ConsoleLogger.ShowErrorMessage("File attachment doesn't exist");
                                            return;
                                        }

                                        //Get password
                                        string pwd = GetNextSwitchValue(args, "-p");
                                        pwd = pwd.Trim();

                                        //Check password length
                                        if (pwd.Length < 4)
                                        {
                                            ConsoleLogger.ShowErrorMessage("Bad password. Password must be at least 4 chars length");
                                            return;
                                        }

                                        await steganoHandler.EmbedFiles(
                                                fileInputLocation: inputFile,
                                                fileOutputLocation: outputFile,
                                                fileAttachments:new string[] { fileAttachment },
                                                password: pwd
                                            );

                                    }
                                    else if (args.Any(f => f.Equals("-d", StringComparison.InvariantCultureIgnoreCase)))
                                    {
                                        //Get text value 
                                        string directory = GetNextSwitchValue(args, "-d");
                                        directory = FixDirectoryBackslash(directory);
                                        if (!Directory.Exists(directory))
                                        {
                                            ConsoleLogger.ShowErrorMessage($"'{directory}' doesn't exist");
                                            return;
                                        }
                                        string[] files = Directory.GetFiles(directory);

                                        if (files.Length == 0)
                                        {
                                            ConsoleLogger.ShowErrorMessage($"'{directory}' has no files within it");
                                        }

                                        //Get password
                                        string pwd = GetNextSwitchValue(args, "-p");
                                        pwd = pwd.Trim();

                                        //Check password length
                                        if (pwd.Length < 4)
                                        {
                                            ConsoleLogger.ShowErrorMessage("Bad password. Password must be at least 4 chars length");
                                            return;
                                        }

                                        await steganoHandler.EmbedFiles(
                                                fileInputLocation: inputFile,
                                                fileOutputLocation: outputFile,
                                                fileAttachments: files,
                                                password: pwd
                                            );

                                    }
                                    else
                                    {
                                        ConsoleLogger.ShowErrorMessage("Invalid command. You must specify -s/-f/-d to proceed the operation");
                                        return;
                                    }
                                }

                            }
                            else
                            {
                                ConsoleLogger.ShowErrorMessage("Invalid command. You must specify -i switch");
                                return;
                            }
                            break;
                        }
                    case "-ex":
                        {
                            //Validate whether -i switch exists
                            if (args.Any(f => f.Equals("-i", StringComparison.InvariantCultureIgnoreCase)))
                            {
                                //Get -i value
                                string inputFile = GetNextSwitchValue(args, "-i");

                                //Check input file existence
                                if (!File.Exists(inputFile))
                                {
                                    ConsoleLogger.ShowErrorMessage("Target input file doesn't exist");
                                    return;
                                }

                                //Check input file extension
                                if (!Path.GetExtension(inputFile).Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    ConsoleLogger.ShowErrorMessage("Target input file extension is not PNG format");
                                    return;
                                }

                                //Get password
                                string pwd = GetNextSwitchValue(args, "-p");
                                pwd = pwd.Trim();

                                //Check password length
                                if (pwd.Length < 4)
                                {
                                    ConsoleLogger.ShowErrorMessage("Bad password. Password must be at least 4 chars length");
                                    return;
                                }
                                
                                //Validate sub operation. Must be -s or -d
                                if(args.Any(f => f.Equals("-s",StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    await steganoHandler.ExtractMessage(
                                            fileInputLocation: inputFile,
                                            password: pwd
                                        );
                                }
                                else if (args.Any(f => f.Equals("-d", StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    //Extract directory location
                                    string directory = GetNextSwitchValue(args, "-d");

                                    directory = FixDirectoryBackslash(directory);

                                    if (!Directory.Exists(directory))
                                    {
                                        Directory.CreateDirectory(directory);
                                    }
                                    await steganoHandler.ExtractFiles(
                                            fileInputLocation: inputFile,
                                            password: pwd,
                                            targetDirectory: directory
                                        );
                                }
                                else
                                {
                                    ConsoleLogger.ShowErrorMessage("Invalid command. You must specify -s/-d to proceed the operation");
                                    return;
                                }
                            }
                            else
                            {
                                ConsoleLogger.ShowErrorMessage("Invalid command. You must specify -i switch");
                                return;
                            }
                            break;
                        }
                }
            }
            catch(Exception ex)
            {
                ConsoleLogger.ShowErrorMessage(ex.Message);
            }
        }
        private static void WelcomeScreen()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("         ~SteganoHydra v1.0.1~          ");
            Console.WriteLine("             CLI Version                ");
            Console.WriteLine("    Created by: Mirza Ghulam Rasyid     ");
            Console.WriteLine("========================================");

            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("########################################");
            Console.WriteLine("########################################");
            Console.WriteLine("########################################");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("########################################");
            Console.WriteLine("########################################");
            Console.WriteLine("########################################");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkYellow;


            Console.WriteLine();

            ConsoleLogger.LogMessage("Getting help", "!");
            ConsoleLogger.LogMessage(" shydra.exe -h");

            Console.WriteLine();

            ConsoleLogger.LogMessage("Embed text string into an image","!");
            ConsoleLogger.LogMessage(" shydra.exe -em -i <input_image.png> -o <output_image.png>  -s <message> -p <your_password>");
            ConsoleLogger.LogMessage(" Example: ");
            ConsoleLogger.LogMessage("\t- shydra.exe -em -i \"D:\\pictures\\sample.png\" -o \"D:\\output\\sample.png\" -s \"Hello World\" -p @Pa$sw0rd");

            Console.WriteLine();

            ConsoleLogger.LogMessage("Embed a file into an image","!");
            ConsoleLogger.LogMessage(" shydra.exe -em -i <input_image.png> -o <output_image.png>  -f <file.ext> -p <your_password>");
            ConsoleLogger.LogMessage(" Examples: ");
            ConsoleLogger.LogMessage("\t- shydra.exe -em -i \"D:\\pictures\\sample.png\" -o \"D:\\output\\sample.png\" -f \"D:\\profile.pdf\" -p @Pa$sw0rd");
            ConsoleLogger.LogMessage("\t- shydra.exe -em -i \"D:\\pictures\\sample.png\" -o \"D:\\output\\sample.png\" -f \"D:\\profile_picture.png\" -p @Pa$sw0rd");

            Console.WriteLine();

            ConsoleLogger.LogMessage("Embed files inside a folder into an image", "!");
            ConsoleLogger.LogMessage(" shydra.exe -em -i <input_image.png> -o <output_image.png> -d <folder_path> -p <your_password>");
            ConsoleLogger.LogMessage(" Example: ");
            ConsoleLogger.LogMessage("\t- shydra.exe -em -i \"D:\\pictures\\sample.png\" -o \"D:\\output\\sample.png\" -d \"D:\\documents\" -p @Pa$sw0rd");

            Console.WriteLine();

            ConsoleLogger.LogMessage("Extract text string from an image", "!");
            ConsoleLogger.LogMessage(" shydra.exe -ex -i <input_image.png> -p <your_password> -s");
            ConsoleLogger.LogMessage(" Example: ");
            ConsoleLogger.LogMessage("\t- shydra.exe -ex -i \"D:\\output\\encoded - image.png\" -p @Pa$sw0rd -s");

            Console.WriteLine();

            ConsoleLogger.LogMessage("Extract file(s) from an image", "!");
            ConsoleLogger.LogMessage(" shydra.exe -ex -i <input_image.png> -p <your_password> -d <folder_path_to_extract>");
            ConsoleLogger.LogMessage(" Example: ");
            ConsoleLogger.LogMessage("\t- shydra.exe -ex -i \"D:\\output\\encoded - image.png\" -p @Pa$sw0rd -d \"D:\\extractions\" ");

            Console.WriteLine();
        }
        private static string FixDirectoryBackslash(string directoryName)
        {
            if (!directoryName.EndsWith("\\"))
                return directoryName;
            return directoryName.Remove(directoryName.LastIndexOf("\\"));
        }
        private static string GetNextSwitchValue(string[] args, string switchParam)
        {
            if(!args.Any(f => f.Equals(switchParam,StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new Exception($"Invalid command. {switchParam} switch doesn't exist");
            }
            for(int index = 0; index < args.Length; index++)
            {
                if(args[index].Equals(switchParam,StringComparison.InvariantCultureIgnoreCase))
                {
                    if (index + 1 < args.Length)
                    {
                        string value = args[index + 1];
                        if (value.StartsWith("-"))
                        {
                            throw new Exception($"Invalid command. Bad command request for switch {switchParam}");

                        }
                        return value;
                    }
                    else
                    {
                        throw new Exception($"Invalid command. No value found for switch {switchParam}");
                    }
                }
            }
            throw new Exception($"Invalid command. No value found for switch {switchParam}");
        }
    }
}

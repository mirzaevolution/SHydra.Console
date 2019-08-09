using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using Pixelator.Api;
using Pixelator.Api.Configuration;
using Pixelator.Api.Utility;
using Input = Pixelator.Api.Input;
using Output = Pixelator.Api.Output;

namespace SHydra
{
    public class SteganoHandler
    {
        public async Task EmbedMessage(
            string fileInputLocation, 
            string fileOutputLocation, 
            string message, 
            string password)
        {
            try
            {
                ConsoleLogger.LogMessage("Creating encoder instance...");
                ImageEncoder imageEncoder = new ImageEncoder(
                        format:ImageFormat.Png,
                        encryptionConfiguration: new EncryptionConfiguration(EncryptionType.Aes256, 20000),
                        compressionConfiguration: new CompressionConfiguration(CompressionType.Gzip, CompressionLevel.Standard),
                        embeddedImage: new EmbeddedImage(Image.FromFile(fileInputLocation), EmbeddedImage.PixelStorage.Auto)
                    );
                ConsoleLogger.LogMessage("Reading message bytes...");
                byte[] contentBytes = Encoding.UTF8.GetBytes(message);
                string outputDirectory = Path.GetDirectoryName(fileOutputLocation);
                if(!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }
                ConsoleLogger.LogMessage("Appending message bytes into a memory stream...");
                MemoryStream contentStream = new MemoryStream();
                await contentStream.WriteAsync(contentBytes, 0, contentBytes.Length);
                ConsoleLogger.LogMessage("Adding memory stream object into encoder...");
                imageEncoder.AddDirectory(new Input.Directory("\\", new[]
                {
                    new Input.File("__payload__.bin",contentStream)
                }));
                using(FileStream outputStream = new FileStream(fileOutputLocation,FileMode.OpenOrCreate,FileAccess.ReadWrite))
                {
                    ConsoleLogger.LogMessage("Encoding data into target image...");
                    ConsoleLogger.LogMessage("Creating output image...");
                    await imageEncoder.SaveAsync(
                        outputStream: outputStream, 
                        encodingConfiguration: new EncodingConfiguration
                        (
                            password: password, 
                            tempStorageProvider: new MemoryStorageProvider(), 
                            bufferSize: 81920, 
                            fileGroupSize: 1024 * 10245
                        ));
                    ConsoleLogger.LogMessage($"Operation succeeded. File saved at {fileOutputLocation}");
                }
            }
            catch(Exception ex)
            {
                ConsoleLogger.ShowErrorMessage(ex.Message);
            }
        }
        public async Task EmbedFiles(
            string fileInputLocation,
            string fileOutputLocation,
            string[] fileAttachments,   
            string password)
        {
            try
            {
                ConsoleLogger.LogMessage("Creating encoder instance...");
                ImageEncoder imageEncoder = new ImageEncoder(
                        format: ImageFormat.Png,
                        encryptionConfiguration: new EncryptionConfiguration(EncryptionType.Aes256, 20000),
                        compressionConfiguration: new CompressionConfiguration(CompressionType.Gzip, CompressionLevel.Standard),
                        embeddedImage: new EmbeddedImage(Image.FromFile(fileInputLocation), EmbeddedImage.PixelStorage.Auto)
                    );
                string outputDirectory = Path.GetDirectoryName(fileOutputLocation);
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                string attachmentDirectory = Path.GetDirectoryName(fileAttachments.FirstOrDefault());
                
                ConsoleLogger.LogMessage("Adding file(s) into encoder instance...");
                List<Input.File> files = new List<Input.File>();
                foreach(string file in fileAttachments)
                {
                    files.Add(new Input.File(new System.IO.FileInfo(file)));
                }
                imageEncoder.AddDirectory(new Input.Directory("\\",files));

                using (FileStream outputStream = new FileStream(fileOutputLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    ConsoleLogger.LogMessage("Encoding data into target image...");
                    ConsoleLogger.LogMessage("Creating output image...");
                    await imageEncoder.SaveAsync(
                        outputStream: outputStream, 
                        encodingConfiguration: new EncodingConfiguration
                        (
                            password: password, 
                            tempStorageProvider: new MemoryStorageProvider(), 
                            bufferSize: 81920, 
                            fileGroupSize: 1024 * 10245
                        ));

                    ConsoleLogger.LogMessage($"Operation succeeded. File saved at {fileOutputLocation}");
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.ShowErrorMessage(ex.ToString());
            }
        }
        public async Task ExtractMessage(
            string fileInputLocation,
            string password)
        {
            try
            {
                using (var input = new FileStream(fileInputLocation, FileMode.Open, FileAccess.Read))
                {
                    ConsoleLogger.LogMessage("Creating decoder instance...");
                    var decoder = await ImageDecoder.LoadAsync(
                        imageStream: input, 
                        decodingConfiguration: new DecodingConfiguration
                        (
                            password: password,
                            tempStorageProvider: new MemoryStorageProvider(),
                            bufferSize: 81920)
                        );
                    ConsoleLogger.LogMessage("Creating decoder output handler...");
                    Output.FileDataOutputHandler handler = new Output.FileDataOutputHandler(
                            (directory, file, stream) =>
                            {
                                if (stream != null)
                                {
                                    using (StreamReader reader = new StreamReader(stream))
                                    {
                                        ConsoleLogger.LogMessage("Decoding completed");
                                        ConsoleLogger.LogMessage("Message payload: ");
                                        ConsoleLogger.LogMessage(reader.ReadToEnd());
                                        
                                    }
                                }
                                else
                                {
                                    ConsoleLogger.ShowErrorMessage("Image doesn't contain any message payload");
                                }
                                return Task.CompletedTask;
                            }
                    );
                    ConsoleLogger.LogMessage("Start decoding image, extracting payload...");
                    await decoder.DecodeAsync(outputHandler: handler);
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.ShowErrorMessage(ex.Message);
            }
        }
        public async Task ExtractFiles(
            string fileInputLocation,
            string password,
            string targetDirectory)
        {
            try
            {
                using (var input = new FileStream(fileInputLocation, FileMode.Open, FileAccess.Read))
                {
                    ConsoleLogger.LogMessage("Creating decoder instance...");
                    var decoder = await ImageDecoder.LoadAsync
                        (
                        imageStream: input, 
                        decodingConfiguration: new DecodingConfiguration
                        (
                            password: password,
                            tempStorageProvider: new MemoryStorageProvider(),
                            bufferSize: 81920
                        ));
                    ConsoleLogger.LogMessage("Creating decoder output handler...");
                    Output.FileDataOutputHandler handler = new Output.FileDataOutputHandler(
                            (directory, file, stream) =>
                            {
                                if (stream != null)
                                {
                                    try
                                    {
                                        string fullName = Path.Combine(targetDirectory, file.Name);
                                        if (File.Exists(fullName))
                                        {
                                            File.Delete(fullName);
                                        }
                                        ConsoleLogger.LogMessage($"'{file.Name}' has been extracted to {fullName}");
                                        using (FileStream saveStream = new FileStream(fullName, FileMode.OpenOrCreate, FileAccess.Write))
                                        {
                                            stream.CopyTo(saveStream);
                                        }
                                    }
                                    catch
                                    {
                                        ConsoleLogger.ShowErrorMessage($"Failed to process '{file.Name}' from the image container");
                                    }
                                }
                                return Task.CompletedTask;
                            }
                    );
                    ConsoleLogger.LogMessage("Start decoding image, extracting payload...");
                    await decoder.DecodeAsync(outputHandler: handler);
                }
            }
            catch (Exception ex)
            {
                ConsoleLogger.ShowErrorMessage(ex.Message);
            }
        }
    }
}

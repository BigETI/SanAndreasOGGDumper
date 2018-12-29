using GTAAudioSharp;
using System;
using System.IO;

/// <summary>
/// San Andreas OGG dumper namespace
/// </summary>
namespace SanAndreasOGGDumper
{
    /// <summary>
    /// Program class
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            try
            {
                string path = null;
                if (args.Length > 0)
                {
                    path = args[0];
                }
                else
                {
                    Console.Write("Please specify a path to dump the audio files from (press return to exit): ");
                    path = Console.ReadLine();
                }
                if (path != null)
                {
                    if (path.Trim().Length > 0)
                    {
                        string streams_directory = Path.Combine(Environment.CurrentDirectory, "streams");
                        byte[] buffer = new byte[4096];
                        if (!(Directory.Exists(streams_directory)))
                        {
                            Directory.CreateDirectory(streams_directory);
                        }
                        using (GTAAudioFiles gta_audio_files = GTAAudio.OpenRead(path))
                        {
                            if (gta_audio_files != null)
                            {
                                GTAAudioStreamsFile[] streams_audio_files = gta_audio_files.StreamsAudioFiles;
                                foreach (GTAAudioStreamsFile streams_audio_file in streams_audio_files)
                                {
                                    if (streams_audio_file != null)
                                    {
                                        string streams_streams_directory = Path.Combine(streams_directory, streams_audio_file.Name);
                                        try
                                        {
                                            if (!(Directory.Exists(streams_streams_directory)))
                                            {
                                                Directory.CreateDirectory(streams_streams_directory);
                                            }
                                            for (int i = 0; i < streams_audio_file.NumBanks; i++)
                                            {
                                                try
                                                {
                                                    using (Stream audio_stream = streams_audio_file.Open((uint)i))
                                                    {
                                                        if (audio_stream != null)
                                                        {
                                                            string audio_file_path = Path.Combine(streams_streams_directory, streams_audio_file.Name + "." + i + ".ogg");
                                                            if (File.Exists(audio_file_path))
                                                            {
                                                                File.Delete(audio_file_path);
                                                            }
                                                            using (FileStream file_stream = File.Open(audio_file_path, FileMode.Create))
                                                            {
                                                                int len;
                                                                long audio_stream_length = audio_stream.Length;
                                                                while ((len = Math.Min((int)(audio_stream_length - audio_stream.Position), buffer.Length)) > 0)
                                                                {
                                                                    if (audio_stream.Read(buffer, 0, len) == len)
                                                                    {
                                                                        file_stream.Write(buffer, 0, len);
                                                                    }
                                                                    else
                                                                    {
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.Error.WriteLine(e);
                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.Error.WriteLine(e);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.Error.WriteLine("Can't open audio directory \"" + path + "\".");
                                Environment.ExitCode = 3;
                            }
                        }
                    }
                    else
                    {
                        Environment.ExitCode = 2;
                    }
                }
                else
                {
                    Environment.ExitCode = 1;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                Environment.ExitCode = -1;
            }
        }
    }
}

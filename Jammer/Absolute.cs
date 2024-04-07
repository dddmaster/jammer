using Spectre.Console;
using System.IO;

namespace jammer
{
    public class Absolute
    {
        public static string[] Correctify(string[] args)
        {
            string title;
            for (int i = 0; i < args.Length; i++)
            {
                title = "";

                // split title by ½
                string[] titleSplit = args[i].Split("½");
    
                if (titleSplit.Length > 1)
                {
                    title = titleSplit[1];
                    args[i] = titleSplit[0];
                }

                string item = args[i];
                #if CLI_UI
                AnsiConsole.MarkupLine($"[green]{Locale.OutsideItems.Checking} {item}[/]");
                #endif
                #if AVALONIA_UI
                // TODO AVALONIA_UI
                #endif

                if (URL.IsUrl(item))
                {
                    // if url doesnt have http:// or https://
                    if (!item.Contains("http://") && !item.Contains("https://"))
                    {
                        item = "https://" + item;
                    }

                    if (URL.IsValidSoundcloudSong(item))
                    {
                        // splice ? and everything after it
                        int index = item.IndexOf("?");
                        if (index > 0)
                        {
                            item = item.Substring(0, index);
                        }
                    }
                    else if (URL.IsValidYoutubeSong(item))
                    {
                        // splice & and everything after it
                        int index = item.IndexOf("&");
                        if (index > 0)
                        {
                            item = item.Substring(0, index);
                        }
                    }
                    else if (URL.IsUrl(item))
                    {
                        #if CLI_UI
                        #endif
                        #if AVALONIA_UI
                        // TODO AVALONIA_UI
                        #endif
                        AnsiConsole.MarkupLine($"[green]URL {item} {Locale.OutsideItems.IsValid}[/]");
                    }
                    else {
                        #if CLI_UI
                        AnsiConsole.MarkupLine($"[red]URL {item} {Locale.OutsideItems.IsntValid}[/]");
                        #endif
                        #if AVALONIA_UI
                        // TODO AVALONIA_UI
                        #endif
                        // delete item from args
                        args = args.Take(i).Concat(args.Skip(i + 1)).ToArray();
                        i--;
                    }

                    args[i] = item;
                }
                // if folder exists, convert to absolute path
                else if (Directory.Exists(item))
                {
                    if (IsRelativePath(item)) {
                        args[i] = ConvertToAbsolutePath(item);
                    }
                }
                // if exits, convert to absolute path
                else if (File.Exists(item))
                {
                    if (IsRelativePath(item)) {
                        args[i] = ConvertToAbsolutePath(item);
                    }
                }
                else if (!File.Exists(item))
                {
                    #if CLI_UI
                    AnsiConsole.MarkupLine($"[red]{Locale.OutsideItems.File} {item} {Locale.OutsideItems.DoesntExist}[/]");
                    #endif
                    #if AVALONIA_UI
                    // TODO AVALONIA_UI
                    #endif
                    // delete item from args
                    args = args.Take(i).Concat(args.Skip(i + 1)).ToArray();
                    i--;
                }

                if (title != "")
                {
                    args[i] = args[i] + "½" + title;
                }
            }

            return args;
        }

        static bool IsAbsolutePath(string path)
        {
            return Path.IsPathRooted(path);
        }

        static bool IsRelativePath(string path)
        {
            return !IsAbsolutePath(path);
        }

        static public string ConvertToAbsolutePath(string relativePath)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            return Path.GetFullPath(Path.Combine(currentDirectory, relativePath));
        }
    }
}

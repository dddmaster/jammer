using Spectre.Console;
using System.IO;

/* using System.ComponentModel.DataAnnotations; */
namespace Jammer {

    internal static partial class TUI {

        public static bool cls = false; // clear screen

        static public string[] GetAllSongs() {
            if (Utils.songs.Length == 0) {
                string[] returnstring = {$"[grey]{Locale.Player.NoSongsInPlaylist}[/]"};
                return returnstring;
            }
            string[] allSongs = {};
            List<string> results = new()
            {
                $"{Locale.OutsideItems.CurrPlaylistView} {Keybindings.PlaylistViewScrollup}, {Keybindings.PlaylistViewScrolldown}",
                $"{Locale.OutsideItems.PlaySongWith} {Keybindings.Choose}. {Locale.OutsideItems.PlaySongWith} {Keybindings.DeleteCurrentSong}. {Locale.OutsideItems.AddToQueue} {Keybindings.AddSongToQueue}"
            };
            int maximum = 6 + results.Count;
            
            for (int i = 0; i < Utils.songs.Length; i++) {
                string keyValue = Utils.songs[i].ToString();
                
                if (i >= IniFileHandling.ScrollIndexLanguage && results.Count != maximum) {
                    if (i == Utils.currentSongIndex) {
                        results.Add($"[green]{i + 1}. {Play.Title(keyValue, "get")}[/]");
                    }
                    else if (i == Utils.currentPlaylistSongIndex) {
                        results.Add($"[yellow]{i + 1}. {Play.Title(keyValue, "get")}[/]");
                    }
                    else if (Utils.currentPlaylistSongIndex <= 3) {
                        results.Add($"{i + 1}. {Play.Title(keyValue, "get")}");
                    }
                    else if(
                        Utils.currentPlaylistSongIndex + 5 >= Utils.songs.Length &&
                        i > Utils.songs.Length - 6
                    ){
                        keyValue = Utils.songs[i].ToString();
                        results.Add($"{i + 1}. {Play.Title(keyValue, "get")}");
                    }
                    else if (i >= Utils.currentPlaylistSongIndex -2 && i < Utils.currentPlaylistSongIndex + 3) {
                        results.Add($"{i + 1}. {Play.Title(keyValue, "get")}");
                    }
                }
            }

            return results.ToArray();
        }
        static string[] GetAllSongsQueue() {
            int maximum = 10;
            List<string> results = new();
            for (int i = 0; i < Utils.queueSongs.Count; i++) {
                if (results.Count != maximum) {
                    results.Add(Utils.queueSongs[i]);
                }
            }

            return Start.Sanitize(results.ToArray());
        }

        static string GetSongWithdots(string song, int length = 80) {
            if (song.Length > length) {
                song = string.Concat("...", song.AsSpan(song.Length - length));
            }
            return song;
        }
        public static string GetPrevCurrentNextSong() {
            // return previous, current and next song in playlist
            string prevSong;
            string nextSong;
            string currentSong;
            int songLength = Start.consoleWidth - 23;
            if (Utils.songs.Length == 0)
            {
                currentSong = $"[grey]{Locale.Player.Current}  : -[/]";
            }
            else
            {
                currentSong = $"[green]{Locale.Player.Current}  : " + Play.Title(GetSongWithdots(Start.Sanitize(Utils.songs[Utils.currentSongIndex]), songLength), "get") + "[/]";
            }

            if (Utils.currentSongIndex > 0)
            {
                prevSong = $"[grey]{Locale.Player.Previos} : " + Play.Title(GetSongWithdots(Start.Sanitize(Utils.songs[Utils.currentSongIndex - 1]), songLength), "get") + "[/]";
            }
            else
            {
                prevSong = $"[grey]{Locale.Player.Previos} : -[/]";
            }


            if (Utils.currentSongIndex < Utils.songs.Length - 1)
            {
                nextSong = $"[grey]{Locale.Player.Next}     : " + Play.Title(GetSongWithdots(Start.Sanitize(Utils.songs[Utils.currentSongIndex + 1]), songLength), "get") + "[/]";
            }
            else
            {
                nextSong = $"[grey]{Locale.Player.Next}     : -[/]";
            }      
            
            return prevSong + $"\n[green]" + currentSong + "[/]\n" + nextSong;
        }

        public static string CalculateTime(double time) {
            int minutes = (int)time / 60;
            int seconds = (int)time % 60;
            string timeString = $"{minutes}:{seconds:D2}";
            return timeString;
        }

        public static void PlaylistInput() {
            AnsiConsole.Markup($"\n{Locale.PlaylistOptions.EnterPlayListCmd} \n");
            AnsiConsole.MarkupLine($"[grey]1. {Locale.PlaylistOptions.AddSongToPlaylist}[/]");
            AnsiConsole.MarkupLine($"[grey]2. {Locale.PlaylistOptions.Deletesong}[/]");
            AnsiConsole.MarkupLine($"[grey]3. {Locale.PlaylistOptions.ShowSongs}[/]");
            AnsiConsole.MarkupLine($"[grey]4. {Locale.PlaylistOptions.ListAll}[/]");
            AnsiConsole.MarkupLine($"[grey]5. {Locale.PlaylistOptions.PlayOther}[/]");
            AnsiConsole.MarkupLine($"[grey]6. {Locale.PlaylistOptions.SaveReplace}[/]");
            // AnsiConsole.MarkupLine($"[grey]7. {Locale.PlaylistOptions.GoToSong}[/]");
            AnsiConsole.MarkupLine($"[grey]8. {Locale.PlaylistOptions.Shuffle}[/]");
            AnsiConsole.MarkupLine($"[grey]9. {Locale.PlaylistOptions.PlaySong}[/]");
            AnsiConsole.MarkupLine($"[grey]0. {Locale.PlaylistOptions.Exit}[/]");

            var playlistInput = Console.ReadKey(true).Key;
            // if (playlistInput == "" || playlistInput == null) { return; }
            switch (playlistInput) {
                // Add song to playlist
                case ConsoleKey.D1:
                    AddSongToPlaylist();
                    break;
                // Delete current song from playlist
                case ConsoleKey.D2:
                    DeleteCurrentSongFromPlaylist();
                    break;
                // Show songs in playlist
                case ConsoleKey.D3:
                    ShowSongsInPlaylist();
                    break;
                // List all playlists
                case ConsoleKey.D4:
                    ListAllPlaylists();
                    break;
                // Play other playlist
                case ConsoleKey.D5:
                    PlayOtherPlaylist();
                    break;
                // Save/replace playlist
                case ConsoleKey.D6:
                    SaveReplacePlaylist();
                    break;
                // Goto song in playlist
                case ConsoleKey.D7:
                    GotoSongInPlaylist();
                    break;
                    
                // Shuffle playlist (randomize)
                case ConsoleKey.D8:
                    ShufflePlaylist();
                    break;
                // Play single song
                case ConsoleKey.D9:
                    PlaySingleSong();
                    break;
                // Exit
                case ConsoleKey.D0:
                    return;
            }
            AnsiConsole.Clear();
        }

        public static void AddSongToPlaylist()
        {
            string songToAdd = jammer.Message.Input(Locale.Player.AddSongToPlaylistMessage1, Locale.Player.AddSongToPlaylistMessage2);
            if (songToAdd == "" || songToAdd == null) {
                jammer.Message.Data(Locale.Player.AddSongToPlaylistError1, Locale.Player.AddSongToPlaylistError2, true);
                return;
            }
            // remove quotes from songToAdd
            songToAdd = songToAdd.Replace("\"", "");
            if (!IsValidSong(songToAdd)) {
                jammer.Message.Data( Locale.Player.AddSongToPlaylistError3+ " " + songToAdd, Locale.Player.AddSongToPlaylistError4, true);
                return;
            }
            songToAdd = Absolute.Correctify(new string[] { songToAdd })[0];
            Play.AddSong(songToAdd);
            Playlists.AutoSave();
        }

        // Delete current song from playlist
        public static void DeleteCurrentSongFromPlaylist()
        {
            Play.DeleteSong(Utils.currentSongIndex, false);
            Playlists.AutoSave();
        }

        // Show songs in playlist
        public static void ShowSongsInPlaylist()
        {
            string? playlistNameToShow = jammer.Message.Input(Locale.Player.ShowSongsInPlaylistMessage1, Locale.Player.ShowSongsInPlaylistMessage2);
            if (playlistNameToShow == "" || playlistNameToShow == null) { 
                jammer.Message.Data(Locale.Player.ShowSongsInPlaylistError1, Locale.Player.ShowSongsInPlaylistError2, true);
                return;
            }
            AnsiConsole.Clear();
            // show songs in playlist
            jammer.Message.Data(Playlists.GetShow(playlistNameToShow), Locale.Player.SongsInPlaylist +" "+ playlistNameToShow);
        }

        // List all playlists
        public static void ListAllPlaylists()
        {
            jammer.Message.Data(Playlists.GetList(), Locale.Player.AllPlaylists);
        }

        // Play other playlist
        public static void PlayOtherPlaylist()
        {
            string? playlistNameToPlay = jammer.Message.Input(Locale.Player.PlayOtherPlaylistMessage1,Locale.Player.PlayOtherPlaylistMessage2);
            if (playlistNameToPlay == "" || playlistNameToPlay == null) { 
                jammer.Message.Data(Locale.Player.PlayOtherPlaylistError1, Locale.Player.PlayOtherPlaylistError2, true);
                return;
            }

            // play other playlist
            Playlists.Play(playlistNameToPlay, false);
        }

        // Save/replace playlist
        public static void SaveReplacePlaylist()
        {
            string playlistNameToSave = jammer.Message.Input(Locale.Player.SaveReplacePlaylistMessage1, Locale.Player.SaveReplacePlaylistMessage2);
            if (playlistNameToSave == "" || playlistNameToSave == null) {
                jammer.Message.Data(Locale.Player.SaveReplacePlaylistError1, Locale.Player.SaveReplacePlaylistError2, true);
                return;
            }
            // save playlist
            Playlists.Save(playlistNameToSave);
        }

        public static void SaveCurrentPlaylist()
        {
            if (Utils.currentPlaylist == "") {
                jammer.Message.Data(Locale.Player.SaveCurrentPlaylistError1,Locale.Player.SaveCurrentPlaylistError2, true);
                return;
            }
            // save playlist
            Playlists.Save(Utils.currentPlaylist, true);
        }

        public static void SaveAsPlaylist()
        {
            string playlistNameToSave = jammer.Message.Input(Locale.Player.SaveAsPlaylistMessage1, Locale.Player.SaveAsPlaylistMessage2);
            if (playlistNameToSave == "" || playlistNameToSave == null) {
                jammer.Message.Data(Locale.Player.SaveAsPlaylistError1, Locale.Player.SaveAsPlaylistError2, true);
                return;
            }
            // save playlist
            Playlists.Save(playlistNameToSave);
        }

        // Goto song in playlist
        public static void GotoSongInPlaylist()
        {
            string songToGoto = jammer.Message.Input(Locale.Player.GotoSongInPlaylistMessage1, Locale.Player.GotoSongInPlaylistMessage2);
            if (songToGoto == "" || songToGoto == null) {
                jammer.Message.Data(Locale.Player.GotoSongInPlaylistError1, Locale.Player.GotoSongInPlaylistError2, true);
                return;
            }
            // songToGoto = GotoSong(songToGoto);
        }

        // Shuffle playlist (randomize)
        public static void ShufflePlaylist()
        {
            // get the name of the current song
            string currentSong = Utils.songs[Utils.currentSongIndex];
            // shuffle playlist
            Play.Shuffle();
            // delete duplicates
            Utils.songs = Utils.songs.Distinct().ToArray();

            Utils.currentSongIndex = Array.IndexOf(Utils.songs, currentSong);
            // set new song from shuffle to the current song
            Utils.currentSong = Utils.songs[Utils.currentSongIndex];
            Playlists.AutoSave();
        }

        // Play single song
        public static void PlaySingleSong()
        {
            string[]? songsToPlay = jammer.Message.Input(Locale.Player.PlaySingleSongMessage1, Locale.Player.PlaySingleSongMessage2).Split(" ");
            
            if (songsToPlay == null || songsToPlay.Length == 0) {
                jammer.Message.Data(Locale.Player.PlaySingleSongError1, Locale.Player.PlaySingleSongError2, true);
                return;
            }

            // if blank "  " remove
            songsToPlay = songsToPlay.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            songsToPlay = Absolute.Correctify(songsToPlay);
            
            // if no songs left, return
            if (songsToPlay.Length == 0) { return; }

            Utils.songs = songsToPlay;
            Utils.currentSongIndex = 0;
            Utils.currentPlaylist = "";
            Play.StopSong();
            Play.PlaySong(Utils.songs, Utils.currentSongIndex);
        }

        public static bool IsValidSong(string song) {
            if (File.Exists(song) || URL.IsUrl(song) || Directory.Exists(song)) {
                AnsiConsole.Markup($"\n[green]{Locale.Player.ValidSong}[/]");
                return true;
            }
            AnsiConsole.Markup($"\n[red]{Locale.Player.InvalidSong}[/]");
            return false;
        }


    }

}
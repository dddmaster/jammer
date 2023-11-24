﻿using NAudio.Wave;
using Spectre.Console;

namespace jammer
{
    internal class UI
    {
        static public bool updated = false;
        static string songList = "";
        static bool updatedSongList = false;
        static bool updatedSettings = false;
        static bool updatedHelp = false;
        static bool updatedPlaylist = false;
        static int songListTimes = 0;
        static int times = 0;
        static public int refreshTimes = JammerFolder.GetRefreshTimes();
        static public void Ui(WaveOutEvent outputDevice)
        {
            var help = new Table();
            if (!updated || !updatedSongList || updatedSettings || updatedHelp || updatedPlaylist) {
                if (Program.textRenderedType == "normal")
                {
                    string loopText = Program.isLoop ? "True" : "False";
                    string isPlayingText = outputDevice.PlaybackState == PlaybackState.Playing ? "Playing" : "Paused";
                    string ismuteText = Program.isMuted ? "Muted" : "";

                    songList = "";
                    for (int i = 0; i < Program.songs.Length; i++)
                    {
                        string? item = Program.songs[i];
                        
                        // show current previous, current and next song
                        if (i == Program.currentSongArgs - 1 || i == Program.currentSongArgs || i == Program.currentSongArgs + 1)
                        {
                            if (i == Program.currentSongArgs - 1)
                            {
                                songList += "[grey]previous:[/] "; // add previous text
                            }
                            if (i == Program.currentSongArgs)
                            {
                                songList += "[grey]current :[/] "; // add current text
                            }
                            if (i == Program.currentSongArgs + 1)
                            {
                                songList += "[grey]next    :[/] "; // add next text
                            }
                            
                            // if soundcloud url
                            if (URL.IsUrl(item))
                            {
                                songList += "[link]";
                            }

                            if (i == Program.currentSongArgs)
                            {
                                if (outputDevice.PlaybackState == PlaybackState.Playing)
                                {
                                    songList += "[green]";
                                }
                                else
                                {
                                    songList += "[yellow]";
                                }
                            }

                            songList += item;
                            if (i == Program.currentSongArgs - 1) {
                            }
                            if (i == Program.currentSongArgs)
                            {
                                songList += "[/]"; // close color tag
                            }
                            if (URL.IsUrl(item))
                            {
                                songList += "[/]"; // close link tag
                            }
                            songList += "\n";
                        }
                        // remove one \n last
                        if (i == Program.songs.Length - 1)
                        {
                            songList = songList.Remove(songList.Length - 1);
                        }
                    }
                    // currentPositionInSeconds
                    int cupMinutes = (int)(Program.currentPositionInSeconds / 60);
                    int cupSeconds = (int)(Program.currentPositionInSeconds % 60);

                    string currentPositionInSecondsText = $"{cupMinutes}:{cupSeconds:D2}";

                    // render table
                    var tableJam = new Table();
                    var table = new Table();

                    tableJam.AddColumn("♫ Jamming to: " + Program.audioFilePath + " ♫");
                    if (Program.songs.Length != 1) { // if more than one song
                        tableJam.AddRow(songList);
                    }

                    table.AddColumn("State");
                    table.AddColumn("Current Position");
                    table.AddColumn("Looping");
                    table.AddColumn("Volume");
                    table.AddColumn("Suffle");
                    table.AddColumn("Muted");
                    table.AddRow(isPlayingText, currentPositionInSecondsText + " / " + Program.positionInSecondsText, loopText, Math.Round(outputDevice.Volume * 100) + " % ", Program.isShuffle + "", ismuteText);

                    AnsiConsole.Clear();
                    AnsiConsole.Write(tableJam);
                    AnsiConsole.Write(table);
                    AnsiConsole.Markup("Press [red]h[/] for help");
                    AnsiConsole.Markup("\nPress [yellow]c[/] for settings");
                    AnsiConsole.Markup("\nPress [green]f[/] to show playlist");
                    updated = true;
                    updatedSongList = true;
                }
                else if (Program.textRenderedType == "help" && updatedHelp)
                {
                    // render help
                    help.AddColumns("Controls", "Description");
                    help.AddRow("Space", "Play/Pause");
                    help.AddRow("Q", "Quit"); 
                    help.AddRow("Left", "Rewind 5 seconds");
                    help.AddRow("Right", "Forward 5 seconds");
                    help.AddRow("Up", "Volume up");
                    help.AddRow("Down", "Volume down");
                    help.AddRow("L", "Toggle looping");
                    help.AddRow("M", "Toggle mute");
                    help.AddRow("S", "Toggle shuffle");
                    help.AddRow("Playlist", "");
                    help.AddRow("P", "Previous song");
                    help.AddRow("N", "Next song");
                    help.AddRow("R", "Play random song");
                    help.AddRow("F2", "Show playlist options");

                    AnsiConsole.Clear();
                    AnsiConsole.Write(help);
                    AnsiConsole.Markup("Press [red]h[/] to hide help");
                    AnsiConsole.Markup("\nPress [yellow]c[/] for settings");
                    AnsiConsole.Markup("\nPress [green]f[/] to show playlist");
                    updatedHelp = false;
                }
                else if (Program.textRenderedType == "settings" && updatedSettings) {
                    var settings = new Table();

                    settings.AddColumns("Settings", "Value", "Change Value");

                    settings.AddRow("refresh UI ", refreshTimes + "", "[green]1[/] to change");
                    settings.AddRow("Forward seconds", Program.forwardSeconds + " sec", "[green]2[/] to change");
                    settings.AddRow("Rewind seconds", Program.rewindSeconds + " sec", "[green]3[/] to change");
                    settings.AddRow("Change Volume by", Program.changeVolumeBy * 100 + " %", "[green]4[/] to change");

                    AnsiConsole.Clear();
                    AnsiConsole.Write(settings);
                    AnsiConsole.Markup("Press [red]h[/] for help");
                    AnsiConsole.Markup("\nPress [yellow]c[/] to hide settings");
                    AnsiConsole.Markup("\nPress [green]f[/] to show playlist");
                    updatedSettings = false;
                }
                else if (Program.textRenderedType == "fakePlayer") {
                    var tableJam = new Table();
                    var table = new Table();

                    tableJam.AddColumn("♫ Jamming to: " + Program.audioFilePath + " ♫");
                    if (Program.songs.Length != 1)
                    {
                        tableJam.AddRow(songList);
                    }

                    string loopText = Program.isLoop ? "True" : "False";
                    string ismuteText = Program.isMuted ? "Muted" : "";

                    table.AddColumn("State");
                    table.AddColumn("Current Position");
                    table.AddColumn("Looping");
                    table.AddColumn("Volume");
                    table.AddColumn("Suffle");
                    table.AddColumn("Muted");
                    table.AddRow("Paused", "0:00" + " / " + "0:00", loopText, Math.Round(Program.volume * 100) + " % ", Program.isShuffle + "", ismuteText);

                    AnsiConsole.Clear();
                    AnsiConsole.Write(tableJam);
                    AnsiConsole.Write(table);
                    AnsiConsole.Markup("Press [red]h[/] for help");
                    AnsiConsole.Markup("\nPress [yellow]c[/] for settings");
                    AnsiConsole.Markup("\nPress [green]f[/] to show playlist");
                }
                else if (Program.textRenderedType == "playlist" && updatedPlaylist)
                {
                    songList = "";
                    for (int i = 0; i < Program.songs.Length; i++)
                    {
                        string? item = Program.songs[i];

                        // if soundcloud url
                        if (URL.IsUrl(item))
                        {
                            songList += "[link]";
                        }

                        if (i == Program.currentSongArgs)
                        {
                            if (outputDevice.PlaybackState == PlaybackState.Playing)
                            {
                                songList += "[green]";
                            }
                            else
                            {
                                songList += "[yellow]";
                            }
                        }
                        songList += item;
                        if (i == Program.currentSongArgs)
                        {
                            songList += "[/]"; // close color tag
                        }
                        if (URL.IsUrl(item))
                        {
                            songList += "[/]"; // close link tag
                        }
                        songList += "\n";
                    }

                    AnsiConsole.Clear();

                    var playlist = new Table();
                    playlist.AddColumn("♫ Jamming to: " + Program.audioFilePath + " ♫");
                    playlist.AddRow(songList);

                    AnsiConsole.Write(playlist);
                    AnsiConsole.Markup("Press [red]h[/] for help");
                    AnsiConsole.Markup("\nPress [yellow]c[/] for settings");
                    AnsiConsole.Markup("\nPress [green]f[/] to hide playlist");
                    updatedPlaylist = false;
                }
            }
        }

        static public void Update()
        {
            if (times > refreshTimes)
            {
                updated = false;
                times = 0;
            }
            else
            {
                times++;
            }
        }
        static public void UpdateSongList()
        {
            if (songListTimes == 1000)
            {
                updatedSongList = false;
                songListTimes = 0;
            }
            else
            {
                songListTimes++;
            }
        }

        static public void ForceUpdate()
        {
            if (Program.textRenderedType == "settings") {
                updatedSettings = true;
                return;
            }
            else if (Program.textRenderedType == "help")
            {
                updatedHelp = true;
                return;
            }
            else if (Program.textRenderedType == "playlist")
            {
                updatedPlaylist = true;
                return;
            }
            else if (Program.textRenderedType == "FakePlayer")
            {
                return;
            }

            updated = false;
            updatedSongList = false;
        }
    }
}

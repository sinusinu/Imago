// Copyright 2025 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Imago;

class Program {
    static void Main(string[] args) {
        List<string> filesToProcess = new();

        if (args.Length == 0) {
            Console.WriteLine(
@"Usage:
    imago [file] run [actions...]                       Single-file conversion
    imago foreach [filter] run [actions...]             Batch conversion on current directory
    image foreach [directory] [filter] run [actions...] Batch conversion on remote directory"
);
            return;
        }

        var actionSeparatorIndex = -1;
        for (int i = 0; i < args.Length; i++) if (args[i] == "run") { actionSeparatorIndex = i; break; }
        if (actionSeparatorIndex == -1 || actionSeparatorIndex == args.Length - 1) {
            Console.WriteLine("No actions found");
            return;
        }

        if (args[0].ToLower(CultureInfo.InvariantCulture) == "foreach") {
            // foreach request
            if (actionSeparatorIndex == 2) {
                // for current directory
                Console.WriteLine($"Batch processing {Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}{args[1]}...");
                var files = Directory.GetFiles(Directory.GetCurrentDirectory(), args[1]);
                foreach (var file in files) {
                    var fileFullPath = Path.GetFullPath(file);
                    filesToProcess.Add(fileFullPath);
                    Console.WriteLine($"Found {fileFullPath}");
                }
            } else if (actionSeparatorIndex == 3) {
                if (Directory.Exists(args[1])) {
                    // for other directory
                    Console.WriteLine($"Batch processing {args[1]}{Path.DirectorySeparatorChar}{args[2]}...");
                    var files = Directory.GetFiles(args[1], args[2]);
                    foreach (var file in files) {
                        var fileFullPath = Path.GetFullPath(file);
                        filesToProcess.Add(fileFullPath);
                        Console.WriteLine($"Found {fileFullPath}");
                    }
                } else {
                    // huh
                    Console.WriteLine($"Path {args[1]} does not exist or illegal parameter?");
                    return;
                }
            } else {
                // huh
                Console.WriteLine("Illegal parameter");
                return;
            }
        } else {
            // single file request
            if (File.Exists(args[0])) {
                Console.WriteLine($"Processing file {args[0]}...");
                filesToProcess.Add(Path.GetFullPath(args[0]));
            } else {
                Console.WriteLine($"File {args[0]} does not exist");
                return;
            }
        }

        Dictionary<string, Type> actions = new();

        var atList = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IAction).IsAssignableFrom(t) && t.IsClass).ToList();

        foreach (var at in atList) {
            var idf = at.GetProperty("Identifier", BindingFlags.Static | BindingFlags.Public);
            if (idf is null) continue;
            var id = idf.GetValue(null);
            if (id is null || id is not string) continue;
            actions.Add((string)id, at);
        }

        List<(IAction, Dictionary<string, string>?)> actionsToExecute = new();

        for (int i = actionSeparatorIndex + 1; i < args.Length; i++) {
            var actionCommand = args[i];
            if (actionCommand.Contains("?")) {
                // with options
                Dictionary<string, string> actionOpts = new();
                var actionCommandSplit = actionCommand.Split("?");
                if (!actions.ContainsKey(actionCommandSplit[0])) {
                    Console.WriteLine($"Unknown action {actionCommand}");
                    return;
                }
                for (int j = 1; j < actionCommandSplit.Length; j++) {
                    if (actionCommandSplit[j].Contains("=")) {
                        var actionCommandOptionSplit = actionCommandSplit[j].Split("=", 2);
                        actionOpts.Add(actionCommandOptionSplit[0], actionCommandOptionSplit[1]);
                    } else {
                        Console.WriteLine($"Option {actionCommandSplit[j]} have no value");
                        return;
                    }
                }
                actionsToExecute.Add(((IAction)Activator.CreateInstance(actions[actionCommandSplit[0]])!, actionOpts));
            } else {
                // without options
                if (actions.ContainsKey(actionCommand)) {
                    actionsToExecute.Add(((IAction)Activator.CreateInstance(actions[actionCommand])!, null));
                } else {
                    Console.WriteLine($"Unknown action {actionCommand}");
                    return;
                }
            }
        }
        
        foreach (var file in filesToProcess) {
            DisposableObjectHandler doh = new();
            Dictionary<string, string> vars = new();

            vars["full_path"] = file;
            vars["full_path_no_ext"] = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file);
            vars["ext"] = Path.GetExtension(file);

            foreach ((var a, var o) in actionsToExecute) {
                Dictionary<string, string> opts = new();
                if (o is not null) foreach ((var k, var v) in o) opts.Add(k, v);

                a.Configure(opts, vars);
                a.Invoke(doh);
            }

            Console.WriteLine($"File {file} processed");

            doh.Dispose();
        }
    }
}
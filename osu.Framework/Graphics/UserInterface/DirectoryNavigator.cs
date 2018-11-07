// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu-framework/master/LICENCE

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using OpenTK.Graphics;
using osu.Framework.Configuration;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.States;

namespace osu.Framework.Graphics.UserInterface
{
    public abstract class DirectoryNavigator : Container, IHasCurrentValue<string>
    {
        private readonly FillFlowContainer<FileSystemEntry> fillFlowContainer;

        public Bindable<string> Current { get; } = new Bindable<string>(string.Empty);

        public DirectoryNavigator(string directory)
        {
            Current.Value = directory;
            Current.ValueChanged += value => updateEntries();

            Add(fillFlowContainer = CreateFillFlow());

            updateEntries();
        }

        /// <summary>
        /// Clears all existing visual entries and creates new ones based on the contents of a directory or the available drives.
        /// </summary>
        private void updateEntries() {

            //clear the previous entries.
            fillFlowContainer.Clear();

            var entries = new Dictionary<string, EntryType>();

            if (String.IsNullOrEmpty(Current.Value))
            {
                //display all available drives
                foreach (var drive in DriveInfo.GetDrives())
                    entries.Add(drive.RootDirectory.FullName, EntryType.Drive);

            } else {
                //for navigating to the parent directory.
                entries.Add("..", EntryType.Dummy);

                DirectoryInfo currentDirectory = new DirectoryInfo(Current.Value);

                //iterating over the directories and files.
                foreach (var directory in currentDirectory.EnumerateDirectories().Select(info => info.Name))
                    entries.Add(directory, EntryType.Directory);
                foreach (var file in currentDirectory.EnumerateFiles().Select(info => info.Name))
                    entries.Add(file, EntryType.File);
            }


            fillFlowContainer.AddRange(entries.Select(entry => {
                var visualEntryType = CreateVisualEntryType(entry.Value, entry.Key);
                visualEntryType.Action += () =>
                {
                    switch (visualEntryType.Type)
                    {
                        case EntryType.Directory:
                        case EntryType.Dummy:

                            // then we are on root we want to go the drives selection.
                            if (Path.GetPathRoot(Current.Value).Equals(Current.Value) && visualEntryType.Path == "..")
                                Current.Value = string.Empty;
                            else
                                Current.Value = Path.GetFullPath(Path.Combine(Current.Value, visualEntryType.Path));
                            break;
                        case EntryType.Drive:
                            Current.Value = visualEntryType.Path;
                            break;
                    }
                };
                return visualEntryType;
            }));
        }

        /// <summary>
        /// Creates a visual representation of the entry.
        /// </summary>
        /// <param name="entryType">The type of the entry.</param>
        /// <param name="name">The name of the entry.</param>
        protected abstract FileSystemEntry CreateVisualEntryType(EntryType entryType, string name);

        /// <summary>
        /// Creates a new FillFlow that is used internally for arranging the FileSystemEntries.
        /// You can use this fillflow container to customize the design of the DirectoryNavigator.
        /// </summary>
        /// <returns>The FillFlowContainer that is used internally for arranging the FileSystemEntries.</returns>
        protected virtual FillFlowContainer<FileSystemEntry> CreateFillFlow()
            => new FillFlowContainer<FileSystemEntry> {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
            };

        public enum EntryType {
            Dummy,
            File,
            Directory,
            Drive,
        }

        public abstract class FileSystemEntry : ClickableContainer {

            public EntryType Type { get; }
            public string Path { get; }

            public FileSystemEntry(EntryType type, string path) {
                Type = type;
                Path = path ?? throw new ArgumentNullException(nameof(path));
            }
        }
    }
}
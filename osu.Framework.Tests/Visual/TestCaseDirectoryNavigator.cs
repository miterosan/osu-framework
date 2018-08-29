using System.Linq;
using osu.Framework.Testing;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using OpenTK.Graphics;
using osu.Framework.Graphics.Sprites;
using System;
using OpenTK;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Graphics.Containers;
using static osu.Framework.Graphics.UserInterface.DirectoryNavigator;

namespace osu.Framework.Tests.Visual
{
    public partial class TestCaseDirectoryNavigator : TestCase
    {
        StyledDirectoryNavigator navigator;

        public TestCaseDirectoryNavigator()
        {
            Add(navigator = new StyledDirectoryNavigator("") 
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.8f),
            });

            AddAssert("There are only drives visible", () => navigator.internalFillFlowContainer.Children.All(entry => entry.Type == EntryType.Drive));
            AddStep("Select the first drive", () => navigator.internalFillFlowContainer.Children.First().TriggerOnClick());
            AddUntilStep(() => {
                navigator.internalFillFlowContainer.Children.Last(entry => entry.Type == EntryType.Directory).TriggerOnClick();
                return navigator.internalFillFlowContainer.Children.Count(entry => entry.Type == EntryType.Directory) == 0;
            });
        }

        class StyledDirectoryNavigator : DirectoryNavigator
        {
            public StyledDirectoryNavigator(string directory) : base(directory)
            {
            }

            protected override FileSystemEntry CreateVisualEntryType(EntryType entryType, string name)
                => new StyledFileSystemEntry(entryType, name);


            public FillFlowContainer<FileSystemEntry> internalFillFlowContainer { get; private set; }

            protected override FillFlowContainer<FileSystemEntry> CreateFillFlow()
                => internalFillFlowContainer = base.CreateFillFlow();
        }

        class StyledFileSystemEntry : FileSystemEntry
        {
            public StyledFileSystemEntry(EntryType type, string name) : base(type, name)
            {
                Color4 color;

                switch(type) {
                    case EntryType.Dummy: 
                        color = Color4.WhiteSmoke;
                        break;
                    case EntryType.File: 
                        color = Color4.Yellow;
                        break;
                    case EntryType.Directory: 
                        color = Color4.YellowGreen;
                        break;
                    case EntryType.Drive: 
                        color = Color4.WhiteSmoke;
                        break;
                    default:
                        throw new ArgumentException($"Entrytype {type} is not supported."); 
                }

                AutoSizeAxes = Axes.Both;
                Margin = new MarginPadding
                {
                    Top = 5
                };

                Add(new FillFlowContainer {
                    AutoSizeAxes = Axes.Both,
                    Spacing = new Vector2(5),
                    Children = new Drawable[]
                    {
                        new Box 
                        {
                            Colour = color,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Size = new Vector2(20)
                        },
                        new SpriteText
                        {
                            Text = name,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft
                        }
                    } 
                });


            }
        }
    }
}
﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace osu.Framework.Tests.Visual.UserInterface
{
    public class TestCaseBreadcrumb : FrameworkTestCase
    {
        private readonly string[] testValues =
        {
            "c://",
            "Windows",
            "Sytem32",
            "drivers",
            "etc",
            "hosts"
        };

        public TestCaseBreadcrumb()
        {
            BreadcrumbNavigation<string> navigation;

            Children = new Drawable[]
            {
                navigation = new BasicBreadcrumbNavigation<string>
                {
                    Height = 20,
                    AutoSizeAxes = Axes.X,
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Spacing = new Vector2(3, 0),
                    Items = testValues
                },
            };

            AddStep("Add range of values", () => navigation.Items = testValues);

            const int test_index = 2;

            AddStep($"Click on {testValues[test_index]} one of the elements", () => { navigation.InternalChildren[test_index].Click(); });
        }
    }
}
